using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.External.OpenFoodFacts;

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
    public async Task<ActionResult<OpenFoodFactsProductResponse>> GetProduct(
        string barcode,
        [FromQuery] string? productType = null,
        [FromQuery] string? fields = null,
        [FromQuery] bool blame = false)
    {
        var result = await _client.GetProductByBarcodeAsync(barcode, productType, fields, blame);

        if (result == null || result.Status == 0)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
