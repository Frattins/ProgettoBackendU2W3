﻿using System.ComponentModel.DataAnnotations;
namespace ProgettoBackendU2W3.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsCompleted { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalPrice
        {
            get
            {
                return OrderItems?.Sum(oi => oi.Product.Price * oi.Quantity) ?? 0;
            }
        }
    }
}
