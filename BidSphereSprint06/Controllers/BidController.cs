using BidSphereProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidSphereProject.Controllers
{
    public class BidController : Controller
    {
        private readonly IBidRepository _bidRepo;
        private readonly IAuctionRepository _auctionRepo;

        public BidController(IBidRepository bidRepo, IAuctionRepository auctionRepo)
        {
            _bidRepo = bidRepo;
            _auctionRepo = auctionRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PlaceBid(int auctionId, decimal amount)
        {
            return View();
        }

        public IActionResult GetBidsByAuction(int auctionId)
        {
            return View();
        }

        public IActionResult GetBidsByUser(int userId)
        {
            return View();
        }

        public IActionResult GetHighestBid(int auctionId)
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBid(int id)
        {
            return View();
        }


    }
}
