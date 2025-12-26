using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.Services;
using BidSphereProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly IAuctionRepository _auctionRepo;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AuctionService _auctionService; // For creating auctions
    private readonly IBidRepository _bidRepo;
    public AdminController(IAuctionRepository auctionRepo,UserManager<IdentityUser> userManager,AuctionService auctionService,IBidRepository bidRepo)
    {
        _auctionRepo = auctionRepo;
        _bidRepo=bidRepo;
        _userManager = userManager;
        _auctionService = auctionService;
    }

    public async Task<IActionResult> Dashboard()
    {
        var dashboardVM = new DashboardViewModel();

        try
        {
            // 1. Live Auctions Count - Using GetAllActiveAuctions()
            var activeAuctions = await _auctionRepo.GetAuctionCount();
            dashboardVM.LiveAuctionsCount = activeAuctions;

            // 2. Total Bids Count
            dashboardVM.TotalBids = await _bidRepo.GetTotalBidsCount();

            // 3. Total Registered Users
            dashboardVM.TotalRegisteredUsers = _userManager.Users.Count();

            // 4. Revenue Generated (Completed auctions revenue)
            dashboardVM.RevenueGenerated = await _auctionRepo.CalculateRevenue(0.02m);

            // 5. Recent Bid Activities 
            dashboardVM.RecentBidActivities= new List<BidActivityVM>();
            Console.WriteLine("5. Getting recent bids...");
            IEnumerable<Bid> bids=await _bidRepo.GetLatestBids(5);
            Console.WriteLine($"Found {bids?.Count() ?? 0} recent bids");
            if (bids != null && bids.Any())
            {
                foreach (Bid bid in bids)
                {
                    Console.WriteLine($"Processing bid ID: {bid.Id}");
                    bid.AuctionInfo = await _auctionRepo.GetAuctionById(bid.AuctionId);

                    if (bid.AuctionInfo == null)
                    {
                        Console.WriteLine($"  ERROR: Auction not found for ID: {bid.AuctionId}");
                        continue;
                    }

                    Console.WriteLine($"  Auction found. ProductId: {bid.AuctionInfo.ProductId}");

                    if (bid.AuctionInfo.Item == null)
                    {
                        Console.WriteLine($"  ERROR: Auction.Item is null");
                        continue;
                    }

                    Console.WriteLine($"  Getting user info for UserId: {bid.UserId}");


                    var user = await _userManager.FindByIdAsync(bid.UserId);
                    string email = user?.Email?? "unknown";
                    Console.WriteLine($"  User email: {email}");
                    dashboardVM.RecentBidActivities.Add(
                        new BidActivityVM
                        {
                            AuctionName = bid.AuctionInfo.Item.Name,
                            BidderUsername = email,
                            Action = "Bid Placed",
                            BidAmount = bid.Amount,
                            TimeAgo = GetTimeAgo(bid.BidTime)
                        });
                    Console.WriteLine($"  Bid activity added successfully");
                }
            }
            else
            {
                Console.WriteLine("No recent bids found, using sample data");
                dashboardVM.RecentBidActivities = GetSampleBidActivities();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION CAUGHT: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            // Fallback to temporary data
            dashboardVM = GetTemporaryDashboardData();
        }

        return View(dashboardVM);
    }

    private string GetTimeAgo(DateTime bidTime)
    {
        var now = DateTime.Now;
        var span = now - bidTime;

        if (span.TotalSeconds < 60)
            return "just now";

        if (span.TotalMinutes < 60)
            return $"{(int)span.TotalMinutes} minute(s) ago";

        if (span.TotalHours < 24)
            return $"{(int)span.TotalHours} hour(s) ago";

        if (span.TotalDays < 30)
            return $"{(int)span.TotalDays} day(s) ago";

        if (span.TotalDays < 365)
            return $"{(int)(span.TotalDays / 30)} month(s) ago";

        return $"{(int)(span.TotalDays / 365)} year(s) ago";
    }



    // Temporary data methods
    private DashboardViewModel GetTemporaryDashboardData()
    {
        return new DashboardViewModel
        {
            LiveAuctionsCount = 12,
            TotalBids = 8,
            TotalRegisteredUsers = _userManager.Users.Count(),
            RevenueGenerated = 12500,
            RecentBidActivities = GetSampleBidActivities()
        };
    }

    private List<BidActivityVM> GetSampleBidActivities()
    {
        return new List<BidActivityVM>{
            new BidActivityVM
            {
                AuctionName = "MacBook Pro 2023",
                BidderUsername = "abc@example.com",
                Action = "Bid Placed",
                BidAmount = 1450m,
                TimeAgo = "2 minutes ago"
            },
            new BidActivityVM
            {
                AuctionName = "Rolex Submariner",
                BidderUsername = "alex_wong@example.com",
                Action = "Bid Placed",
                BidAmount = 7200m,
                TimeAgo = "5 minutes ago"
            },
            new BidActivityVM
            {
                AuctionName = "iPhone 14 Pro",
                BidderUsername = "mike_jones@example.com",
                Action = "Bid Placed",
                BidAmount = 950m,
                TimeAgo = "15 minutes ago"
            },
            new BidActivityVM
            {
                AuctionName = "Designer Dress",
                BidderUsername = "emma_roberts@example.com",
                Action = "Bid Placed",
                BidAmount = 220m,
                TimeAgo = "25 minutes ago"
            },
            new BidActivityVM
            {
                AuctionName = "Refrigerator",
                BidderUsername = "sarah_wilson@example.com",
                Action = "Bid Placed",
                BidAmount = 550m,
                TimeAgo = "30 minutes ago"
            }
        };
    }


    // Create Auction View
    public IActionResult CreateAuction()
    {
        return View(new CreateAuctionViewModel());
    }

    [HttpGet]
    public IActionResult CreateAuctionPartial()
    {
        // Return partial view for dashboard
        var model = new CreateAuctionViewModel();
        return PartialView("_CreateAuctionPartial", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAuction(CreateAuctionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new
            {
                success = false,
                message = "Please fix the following errors:",
                errors = errors
            });
        }

        try
        {
            int auctionId = await _auctionService.CreateAuction(model);

            return Json(new
            {
                success = true,
                message = $"Auction created successfully! (ID: {auctionId})",
                auctionId = auctionId,
                redirectUrl = Url.Action("Dashboard", "Admin")
            });
        }
        catch (ArgumentException ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"An error occurred while creating the auction. Please try again later, {ex.Message}"
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLiveAuctions()
    {
        try
        {
            var activeAuctions = await _auctionRepo.GetAllActiveAuctions();

            var liveAuctions = activeAuctions.Where(a => a.EndTime > DateTime.Now)
                .Select(a => new
                {
                    AuctionId = a.Id,
                    ProductName = a.Item?.Name ?? "Unknown",
                    Category = a.Item?.Category ?? "Uncategorized",
                    StartingPrice = a.StartingPrice,
                    CurrentPrice = a.CurrentPrice,
                    EndTime = a.EndTime
                }).ToList();

            return Json(liveAuctions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Json(new List<object>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEndTime([FromBody] SimpleTimeUpdate request)
    {
        try
        {
            var auction = await _auctionRepo.GetAuctionById(request.AuctionId);

            if (auction == null)
                return Json(new { success = false, message = "Auction not found" });

            auction.EndTime = DateTime.Parse(request.NewEndTime);
            await _auctionRepo.UpdateAuction(auction);

            return Json(new { success = true, message = "End time updated" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    public class SimpleTimeUpdate
    {
        public int AuctionId { get; set; }
        public string NewEndTime { get; set; }
    }
}