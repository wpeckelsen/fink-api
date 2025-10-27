using System;

public class PriceService
{
    public decimal CalculatePricePerUnit(Price price)
    {
        if (price == null)
        {
            throw new ArgumentNullException(nameof(price));
        }

        if (price.Product == null)
        {
            throw new ArgumentException("Price must include a Product to calculate the price per unit.", nameof(price));
        }

        if (price.Product.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price.Product.Quantity), "Product quantity must be greater than zero.");
        }

        var pricePerUnit = price.Value / (decimal)price.Product.Quantity;
        price.PricePerUnit = pricePerUnit;

        return pricePerUnit;
    }
}
