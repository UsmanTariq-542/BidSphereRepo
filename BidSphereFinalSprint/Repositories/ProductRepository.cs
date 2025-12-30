using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BidSphereProject.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }


        // insert 
        public async Task<int> AddProduct(Product p)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sql = @"
                INSERT INTO Product (Name, Category, Description)  
                VALUES (@Name, @Category, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT);   ";
                // do two things insert record first and returns id of newly inserted record.
                // select Scope_Identity returns id of newly inserted auto id of this product
                return await con.QuerySingleAsync<int>(sql, p);  // auto mapping happens by Name=p.Name,Category=p.Category etc
            }
        }

        // select methods
        public async Task<Product> GetProductById(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlcmd = "Select * from Product where ID=@id";
                Product prod=await con.QueryFirstOrDefaultAsync<Product>(sqlcmd, new {ID=id});
                return prod;
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlcmd = "Select * from Product";
                IEnumerable<Product> products = await con.QueryAsync<Product>(sqlcmd);
                return products;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            using(SqlConnection con= new SqlConnection(_connectionString))
            {
                string sqlcmd = "Select * from Product where LOWER(Category)=LOWER(@category)";
                IEnumerable<Product> products=await con.QueryAsync<Product>(sqlcmd,new {Category=category});
                return products;
            }
        }

        // Update
        public async Task UpdateProduct(Product p)
        {
            using(SqlConnection con=new SqlConnection(_connectionString))
            {
                string sqlcmd = "Update Product SET Name = @Name, Category = @Category, Price = @Price,Description = @Description WHERE Id = @Id";
                await con.ExecuteAsync(sqlcmd, p);  // automatic maps p like we did earlier using anonymous object creation method.
            }
        }

        // Delete
        public async Task DeleteProduct(int id)
        {
            using(SqlConnection con=new SqlConnection(_connectionString))
            {
                string sqlcmd = "Delete from Product where Id=@id";
                await con.ExecuteAsync(sqlcmd,new {Id=id});  // parameterized queries expect an object(possibly anonymous) to be passed in execute statement
            }
        }
    }
}
