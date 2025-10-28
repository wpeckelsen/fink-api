public class Product
{
    public int Id { get; set; }

    // variable for future use
    // public int? GroupId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public UnitType Unit { get; set; }
    
    // might use categories later
    // public string Category { get; set; } = string.Empty;
    public ICollection<Price> Prices { get; set; } = new List<Price>();

}
