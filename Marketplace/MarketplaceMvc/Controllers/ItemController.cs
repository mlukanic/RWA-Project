using AutoMapper;
using Azure;
using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System.Text.Json;

namespace MarketplaceMvc.Controllers
{
    public class ItemController : Controller
    {
        private readonly MarketplaceContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public ItemController(MarketplaceContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        private List<SelectListItem> GetTypeListItems()
        {
            var typeListItemsJson = HttpContext.Session.GetString("TypeListItems");

            List<SelectListItem> typeListItems;
            if (typeListItemsJson == null)
            {
                typeListItems = _context.ItemTypes
                    .Select(x => new SelectListItem
                    {
                        Text = x.TypeName,
                        Value = x.ItemTypeId.ToString()
                    }).ToList();

                HttpContext.Session.SetString("TypeListItems", JsonSerializer.Serialize(typeListItems));
            }
            else
            {
                typeListItems = JsonSerializer.Deserialize<List<SelectListItem>>(typeListItemsJson);
            }

            return typeListItems;
        }

        public IActionResult Index(int page = 1, int size = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (size < 1) size = 10;

                IEnumerable<Item> items = _context.Items
                    .Include(x => x.ItemType)
                    .OrderByDescending(x => x.ItemId);

                var totalCount = items.Count();

                items = items
                .Skip((page - 1) * size)
                    .Take(size);

                var itemVMs = items.Select(x => new ItemVM
                {
                    ItemId = x.ItemId,
                    ItemTypeId = x.ItemTypeId,
                    TypeName = x.ItemType.TypeName,
                    Title = x.Title,
                    Description = x.Description,
                    Condition = x.Condition
                }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = size;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)size);
                ViewBag.FromPager = page > 4 ? page - 4 : 1;
                ViewBag.ToPager = page + 4 <= ViewBag.TotalPages ? page + 4 : ViewBag.TotalPages;

                return View(itemVMs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        public ActionResult GetItemsByUser(int? max)
        {
            try
            {
                IQueryable<Item> items = _context.Items
                    .Include(x => x.ItemType);

                if (max != null)
                {
                    items = items.Where(x => x.Reservations.Count() <= max);
                }

                var itemsVMs = items.Select(x => new ItemVM
                {
                    ItemId = x.ItemId,
                    ItemTypeId = x.ItemTypeId,
                    TypeName = x.ItemType.TypeName,
                    Title = x.Title,
                    Description = x.Description,
                    Condition = x.Condition
                }).ToList();

                return View("Index", itemsVMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                var item = _context.Items
                    .Include(x => x.ItemType)
                    .Include(x => x.ItemTags)
                        .ThenInclude(pa => pa.Tag)
                    .FirstOrDefault(x => x.ItemId == id);

                if (item == null)
                {
                    return NotFound();
                }

                var allTags = _context.Tags.ToList();

                var itemVM = new ItemVM
                {
                    ItemId = item.ItemId,
                    ItemTypeId = item.ItemTypeId,
                    TypeName = item.ItemType.TypeName,
                    Title = item.Title,
                    Description = item.Description,
                    Condition = item.Condition,

                    TagIds = item.ItemTags
                    .Select(pa => pa.TagId)
                    .ToList(),

                    Tags = allTags.Select(a => new TagVM
                    {
                        TagId = a.TagId,
                        TagName = a.TagName
                    }).ToList()
                };

                return View(itemVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Create()
        {
            ViewBag.ItemTypeDdlItems = GetTypeListItems();

            var item = new ItemVM();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemVM item)
        {
            var trimmedItem = item.Title.Trim();
            if (_context.Items.Any(x => x.Title.Equals(trimmedItem)))
            {
                ModelState.AddModelError("", "This item already exists");
                return View();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ItemTypeDdlItems = GetTypeListItems();

                    ModelState.AddModelError("", "Failed to create item");

                    return View(item);
                }

                var newItem = new Item
                {
                    ItemId = item.ItemId,
                    ItemTypeId = (int)item.ItemTypeId,
                    Title = item.Title,
                    Description = item.Description,
                    Condition = item.Condition
                };

                _context.Items.Add(newItem);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the item: " + ex.Message);
                ViewBag.ItemTypeDdlItems = GetTypeListItems();
                return View(item);
            }
        }

        private IQueryable<SelectListItem> GetItemTypes()
        {
            return _context.ItemTypes.Select(x =>
            new SelectListItem
            {
                Text = x.TypeName,
                Value = x.ItemTypeId.ToString()
            });
        }

        private IQueryable<SelectListItem> GetUser()
        {
            return _context.Users.Select(x =>
            new SelectListItem
            {
                Text = x.Username,
                Value = x.UserId.ToString()
            });
        }

        private List<SelectListItem> GetTagListItems()
        {
            return _context.Tags
                .Select(x => new SelectListItem
                {
                    Text = x.TagName,
                    Value = x.TagId.ToString()
                }).ToList();
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ItemTypeDdlItems = GetItemTypes();

            ViewBag.TagDdlItems = GetTagListItems();

            var item = _context.Items
                .Include(x => x.ItemTags)
                .FirstOrDefault(x => x.ItemId == id);

            var allTags = _context.Tags.ToList();

            var itemVM = new ItemVM
            {
                ItemId = item.ItemId,
                ItemTypeId = item.ItemTypeId,
                Title = item.Title,
                Description = item.Description,
                Condition = item.Condition,
                TagIds = item.ItemTags
                .Select(pa => pa.TagId)
                .ToList(),

                Tags = allTags.Select(a => new TagVM
                {
                    TagId = a.TagId,
                    TagName = a.TagName
                }).ToList()
            };

            return View(itemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemVM item)
        {
            try
            {
                var dbItem = _context.Items.Include(x => x.ItemTags).FirstOrDefault(x => x.ItemId == id);
                dbItem.ItemTypeId = (int)item.ItemTypeId;
                dbItem.Title = item.Title;
                dbItem.Description = item.Description;
                dbItem.Condition = item.Condition;

                _context.RemoveRange(dbItem.ItemTags);
                var itemTags = item.TagIds.Select(x => new ItemTag { ItemId = id, TagId = x }).ToList();
                foreach (var tag in itemTags)
                {
                    dbItem.ItemTags.Add(tag);
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            var item = _context.Items
                .Include(x => x.ItemType)
                .FirstOrDefault(x => x.ItemId == id);

            var itemVM = new ItemVM
            {
                ItemId = item.ItemId,
                ItemTypeId = item.ItemTypeId,
                TypeName = item.ItemType.TypeName,
                Title = item.Title,
                Description = item.Description,
                Condition = item.Condition
            };

            return View(itemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var dbItems = _context.Items.FirstOrDefault(x => x.ItemId == id);

                _context.Items.Remove(dbItems);

                _context.SaveChanges();

                return RedirectToAction("Index", "Property");
            }
            catch
            {
                return View();
            }
        }

    }
}
