using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDonationPlatform.Data;
using FoodDonationPlatform.Models;
using FoodDonationPlatform.Models.ViewModels;

namespace FoodDonationPlatform.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: My Orders (LocalUser) or Orders on my listings (Admin)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            if (User.IsInRole("Admin"))
            {
                // Admin sees orders placed on their food listings
                var orders = await _context.Orders
                    .Where(o => o.FoodListing!.DonorId == userId)
                    .Include(o => o.User)
                    .Include(o => o.FoodListing)
                    .OrderByDescending(o => o.OrderedAt)
                    .ToListAsync();

                return View("AdminOrders", orders);
            }
            else
            {
                // LocalUser sees their own orders
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.FoodListing)
                        .ThenInclude(f => f!.Donor)
                    .OrderByDescending(o => o.OrderedAt)
                    .ToListAsync();

                return View("UserOrders", orders);
            }
        }

        // GET: Place order form
        [HttpGet]
        [Authorize(Roles = "LocalUser")]
        public async Task<IActionResult> PlaceOrder(int foodListingId)
        {
            var listing = await _context.FoodListings
                .Include(f => f.Donor)
                .FirstOrDefaultAsync(f => f.Id == foodListingId);

            if (listing == null) return NotFound();
            if (!listing.IsAvailable)
            {
                TempData["Error"] = "This food item is no longer available.";
                return RedirectToAction("Browse", "FoodListings");
            }

            var userId = _userManager.GetUserId(User);
            if (listing.DonorId == userId)
            {
                TempData["Error"] = "You cannot order your own food listing.";
                return RedirectToAction("Browse", "FoodListings");
            }

            var user = await _userManager.GetUserAsync(User);
            var vm = new PlaceOrderViewModel
            {
                FoodListingId = foodListingId,
                FoodListing = listing,
                PickupAddress = user?.Address
            };

            return View(vm);
        }

        // POST: Place order
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "LocalUser")]
        public async Task<IActionResult> PlaceOrder(PlaceOrderViewModel model)
        {
            var listing = await _context.FoodListings.FindAsync(model.FoodListingId);
            if (listing == null) return NotFound();

            if (!listing.IsAvailable)
            {
                TempData["Error"] = "This item is no longer available.";
                return RedirectToAction("Browse", "FoodListings");
            }

            if (model.QuantityRequested > listing.Quantity)
                ModelState.AddModelError("QuantityRequested", $"Only {listing.Quantity} {listing.Unit} available.");

            model.FoodListing = listing;
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            var order = new Order
            {
                FoodListingId = model.FoodListingId,
                UserId = userId,
                QuantityRequested = model.QuantityRequested,
                Notes = model.Notes,
                PickupAddress = model.PickupAddress,
                Status = OrderStatus.Pending
            };

            // Update listing quantity/status
            listing.Quantity -= model.QuantityRequested;
            listing.UpdatedAt = DateTime.UtcNow;
            if (listing.Quantity == 0)
                listing.Status = FoodListingStatus.FullyReserved;
            else if (listing.Quantity < listing.Quantity + model.QuantityRequested)
                listing.Status = FoodListingStatus.PartiallyReserved;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order placed successfully! The donor will confirm your pickup.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Order details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.FoodListing)
                    .ThenInclude(f => f!.Donor)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Security: only the orderer or the food listing owner can view
            var isOrderer = order.UserId == userId;
            var isDonor = order.FoodListing?.DonorId == userId;
            if (!isOrderer && !isDonor) return Forbid();

            return View(order);
        }

        // POST: Update order status (Admin/Donor only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.FoodListing)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Only the food listing donor can update status
            if (order.FoodListing?.DonorId != userId) return Forbid();

            // If cancelling, restore quantity
            if (status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                var listing = order.FoodListing;
                if (listing != null)
                {
                    listing.Quantity += order.QuantityRequested;
                    listing.Status = listing.Quantity > 0 ? FoodListingStatus.Available : listing.Status;
                    listing.UpdatedAt = DateTime.UtcNow;
                }
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order status updated to {status}.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cancel own order (LocalUser)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "LocalUser")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.FoodListing)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();
            if (order.Status != OrderStatus.Pending)
            {
                TempData["Error"] = "Only pending orders can be cancelled.";
                return RedirectToAction(nameof(Index));
            }

            // Restore quantity
            if (order.FoodListing != null)
            {
                order.FoodListing.Quantity += order.QuantityRequested;
                order.FoodListing.Status = FoodListingStatus.Available;
                order.FoodListing.UpdatedAt = DateTime.UtcNow;
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
