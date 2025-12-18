namespace BidSphereProject.Models
{
    public class Auction
    {
        public int Id { get; set; }
        public int ProductId { get; set; } // Primary key of Product used as foreign key in Auction child class 

        public Product Item { get; set; } // for navigation purpose only, doesnot mean that Auction is composed of Product

        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int BidCount { get; set; }

        public int? WinnerUserId { get; set; }
        public int? RunnerUpUserId { get; set; }

        public string Status { get; set; }
        public List<Bid> Bids { get; set; } = new List<Bid>();
    }
}
