using BidSphereProject.Interfaces;
using BidSphereProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BidSphereProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        public ProductController(IProductRepository prodRepo) { 
            _productRepo=prodRepo;
        }

        // static data handling for views but later we make it dynamic 
        private readonly List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "MacBook Pro 2023", Category = "Laptops",Description="heavy laptops",CurrentAuction=new Auction{ Status="active" } },
            new Product { Id = 2, Name = "Dell XPS 15", Category = "Laptops",Description="xyz",CurrentAuction=new Auction{ Status="active" }},
            new Product { Id = 3, Name = "HP Spectre x360", Category = "Laptops",Description= "xyz", CurrentAuction = new Auction { Status = "active" }},
            new Product { Id = 4, Name = "Asus ROG Strix", Category = "Laptops",Description= "xyz" , CurrentAuction = new Auction { Status = "active" }},
            new Product { Id = 5, Name = "Rolex Submariner", Category = "Luxury Watches",Description= "xyz",CurrentAuction=new Auction{ Status="active" } },
            new Product { Id = 6, Name = "Designer Handbag", Category = "Fashion Outfit",Description= "xyz",CurrentAuction=new Auction{ Status="active" } },
            new Product { Id = 7, Name = "Samsung Refrigerator", Category = "Home Appliances",Description= "xyz",CurrentAuction=new Auction{ Status="active" } },
            new Product { Id = 8, Name = "Chanel Perfume", Category = "Perfumes", Description= "xyz",CurrentAuction=new Auction{ Status="active" } },
            new Product { Id = 9, Name = "Tesla Model 3", Category = "Automobile", Description= "xyz",CurrentAuction=new Auction{ Status="active" } }
        };

        private readonly List<string> _validCategories = new List<string>
        {
            "Fashion Outfit", "Home Appliances", "Laptops",
            "Perfumes", "Luxury Watches", "Automobile"
        };

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Category(string id)
        {
            // Filter products by category and active status
            var categoryProducts = _products
                .Where(p => p.Category == id && p.CurrentAuction?.Status == "active")
                .OrderBy(p => p.CurrentAuction.EndTime)
                .ToList();

            ViewBag.CategoryName = id;
            ViewBag.LiveProductsCount = categoryProducts.Count;
            ViewBag.TotalBidders = categoryProducts.Sum(p => p.CurrentAuction.BidCount);
            ViewBag.TotalBidValue = categoryProducts.Sum(p => p.CurrentAuction.CurrentPrice - p.CurrentAuction.StartingPrice);
            ViewBag.EndingSoon = categoryProducts.Count(p => (p.CurrentAuction.EndTime - System.DateTime.Now).TotalMinutes < 30);

            return View(categoryProducts);
        }

        public IActionResult AllProducts()
        {
            var allProducts = _products
                .Where(p => p.CurrentAuction.Status == "active")
                .OrderBy(p => p.CurrentAuction.EndTime)
                .ToList();

            ViewBag.CategoryName = "All Categories";
            ViewBag.LiveProductsCount = allProducts.Count;
            ViewBag.TotalBidders = allProducts.Sum(p => p.CurrentAuction.BidCount);
            ViewBag.TotalBidValue = allProducts.Sum(p => p.CurrentAuction.CurrentPrice - p.CurrentAuction.StartingPrice);
            ViewBag.EndingSoon = allProducts.Count(p => (p.CurrentAuction.EndTime - System.DateTime.Now).TotalMinutes < 30);

            return View("Category", allProducts);
        }

    }
}
