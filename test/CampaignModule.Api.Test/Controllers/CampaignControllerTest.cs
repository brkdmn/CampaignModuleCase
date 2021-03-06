using System.Net;
using CampaignModule.Api.Controllers;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Enum;
using CampaignModule.Domain.Request;
using CampaignModule.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CampaignModule.Api.Test.Controllers;

public class CampaignControllerTest
{
    private readonly CampaignController _controller;
    private readonly Mock<ICampaignService> _mockService;

    public CampaignControllerTest()
    {
        var logger = new Mock<ILogger<CampaignController>>();
        _mockService = new Mock<ICampaignService>();
        _controller = new CampaignController(_mockService.Object)
        {
            _logger = logger.Object
        };
    }

    [Fact]
    public async Task GetCampaignInfo_CallEndpoint_ReturnsHttpStatusCodeAndCampaignInfo()
    {
        //arrange
        const string campaignName = "C1";

        var tcs = new TaskCompletionSource<CampaignInfoDTO>();
        tcs.SetResult(new CampaignInfoDTO
        {
            AvarageItemPrice = 100,
            Name = campaignName,
            Status = CampaignStatus.Active,
            TargetSales = 10,
            TotalSales = 1,
            Turnover = 100
        });

        _mockService.Setup(service => service.GetCampaign(campaignName))
            .Returns(tcs.Task);

        //act
        var result = await _controller.GetCampaignInfo(campaignName);

        //assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BaseResponse<CampaignInfoDTO>>(objectResult.Value);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(CampaignStatus.Active, response.Result!.Status);
        Assert.Equal(campaignName, response.Result!.Name);
        Assert.Equal(100, response.Result!.AvarageItemPrice);
        Assert.Equal(10, response.Result!.TargetSales);
        Assert.Equal(1, response.Result!.TotalSales);
        Assert.Equal(100, response.Result!.Turnover);
    }

    [Fact]
    public async Task GetCampaignInfo_ErrorOccuring_ThrowException()
    {
        //arrange

        //act
        async Task Act()
        {
            await _controller.GetCampaignInfo("");
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when getting campaign info.", exception.Message);
    }

    [Fact]
    public async Task CreateCampaign_CallEndpoint_ReturnsHttpStatusCodeAndCreatedCampaign()
    {
        //arrange
        const string campaignName = "C1";
        const string productCode = "P1";
        var campaignRequest = new CampaignCreateRequest
        {
            Name = campaignName,
            ProductCode = productCode,
            Duration = 5,
            PriceManipulationLimit = 20,
            TargetSalesCount = 10
        };

        var campaignDto = new CampaignDTO
        {
            Name = campaignName,
            ProductCode = productCode,
            Duration = 5,
            PriceManipulationLimit = 20,
            TargetSalesCount = 10
        };

        var tcs = new TaskCompletionSource<CampaignDTO>();
        tcs.SetResult(campaignDto);

        _mockService.Setup(service => service.CreateCampaign(It.IsAny<CampaignDTO>()))
            .Returns(tcs.Task);

        //act
        var result = await _controller.CreateCampaign(campaignRequest);

        //assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<BaseResponse<CampaignDTO>>(createdAtActionResult.Value);
        Assert.NotNull(createdAtActionResult.StatusCode);
        Assert.Equal((int)HttpStatusCode.Created, createdAtActionResult.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(campaignName, response.Result!.Name);
        Assert.Equal(productCode, response.Result!.ProductCode);
        Assert.Equal(5, response.Result!.Duration);
        Assert.Equal(20, response.Result!.PriceManipulationLimit);
        Assert.Equal(10, response.Result!.TargetSalesCount);
        Assert.Equal(campaignDto.ToString(), response.Message);
    }

    [Fact]
    public async Task CreateCampaign_ErrorOccuring_ThrowException()
    {
        //arrange
        _mockService.Setup(service => service.CreateCampaign(It.IsAny<CampaignDTO>()))
            .Throws(new Exception());

        //act
        async Task Act()
        {
            await _controller.CreateCampaign(new CampaignCreateRequest());
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when creating campaign with campaign.", exception.Message);
    }

    [Fact]
    public async Task IncreaseTime_CallEndpoint_ReturnsIncreasedCampaignTime()
    {
        //arrange
        const int time = 1;
        const string name = "C1";

        var tcs = new TaskCompletionSource<string>();
        tcs.SetResult("02:00");

        _mockService.Setup(service => service.IncreaseTime(time, name))
            .Returns(tcs.Task);

        //act
        var result = await _controller.IncreaseTime(time, name);

        //assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BaseResponse<string>>(objectResult.Value);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal("02:00", response.Result);
    }

    [Fact]
    public async Task IncreaseTime_ErrorOccuring_ThrowException()
    {
        //arrange
        const int time = 1;
        const string name = "C1";

        _mockService.Setup(service => service.IncreaseTime(time, name))
            .Throws(new Exception());

        //act
        async Task Act()
        {
            await _controller.IncreaseTime(time, name);
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when increase time.", exception.Message);
    }
}