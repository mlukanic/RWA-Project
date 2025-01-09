using AutoMapper;
using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MarketplaceMvc.Controllers
{
    public class TagController : Controller
    {
        private readonly MarketplaceContext _context;

        private readonly IMapper _mapper;
        public TagController(MarketplaceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            try
            {
                TagVM? newTag = null;

                if (TempData.ContainsKey("newGenre"))
                {
                    var newGenreJson = TempData["newTag"] as string;
                    if (!string.IsNullOrEmpty(newGenreJson))
                    {
                        newTag = JsonConvert.DeserializeObject<TagVM>(newGenreJson);
                    }
                }

                var tagVms = _context.Tags.Select(x => new TagVM
                {
                    TagId = x.TagId,
                    TagName = x.TagName
                }).ToList();

                var tags = _context.Tags;
                var tagsVMs = _mapper.Map<IEnumerable<TagVM>>(tags);

                // Ensure itemTypeVMs is not null
                if (tagsVMs == null)
                {
                    tagsVMs = new List<TagVM>();
                }

                return View(tagsVMs);
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
                var tag = _context.Tags.FirstOrDefault(x => x.TagId == id);
                var tagsVM = new TagVM
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName
                };

                return View(tagsVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TagVM tag)
        {
            try
            {
                var newTag = new Tag
                {
                    TagName = tag.TagName
                };

                _context.Tags.Add(newTag);

                _context.SaveChanges();

                TempData["newTag"] = JsonConvert.SerializeObject(newTag);

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
                var tag = _context.Tags.FirstOrDefault(x => x.TagId == id);
                var tagVM = new TagVM
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName
                };

                return View(tagVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TagVM tag)
        {
            try
            {
                var dbTag = _context.Tags.FirstOrDefault(x => x.TagId == id);
                dbTag.TagName = tag.TagName;

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
                var tag = _context.Tags.FirstOrDefault(x => x.TagId == id);
                var tagVM = new TagVM
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName
                };

                return View(tagVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TagVM tag)
        {
            try
            {
                var dbTag = _context.Tags.FirstOrDefault(x => x.TagId == id);

                _context.Tags.Remove(dbTag);

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
