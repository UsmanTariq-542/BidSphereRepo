using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.ViewModels;

namespace BidSphereProject.Services
{
    public class AuctionService
    {
        private readonly IProductRepository _productRepo;
        private readonly IAuctionRepository _auctionRepo;

        public AuctionService(IProductRepository productRepo, IAuctionRepository auctionRepo)
        {
            _productRepo = productRepo;
            _auctionRepo = auctionRepo;
        }

        public async Task CreateAuction(CreateAuctionViewModel model)
        {
            // 1️⃣ Create product
            var product = new Product
            {
                Name = model.Name.Trim(),
                Category = model.Category.Trim(),
                Description = model.Description.Trim()
            };
            int productId = await _productRepo.AddProduct(product);

            // 2️⃣ Calculate times
            DateTime start = DateTime.Now;
            DateTime end = model.DurationUnit switch
            {
                "Minutes" => start.AddMinutes(model.DurationValue),
                "Hours" => start.AddHours(model.DurationValue),
                "Days" => start.AddDays(model.DurationValue),
                _ => start.AddMinutes(model.DurationValue)
            };

            // 3️⃣ Create auction
            var auction = new Auction
            {
                ProductId = productId,
                StartingPrice = model.StartingPrice,
                StartTime = start,
                EndTime = end
            };
            await _auctionRepo.AddAuction(auction);
        }

    }
}
