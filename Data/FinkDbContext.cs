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

            entity.Property(p => p.Quantity)
                .HasColumnType("float");

            entity.Property(p => p.Unit)
                .HasConversion<int>();
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Stores");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.ChainName)
                .HasConversion<int>();

            entity.Property(s => s.Latitude)
                .HasColumnType("float");

            entity.Property(s => s.Longitude)
                .HasColumnType("float");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.ToTable("Prices");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Value)
                .HasColumnType("decimal(18,2)");

            entity.Property(p => p.PricePerUnit)
                .HasColumnType("decimal(18,4)");

            entity.Property(p => p.Currency)
                .HasConversion<int>();

            entity.Property(p => p.CollectedAt)
                .HasColumnType("datetime2");

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
