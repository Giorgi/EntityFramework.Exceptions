using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Tests;

public class DemoContext : DbContext
{
    public DemoContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSale> ProductSales { get; set; }
    public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().HasIndex(u => u.Name).IsUnique();
        builder.Entity<Product>().Property(b => b.Name).IsRequired().HasMaxLength(ProductNameMaxLength);
        builder.Entity<ProductSale>().Property(b => b.Price).HasColumnType("decimal(5,2)").IsRequired();
        builder.Entity<ProductPriceHistory>().Property(b => b.Price).HasColumnType("decimal(5,2)").IsRequired();
        builder.Entity<ProductPriceHistory>().Property(p => p.EffectiveDate).IsRequired();
        builder.Entity<ProductPriceHistory>().HasOne(p => p.Product).WithMany().OnDelete(DeleteBehavior.NoAction);
    }

    public const int ProductNameMaxLength = 25;
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

    public int ProductId { get; set; }
    public Product Product { get; set; }
}

public class ProductPriceHistory
{
    public long Id { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset EffectiveDate { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}