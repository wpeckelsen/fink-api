namespace Service.Dtos.OpenFoodFactsDtos;


// simple Dto to pass the essential data to a frontend
public class OpenFoodFactsDto
{
    public string Barcode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public double? Quantity { get; set; }

    public UnitType? Unit { get; set; }

    public CategoryType? Category { get; set; }
}
