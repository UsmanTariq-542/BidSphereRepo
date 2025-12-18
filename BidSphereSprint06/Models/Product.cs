namespace BidSphereProject.Models
{
    public class Product
    {
        
        // updated properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Auction CurrentAuction { get; set; }  // Product is composed of auction like auction cant exist without product, so a strong relationship exist here.
    }
}
