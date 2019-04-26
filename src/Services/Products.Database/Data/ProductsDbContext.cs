using Microsoft.EntityFrameworkCore;
using Model;

namespace Data
{
    public class ProductsDbContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }
    }
}