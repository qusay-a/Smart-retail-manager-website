using Microsoft.EntityFrameworkCore;
using Smart_retail_manager_website.Models;
using System.Collections.Generic;

namespace Smart_retail_manager_website.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Bill> Bills { get; set; }

        // This is the linking table between Bill and Product in the DB
        public DbSet<BillProduct> BillProducts { get; set; }

        public DbSet<UserLogin> UserLogin { get; set; }
    }
}

