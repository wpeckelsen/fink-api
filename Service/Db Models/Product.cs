using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    // variable for future use
    // public int? GroupId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Barcode { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public UnitType Unit { get; set; }
    public CategoryType Category { get; set; } 
    public ICollection<Price> Prices { get; set; } = new List<Price>();
}
