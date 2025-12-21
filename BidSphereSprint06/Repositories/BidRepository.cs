using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
namespace BidSphereProject.Repositories
{
    public class BidRepository   : IBidRepository 
    {
        private readonly string _connectionString;

        public BidRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        // insert bid
        public async Task<int> AddBid(Bid bid)
        {
            using (SqlConnection con = new SqlConnection(_connectionString)){
                string sql = @"Insert Into Bid(AuctionId,UserId,Amount,BidTime) 
                               Values(@AuctionId,@UserId,@Amount,@BidTime);
                               SELECT CAST(SCOPE_IDENTITY() AS INT);";
                return await con.QuerySingleAsync<int>(sql, bid);  
            }
        }

        // Read
        public async Task<Bid> GetBidById(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"Select * from Bid where Id=@Id";
                Bid bid=await con.QuerySingleAsync<Bid>(sql,new { Id=id});
                return bid;
            }
        }
        public async Task<IEnumerable<Bid>> GetBidsByAuctionId(int auctionId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"Select * from Bid where AuctionId=@AuctionId";
                IEnumerable<Bid> bids = await con.QueryAsync<Bid>(sql, new { AuctionId = auctionId });
                return bids;
            }
        }

         
        public async Task<IEnumerable<Bid>> GetBidsByUserId(string userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Get bids
                var bids = await con.QueryAsync<Bid>(
                    "SELECT * FROM Bid WHERE UserId = @UserId ORDER BY BidTime DESC",
                    new { UserId = userId }
                );

                var result = new List<Bid>();

                foreach (var bid in bids)
                {
                    // Get auction for each bid
                    var auction = await con.QuerySingleOrDefaultAsync<Auction>(
                        "SELECT * FROM Auction WHERE Id = @Id",
                        new { Id = bid.AuctionId }
                    );

                    if (auction != null)
                    {
                        // Get product for the auction
                        var product = await con.QuerySingleOrDefaultAsync<Product>(
                            "SELECT * FROM Product WHERE Id = @ProductId",
                            new { ProductId = auction.ProductId }
                        );

                        auction.Item = product; // Attach product to auction
                    }

                    bid.AuctionInfo = auction; // Could be null
                    result.Add(bid);
                }

                return result;
            }
        }

        public async Task<Bid> GetHighestBidForAuction(int auctionId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"Select TOP 1 * from Bid where AuctionId=@AuctionId Order by Amount DESC ";
                Bid bid = await con.QuerySingleOrDefaultAsync<Bid>(sql, new { AuctionId = auctionId });
                return bid;
            }
        }
        public async Task<IEnumerable<Bid>> GetTopBidsForAuction(int auctionId, int top = 2)  // top N bids, default 2
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"Select TOP {top} * from Bid where AuctionId=@AuctionId Order by Amount DESC ";
                IEnumerable<Bid> bids = await con.QueryAsync<Bid>(sql, new { AuctionId = auctionId });
                return bids;
            }
        }

        // Update
        public async Task<bool> UpdateBid(Bid bid)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"Update Bid Set Amount=@Amount ,BidTime = @BidTime where Id=@id";
                int rows=await con.ExecuteAsync(sql, bid);
                return rows > 0;
            }
        }

        // Delete
        public async Task<bool> DeleteBid(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"Delete Bid where Id=@Id";
                int rows = await con.ExecuteAsync(sql, new { Id = id });
                return rows > 0;
            }
        }

    }
}
