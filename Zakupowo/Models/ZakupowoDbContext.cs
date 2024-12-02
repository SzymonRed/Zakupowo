using System.Data.Entity;

namespace Zakupowo.Models
{
    public class ZakupowoDbContext : DbContext
    {
        public ZakupowoDbContext() : base("name=ZakupowoDbContext")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<User> User { get; set; }
        public DbSet<VatRate> VatRates { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFile> ProductFiles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Konfiguracja kluczy złożonych w tabelach, które ich wymagają
            modelBuilder.Entity<Product>()
                .HasKey(p =>  p.ProductId);

            modelBuilder.Entity<ProductFile>()
                .HasRequired(pf => pf.Product)
                .WithMany()
                .HasForeignKey(pf => new { pf.ProductId });

            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => new { oi.ProductId });

            modelBuilder.Entity<Promotion>()
                .HasRequired(p => p.Product)
                .WithMany()
                .HasForeignKey(p => new { p.ProductId });
            modelBuilder.Entity<CartItem>()
                   .HasRequired(ci => ci.Product)
                   .WithMany(p => p.CartItems)
                   .HasForeignKey(ci => ci.ProductId); // Używamy tylko `ProductId`

            modelBuilder.Entity<CartItem>()
                .HasRequired(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId);


            base.OnModelCreating(modelBuilder);
        }*/
    }
}