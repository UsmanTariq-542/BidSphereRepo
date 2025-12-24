using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using Dapper;
using Microsoft.Data.SqlClient;
namespace BidSphereProject.Repositories
{
    public class AuctionRepository: IAuctionRepository
    {
        private readonly string _connectionString;
        private readonly IProductRepository _productRepo;

        public AuctionRepository(IConfiguration config,IProductRepository productRepo)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _productRepo = productRepo;
        }


        // insert auction and returns id of newly inserted auction.
        public async Task<int> AddAuction(Auction auction)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Auction (ProductId, StartingPrice, StartTime, EndTime,Status,BidCount,CurrentPrice)
                               VALUES (@ProductId, @StartingPrice, @StartTime, @EndTime,@Status,@BidCount,@CurrentPrice);
                               SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return await con.QuerySingleAsync<int>(sql, auction);
            }
        }

        // Read
        public async Task<Auction> GetAuctionById(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlcmd = "Select * from Auction where Id=@Id";
                Auction auction = await con.QuerySingleOrDefaultAsync<Auction>(sqlcmd, new { Id = id });
                auction.Item = await _productRepo.GetProductById(auction.Id);
                return auction;
            }
        }
        public async Task<Auction> GetAuctionsByProductId(int productId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlcmd = "Select * from Auction where ProductId=@ProductId";
                Auction auction = await con.QuerySingleOrDefaultAsync<Auction>(sqlcmd, new { ProductId = productId });
                return auction;
            }
        }
        public async Task<IEnumerable<Auction>> GetAllAuctions()
        {
            using(SqlConnection con=new SqlConnection(_connectionString))
            {
                string sqlcmd = "Select * from Auction";
                IEnumerable<Auction> auctions=await con.QueryAsync<Auction>(sqlcmd);
                return auctions;
            }
        }

        public async Task<int> GetAuctionCount()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Auction where EndTime>GETDATE()";
                int count = await con.ExecuteScalarAsync<int>(sql);
                return count;
            }
        }

        public async Task<decimal> CalculateRevenue(decimal commissionRate = 0.05m)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"
            SELECT ISNULL(SUM(CurrentPrice * @Rate), 0)
            FROM Auction";

                return await con.ExecuteScalarAsync<decimal>(sql, new
                {
                    Rate = commissionRate
                });
            }
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByCategory(string category)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT a.*, p.*
                       FROM Auction a
                       INNER JOIN Product p ON a.ProductId = p.Id
                       WHERE LOWER(p.Category) = LOWER(@Category)";

                return await con.QueryAsync<Auction, Product, Auction>(   // obj1,obj2,returntype
                    sql,
                    (auction, product) =>
                    {
                        auction.Item = product;   // same Product obj item which we created for referencing 
                        return auction;
                    },
                    new { Category = category }
                );
            }
        }

        public async Task<IEnumerable<Auction>> GetAllActiveAuctions()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"
                                SELECT a.*, p.*
                                FROM Auction a
                                INNER JOIN Product p ON a.ProductId = p.Id
                                WHERE a.StartTime <= GETDATE()
                                  AND a.EndTime > GETDATE()
                                ORDER BY a.StartTime DESC";

                var auctions = await con.QueryAsync<Auction, Product, Auction>(
                    sql,
                    (auction, product) =>
                    {
                        auction.Item = product;
                        return auction;
                    },
                    splitOn: "Id"  // Dapper needs this
                );

                return auctions;
            }
        }


        // Update
        public async Task<bool> UpdateAuction(Auction auction)
        {
            using(SqlConnection con=new SqlConnection(_connectionString))
            {
                string sqlcmd = "Update Auction SET CurrentPrice=@CurrentPrice,EndTime=@EndTime, " +
                    "BidCount=@BidCount,WinnerUserId=@WinnerUserId,RunnerUpUserId=@RunnerUpUserId," +
                    "Status=@Status where Id=@Id";
                int rows = await con.ExecuteAsync(sqlcmd, auction);
                return rows > 0;
            }
        }

        // Delete
        public async Task<bool> DeleteAuction(int id)
        {
            using(SqlConnection con=new SqlConnection(_connectionString))
            {
                string sqlcmd = "Delete Auction where Id=@Id";
                int rows=await con.ExecuteAsync(sqlcmd, new {Id=id});
                return rows > 0;
            }
        }

        public async Task<int> GetTotalUniqueBiddersCount()
        {
            using(SqlConnection con=new SqlConnection(_connectionString))
            {
                var sql = "SELECT COUNT(DISTINCT UserId) FROM Bid";
                return await con.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<bool> IncrementBidCountandbidamount(int auctionId, decimal bidamount)
        {
            try
            {
                var sql = "UPDATE Auction SET BidCount = BidCount + 1 , CurrentPrice=bidamount WHERE Id = @AuctionId";

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    var rowsAffected = await con.ExecuteAsync(sql,new { AuctionId = auctionId });

                    return rowsAffected > 0;
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IncrementBidCount: {ex.Message}");
                return false;
            }
        }
    }
}
