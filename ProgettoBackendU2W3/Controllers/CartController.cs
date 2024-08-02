using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgettoBackendU2W3.Data;
using ProgettoBackendU2W3.Models;
using ProgettoBackendU2W3.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProgettoBackendU2W3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, string returnUrl = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Prodotto non trovato");
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem { ProductId = productId, Quantity = 1, Price = product.Price };
                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            try
            {
                await _context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Products");
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Index", "Products", new { error = "Errore durante l'aggiunta del prodotto al carrello" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (cartItem != null)
                {
                    cart.Items.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
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

            return View(cart);  // Passa direttamente il carrello alla vista
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string shippingAddress, string notes)
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

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                Notes = notes,
                OrderDate = DateTime.Now,
                IsCompleted = false,
                TotalCost = cart.Items.Sum(i => i.Quantity * i.Product.Price),
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