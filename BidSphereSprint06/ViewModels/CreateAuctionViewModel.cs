// ViewModels/CreateAuctionViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace BidSphereProject.ViewModels
{
    public class CreateAuctionViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Starting price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal StartingPrice { get; set; }

        public string Description { get; set; }

        // Duration Fields
        [Range(0, 168, ErrorMessage = "Hours must be between 0 and 168 (7 days)")]
        public int DurationHours { get; set; } = 0;

        [Range(0, 59, ErrorMessage = "Minutes must be between 0 and 59")]
        public int DurationMinutes { get; set; } = 30; // Default 30 minutes

        [Range(0, 59, ErrorMessage = "Seconds must be between 0 and 59")]
        public int DurationSeconds { get; set; } = 0;

        [Range(0, 30, ErrorMessage = "Days must be between 0 and 30")]
        public int DurationDays { get; set; } = 0;

        // Helper property to check if duration is valid
        public bool HasValidDuration =>
            DurationDays > 0 || DurationHours > 0 || DurationMinutes > 0 || DurationSeconds > 0;

        // Get total duration in minutes for display
        public int TotalDurationInMinutes =>
            (DurationDays * 24 * 60) + (DurationHours * 60) + DurationMinutes + (DurationSeconds > 0 ? 1 : 0);

        // Get total duration in hours for display
        public decimal TotalDurationInHours =>
            Math.Round((DurationDays * 24m) + DurationHours + (DurationMinutes / 60m) + (DurationSeconds / 3600m), 2);
    }
}