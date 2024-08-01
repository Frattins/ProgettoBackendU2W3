namespace ProgettoBackendU2W3.Models
{
    public class Product
    {
        public int Id { get; set; }


        public string Name { get; set; }

        public string? PhotoUrl { get; set; }


        public decimal Price { get; set; }


        public int? DeliveryTime { get; set; } // tempo in minuti

        public List<ProductIngredient>? ProductIngredients { get; set; } = new List<ProductIngredient>();
    }
}