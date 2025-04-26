using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Exceptions.Tests.ConstraintTests
{
    public class SameNameIndexesContext : DbContext
    {
        public SameNameIndexesContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<EFExceptionSchema.Entities.Inventory.Category> InventoryCategories => Set<EFExceptionSchema.Entities.Inventory.Category>();
        public DbSet<EFExceptionSchema.Entities.Incidents.Category> IncidentCategories => Set<EFExceptionSchema.Entities.Incidents.Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Table [Inventory].[Category] and [Incidents].[Category] have both an index names [IX_Category_Name]. This is allowed
            // in SQL Server because indexes are scoped to their associated tables.
            // Both tables are in different schemas.

            modelBuilder.Entity<EFExceptionSchema.Entities.Inventory.Category>(x =>
            {
                x.ToTable("Category", "Inventory");
                x.HasIndex(category => category.Name).IsUnique();
                x.Property(category => category.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<EFExceptionSchema.Entities.Incidents.Category>(x =>
            {
                x.ToTable("Category", "Incidents");
                x.HasIndex(category => category.Name).IsUnique();
                x.Property(category => category.Name).HasMaxLength(100).IsRequired();
            });
        }
    }
}

namespace EFExceptionSchema.Entities.Incidents
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

namespace EFExceptionSchema.Entities.Inventory
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}