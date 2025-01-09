using MarketplaceClassLibrary.Models;
using MarketplaceMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceMvc.Controllers
{
    public class ReservationController : Controller
    {
        private readonly MarketplaceContext _context;
        private readonly IConfiguration _configuration;

        public ReservationController(MarketplaceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index(int page = 1, int size = 10)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;

            var reservations = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReservationId);

            var totalCount = reservations.Count();

            var paginatedReservations = reservations
                .Skip((page - 1) * size)
                .Take(size)
                .Select(r => new ReservationVM
                {
                    ReservationId = r.ReservationId,
                    ItemId = r.ItemId,
                    ItemTitle = r.Item.Title,
                    Username = r.User.Username
                }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = size;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)size);
            ViewBag.FromPager = page > 4 ? page - 4 : 1;
            ViewBag.ToPager = page + 4 <= ViewBag.TotalPages ? page + 4 : ViewBag.TotalPages;

            return View(paginatedReservations);
        }

        public IActionResult UserIndex(ReservationUserVM reservationUserVM)
        {
            if (string.IsNullOrEmpty(reservationUserVM.Username) && string.IsNullOrEmpty(reservationUserVM.Submit))
            {
                reservationUserVM.Username = Request.Cookies["query"];
            }

            Response.Cookies.Append("query", reservationUserVM.Username ?? "");

            var reservations = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.User)
                .OrderByDescending(r => r.ReservationId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(reservationUserVM.Username))
            {
                reservations = reservations.Where(r => r.User.Username.Contains(reservationUserVM.Username));
            }

            var filteredCount = reservations.Count();

            var paginatedReservations = reservations
                .Skip((reservationUserVM.Page - 1) * reservationUserVM.Size)
                .Take(reservationUserVM.Size)
                .Select(r => new ReservationVM
                {
                    ReservationId = r.ReservationId,
                    ItemId = r.ItemId,
                    ItemTitle = r.Item.Title,
                    Username = r.User.Username
                }).ToList();

            var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
            reservationUserVM.LastPage = (int)Math.Ceiling(1.0 * filteredCount / reservationUserVM.Size);
            reservationUserVM.FromPager = reservationUserVM.Page > expandPages ? reservationUserVM.Page - expandPages : 1;
            reservationUserVM.ToPager = (reservationUserVM.Page + expandPages) < reservationUserVM.LastPage ? reservationUserVM.Page + expandPages : reservationUserVM.LastPage;
            reservationUserVM.Reservations = paginatedReservations;

            return View(reservationUserVM);
        }

        public IActionResult Details(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.User)
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationVM = new ReservationVM
            {
                ReservationId = reservation.ReservationId,
                ItemTitle = reservation.Item.Title,
                Username = reservation.User.Username
            };

            return View(reservationVM);
        }

        [HttpGet]
        public IActionResult Create(int itemId)
        {
            if (itemId <= 0)
            {
                return BadRequest("Invalid item ID.");
            }

            var item = _context.Items.Find(itemId);
            if (item == null)
            {
                return NotFound($"Item with ID {itemId} not found.");
            }

            var reservationVM = new ReservationVM
            {
                ItemId = itemId,
                ItemTitle = item.Title,
                Username = User.Identity.Name // Get the currently logged-in user's username
            };

            return View(reservationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservationVM reservationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(reservationVM);
            }

            var item = _context.Items.Find(reservationVM.ItemId);
            if (item == null)
            {
                return NotFound($"Item with ID {reservationVM.ItemId} not found.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(reservationVM);
            }

            var reservation = new Reservation
            {
                ItemId = (int)reservationVM.ItemId,
                UserId = user.UserId,
                Username = reservationVM.Username
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return RedirectToAction("Confirmation", new
            {
                itemId = reservation.ItemId,
                username = reservation.Username
            });
        }

        public IActionResult Confirmation(int itemId, string username)
        {
            var item = _context.Items.Find(itemId);
            if (item == null)
            {
                return NotFound();
            }

            var reservationVM = new ReservationVM
            {
                ItemId = itemId,
                ItemTitle = item.Title,
                Username = username
            };

            return View(reservationVM);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Item)
                .Include(r => r.User)
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationVM = new ReservationVM
            {
                ReservationId = reservation.ReservationId,
                ItemId = reservation.ItemId,
                ItemTitle = reservation.Item.Title,
                Username = reservation.Username,
            };

            return View(reservationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection form)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
