using System.ComponentModel.DataAnnotations;

public class CreatePriceDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Value { get; set; }

    [Required]
    public CurrencyType Currency { get; set; }

    [Required]
    public int StoreId { get; set; }
}


public class ReadPriceDto
{
    
}