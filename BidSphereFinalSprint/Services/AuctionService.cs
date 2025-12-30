// Services/AuctionService.cs
using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using BidSphereProject.ViewModels;

namespace BidSphereProject.Services
{
    public class AuctionService
    {
        private readonly IProductRepository _productRepo;
        private readonly IAuctionRepository _auctionRepo;
        private readonly ILogger<AuctionService> _logger;

        public AuctionService(
            IProductRepository productRepo,
            IAuctionRepository auctionRepo,
            ILogger<AuctionService> logger = null)
        {
            _productRepo = productRepo;
            _auctionRepo = auctionRepo;
            _logger = logger;
        }

        public async Task<int> CreateAuction(CreateAuctionViewModel model)
        {
            // Validate duration
            ValidateDuration(model);

            // 1️⃣ Create product
            var product = new Product
            {
                Name = model.Name.Trim(),
                Category = model.Category.Trim(),
                Description = model.Description?.Trim() ?? string.Empty
            };

            int productId = await _productRepo.AddProduct(product);

            // 2️⃣ Calculate end time using 4 duration fields
            DateTime start = DateTime.Now;
            DateTime end = CalculateEndTime(start, model);

            // 3️⃣ Create auction
            var auction = new Auction
            {
                ProductId = productId,
                StartingPrice = model.StartingPrice,
                CurrentPrice = model.StartingPrice, // Start with starting price
                StartTime = start,
                EndTime = end,
                Status = "Active",
                BidCount = 0
            };

            int auctionId = await _auctionRepo.AddAuction(auction);

            _logger?.LogInformation($"Auction created: ID={auctionId}, Product={model.Name}, Duration={GetDurationString(model)}");

            return auctionId;
        }

        private DateTime CalculateEndTime(DateTime start, CreateAuctionViewModel model)
        {
            return start
                .AddDays(model.DurationDays)
                .AddHours(model.DurationHours)
                .AddMinutes(model.DurationMinutes)
                .AddSeconds(model.DurationSeconds);
        }

        private void ValidateDuration(CreateAuctionViewModel model)
        {
            // Check if any duration is set
            if (!model.HasValidDuration)
            {
                throw new ArgumentException("Auction duration must be at least 1 minute");
            }

            // Calculate total minutes
            var totalMinutes = model.TotalDurationInMinutes;

            // Minimum validation (at least 1 minute)
            if (totalMinutes < 1)
            {
                throw new ArgumentException("Auction duration must be at least 1 minute");
            }

            // Maximum validation (30 days = 43,200 minutes)
            if (totalMinutes > 43200) // 30 days * 24 hours * 60 minutes
            {
                throw new ArgumentException("Auction cannot exceed 30 days (1 month)");
            }

            // Additional validation: if days > 0, hours must be 0-23
            if (model.DurationDays > 0 && model.DurationHours >= 24)
            {
                throw new ArgumentException("Hours must be between 0 and 23 when days are specified");
            }
        }

        private string GetDurationString(CreateAuctionViewModel model)
        {
            var parts = new List<string>();

            if (model.DurationDays > 0) parts.Add($"{model.DurationDays} day{(model.DurationDays > 1 ? "s" : "")}");
            if (model.DurationHours > 0) parts.Add($"{model.DurationHours} hour{(model.DurationHours > 1 ? "s" : "")}");
            if (model.DurationMinutes > 0) parts.Add($"{model.DurationMinutes} minute{(model.DurationMinutes > 1 ? "s" : "")}");
            if (model.DurationSeconds > 0) parts.Add($"{model.DurationSeconds} second{(model.DurationSeconds > 1 ? "s" : "")}");

            return string.Join(", ", parts);
        }

    }
}