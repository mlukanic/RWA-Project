using Marketplace.Dtos;
using Marketplace.Interfaces;
using MarketplaceClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly MarketplaceContext _context;
        private readonly ILogService _logService;

        public ItemController(MarketplaceContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [HttpGet("[action]")]
        public ActionResult<ItemDto> GetAllItems()
        {
            try
            {
                var result = _context.Items
                    .Include(i => i.ItemTags)
                    .ThenInclude(it => it.Tag)
                    .Select(x => new ItemDto
                    {
                        ItemId = x.ItemId,
                        Title = x.Title,
                        Description = x.Description,
                        Condition = x.Condition,
                        TypeName = x.ItemType.TypeName,
                        Tags = x.ItemTags.Select(it => it.Tag.TagName).ToList()
                    });

                _logService.Log("INFO", "All items were successfully retrieved");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error getting all items: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]/{id}")]
        public ActionResult<ItemDto> GetSpecificItem(int id)
        {
            try
            {
                var item = _context.Items
                    .Include(i => i.ItemTags)
                    .ThenInclude(it => it.Tag)
                    .Include(i => i.ItemType)
                    .FirstOrDefault(x => x.ItemId == id);

                if (item == null)
                {
                    return NotFound($"Item with id: {id} not found");
                }

                var itemDto = new ItemDto
                {
                    ItemId = item.ItemId,
                    Title = item.Title,
                    Description = item.Description,
                    Condition = item.Condition,
                    TypeName = item.ItemType.TypeName,
                    Tags = item.ItemTags.Select(it => it.Tag.TagName).ToList()
                };

                _logService.Log("INFO", $"Item with id: {id} was successfully retrieved");
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error getting item with id: {id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("[action]")]
        public ActionResult<ItemDto> NewItem([FromBody] ItemDto value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest("Item data is null");
                }

                if (!ModelState.IsValid)
                {
                    _logService.Log("ERROR", $"Invalid model state: {ModelState.ToString()}");
                    return BadRequest(ModelState.ToString());
                }

                if (string.IsNullOrEmpty(value.TypeName))
                {
                    return BadRequest("Item type is required");
                }

                var trimmedItemTypeName = value.TypeName.Trim();
                var itemType = _context.ItemTypes.FirstOrDefault(x => x.TypeName == trimmedItemTypeName);

                if (itemType == null)
                {
                    itemType = new ItemType { TypeName = trimmedItemTypeName };
                    _context.ItemTypes.Add(itemType);
                    _context.SaveChanges();
                }

                var trimmedItemTitle = value.Title.Trim();
                var item = _context.Items.FirstOrDefault(x => x.Title == trimmedItemTitle);
                if (item != null)
                {
                    return BadRequest($"Item with title: {trimmedItemTitle} already exists");
                }

                var newItem = new Item
                {
                    Title = trimmedItemTitle,
                    Description = value.Description,
                    Condition = value.Condition,
                    DatePosted = DateTime.Now,
                    ItemTypeId = itemType.ItemTypeId
                };

                foreach (var tagName in value.Tags)
                {
                    var trimmedTagName = tagName.Trim();
                    var tag = _context.Tags.FirstOrDefault(t => t.TagName == trimmedTagName);
                    if (tag == null)
                    {
                        tag = new Tag { TagName = trimmedTagName };
                        _context.Tags.Add(tag);
                        _context.SaveChanges();
                    }
                    newItem.ItemTags.Add(new ItemTag { Item = newItem, Tag = tag });
                }

                _context.Items.Add(newItem);
                _context.SaveChanges();

                value.ItemId = newItem.ItemId;
                _logService.Log("INFO", $"New item with title: {value.Title} was successfully created");
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error creating new item: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("[action]/{id}")]
        public ActionResult<ItemDto> UpdateItem(int id, [FromBody] ItemDto value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest("Item data is null");
                }

                if (!ModelState.IsValid)
                {
                    _logService.Log("ERROR", $"Invalid model state: {ModelState.ToString()}");
                    return BadRequest(ModelState.ToString());
                }

                var item = _context.Items
                    .Include(i => i.ItemTags)
                    .ThenInclude(it => it.Tag)
                    .FirstOrDefault(x => x.ItemId == id);

                if (item == null)
                {
                    return NotFound($"Item with id: {id} not found");
                }

                if (string.IsNullOrEmpty(value.TypeName))
                {
                    return BadRequest("Item type is required");
                }

                var trimmedItemTypeName = value.TypeName.Trim();
                var itemType = _context.ItemTypes.FirstOrDefault(x => x.TypeName == trimmedItemTypeName);

                if (itemType == null)
                {
                    itemType = new ItemType { TypeName = trimmedItemTypeName };
                    _context.ItemTypes.Add(itemType);
                    _context.SaveChanges();
                }

                item.Title = value.Title?.Trim() ?? throw new ArgumentNullException(nameof(value.Title));
                item.Description = value.Description;
                item.Condition = value.Condition;
                item.ItemTypeId = itemType.ItemTypeId;

                var existingTags = item.ItemTags.ToList();
                foreach (var tagName in value.Tags)
                {
                    var trimmedTagName = tagName?.Trim() ?? throw new ArgumentNullException(nameof(tagName));
                    var tag = _context.Tags.FirstOrDefault(t => t.TagName == trimmedTagName);
                    if (tag == null)
                    {
                        tag = new Tag { TagName = trimmedTagName };
                        _context.Tags.Add(tag);
                        _context.SaveChanges();
                    }

                    if (!existingTags.Any(it => it.TagId == tag.TagId))
                    {
                        item.ItemTags.Add(new ItemTag { ItemId = item.ItemId, TagId = tag.TagId });
                    }
                }

                foreach (var existingTag in existingTags)
                {
                    if (!value.Tags.Any(t => t.Trim() == existingTag.Tag.TagName))
                    {
                        _context.ItemTags.Remove(existingTag);
                    }
                }

                _context.Items.Update(item);
                _context.SaveChanges();

                value.ItemId = item.ItemId;
                _logService.Log("INFO", $"Item with id: {id} was successfully updated");
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error updating item: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("[action]/{id}")]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                var item = _context.Items
                    .Include(i => i.ItemTags)
                    .ThenInclude(it => it.Tag)
                    .FirstOrDefault(x => x.ItemId == id);

                if (item == null)
                {
                    return NotFound($"Item with id: {id} not found");
                }

                foreach (var itemTag in item.ItemTags.ToList())
                {
                    _context.ItemTags.Remove(itemTag);

                    if (!_context.ItemTags.Any(it => it.TagId == itemTag.TagId))
                    {
                        _context.Tags.Remove(itemTag.Tag);
                    }
                }

                _context.Items.Remove(item);

                if (!_context.Items.Any(i => i.ItemTypeId == item.ItemTypeId))
                {
                    _context.ItemTypes.Remove(item.ItemType);
                }

                _context.SaveChanges();

                _logService.Log("INFO", $"Item with id: {id} was successfully deleted");
                return Ok($"Item with id: {id} was successfully deleted");
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"Error deleting item: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
