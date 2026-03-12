using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDonationPlatform.Data;
using FoodDonationPlatform.Models;
using FoodDonationPlatform.Models.ViewModels;

namespace FoodDonationPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated)
                return View("Landing");

            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                var adminVM = new AdminDashboardViewModel
                {
                    TotalListings = await _context.FoodListings.CountAsync(f => f.DonorId == userId),
                    ActiveListings = await _context.FoodListings.CountAsync(f => f.DonorId == userId && f.Status == FoodListingStatus.Available),
                    TotalOrders = await _context.Orders.CountAsync(o => o.FoodListing!.DonorId == userId),
                    PendingOrders = await _context.Orders.CountAsync(o => o.FoodListing!.DonorId == userId && o.Status == OrderStatus.Pending),
                    RecentListings = await _context.FoodListings
                        .Where(f => f.DonorId == userId)
                        .OrderByDescending(f => f.CreatedAt)
                        .Take(5)
                        .Include(f => f.Orders)
                        .ToListAsync(),
                    RecentOrders = await _context.Orders
                        .Where(o => o.FoodListing!.DonorId == userId)
                        .OrderByDescending(o => o.OrderedAt)
                        .Take(5)
                        .Include(o => o.User)
                        .Include(o => o.FoodListing)
                        .ToListAsync()
                };
                return View("AdminDashboard", adminVM);
            }
            else
            {
                var userVM = new UserDashboardViewModel
                {
                    AvailableListings = await _context.FoodListings.CountAsync(f => f.Status == FoodListingStatus.Available && f.ExpirationDate > DateTime.UtcNow),
                    MyOrders = await _context.Orders.CountAsync(o => o.UserId == userId),
                    MyListings = await _context.FoodListings.CountAsync(f => f.DonorId == userId),
                    RecentListings = await _context.FoodListings
                        .Where(f => f.Status == FoodListingStatus.Available && f.ExpirationDate > DateTime.UtcNow)
                        .OrderByDescending(f => f.CreatedAt)
                        .Take(6)
                        .Include(f => f.Donor)
                        .ToListAsync(),
                    MyRecentOrders = await _context.Orders
                        .Where(o => o.UserId == userId)
                        .OrderByDescending(o => o.OrderedAt)
                        .Take(5)
                        .Include(o => o.FoodListing)
                        .ToListAsync()
                };
                return View("UserDashboard", userVM);
            }
        }
    }
}
