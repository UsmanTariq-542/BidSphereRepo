using BidSphereProject.Models;

namespace BidSphereProject.Interfaces
{
    public interface IBidRepository
    {
        // Create
        Task<int> AddBid(Bid bid);  // returns newly inserted Bid Id

        // Read
        Task<Bid> GetBidById(int id);
        Task<IEnumerable<Bid>> GetBidsByAuctionId(int auctionId);
        Task<IEnumerable<Bid>> GetBidsByUserId(string userId);
        Task<Bid> GetHighestBidForAuction(int auctionId);
        Task<IEnumerable<Bid>> GetTopBidsForAuction(int auctionId, int top = 2);  // top N bids, default 2

        // Update
        Task<bool> UpdateBid(Bid bid);

        // Delete
        Task<bool> DeleteBid(int id);
    }
}
