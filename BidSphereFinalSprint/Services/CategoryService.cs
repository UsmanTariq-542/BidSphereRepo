using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.ViewModels;

namespace BidSphereProject.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryLinkViewModel>> GetCategoryLinks();
    }

    public class CategoryService : ICategoryService
    {
        private readonly IAuctionRepository _auctionRepository;

        public CategoryService(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<List<CategoryLinkViewModel>> GetCategoryLinks()
        {
            var allAuctions = await _auctionRepository.GetAllAuctions();
            if (allAuctions == null)
                allAuctions = new List<Auction>();

            var categories = new List<CategoryLinkViewModel>
            {
                new CategoryLinkViewModel { CategoryName = "Laptops", DisplayName = "Laptops", IconClass = "fas fa-laptop", ColorClass = "text-primary" },
                new CategoryLinkViewModel { CategoryName = "Fashion Outfit", DisplayName = "Fashion", IconClass = "fas fa-tshirt", ColorClass = "text-success" },
                new CategoryLinkViewModel { CategoryName = "Home Appliances", DisplayName = "Appliances", IconClass = "fas fa-tv", ColorClass = "text-info" },
                new CategoryLinkViewModel { CategoryName = "Perfumes", DisplayName = "Perfumes", IconClass = "fas fa-spray-can", ColorClass = "text-warning" },
                new CategoryLinkViewModel { CategoryName = "Luxury Watches", DisplayName = "Watches", IconClass = "fas fa-clock", ColorClass = "text-danger" },
                new CategoryLinkViewModel { CategoryName = "Automobiles", DisplayName = "Automobile", IconClass = "fas fa-car", ColorClass = "text-secondary" },
                new CategoryLinkViewModel { CategoryName = "Others", DisplayName = "Others", IconClass = "fas fa-box", ColorClass = "text-muted" }
            };

            return categories;
        }
    }
}