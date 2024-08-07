﻿using System.ComponentModel.DataAnnotations;

namespace ProgettoBackendU2W3.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public List<ProductIngredient>? ProductIngredients { get; set; }
    }
}