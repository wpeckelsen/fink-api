using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinkDbContext>();
        var priceService = scope.ServiceProvider.GetRequiredService<PriceService>();

        await context.Database.MigrateAsync();

        if (!await context.Stores.AnyAsync())
        {
            var stores = new List<Store>
            {
                new Store { ChainName = ChainType.Rema1000, Latitude = 55.6761, Longitude = 12.5683 },
                new Store { ChainName = ChainType.Netto, Latitude = 56.1629, Longitude = 10.2039 },
                new Store { ChainName = ChainType.Føtex, Latitude = 57.0488, Longitude = 9.9217 }
            };

            await context.Stores.AddRangeAsync(stores);
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var defaultStore = await context.Stores.FirstAsync();

            var products = new List<Product>
            {
                new Product
                {
                    Barcode = "0000000000001",
                    Name = "Sample Organic Milk",
                    Brand = "Fink Farms",
                    Quantity = 1.0,
                    Unit = UnitType.Liter,
                    Category = CategoryType.Dairy
                },
                new Product
                {
                    Barcode = "0000000000002",
                    Name = "Arabica Coffee Beans",
                    Brand = "Fink Roasters",
                    Quantity = 0.5,
                    Unit = UnitType.Kilogram,
                    Category = CategoryType.Beverages
                },
                new Product
                {
                    Barcode = "0000000000003",
                    Name = "South American Coffee Beans",
                    Brand = "Fink Roasters",
                    Quantity = 0.5,
                    Unit = UnitType.Kilogram,
                    Category = CategoryType.Beverages
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            var prices = new List<Price>
            {
                CreatePrice(priceService, defaultStore.Id, 18.90m, products[0]),
                CreatePrice(priceService, defaultStore.Id, 89.50m, products[1]),
                CreatePrice(priceService, defaultStore.Id, 94.75m, products[2])
            };

            await context.Prices.AddRangeAsync(prices);
            await context.SaveChangesAsync();
        }
    }

    private static Price CreatePrice(PriceService priceService, int storeId, decimal value, Product product)
    {
        var price = new Price
        {
            Value = value,
            Currency = CurrencyType.DKK,
            StoreId = storeId,
            Product = product
        };

        priceService.CalculatePricePerUnit(price);
        product.Prices.Add(price);
        return price;
    }
}
