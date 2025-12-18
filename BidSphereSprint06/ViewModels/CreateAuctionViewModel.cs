using System.ComponentModel.DataAnnotations;

namespace BidSphereProject.ViewModels
{
    public class CreateAuctionViewModel
    {
            // Product info
            public string Name { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }

            // Auction info

            [Range(0, double.MaxValue, ErrorMessage = "Starting price cannot be negative.")]
            public decimal StartingPrice { get; set; }

            // Duration info
            public int DurationValue { get; set; }        // numeric value, e.g., 10
            public string DurationUnit { get; set; }      // "Minutes", "Hours", "Days"
    }
}
