using Microsoft.EntityFrameworkCore;
using Smart_retail_manager_website.Models;

namespace Smart_retail_manager_website.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Bill> Bill { get; set; }

        // linking table
        public DbSet<BillProduct> Bill_Products { get; set; }

        public DbSet<UserLogin> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // match your SQL table names
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Bill>().ToTable("Bill");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<BillProduct>().ToTable("Bill_Products");

            // composite PK for Bill_Products
            modelBuilder.Entity<BillProduct>()
                .HasKey(bp => new { bp.BillID, bp.ProductID });


            modelBuilder.Entity<Bill>()
                .Property(b => b.TaxRate)
                .HasColumnType("decimal(5,4)");

            modelBuilder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BillProduct>()
                .Property(bp => bp.Price)
                .HasColumnType("float");
        }

    }
}
