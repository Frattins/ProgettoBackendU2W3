using System.Collections.Generic;
using ProgettoBackendU2W3.Models;

namespace ProgettoBackendU2W3.ViewModels
{
    public class CartViewModel
    {
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
    }
}
