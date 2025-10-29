using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service.Dto;

public class ProductService
{
    private readonly FinkDbContext _dbContext;
    private readonly PriceService _priceService;

    public ProductService(FinkDbContext dbContext, PriceService priceService)
    {
        _dbContext = dbContext;
        _priceService = priceService;
    }

    public async Task<ReadProductDto> CreateProductAsync(CreateProductDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var exists = await _dbContext.Products.AnyAsync(p => p.Barcode == dto.Barcode);
        if (exists)
        {
            throw new InvalidOperationException($"A product with barcode '{dto.Barcode}' already exists.");
        }

        var product = new Product
        {
            Barcode = dto.Barcode,
            Name = dto.Name,
            Brand = dto.Brand,
            Quantity = dto.Quantity,
            Unit = dto.Unit
        };

        if (product.Prices?.Count > 0)
        {
            foreach (var price in product.Prices)
            {
                price.Product = product;
                _priceService.CalculatePricePerUnit(price);
            }
        }

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

        if (!string.Equals(product.Barcode, dto.Barcode, StringComparison.OrdinalIgnoreCase))
        {
            var barcodeExists = await _dbContext.Products
                .AnyAsync(p => p.Barcode == dto.Barcode && p.Id != dto.Id);

            if (barcodeExists)
            {
                throw new InvalidOperationException($"A product with barcode '{dto.Barcode}' already exists.");
            }

            product.Barcode = dto.Barcode;
        }

        product.Name = dto.Name;
        product.Brand = dto.Brand;
        product.Quantity = dto.Quantity;
        product.Unit = dto.Unit;

        if (product.Prices?.Any() == true)
        {
            foreach (var price in product.Prices)
            {
                price.Product = product;
                _priceService.CalculatePricePerUnit(price);
            }
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

    public ShortProductDto MapToShortDto(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        return new ShortProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode
        };
    }

    private static ReadProductDto MapToReadDto(Product product)
    {
        return new ReadProductDto
        {
            Id = product.Id,
            Barcode = product.Barcode,
            Name = product.Name,
            Brand = product.Brand,
            Quantity = product.Quantity,
            Unit = product.Unit
        };
    }
}
