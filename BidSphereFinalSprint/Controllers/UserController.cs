using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BidSphereProject.Controllers
{
    public class UserController : Controller
    {
        private readonly IBidRepository _bidRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuctionRepository _auctionRepo;
        private readonly IProductRepository _productRepo;

        public UserController(IBidRepository bidRepo,UserManager<IdentityUser> userManager,IAuctionRepository auctionRepo,IProductRepository productRepo)
        {
            _userManager = userManager;
            _bidRepo = bidRepo;
            _auctionRepo = auctionRepo;
            _productRepo = productRepo;
        }


        [Authorize]
        public async Task<IActionResult> MyBids()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            try
            {
                // Get user's bids
                var userBids = (await _bidRepo.GetBidsByUserId(user.Id))
                    .OrderByDescending(b => b.BidTime)
                    .ToList();

                // Process each bid to ensure consistency
                foreach (var bid in userBids)
                {
                    if (bid.AuctionInfo == null)
                    {
                        // Get auction for this bid
                        var auction = await _auctionRepo.GetAuctionById(bid.AuctionId);
                        if (auction != null)
                        {
                            // Get product for the auction
                            var product = await _productRepo.GetProductById(auction.ProductId);
                            auction.Item = product;
                            bid.AuctionInfo = auction;

                            // Check and update auction price consistency
                            await EnsureAuctionPriceConsistency(auction.Id, bid.UserId);
                        }
                    }
                    else
                    {
                        // AuctionInfo already exists, still check consistency
                        await EnsureAuctionPriceConsistency(bid.AuctionInfo.Id, bid.UserId);
                    }
                }

                // Re-fetch to get updated auction prices
                userBids = (await _bidRepo.GetBidsByUserId(user.Id))
                    .OrderByDescending(b => b.BidTime)
                    .ToList();

                // Calculate statistics
                ViewBag.TotalBids = userBids.Count;
                ViewBag.ActiveBids = userBids.Count(b =>
                    b.AuctionInfo?.EndTime > DateTime.Now);
                ViewBag.WinningBids = userBids.Count(b =>
                    b.AuctionInfo != null &&
                    b.AuctionInfo.EndTime > DateTime.Now &&
                    b.AuctionInfo.WinnerUserId == user.Id);
                ViewBag.WonBids = userBids.Count(b =>
                    b.AuctionInfo != null &&
                    b.AuctionInfo.EndTime <= DateTime.Now &&
                    b.AuctionInfo.WinnerUserId == user.Id);

                return View(userBids);
            }
            catch (Exception ex)
            {
                return View(new List<Bid>());
            }
        }

        private async Task EnsureAuctionPriceConsistency(int auctionId, string userId)
        {
            // Get the highest bid for this auction
            var highestBid = await _bidRepo.GetHighestBidForAuction(auctionId);

            if (highestBid != null)
            {
                var auction = await _auctionRepo.GetAuctionById(auctionId);

                // If auction's current price doesn't match highest bid, update it
                if (auction != null && auction.CurrentPrice != highestBid.Amount)
                {
                    // Update auction's current price
                    auction.CurrentPrice = highestBid.Amount;

                    // Update winner if needed
                    if (auction.WinnerUserId != highestBid.UserId)
                    {
                        auction.WinnerUserId = highestBid.UserId;
                    }

                    // Save changes to auction
                    await _auctionRepo.UpdateAuction(auction);
                }
            }
        }

        //-------------------------------------
    }
}
