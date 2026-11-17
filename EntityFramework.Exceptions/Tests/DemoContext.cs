using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Tests;

public class DemoContext : DbContext
{
    public DemoContext(DbContextOptions options) : base(options) => Options = options;

    public DbContextOptions Options { get; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSale> ProductSales { get; set; }
    public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().HasIndex(u => u.Name).IsUnique();
        builder.Entity<Product>().Property(b => b.Name).IsRequired().HasMaxLength(ProductNameMaxLength);
        builder.Entity<ProductSale>().Property(b => b.Price).HasColumnType("decimal(5,2)").IsRequired();
        builder.Entity<ProductPriceHistory>().Property(b => b.Price).HasColumnType("decimal(5,2)").IsRequired();
        builder.Entity<ProductPriceHistory>().Property(p => p.EffectiveDate).IsRequired();
        builder.Entity<ProductPriceHistory>().HasOne(p => p.Product).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<ProductReview>().HasOne(p => p.Product).WithMany().OnDelete(DeleteBehavior.Restrict);

        // The default index names are IX_Employees_Badge and IX_Employees_BadgeNumber: one is a prefix of the other.
        builder.Entity<Employee>().HasIndex(e => e.Badge).IsUnique();
        builder.Entity<Employee>().HasIndex(e => e.BadgeNumber).IsUnique();

        builder.Entity<Department>().HasAlternateKey(d => d.Code);

        // A TPH hierarchy shares one primary key. The key name deliberately does not contain the table name,
        // so a duplicate key entry per entity type could not be resolved from the table name in the message.
        builder.Entity<Vehicle>().HasKey(v => v.Id).HasName("pk_fleet");
        builder.Entity<Vehicle>().Property(v => v.Id).ValueGeneratedNever();
        builder.Entity<Car>();
        builder.Entity<Truck>();

        // FK_Tickets_Product_Spare contains FK_Tickets_Product. NoAction avoids multiple cascade paths on SQL Server.
        builder.Entity<Ticket>().HasOne<Product>().WithMany().HasForeignKey(t => t.ProductId)
            .HasConstraintName("FK_Tickets_Product").OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Ticket>().HasOne<Product>().WithMany().HasForeignKey(t => t.SpareProductId)
            .HasConstraintName("FK_Tickets_Product_Spare").OnDelete(DeleteBehavior.NoAction);
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

public class ProductReview
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public int Badge { get; set; }
    public int BadgeNumber { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public int Code { get; set; }
}

public abstract class Vehicle
{
    public int Id { get; set; }
}

public class Car : Vehicle
{
    public int Seats { get; set; }
}

public class Truck : Vehicle
{
    public int Axles { get; set; }
}

public class Ticket
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SpareProductId { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Fullname { get; set; }
}
