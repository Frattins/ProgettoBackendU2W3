using System.Collections.Generic;

namespace ProgettoBackendU2W3.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CheckoutItemViewModel> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
    }

    public class CheckoutItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
