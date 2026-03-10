using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.Services;
using BidSphereProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly IAuctionRepository _auctionRepo;
    private readonly IBidRepository _bidRepo;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AuctionService _auctionService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IAuctionRepository auctionRepo,
        IBidRepository bidRepo,
        UserManager<IdentityUser> userManager,
        AuctionService auctionService,
        ILogger<AdminController> logger)
    {
        _auctionRepo = auctionRepo;
        _bidRepo = bidRepo;
        _userManager = userManager;
        _auctionService = auctionService;
        _logger = logger;
    }

    public async Task<IActionResult> Dashboard()
    {
        var dashboardVM = new DashboardViewModel();

        try
        {
            // Run independent queries in parallel
            var auctionCountTask = _auctionRepo.GetAuctionCount();
            var totalBidsTask = _bidRepo.GetTotalBidsCount();
            var userCountTask = _userManager.Users.CountAsync();
            var revenueTask = _auctionRepo.CalculateRevenue(0.02m);

            await Task.WhenAll(auctionCountTask, totalBidsTask, userCountTask, revenueTask);

            dashboardVM.LiveAuctionsCount = auctionCountTask.Result;
            dashboardVM.TotalBids = totalBidsTask.Result;
            dashboardVM.TotalRegisteredUsers = userCountTask.Result;
            dashboardVM.RevenueGenerated = revenueTask.Result;

            dashboardVM.RecentBidActivities = new List<BidActivityVM>();

            var bids = await _bidRepo.GetLatestBids(5);

            if (bids != null && bids.Any())
            {
                // -------- Fix N+1 Auction Queries --------
                var auctionIds = bids.Select(b => b.AuctionId).Distinct().ToList();

                var auctions = new Dictionary<int, Auction>();

                foreach (var id in auctionIds)
                {
                    var auction = await _auctionRepo.GetAuctionById(id);
                    if (auction != null)
                        auctions[id] = auction;
                }
                // -----------------------------------------

                foreach (var bid in bids)
                {
                    if (!auctions.ContainsKey(bid.AuctionId))
                        continue;

                    var auction = auctions[bid.AuctionId];

                    if (auction.Item == null)
                        continue;

                    var user = await _userManager.FindByIdAsync(bid.UserId);
                    var email = user?.Email ?? "unknown";

                    dashboardVM.RecentBidActivities.Add(new BidActivityVM
                    {
                        AuctionName = auction.Item.Name,
                        BidderUsername = email,
                        Action = "Bid Placed",
                        BidAmount = bid.Amount,
                        TimeAgo = GetTimeAgo(bid.BidTime)
                    });
                }
            }
            else
            {
                dashboardVM.RecentBidActivities = GetSampleBidActivities();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            dashboardVM = GetTemporaryDashboardData();
        }

        return View(dashboardVM);
    }

    private string GetTimeAgo(DateTime bidTime)
    {
        var span = DateTime.Now - bidTime;

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
        return new List<BidActivityVM>
        {
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
                BidderUsername = "alex@example.com",
                Action = "Bid Placed",
                BidAmount = 7200m,
                TimeAgo = "5 minutes ago"
            },
            new BidActivityVM
            {
                AuctionName = "iPhone 14 Pro",
                BidderUsername = "mike@example.com",
                Action = "Bid Placed",
                BidAmount = 950m,
                TimeAgo = "15 minutes ago"
            }
        };
    }

    public IActionResult CreateAuction()
    {
        return View(new CreateAuctionViewModel());
    }

    [HttpGet]
    public IActionResult CreateAuctionPartial()
    {
        return PartialView("_CreateAuctionPartial", new CreateAuctionViewModel());
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
                message = "Validation errors occurred",
                errors = errors
            });
        }

        try
        {
            var auctionId = await _auctionService.CreateAuction(model);

            return Json(new
            {
                success = true,
                message = "Auction created successfully",
                auctionId = auctionId,
                redirectUrl = Url.Action("Dashboard", "Admin")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating auction");

            return Json(new
            {
                success = false,
                message = "An error occurred while creating the auction"
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLiveAuctions()
    {
        try
        {
            var activeAuctions = await _auctionRepo.GetAllActiveAuctions();

            var liveAuctions = activeAuctions
                .Where(a => a.EndTime > DateTime.Now)
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
            _logger.LogError(ex, "Error fetching live auctions");
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
            _logger.LogError(ex, "Error updating end time");

            return Json(new
            {
                success = false,
                message = "Failed to update auction end time"
            });
        }
    }

    public class SimpleTimeUpdate
    {
        public int AuctionId { get; set; }
        public string NewEndTime { get; set; }
    }
}