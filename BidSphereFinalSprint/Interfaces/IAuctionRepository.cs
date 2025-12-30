using BidSphereProject.Models;

namespace BidSphereProject.Interfaces
{
    public interface IAuctionRepository
    {

        // Create
        Task<int> AddAuction(Auction auction);  // returns newly inserted Auction Id

        // Read
        Task<Auction> GetAuctionById(int id);
        Task<IEnumerable<Auction>> GetAllAuctions();
        Task<Auction> GetAuctionsByProductId(int productId);

        Task<IEnumerable<Auction>> GetAuctionsByCategory(string categoryName);
        Task<IEnumerable<Auction>> GetAllActiveAuctions(); // filter by Status = 'Active'

        // Update
        Task<bool> UpdateAuction(Auction auction);

        // Delete
        Task<bool> DeleteAuction(int id);

        Task<int> GetTotalUniqueBiddersCount();

        Task<bool> IncrementBidCountandbidamount(int auctionid,decimal bidamount);

        Task<int> GetAuctionCount();

        Task<decimal> CalculateRevenue(decimal commissionRate = 0.05m);
    }
}
