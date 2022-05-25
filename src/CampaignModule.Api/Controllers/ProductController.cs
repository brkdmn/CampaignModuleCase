using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[Route("api/product")]
public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetProduct(string productCode)
    {
        var result = await _productService.GetProduct(productCode);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct(ProductCreateRequest productCreateRequest)
    {
        var result = await _productService.CreateProduct(productCreateRequest.Adapt<ProductDTO>());
        return StatusCode(result.StatusCode, result);
    }
}