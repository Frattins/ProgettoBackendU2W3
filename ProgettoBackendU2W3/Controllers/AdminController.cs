using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgettoBackendU2W3.Data;
using ProgettoBackendU2W3.Models;
using ProgettoBackendU2W3.ViewModels;

namespace ProgettoBackendU2W3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
                .ToList();

            if (products == null)
            {
                return NotFound("Products not found.");
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product model, int[] selectedIngredientIds)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    DeliveryTime = model.DeliveryTime,
                    PhotoUrl = model.PhotoUrl,
                    ProductIngredients = selectedIngredientIds.Select(id => new ProductIngredient { IngredientId = id }).ToList()
                };

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name", selectedIngredientIds);
            return View(model);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name", product.ProductIngredients.Select(pi => pi.IngredientId));
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product model, int[] selectedIngredientIds)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products
                    .Include(p => p.ProductIngredients)
                    .FirstOrDefault(p => p.Id == model.Id);

                if (product == null)
                {
                    return NotFound();
                }

                product.Name = model.Name;
                product.Price = model.Price;
                product.DeliveryTime = model.DeliveryTime;
                product.PhotoUrl = model.PhotoUrl;

                product.ProductIngredients.Clear();
                foreach (var ingredientId in selectedIngredientIds)
                {
                    product.ProductIngredients.Add(new ProductIngredient { ProductId = product.Id, IngredientId = ingredientId });
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name", selectedIngredientIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ManageOrders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkOrderAsCompleted(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsCompleted = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrders));
        }

        [HttpGet]
        public IActionResult DailyStatistics()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetDailyStatistics(DateTime date)
        {
            var completedOrders = _context.Orders
                .Where(o => o.OrderDate.Date == date.Date && o.IsCompleted)
                .ToList();

            var totalOrders = completedOrders.Count;
            var totalRevenue = completedOrders.Sum(o => o.TotalPrice);

            return Json(new { totalOrders, totalRevenue });
        }
    }
}
