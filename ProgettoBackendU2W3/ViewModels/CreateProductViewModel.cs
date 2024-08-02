using System.ComponentModel.DataAnnotations;

namespace ProgettoBackendU2W3.ViewModels
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int DeliveryTime { get; set; }

        public string PhotoUrl { get; set; }

        public List<int> SelectedIngredientIds { get; set; }
    }
}