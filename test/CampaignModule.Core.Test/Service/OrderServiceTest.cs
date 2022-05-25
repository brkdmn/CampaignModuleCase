using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using Moq;

namespace CampaignModule.Core.Test.Service;

public class OrderServiceTest
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly OrderService _service;

    public OrderServiceTest()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductService = new Mock<IProductService>();
        _service = new OrderService(_mockProductService.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateOrder_ReturnsOrderDtoResponse()
    {
        //arrange
        const string productCode = "P1";
        var orderDTO = new OrderDTO
        {
            ProductCode = productCode,
            Quantity = 3
        };

        var productInfoDTO = new ProductInfoDTO
        {
            Price = 100,
            Stock = 1000,
            CampaignName = "C1"
        };

        var tcsProductInfo = new TaskCompletionSource<ProductInfoDTO>();
        tcsProductInfo.SetResult(productInfoDTO);
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task);

        var tcsCreateOrder = new TaskCompletionSource<int>();
        tcsCreateOrder.SetResult(1);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Order.AddAsync(It.IsAny<Order>()))
            .Returns(tcsCreateOrder.Task);

        //act
        var response = await _service.CreateOrder(orderDTO);

        //assert
        Assert.IsType<OrderDTO>(response);
        Assert.NotNull(response);
        Assert.Equal(productCode, response.ProductCode);
        Assert.Equal(3, response.Quantity);
    }

    [Fact]
    public async Task CreateOrder_InvalidProductCode_ReturnsNullAndStatusNotFound()
    {
        //arrange
        const string productCode = "P1";
        var orderDto = new OrderDTO
        {
            ProductCode = productCode,
            Quantity = 3
        };

        var tcsProductInfo = new TaskCompletionSource<ProductInfoDTO?>();
        tcsProductInfo.SetResult(null);
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task!);

        _mockUnitOfWork.VerifyNoOtherCalls();

        //act
        async Task Act()
        {
            await _service.CreateOrder(orderDto);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Product is not found.", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }


    [Fact]
    public async Task CreateOrder_ProductStockLowerThan_ReturnsNullAndStatusNotFound()
    {
        //arrange
        const string productCode = "P1";
        var orderDTO = new OrderDTO
        {
            ProductCode = productCode,
            Quantity = 6
        };

        var productInfoDTO = new ProductInfoDTO
        {
            Price = 100,
            Stock = 5,
            CampaignName = "C1"
        };

        var tcsProductInfo = new TaskCompletionSource<ProductInfoDTO>();
        tcsProductInfo.SetResult(productInfoDTO);
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task);

        _mockUnitOfWork.VerifyNoOtherCalls();

        //act
        async Task Act()
        {
            await _service.CreateOrder(orderDTO);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Product stock is not enough.", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_CreateOrderIsNotSuccess_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        var orderDTO = new OrderDTO
        {
            ProductCode = productCode,
            Quantity = 3
        };

        var productInfoDTO = new ProductInfoDTO
        {
            Price = 100,
            Stock = 1000,
            CampaignName = "C1"
        };

        var tcsProductInfo = new TaskCompletionSource<ProductInfoDTO>();
        tcsProductInfo.SetResult(productInfoDTO);
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task);

        var tcsCreateOrder = new TaskCompletionSource<int>();
        tcsCreateOrder.SetResult(0);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Order.AddAsync(It.IsAny<Order>()))
            .Returns(tcsCreateOrder.Task);

        //act
        async Task Act()
        {
            await _service.CreateOrder(orderDTO);
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("There is a technical problem.", exception.Message);
    }
}