using System.Threading.Tasks;
using Service.Dtos.OpenFoodFactsDtos;

namespace Service.Services.OpenFoodFactsService;

public interface IOpenFoodFactsClient
{
    Task<OpenFoodFactsDto?> GetProductByBarcodeAsync(
        string barcode,
        string? productType = null,
        string? fields = null);
}
