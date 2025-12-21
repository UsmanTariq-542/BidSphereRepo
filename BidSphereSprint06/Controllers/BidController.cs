using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidSphereProject.Controllers
{
    public class BidController : Controller
    {
        private readonly IBidRepository _bidRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public BidController(IBidRepository bidRepo, IAuctionRepository auctionRepo, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _bidRepo = bidRepo;
            _auctionRepo = auctionRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceBid(int auctionId, decimal bidAmount)
        {
            //return Ok("CONTROLLER_HIT");
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id; // string by default
            var bid = new Bid
            {
                AuctionId = auctionId,
                UserId = userId,
                Amount = bidAmount,
                BidTime = DateTime.UtcNow
            };
            int bidId = await _bidRepo.AddBid(bid);

            return Ok($"Your bid is placed successfully with ID: {bidId}");
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
