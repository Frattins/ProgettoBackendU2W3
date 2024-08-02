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
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(fileStream);
                    }
                }

                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    DeliveryTime = model.DeliveryTime,
                    PhotoUrl = uniqueFileName != null ? "/images/" + uniqueFileName : null,
                    ProductIngredients = model.SelectedIngredientIds.Select(id => new ProductIngredient { IngredientId = id }).ToList()
                };

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name", model.SelectedIngredientIds);
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name", product.ProductIngredients.Select(pi => pi.IngredientId));
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product model, int[] selectedIngredientIds, IFormFile newPhoto)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products
                        .Include(p => p.ProductIngredients)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    // Gestione della nuova foto
                    if (newPhoto != null)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + newPhoto.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await newPhoto.CopyToAsync(fileStream);
                        }
                        product.PhotoUrl = "/images/" + uniqueFileName;
                    }

                    // Aggiorna le altre proprietà
                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.DeliveryTime = model.DeliveryTime;

                    // Aggiorna gli ingredienti
                    product.ProductIngredients.Clear();
                    foreach (var ingredientId in selectedIngredientIds)
                    {
                        product.ProductIngredients.Add(new ProductIngredient { ProductId = product.Id, IngredientId = ingredientId });
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ingredients"] = new MultiSelectList(_context.Ingredients, "Id", "Name", selectedIngredientIds);
            return View(model);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
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
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.IsCompleted = true;
            order.TotalCost = order.TotalPrice; 

            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrders));
        }

        [HttpGet]
        public IActionResult DailyStats()
        {
            var completedOrders = _context.Orders
                .Where(o => o.IsCompleted)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();

            var viewModel = new DailyStatsViewModel
            {
                TotalOrders = completedOrders.Count,
                TotalRevenue = completedOrders.Sum(o => o.TotalCost)
            };

            return View(viewModel);
        }
    }
}