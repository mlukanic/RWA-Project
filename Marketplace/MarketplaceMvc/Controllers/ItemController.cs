using Microsoft.AspNetCore.Mvc;

namespace MarketplaceMvc.Controllers
{
    public class ItemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
