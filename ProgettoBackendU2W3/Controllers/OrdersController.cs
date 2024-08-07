﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgettoBackendU2W3.Data;
using ProgettoBackendU2W3.Models;
using ProgettoBackendU2W3.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProgettoBackendU2W3.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Products");
            }

            var orderViewModel = new OrderViewModel
            {
                Items = cart.Items.Select(i => new OrderItemViewModel
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };

            return View(orderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Products");
            }

            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddress = model.ShippingAddress,
                    Notes = model.Notes,
                    OrderDate = DateTime.Now,
                    IsCompleted = false,
                    TotalCost = cart.TotalPrice,
                    OrderItems = cart.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Product.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            model.Items = cart.Items.Select(i => new OrderItemViewModel
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                Price = i.Product.Price
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
