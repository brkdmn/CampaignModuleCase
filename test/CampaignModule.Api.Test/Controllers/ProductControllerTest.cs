using System.Net;
using CampaignModule.Api.Controllers;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using CampaignModule.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CampaignModule.Api.Test.Controllers;

public class ProductControllerTest
{
    private readonly ProductController _controller;
    private readonly Mock<IProductService> _mockService;

    public ProductControllerTest()
    {
        var logger = new Mock<ILogger<ProductController>>();
        _mockService = new Mock<IProductService>();
        _controller = new ProductController(_mockService.Object)
        {
            _logger = logger.Object
        };
    }

    [Fact]
    public async Task CreateProduct_CallEndpoint_ReturnsHttpStatusCodeAndCreatedProduct()
    {
        //arrange
        const string productCode = "P1";
        var productCreateRequest = new ProductCreateRequest
        {
            ProductCode = productCode,
            Stock = 1000,
            Price = 100
        };

        var productDto = new ProductDTO(productCode, 100, 1000);

        var tcs = new TaskCompletionSource<ProductDTO>();
        tcs.SetResult(productDto);

        _mockService.Setup(service => service.CreateProduct(It.IsAny<ProductDTO>()))
            .Returns(tcs.Task);

        //act
        var result = await _controller.CreateProduct(productCreateRequest);

        //assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<BaseResponse<ProductDTO>>(createdAtActionResult.Value);
        Assert.NotNull(createdAtActionResult.StatusCode);
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productCode, response.Result!.ProductCode);
        Assert.Equal(1000, response.Result!.Stock);
        Assert.Equal(100, response.Result!.Price);
        Assert.Equal(productDto.ToString(), response.Message);
    }

    [Fact]
    public async Task CreateProduct_ErrorOccuring_ThrowException()
    {
        //arrange
        _mockService.Setup(service => service.CreateProduct(It.IsAny<ProductDTO>()))
            .Throws(new Exception());

        //act
        async Task Act()
        {
            await _controller.CreateProduct(new ProductCreateRequest());
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when creating product.", exception.Message);
    }

    [Fact]
    public async Task GetProductInfo_CallEndpoint_ReturnsHttpStatusCodeAndProductInfo()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "P1";
        var productInfoDto = new ProductInfoDTO
        {
            Price = 100,
            Stock = 1000,
            CampaignName = campaignName
        };
        var tcs = new TaskCompletionSource<ProductInfoDTO>();
        tcs.SetResult(productInfoDto);

        _mockService.Setup(service => service.GetProduct(productCode))
            .Returns(tcs.Task);

        //act
        var result = await _controller.GetProduct(productCode);

        //assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BaseResponse<ProductInfoDTO>>(objectResult.Value);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productInfoDto.ToString(), response.Message);
        Assert.Equal(campaignName, response.Result!.CampaignName);
        Assert.Equal(100, response.Result!.Price);
        Assert.Equal(1000, response.Result!.Stock);
    }

    [Fact]
    public async Task GetProductInfo_ErrorOccuring_ThrowException()
    {
        //arrange
        const string productCode = "P1";

        _mockService.Setup(service => service.GetProduct(productCode))
            .Throws(new Exception());

        //act
        async Task Act()
        {
            await _controller.GetProduct(productCode);
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when getting product info.", exception.Message);
    }
}