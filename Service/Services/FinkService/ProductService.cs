using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.Dto;
using Service.Services.OpenFoodFactsService;

public class ProductService
{
    private readonly FinkDbContext _dbContext;
    private readonly PriceService _priceService;
    private readonly IOpenFoodFactsClient _openFoodFactsClient;

    public ProductService(FinkDbContext dbContext, PriceService priceService, IOpenFoodFactsClient openFoodFactsClient)
    {
        _dbContext = dbContext;
        _priceService = priceService;
        _openFoodFactsClient = openFoodFactsClient ?? throw new ArgumentNullException(nameof(openFoodFactsClient));
    }

    public async Task<ProductPrefillDto?> PrefillProductFromOpenFoodFactsAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new ArgumentException("Barcode is required.", nameof(barcode));
        }

        var exists = await _dbContext.Products.AnyAsync(p => p.Barcode == barcode);
        if (exists)
        {
            throw new InvalidOperationException($"A product with barcode '{barcode}' already exists.");
        }

        var openFoodFactsProduct = await _openFoodFactsClient.GetProductByBarcodeAsync(barcode);
        if (openFoodFactsProduct == null)
        {
            return null;
        }

        return new ProductPrefillDto
        {
            Barcode = string.IsNullOrWhiteSpace(openFoodFactsProduct.Barcode) ? barcode : openFoodFactsProduct.Barcode,
            Name = string.IsNullOrWhiteSpace(openFoodFactsProduct.Name) ? null : openFoodFactsProduct.Name,
            Brand = string.IsNullOrWhiteSpace(openFoodFactsProduct.Brand) ? null : openFoodFactsProduct.Brand,
            Quantity = openFoodFactsProduct.Quantity,
            Unit = openFoodFactsProduct.Unit,
            Category = openFoodFactsProduct.Category,
            InitialPrice = null
        };
    }

    public async Task<ReadProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        if (createProductDto == null)
        {
            throw new ArgumentNullException(nameof(createProductDto));
        }

        if (createProductDto.InitialPrice == null)
        {
            throw new ArgumentException("Initial price is required.", nameof(createProductDto.InitialPrice));
        }

        var exists = await _dbContext.Products.AnyAsync(p => p.Barcode == createProductDto.Barcode);
        if (exists)
        {
            throw new InvalidOperationException($"A product with barcode '{createProductDto.Barcode}' already exists.");
        }

        var product = new Product
        {
            Barcode = createProductDto.Barcode,
            Name = createProductDto.Name,
            Brand = createProductDto.Brand,
            Quantity = createProductDto.Quantity,
            Unit = createProductDto.Unit,
            Category = createProductDto.Category
        };

        var price = new Price
        {
            Value = createProductDto.InitialPrice.Value,
            Currency = createProductDto.InitialPrice.Currency,
            StoreId = createProductDto.InitialPrice.StoreId,
            Product = product
        };

        _priceService.CalculatePricePerUnit(price);
        product.Prices.Add(price);

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        return MapToReadDto(product);
    }

    public async Task<ReadProductDto?> GetProductByBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new ArgumentException("Barcode is required.", nameof(barcode));
        }

        var product = await _dbContext.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Barcode == barcode);

        return product == null ? null : MapToReadDto(product);
    }

// will be removed on production
    public async Task<IEnumerable<QuickReadProductDto>> GetAllQuickReadProductsAsync()
    {
        var products = await _dbContext.Products
            .Include(p => p.Prices)
            .AsNoTracking()
            .ToListAsync();

        return products.Select(MapToQuickReadDto).ToList();
    }

    public async Task<IReadOnlyList<QuickReadProductDto>> SearchProductsAsync(string term, int take = 20)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            throw new ArgumentException("Search term is required.", nameof(term));
        }

        if (take <= 0)
        {
            take = 20;
        }

        var pattern = $"%{term.Trim()}%";

        var products = await _dbContext.Products
            .Include(p => p.Prices)
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Name, pattern))
            .OrderBy(p => p.Name)
            .Take(take)
            .ToListAsync();

        return products.Select(MapToQuickReadDto).ToList();
    }

    public async Task<ReadProductDto> UpdateProductAsync(EditProductDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var product = await _dbContext.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id '{dto.Id}' was not found.");
        }

        product.Name = dto.Name;
        product.Brand = dto.Brand;
        product.Quantity = dto.Quantity;
        product.Unit = dto.Unit;
        product.Category = dto.Category;

        if (product.Prices?.Any() == true)
        {
            foreach (var price in product.Prices)
            {
                price.Product = product;
                _priceService.CalculatePricePerUnit(price);
            }
        }

        if (dto.NewPrice != null)
        {
            var newPrice = new Price
            {
                Value = dto.NewPrice.Value,
                Currency = dto.NewPrice.Currency,
                StoreId = dto.NewPrice.StoreId,
                Product = product
            };

            _priceService.CalculatePricePerUnit(newPrice);
            product.Prices ??= new List<Price>();
            product.Prices.Add(newPrice);
            await _dbContext.Prices.AddAsync(newPrice);
        }

        await _dbContext.SaveChangesAsync();

        return MapToReadDto(product);
    }

    public async Task DeleteProductAsync(int productId)
    {
        var product = await _dbContext.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return;
        }

        _dbContext.Prices.RemoveRange(product.Prices);
        _dbContext.Products.Remove(product);

        await _dbContext.SaveChangesAsync();
    }

    public QuickReadProductDto MapToQuickReadDto(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var latestPrice = product.Prices?
            .OrderByDescending(p => p.CollectedAt)
            .FirstOrDefault();

        return new QuickReadProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            PricePerUnit = latestPrice?.PricePerUnit
        };
    }

    private static ReadProductDto MapToReadDto(Product product)
    {
        var latestPrice = product.Prices?
            .OrderByDescending(p => p.CollectedAt)
            .FirstOrDefault();

        return new ReadProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            Quantity = product.Quantity,
            Unit = product.Unit,
            Category = product.Category,
            PricePerUnit = latestPrice?.PricePerUnit,
            LatestPrice = latestPrice?.Value
        };
    }
}
