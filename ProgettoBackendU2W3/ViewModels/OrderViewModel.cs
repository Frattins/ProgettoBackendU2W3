using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProgettoBackendU2W3.ViewModels
{
    public class OrderViewModel
    {
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

        [Required]
        public string ShippingAddress { get; set; }

        public string Notes { get; set; }

        public decimal TotalCost
        {
            get
            {
                return Items?.Sum(i => i.Price * i.Quantity) ?? 0;
            }
        }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
