using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidSphereProject.Controllers
{
    public class AuctionController : Controller
    {
        
        private readonly IAuctionRepository _auctionRepo; // so we did dependency injection to use our repositories classes.

        public AuctionController(IAuctionRepository auctionRepo)
        {
            _auctionRepo = auctionRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AllAuctions()
        {
            IEnumerable<Auction> auctions= await _auctionRepo.GetAllActiveAuctions();
            // for viewbag : 
            ViewBag.CategoryName = "All Categories";
            ViewBag.LiveProductsCount = auctions.Count();
            ViewBag.TotalBidders = auctions.Sum(a => a.BidCount);
            ViewBag.TotalBidValue = auctions.Sum(a => a.CurrentPrice);
            ViewBag.EndingSoon = auctions.Count(a => a.EndTime != null && a.EndTime < DateTime.Now.AddHours(1));

            return View(auctions);
        }

        public async Task<IActionResult> IndividualCategory(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("AllAuctions");
            IEnumerable<Auction> auctions=await _auctionRepo.GetAuctionsByCategory(id);
            return View(auctions);
        }



    }
}
