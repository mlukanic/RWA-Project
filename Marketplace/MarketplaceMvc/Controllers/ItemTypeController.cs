using AutoMapper;
using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MarketplaceMvc.Controllers
{
    public class ItemTypeController : Controller
    {
        private readonly MarketplaceContext _context;
        private readonly IMapper _mapper;

        public ItemTypeController(MarketplaceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            try
            {
                ItemTypeVM? newItemTypeVM = null;

                if (TempData.ContainsKey("newGenre"))
                {
                    var newGenreJson = TempData["newGenre"] as string;
                    if (!string.IsNullOrEmpty(newGenreJson))
                    {
                        newItemTypeVM = JsonConvert.DeserializeObject<ItemTypeVM>(newGenreJson);
                    }
                }

                var itemTypes = _context.ItemTypes.ToList();
                var itemTypeVMs = _mapper.Map<IEnumerable<ItemType>, IEnumerable<ItemTypeVM>>(itemTypes);

                // Ensure itemTypeVMs is not null
                if (itemTypeVMs == null)
                {
                    itemTypeVMs = new List<ItemTypeVM>();
                }

                return View(itemTypeVMs);
            }
            catch (Exception)
            {
                throw; // Rethrow the caught exception without changing the stack trace
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == id);
                var itemTypesVM = _mapper.Map<IEnumerable<ItemTypeVM>>(itemType);

                return View(itemTypesVM);
            }
            catch (Exception)
            {
                throw; // Rethrow the caught exception without changing the stack trace
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemTypeVM itemType)
        {
            var trimmedItemType = itemType.TypeName?.Trim(); // Ensure TypeName is not null
            if (trimmedItemType != null && _context.ItemTypes.Any(x => x.TypeName.Equals(trimmedItemType)))
            {
                ModelState.AddModelError("", "This type already exists");
                return View();
            }

            try
            {
                var newItemType = new ItemType
                {
                    TypeName = itemType.TypeName
                };

                _context.ItemTypes.Add(newItemType);
                _context.SaveChanges();

                TempData["newPropertyType"] = JsonConvert.SerializeObject(newItemType); // Use JsonConvert.SerializeObject

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == id);
                var itemTypeVM = new ItemTypeVM
                {
                    ItemTypeId = itemType.ItemTypeId,
                    TypeName = itemType.TypeName
                };

                return View(itemTypeVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ItemTypeVM itemType)
        {
            try
            {
                var dbItemType = _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == id);
                dbItemType.TypeName = itemType.TypeName;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var itemType = _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == id);
                var itemTypeVM = new ItemTypeVM
                {
                    ItemTypeId = itemType.ItemTypeId,
                    TypeName = itemType.TypeName
                };

                return View(itemTypeVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ItemTypeVM itemType)
        {
            try
            {
                var dbItemType = _context.ItemTypes.FirstOrDefault(x => x.ItemTypeId == id);

                _context.ItemTypes.Remove(dbItemType);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
