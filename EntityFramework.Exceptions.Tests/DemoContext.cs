using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Tests
{
    public class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSale> ProductSales { get; set; }

        public DbSet<CompositeKeyItem> CompositeKeyItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasIndex(u => u.Name).IsUnique();
            builder.Entity<Product>().Property(b => b.Name).IsRequired().HasMaxLength(15);
            builder.Entity<ProductSale>().Property(b => b.Price).HasColumnType("decimal(5,2)").IsRequired();
            builder.Entity<CompositeKeyItem>().HasKey(x => new {x.ProductId, x.ProductSaleId});
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductSale
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime SoldAt { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

    public class CompositeKeyItem
    {
        public int ProductId { get; set; }
        public int ProductSaleId { get; set; }
    }
}