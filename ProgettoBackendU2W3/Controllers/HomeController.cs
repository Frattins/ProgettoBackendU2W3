using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgettoBackendU2W3.Models;
using System.Diagnostics;
using ProgettoBackendU2W3.Data;

namespace ProgettoBackendU2W3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var products = _context.Products
                    .Include(p => p.ProductIngredients)
                        .ThenInclude(pi => pi.Ingredient)
                    .ToList();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei prodotti");
                return View(new List<Product>()); // Ritorna una lista vuota in caso di errore
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}