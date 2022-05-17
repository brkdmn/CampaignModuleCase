using System.Net;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using Microsoft.AspNetCore.Mvc;
using Mapster;

namespace CampaignModule.Api.Controllers;

[Route("api/product")]
[ApiController]
public class ProductController : Controller
{
    private readonly IProductService _productService; 
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    [HttpGet("/")]
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