using System.ComponentModel.DataAnnotations;


namespace Service.Dto;

public class CreateProductDto
{

    [StringLength(128)]
    public string Barcode { get; set; } = string.Empty;
    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [StringLength(128)]
    public string Brand { get; set; } = string.Empty;

    [Range(0.0001, double.MaxValue)]
    public double Quantity { get; set; }

    [Required]
    public UnitType Unit { get; set; }

    [Required]
    public CategoryType Category { get; set; }

    [Required]
    public CreatePriceDto InitialPrice { get; set; } = new();
}

public class ProductPrefillDto
{
    public string? Barcode { get; set; }

    public string? Name { get; set; }

    public string? Brand { get; set; }

    public double? Quantity { get; set; }

    public UnitType? Unit { get; set; }

    public CategoryType? Category { get; set; }

    public CreatePriceDto? InitialPrice { get; set; }
}

public class ReadProductDto
{
    public int Id { get; set; }

    // public string Barcode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public UnitType Unit { get; set; }

    public CategoryType Category { get; set; }

    public decimal? PricePerUnit { get; set; }

    public decimal? LatestPrice { get; set; }
}

public class EditProductDto
{
    [Required]
    public int Id { get; set; }

    // [Required]
    // [StringLength(128)]
    // public string Barcode { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Name { get; set; } = string.Empty;

    [StringLength(128)]
    public string Brand { get; set; } = string.Empty;

    [Range(0.0001, double.MaxValue)]
    public double Quantity { get; set; }

    [Required]
    public UnitType Unit { get; set; }

    [Required]
    public CategoryType Category { get; set; }

    public CreatePriceDto? NewPrice { get; set; }
}

public class QuickReadProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public decimal? PricePerUnit { get; set; }
}
