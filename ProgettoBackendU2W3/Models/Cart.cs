using ProgettoBackendU2W3.Models;

namespace ProgettoBackendU2W3.Models
{

    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal TotalPrice => Items.Sum(i => i.Product.Price * i.Quantity);
    }
}