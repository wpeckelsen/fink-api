using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Dtos.OpenFoodFactsDtos;
using Service.Services.OpenFoodFactsService;

namespace fink_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenFoodFactsController : ControllerBase
{
    private readonly IOpenFoodFactsClient _client;

    public OpenFoodFactsController(IOpenFoodFactsClient client)
    {
        _client = client;
    }

    [HttpGet("{barcode}")]
    public async Task<ActionResult<OpenFoodFactsDto>> GetProduct(
        string barcode,
        [FromQuery] string? productType = null,
        [FromQuery] string? fields = null)
    {
        var result = await _client.GetProductByBarcodeAsync(barcode, productType, fields);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
