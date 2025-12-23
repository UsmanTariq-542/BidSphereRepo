using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.Repositories;
using BidSphereProject.Services;
using BidSphereProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidSphereProject.Controllers
{
    public class AuctionController : Controller
    {
        
        private readonly IAuctionRepository _auctionRepo; // so we did dependency injection to use our repositories classes.
        private readonly ICategoryService _categoryService;

        public AuctionController(IAuctionRepository auctionRepo,ICategoryService categoryService)
        {
            _auctionRepo = auctionRepo;
            _categoryService = categoryService;
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
            ViewBag.TotalBidders = await _auctionRepo.GetTotalUniqueBiddersCount(); // ← Changed this
            ViewBag.TotalBidValue = auctions.Sum(a => a.CurrentPrice);
            ViewBag.EndingSoon = auctions.Count(a => a.EndTime != null && a.EndTime < DateTime.Now.AddHours(1));

            // Get category links
            var categoryLinks = await _categoryService.GetCategoryLinks();
            ViewBag.CategoryLinks = categoryLinks;

            return View(auctions);
        }

        public async Task<IActionResult> IndividualCategory(string id)
        {
            // Get auctions for specific category
            var categoryAuctions = await _auctionRepo.GetAuctionsByCategory(id);

            // Get all category links for navigation
            var categoryLinks = await _categoryService.GetCategoryLinks();
            ViewBag.CategoryLinks = categoryLinks;

            ViewBag.CategoryName = id;
            ViewBag.ProductCount = categoryAuctions.Count();

            return View(categoryAuctions);
        }



    }
}
