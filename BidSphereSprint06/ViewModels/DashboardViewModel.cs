using BidSphereProject.Models;
using System.Collections.Generic;

namespace BidSphereProject.ViewModels
{
    public class DashboardViewModel
    {
        public int LiveAuctionsCount { get; set; }
        public int TotalBids { get; set; }
        public int TotalRegisteredUsers { get; set; }
        public decimal RevenueGenerated { get; set; }
        public List<BidActivityVM> RecentBidActivities { get; set; }
    }

    public class BidActivityVM
    {
        public string AuctionName { get; set; }
        public string BidderUsername { get; set; }
        public string Action { get; set; }
        public decimal BidAmount { get; set; }
        public string TimeAgo { get; set; }
    }
}