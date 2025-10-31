using System.Threading.Tasks;

namespace Service.External.OpenFoodFacts;

public interface IOpenFoodFactsClient
{
    Task<OpenFoodFactsProductResponse?> GetProductByBarcodeAsync(
        string barcode,
        string? productType = null,
        string? fields = null,
        bool includeBlame = false);
}
