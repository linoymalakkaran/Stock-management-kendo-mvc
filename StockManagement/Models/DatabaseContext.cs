using System.Data.Entity;

namespace StockManagement.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Category> Categories { get; set; }

    }
}