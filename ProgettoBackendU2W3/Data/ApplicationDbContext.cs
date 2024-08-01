using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgettoBackendU2W3.Models;

namespace ProgettoBackendU2W3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductIngredient>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);

            // Configurazione del tipo di colonna per il prezzo
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Aggiunta di ingredienti predefiniti
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Mozzarella di Bufala" },
                new Ingredient { Id = 2, Name = "Pomodorini" },
                new Ingredient { Id = 3, Name = "Rucola" },
                new Ingredient { Id = 4, Name = "Olio al tartufo" },
                new Ingredient { Id = 5, Name = "Grana Padano" },
                new Ingredient { Id = 6, Name = "Pesto" },
                new Ingredient { Id = 7, Name = "Salame Piccante" },
                new Ingredient { Id = 8, Name = "Bresaola" },
                new Ingredient { Id = 9, Name = "Gorgonzola" },
                new Ingredient { Id = 10, Name = "Prosciutto Cotto" },
                new Ingredient { Id = 11, Name = "Zucchine" }
            );

            // Aggiunta di prodotti predefiniti
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Pizza Margherita DOP",
                    Price = 5.00M,
                    DeliveryTime = 20
                },
                new Product
                {
                    Id = 2,
                    Name = "Pizza Diavola",
                    Price = 7.50M,
                    DeliveryTime = 25
                },
                new Product
                {
                    Id = 3,
                    Name = "Pizza Boscaiola",
                    Price = 7.50M,
                    DeliveryTime = 25
                }
            );

            // Associazione degli ingredienti ai prodotti
            modelBuilder.Entity<ProductIngredient>().HasData(
                new ProductIngredient { ProductId = 1, IngredientId = 1 }, // Mozzarella di Bufala
                new ProductIngredient { ProductId = 1, IngredientId = 2 }, // Pomodorini
                new ProductIngredient { ProductId = 2, IngredientId = 1 }, // Mozzarella di Bufala
                new ProductIngredient { ProductId = 2, IngredientId = 7 }, // Salame Piccante
                new ProductIngredient { ProductId = 3, IngredientId = 1 }, // Mozzarella di Bufala
                new ProductIngredient { ProductId = 3, IngredientId = 10 }, // Prosciutto Cotto
                new ProductIngredient { ProductId = 3, IngredientId = 11 } // Zucchine
            );
        }
    }
}
