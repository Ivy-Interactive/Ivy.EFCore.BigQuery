using Microsoft.EntityFrameworkCore;
using Ivy.EFCore.BigQuery.Extensions;

namespace Ivy.EFCore.BigQuery.Sample.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DistributionCenter> DistributionCenters { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<InventoryItem> InventoryItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){ }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DistributionCenter>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            
            // Example: Create table with IF NOT EXISTS
            entity.HasBigQueryIfNotExists();
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            
            // Example: Create temporary table
            entity.HasBigQueryTempTable();
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).ValueGeneratedNever();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            
            // Example: Add a search index on user searchable fields
            entity.HasIndex(e => new { e.FirstName, e.LastName, e.Email })
                .HasBigQuerySearchIndex(ifNotExists: true)
                .HasBigQuerySearchIndexOptions("analyzer='LOG_ANALYZER'")
                .HasBigQuerySearchIndexColumnOptions("Email", "index_granularity='GLOBAL'");
        });

        // Example: Add a search index on all string columns for documents
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            
            // Create a search index on all string columns
            entity.HasIndex("SIDX_Inventory_All")
                .HasBigQuerySearchIndexOnAllColumns(ifNotExists: true)
                .HasBigQuerySearchIndexColumnOptions(new Dictionary<string, string>
                {
                    ["ProductName"] = "index_granularity='COLUMN'",
                    ["ProductBrand"] = "index_granularity='GLOBAL'"
                });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
