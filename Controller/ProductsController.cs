using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;

namespace fink_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{barcode}")]
    public async Task<ActionResult<ReadProductDto>> GetProductByBarcode(string barcode)
    {
        var product = await _productService.GetProductByBarcodeAsync(barcode);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    
    // will be removed for production. never will all +10k products be returned in one query.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuickReadProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllQuickReadProductsAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ReadProductDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = await _productService.CreateProductAsync(dto);
        //a readproductdto doesnt have a barcode. so it mustn't try to return a barcode. 
        return CreatedAtAction(nameof(GetProductByBarcode), new { barcode = dto.Barcode }, product);

        // return CreatedAtAction(nameof(GetProductByBarcode), new { barcode = product.Barcode }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReadProductDto>> UpdateProduct(int id, [FromBody] EditProductDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("Route id and payload id must match.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = await _productService.UpdateProductAsync(dto);
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
