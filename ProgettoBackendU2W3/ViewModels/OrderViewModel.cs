using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProgettoBackendU2W3.ViewModels
{
    public class OrderViewModel
    {
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

        [Required]
        [Display(Name = "Indirizzo di spedizione")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Note per la pizzeria")]
        public string Notes { get; set; }

        public decimal TotalCost => CalculateTotalCost();

        private decimal CalculateTotalCost()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Quantity * item.Price;
            }
            return total;
        }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
