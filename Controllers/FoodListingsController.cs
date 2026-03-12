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
    public class FoodListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FoodListingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: All available food listings (both roles)
        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? category, string? location)
        {
            var query = _context.FoodListings
                .Include(f => f.Donor)
                .Include(f => f.Orders)
                .AsQueryable();

            // Admins see only their own listings here; use Browse for all
            if (User.IsInRole("Admin"))
                query = query.Where(f => f.DonorId == _userManager.GetUserId(User));

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f => f.FoodName.Contains(search) || f.Description.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(f => f.Category == category);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(f => f.Location.Contains(location));

            var categories = await _context.FoodListings
                .Where(f => f.Category != null)
                .Select(f => f.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var vm = new FoodListingIndexViewModel
            {
                Listings = await query.OrderByDescending(f => f.CreatedAt).ToListAsync(),
                SearchTerm = search,
                Category = category,
                Location = location,
                Categories = categories
            };

            return View(vm);
        }

        // GET: Browse all available listings (LocalUser)
        [HttpGet]
        [Authorize(Roles = "LocalUser")]
        public async Task<IActionResult> Browse(string? search, string? category, string? location)
        {
            var query = _context.FoodListings
                .Include(f => f.Donor)
                .Where(f => f.Status == FoodListingStatus.Available && f.ExpirationDate > DateTime.UtcNow)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(f => f.FoodName.Contains(search) || f.Description.Contains(search) || f.Location.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(f => f.Category == category);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(f => f.Location.Contains(location));

            var categories = await _context.FoodListings
                .Where(f => f.Category != null)
                .Select(f => f.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var vm = new FoodListingIndexViewModel
            {
                Listings = await query.OrderByDescending(f => f.CreatedAt).ToListAsync(),
                SearchTerm = search,
                Category = category,
                Location = location,
                Categories = categories
            };

            return View(vm);
        }

        // GET: Details of a food listing
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _context.FoodListings
                .Include(f => f.Donor)
                .Include(f => f.Orders)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (listing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var vm = new FoodListingDetailViewModel
            {
                Listing = listing,
                CanOrder = User.IsInRole("LocalUser") && listing.IsAvailable && listing.DonorId != userId,
                IsOwner = listing.DonorId == userId
            };

            return View(vm);
        }

        // GET: Create new listing
        [HttpGet]
        public IActionResult Create() => View(new FoodListingViewModel { ExpirationDate = DateTime.Today.AddDays(3) });

        // POST: Create new listing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodListingViewModel model)
        {
            if (model.ExpirationDate <= DateTime.Today)
                ModelState.AddModelError("ExpirationDate", "Expiration date must be in the future.");

            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User)!;
            var listing = new FoodListing
            {
                FoodName = model.FoodName,
                Description = model.Description,
                Quantity = model.Quantity,
                Unit = model.Unit,
                ExpirationDate = model.ExpirationDate.ToUniversalTime(),
                Location = model.Location,
                Category = model.Category,
                DonorId = userId,
                Status = FoodListingStatus.Available
            };

            _context.FoodListings.Add(listing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Food listing created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit listing
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var listing = await _context.FoodListings.FindAsync(id);
            if (listing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (listing.DonorId != userId) return Forbid();

            var vm = new FoodListingViewModel
            {
                Id = listing.Id,
                FoodName = listing.FoodName,
                Description = listing.Description,
                Quantity = listing.Quantity,
                Unit = listing.Unit,
                ExpirationDate = listing.ExpirationDate,
                Location = listing.Location,
                Category = listing.Category
            };

            return View(vm);
        }

        // POST: Edit listing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FoodListingViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (model.ExpirationDate <= DateTime.Today)
                ModelState.AddModelError("ExpirationDate", "Expiration date must be in the future.");

            if (!ModelState.IsValid) return View(model);

            var listing = await _context.FoodListings.FindAsync(id);
            if (listing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (listing.DonorId != userId) return Forbid();

            listing.FoodName = model.FoodName;
            listing.Description = model.Description;
            listing.Quantity = model.Quantity;
            listing.Unit = model.Unit;
            listing.ExpirationDate = model.ExpirationDate.ToUniversalTime();
            listing.Location = model.Location;
            listing.Category = model.Category;
            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Food listing updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete listing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var listing = await _context.FoodListings.FindAsync(id);
            if (listing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (listing.DonorId != userId) return Forbid();

            _context.FoodListings.Remove(listing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Food listing deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Update listing status (owner only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, FoodListingStatus status)
        {
            var listing = await _context.FoodListings.FindAsync(id);
            if (listing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (listing.DonorId != userId) return Forbid();

            listing.Status = status;
            listing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Listing status updated!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
