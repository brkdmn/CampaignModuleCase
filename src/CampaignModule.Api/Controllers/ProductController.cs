using System.Net;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using CampaignModule.Domain.Response;
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
        try
        {
            var productInfoDto = await _productService.GetProduct(productCode);
            var response = new BaseResponse<ProductInfoDTO>
            {
                IsSuccess = true,
                Result = productInfoDto,
                Message = productInfoDto.ToString(),
                StatusCode = (int)HttpStatusCode.OK
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error,
                "Error occurred when getting product info, productCode:{}, Ex: {}", productCode,
                e.Message);
            throw new Exception("Error occurred when getting product info.");
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct(ProductCreateRequest productCreateRequest)
    {
        try
        {
            var productDto = await _productService.CreateProduct(productCreateRequest.Adapt<ProductDTO>());
            var response = new BaseResponse<ProductDTO>
            {
                IsSuccess = true,
                Result = productDto,
                Message = productDto.ToString(),
                StatusCode = (int)HttpStatusCode.Created
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error,
                "Error occurred when creating product, productCode:{}, Ex: {}", productCreateRequest.ProductCode,
                e.Message);
            throw new Exception("Error occurred when creating product.");
        }
    }
}