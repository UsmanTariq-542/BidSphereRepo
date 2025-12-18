using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BidSphereProject.Models;
using Microsoft.AspNetCore.Identity;

namespace BidSphereProject.Controllers
{
    public class UserController : Controller
    {
        private readonly List<Bid> _userBids = new List<Bid>
        {
            new Bid {
                Id = 1,
                AuctionId = 1,
                UserId = 1,
                Amount =1409,
                BidTime=DateTime.Now,
            },
            new Bid {
                Id = 2,
                AuctionId = 2,
                UserId = 4,
                Amount =150,
                BidTime=DateTime.Now,
            },
            new Bid {
                Id = 3,
                AuctionId = 1,
                UserId = 7,
                Amount =23,
                BidTime=DateTime.Now,
            },
            new Bid {
                Id = 4,
                AuctionId = 6,
                UserId = 1,
                Amount =1409,
                BidTime=DateTime.Now,
            },
            new Bid {
                Id = 5,
                AuctionId = 9,
                UserId = 1,
                Amount =290,
                BidTime=DateTime.Now,
            },
            new Bid {
                Id = 6,
                AuctionId = 7,
                UserId = 1,
                Amount =1409,
                BidTime=DateTime.Now,
            }
        };


        [Authorize]
        public IActionResult MyBids()
        {
            // In real application, we would filter by current user ID
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userBids = _context.Bids.Where(b => b.UserId == userId).ToList();

            var userBids = _userBids.Where(b => b.AuctionInfo != null).OrderByDescending(b => b.BidTime).ToList();

            // Calculate stats
            ViewBag.TotalBids = userBids.Count;
            ViewBag.WinningBids = userBids.Count(b => b.AuctionInfo.EndTime > DateTime.Now && b.AuctionInfo.WinnerUserId == b.UserId);
            ViewBag.WonBids = userBids.Count(b => b.AuctionInfo.EndTime <= DateTime.Now && b.AuctionInfo.WinnerUserId == b.UserId);
            ViewBag.ActiveBids = userBids.Count(b => b.AuctionInfo.EndTime > DateTime.Now);

            return View(userBids);
        }
    }
}
