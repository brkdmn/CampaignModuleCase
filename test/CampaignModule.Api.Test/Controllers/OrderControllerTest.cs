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

public class OrderControllerTest
{
    private readonly OrderController _controller;
    private readonly Mock<IOrderService> _mockService;

    public OrderControllerTest()
    {
        var logger = new Mock<ILogger<OrderController>>();
        _mockService = new Mock<IOrderService>();
        _controller = new OrderController(_mockService.Object)
        {
            _logger = logger.Object
        };
    }

    [Fact]
    public async Task CreateOrder_CallEndpoint_ReturnsHttpStatusCodeAndCreatedOrder()
    {
        //arrange
        const string productCode = "P1";
        var orderCreateRequest = new OrderCreateRequest
        {
            ProductCode = productCode,
            Quantity = 5
        };

        var orderDto = new OrderDTO
        {
            ProductCode = productCode,
            Quantity = 5
        };

        var tcs = new TaskCompletionSource<OrderDTO>();
        tcs.SetResult(orderDto);

        _mockService.Setup(service => service.CreateOrder(It.IsAny<OrderDTO>()))
            .Returns(tcs.Task);

        //act
        var result = await _controller.CreateOrder(orderCreateRequest);

        //assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<BaseResponse<OrderDTO>>(objectResult.Value);
        Assert.NotNull(objectResult.StatusCode);
        Assert.Equal((int)HttpStatusCode.Created, objectResult.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productCode, response.Result!.ProductCode);
        Assert.Equal(5, response.Result!.Quantity);
        Assert.Equal(orderDto.ToString(), response.Message);
    }

    [Fact]
    public async Task CreateOrder_ErrorOccuring_ThrowException()
    {
        //arrange
        _mockService.Setup(service => service.CreateOrder(It.IsAny<OrderDTO>()))
            .Throws(new Exception());

        //act
        async Task Act()
        {
            await _controller.CreateOrder(new OrderCreateRequest());
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when creating order.", exception.Message);
    }
}