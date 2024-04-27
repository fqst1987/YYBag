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
            modelBuilder.Entity<CarouselSetting>().HasKey(p => new { p.strBagsId });
            modelBuilder.Entity<Classification>().HasKey(p => new { p.Id });
            modelBuilder.Entity<ClassificationDetail>().HasKey(p => new { p.Id, p.strBagsId });
            modelBuilder.Entity<Members>().HasKey(p => new { p.strMemberEmail });
            modelBuilder.Entity<MemberRole>().HasKey(p => new { p.MemberId });
        }


        public DbSet<Product> Product { get; set; } = default!;

        public DbSet<ProductsColorDetail> ProductsColorDetail { get; set; }

        public DbSet<ProductColor> ProductColor { get; set; }

        public DbSet<Members> Member { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }

        public DbSet<CarouselSetting> CarouselSetting { get; set; }

        public DbSet<Classification> Classification { get; set; }

        public DbSet<ClassificationDetail> ClassificationDetail { get; set; }

        public DbSet<MemberRole> MemberRole { get; set; }
    }
}
