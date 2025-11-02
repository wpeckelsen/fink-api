public class Price
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public decimal Value { get; set; }
    public decimal PricePerUnit { get; set; }
    public CurrencyType Currency { get; set; }
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
}
