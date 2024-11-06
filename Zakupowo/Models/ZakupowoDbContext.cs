using System.Data.Entity;

namespace Zakupowo.Models
{
    public class ZakupowoDbContext : DbContext
    {
        public ZakupowoDbContext() : base("name=ZakupowoDbContext")
        {
        }

        public DbSet<User> User { get; set; }
    }
}