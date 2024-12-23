using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MarketplaceMvc.Controllers
{
    public class ItemTagController : Controller
    {
        private readonly MarketplaceContext _context;

        public ItemTagController(MarketplaceContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            try
            {
                var itemTagVMs = _context.ItemTags
                    .Include(x => x.Item)
                    .Include(x => x.Tag)
                    .Select(x => new ItemTagVM
                    {
                        ItemTagId = x.ItemTagId,
                        ItemId = x.ItemId,
                        ItemTitle = x.Item.Title,
                        TagId = x.TagId,
                        TagName = x.Tag.TagName
                    })
                    .ToList();

                return View(itemTagVMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var itemTag = _context.ItemTags
                    .Include(pa => pa.Item)
                    .Include(pa => pa.Tag)
                    .FirstOrDefault(pa => pa.ItemTagId == id);

                if (itemTag == null)
                {
                    return NotFound();
                }

                var itemTagVM = new ItemTagVM
                {
                    ItemTagId = itemTag.ItemTagId,
                    ItemId = itemTag.ItemId,
                    ItemTitle = itemTag.Item.Title,
                    TagId = itemTag.TagId,
                    TagName = itemTag.Tag.TagName
                };

                return View(itemTagVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IQueryable<SelectListItem> GetItem()
        {
            return _context.Items.Select(x =>
            new SelectListItem
            {
                Text = x.Title,
                Value = x.ItemId.ToString()
            });
        }

        private IQueryable<SelectListItem> GetTag()
        {
            return _context.Tags.Select(x =>
            new SelectListItem
            {
                Text = x.TagName,
                Value = x.TagId.ToString()
            });
        }

        private List<SelectListItem> GetItemListItems()
        {
            var itemListItemsJson = HttpContext.Session.GetString("ItemListItems");

            List<SelectListItem> itemListItems;
            if (itemListItemsJson == null)
            {
                itemListItems = _context.Items
                    .Select(x => new SelectListItem
                    {
                        Text = x.Title,
                        Value = x.ItemId.ToString()
                    }).ToList();

                HttpContext.Session.SetString("ItemListItems", JsonSerializer.Serialize(itemListItems));
            }
            else
            {
                itemListItems = JsonSerializer.Deserialize<List<SelectListItem>>(itemListItemsJson);
            }

            return itemListItems;
        }

        private List<SelectListItem> GetTagListItems()
        {
            var tagListItemsJson = HttpContext.Session.GetString("TagListItems");

            List<SelectListItem> tagListItems;
            if (tagListItemsJson == null)
            {
                tagListItems = _context.Tags
                      .Select(x => new SelectListItem
                      {
                          Text = x.TagName,
                          Value = x.TagId.ToString()
                      }).ToList();

                HttpContext.Session.SetString("TagListItems", JsonSerializer.Serialize(tagListItems));
            }
            else
            {
                tagListItems = JsonSerializer.Deserialize<List<SelectListItem>>(tagListItemsJson);
            }

            return tagListItems;
        }

        public ActionResult Create()
        {
            ViewBag.ItemDdlItems = GetItemListItems();
            ViewBag.TagDdlItems = GetTagListItems();

            var itemTag = new ItemTagVM();

            return View(itemTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemTagVM itemTagVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ItemDdlItems = GetItemListItems();
                ViewBag.TagDdlItems = GetTagListItems();
                ModelState.AddModelError("", "Failed to create this");
                return View();
            }

            var exists = _context.ItemTags
           .Any(x => x.ItemId == itemTagVM.ItemId && x.TagId == itemTagVM.TagId);

            if (exists)
            {
                ModelState.AddModelError("", "This tag is already assigned to the item.");
                ViewBag.ItemDdlItems = GetItemListItems();
                ViewBag.TagDdlItems = GetTagListItems();
                return View(itemTagVM);
            }

            try
            {
                var newItemTag = new ItemTag
                {
                    ItemId = (int)itemTagVM.ItemId,
                    TagId = (int)itemTagVM.TagId
                };

                _context.ItemTags.Add(newItemTag);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the item: " + ex.Message);

                ViewBag.ItemDdlItems = GetItemListItems();
                ViewBag.TagDdlItems = GetTagListItems();

                return View(itemTagVM);
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.ItemDdlItems = GetItemListItems();
            ViewBag.TagDdlItems = GetTagListItems();

            var itemTag = _context.ItemTags
                    .Include(x => x.Item)
                    .Include(x => x.Tag)
                    .FirstOrDefault(x => x.ItemTagId == id);

            if (itemTag == null)
            {
                return NotFound();
            }

            var itemTagVM = new ItemTagVM
            {
                ItemTagId = itemTag.ItemTagId,
                ItemId = itemTag.ItemId,
                TagId = itemTag.TagId
            };

            return View(itemTagVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemTagVM itemTagVM)
        {
            try
            {
                var dbItemTag = _context.ItemTags
                .Include(x => x.Item)
                .Include(x => x.Tag)
                .FirstOrDefault(x => x.ItemTagId == id);

                dbItemTag.ItemId = (int)itemTagVM.ItemId;
                dbItemTag.TagId = (int)itemTagVM.TagId;

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
            try
            {
                var itemTag = _context.ItemTags
                    .Include(x => x.Item)
                    .Include(x => x.Tag)
                    .FirstOrDefault(x => x.ItemTagId == id);

                if (itemTag == null)
                {
                    return NotFound();
                }

                var itemTagVM = new ItemTagVM
                {
                    ItemTagId = itemTag.ItemTagId,
                    ItemId = itemTag.ItemId,
                    ItemTitle = itemTag.Item.Title,
                    TagName = itemTag.Tag.TagName,
                    TagId = itemTag.TagId
                };

                return View(itemTagVM);
            }
            catch (Exception ex)
            {
                // Handle exception
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var itemTag = _context.ItemTags.Find(id);
                if (itemTag != null)
                {
                    _context.ItemTags.Remove(itemTag);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Handle exception
                return View();
            }
        }
    }

}
