using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YYBagProgram.Models;

namespace YYBagProgram.Data
{
    public class YYBagProgramContext : DbContext
    {
        public YYBagProgramContext (DbContextOptions<YYBagProgramContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductsColorDetail>().HasKey(p => new { p.strID ,p.strColor, p.ProductStatus });
            modelBuilder.Entity<ProductColor>().HasKey(p => new { p.strID});
            modelBuilder.Entity<Product>().HasKey(p => new { p.strBagsId });
            modelBuilder.Entity<MonthlyHot>().HasKey(p => new { p.Year, p.Month, p.strBagsId });
        }


        public DbSet<Product> Product { get; set; } = default!;

        public DbSet<ProductsColorDetail> ProductsColorDetail { get; set; }

        public DbSet<ProductColor> ProductColor { get; set; }

        public DbSet<Members> Member { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }

        public DbSet<MonthlyHot> MonthlyHots { get; set; }
    }
}
