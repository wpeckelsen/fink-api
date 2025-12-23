using Microsoft.EntityFrameworkCore;

public class FinkDbContext : DbContext
{
    public FinkDbContext(DbContextOptions<FinkDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Price> Prices => Set<Price>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Barcode)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(p => p.Brand)
                .HasMaxLength(128);

            // ===== CHANGED FOR SQLITE =====
            // SQLite uses REAL for floating point numbers
            entity.Property(p => p.Quantity)
                .HasColumnType("REAL");  // Changed from "float"
            // ==============================

            entity.Property(p => p.Unit)
                .HasConversion<int>();
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Stores");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.ChainName)
                .HasConversion<int>();

            // ===== CHANGED FOR SQLITE =====
            entity.Property(s => s.Latitude)
                .HasColumnType("REAL");  // Changed from "float"

            entity.Property(s => s.Longitude)
                .HasColumnType("REAL");  // Changed from "float"
            // ==============================
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.ToTable("Prices");
            entity.HasKey(p => p.Id);

            // ===== CHANGED FOR SQLITE =====
            // SQLite doesn't have decimal type - store as TEXT
            entity.Property(p => p.Value)
                .HasConversion(
                    v => v.ToString("0.00"),          // Convert decimal to string
                    v => decimal.Parse(v))            // Convert string back to decimal
                .HasColumnType("TEXT");                   // Changed from "decimal(18,2)"

            entity.Property(p => p.PricePerUnit)
                .HasConversion(
                    v => v.ToString("0.0000"),        // More precision for unit price
                    v => decimal.Parse(v))
                .HasColumnType("TEXT");                   // Changed from "decimal(18,4)"
            // ==============================

            entity.Property(p => p.Currency)
                .HasConversion<int>();

            // ===== CHANGED FOR SQLITE =====
            // SQLite stores dates as TEXT in ISO8601 format
            entity.Property(p => p.CollectedAt)
                .HasConversion(
                    v => v.ToString("yyyy-MM-dd HH:mm:ss"),  // Convert to string
                    v => DateTime.Parse(v))                   // Convert back
                .HasColumnType("TEXT");                           // Changed from "datetime2"
            // ==============================

            entity.HasOne(p => p.Product)
                .WithMany(p => p.Prices)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Store)
                .WithMany()
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}