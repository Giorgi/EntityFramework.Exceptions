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
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasIndex(u => u.Name).IsUnique();
            builder.Entity<Product>().Property(b => b.Name).IsRequired().HasMaxLength(15);
            builder.Entity<ProductSale>().Property(b => b.Price).HasColumnType("decimal(5,2)").IsRequired();
            builder.Entity<Book>().Property(b => b.Name).IsRequired().HasMaxLength(200);
            builder.Entity<Author>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class Book
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long AuthorId { get; set; }
        public Author Author { get; set; }
    }

    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
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
}