using Microsoft.EntityFrameworkCore;
using Service.Services.OpenFoodFactsService;

var builder = WebApplication.CreateBuilder(args);

// Always load user secrets
builder.Configuration.AddUserSecrets<Program>();

// Get connection string with SQLite fallback
var connectionString = builder.Configuration.GetConnectionString("FinkSql") 
    ?? "Data Source=fink.db";

// Register DbContext with SQLite
builder.Services.AddDbContext<FinkDbContext>(options =>
    options.UseSqlite(connectionString));

// API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Custom Services
builder.Services.AddScoped<PriceService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StoreService>();
builder.Services.AddOpenFoodFacts(builder.Configuration);

var app = builder.Build();

// Development middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


// Seed database in development
if (app.Environment.IsDevelopment())
{
    try
    {
        Console.WriteLine("🌱 Seeding development database...");
        await DatabaseSeeder.SeedAsync(app.Services);
        Console.WriteLine("✅ Database seeded successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Seeding failed: {ex.Message}");
        
    }
}

app.Run();