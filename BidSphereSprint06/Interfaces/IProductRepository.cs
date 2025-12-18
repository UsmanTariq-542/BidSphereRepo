using BidSphereProject.Models;

namespace BidSphereProject.Interfaces
{
    public interface IProductRepository
    {
        // Insert 
        Task<int> AddProduct(Product p);  // just signature and no return type needed

        // Read
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetProductsByCategory(string category);

        // Update
        Task UpdateProduct(Product p);

        // Delete
        Task DeleteProduct(int id);
    }
}
