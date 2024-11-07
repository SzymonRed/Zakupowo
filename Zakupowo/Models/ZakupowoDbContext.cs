using System.Data.Entity;

namespace Zakupowo.Models
{
    public class ZakupowoDbContext : DbContext
    {
        public ZakupowoDbContext() : base("name=ZakupowoDbContext")
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<VatRate> VatRates { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductFile> ProductFiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Konfiguracja kluczy złożonych w tabelach, które ich wymagają
            modelBuilder.Entity<Product>()
                .HasKey(p => new { p.ProductId, p.CurrencyId });

            modelBuilder.Entity<ProductImage>()
                .HasRequired(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => new { pi.ProductId, pi.CurrencyId });

            modelBuilder.Entity<ProductFile>()
                .HasRequired(pf => pf.Product)
                .WithMany()
                .HasForeignKey(pf => new { pf.ProductId, pf.CurrencyId });

            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => new { oi.ProductId, oi.CurrencyId });

            modelBuilder.Entity<Promotion>()
                .HasRequired(p => p.Product)
                .WithMany()
                .HasForeignKey(p => new { p.ProductId, p.CurrencyId });

            base.OnModelCreating(modelBuilder);
        }
    }
}