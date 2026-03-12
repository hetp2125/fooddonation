using System.ComponentModel.DataAnnotations;

namespace FoodDonationPlatform.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string Role { get; set; } = "LocalUser";
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class FoodListingViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Food Name")]
        public string FoodName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "servings";

        [Required]
        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; } = DateTime.Today.AddDays(3);

        [Required]
        [StringLength(300)]
        public string Location { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }
    }

    public class FoodListingDetailViewModel
    {
        public FoodListing Listing { get; set; } = null!;
        public bool CanOrder { get; set; }
        public bool IsOwner { get; set; }
    }

    public class PlaceOrderViewModel
    {
        public int FoodListingId { get; set; }
        public FoodListing? FoodListing { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Quantity")]
        public int QuantityRequested { get; set; } = 1;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        [Display(Name = "Your Pickup Address")]
        public string? PickupAddress { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public List<FoodListing> RecentListings { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }

    public class UserDashboardViewModel
    {
        public int AvailableListings { get; set; }
        public int MyOrders { get; set; }
        public int MyListings { get; set; }
        public List<FoodListing> RecentListings { get; set; } = new();
        public List<Order> MyRecentOrders { get; set; } = new();
    }

    public class FoodListingIndexViewModel
    {
        public List<FoodListing> Listings { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Location { get; set; }
        public List<string> Categories { get; set; } = new();
    }

    public class UpdateOrderStatusViewModel
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
