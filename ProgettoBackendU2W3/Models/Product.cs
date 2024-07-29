using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProgettoBackendU2W3.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }
        [Range(0, 100)]
        public decimal Price { get; set; }
        [Required, StringLength(128)]
        public required string Photo { get; set; }
        [Range(0, 60)]
        public int DeliveryTimeInMinutes { get; set; }
        public List<Ingredient> Ingredients { get; set; } = [];
    }
}
