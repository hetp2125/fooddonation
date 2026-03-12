using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FoodDonationPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FoodListing> FoodListings { get; set; } = new List<FoodListing>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public class FoodListing
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
        public DateTime ExpirationDate { get; set; }

        [Required]
        [StringLength(300)]
        public string Location { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        public FoodListingStatus Status { get; set; } = FoodListingStatus.Available;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string DonorId { get; set; } = string.Empty;
        public ApplicationUser? Donor { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
        public bool IsAvailable => Status == FoodListingStatus.Available && !IsExpired;
    }

    public enum FoodListingStatus
    {
        Available,
        PartiallyReserved,
        FullyReserved,
        Completed,
        Expired,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Quantity Requested")]
        public int QuantityRequested { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        [Display(Name = "Pickup Address")]
        public string? PickupAddress { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int FoodListingId { get; set; }
        public FoodListing? FoodListing { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        ReadyForPickup,
        Completed,
        Cancelled
    }
}
