using BidSphereProject.Models;
using BidSphereProject.Services;
using BidSphereProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidSphereProject.Controllers
{
    [Authorize(Policy= "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly AuctionService _auctionService;

        public AdminController(AuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            Auction a1 = new Auction { Id = 1, ProductId = 2, StartingPrice = 300, CurrentPrice = 400, 
            StartTime = DateTime.Now.AddDays(-2),EndTime=DateTime.Now.AddDays(1),BidCount=2,WinnerUserId="4",RunnerUpUserId="2",Status="Live"
            };

            List<Product> auctions = new List<Product> // temporary data passed
            {
                new Product{Id=3,Name="PremiumWatch878",Category="Watches",Description="This is watch",CurrentAuction=a1}
            };
            return View(auctions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAuction(CreateAuctionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _auctionService.CreateAuction(model); 

            TempData["SuccessMessage"] = "Auction created successfully!";
            return RedirectToAction("Dashboard");
        }
    }
}
