using System.Net;
using CampaignModule.Core.Repository;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Response;
using Moq;

namespace CampaignModule.Core.Test.Service;

public class OrderServiceTest
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IProductService> _mockProductService;
    private readonly OrderService _service;

    public OrderServiceTest()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductService = new Mock<IProductService>();
        _service = new OrderService(_mockOrderRepository.Object,
            _mockProductService.Object);
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
        
        var tcsProductInfo = new TaskCompletionSource<BaseResponse<ProductInfoDTO>>();
        tcsProductInfo.SetResult(new BaseResponse<ProductInfoDTO>
        {
            Result = productInfoDTO,
            IsSuccess = true,
            Message = productInfoDTO.ToString(),
            StatusCode = (int)HttpStatusCode.OK
        });
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task);
        
        var tcsCreateOrder = new TaskCompletionSource<int>();
        tcsCreateOrder.SetResult(1);
        _mockOrderRepository.Setup(repository => repository.CreateOrder(It.IsAny<OrderEntity>()))
            .Returns(tcsCreateOrder.Task);
        
        //act
        var response = await _service.CreateOrder(orderDTO);
        
        //assert
        Assert.IsType<BaseResponse<OrderDTO>>(response);
        Assert.Equal((int)HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productCode, response.Result!.ProductCode);
        Assert.Equal(3, response.Result!.Quantity);
        Assert.Equal(orderDTO.ToString(), response.Message);
    }
    
    [Fact]
    public async Task CreateOrder_InvalidProductCode_ReturnsNullAndStatusNotFound()
    {
        //arrange
        const string productCode = "P1";
        var orderDTO = new OrderDTO
        {
            ProductCode = productCode,
            Quantity = 3
        };

        var tcsProductInfo = new TaskCompletionSource<BaseResponse<ProductInfoDTO?>>();
        tcsProductInfo.SetResult(new BaseResponse<ProductInfoDTO?>
        {
            Result = null,
            IsSuccess = true,
            Message = string.Empty,
            StatusCode = (int)HttpStatusCode.OK
        });
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task!);
        
        _mockOrderRepository.VerifyNoOtherCalls();
        
        //act
        var response = await _service.CreateOrder(orderDTO);
        
        //assert
        Assert.IsType<BaseResponse<OrderDTO>>(response);
        Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.IsSuccess);
        Assert.Null(response.Result);
        Assert.Equal("Product is not found.", response.Message);
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
        
        var tcsProductInfo = new TaskCompletionSource<BaseResponse<ProductInfoDTO>>();
        tcsProductInfo.SetResult(new BaseResponse<ProductInfoDTO>
        {
            Result = productInfoDTO,
            IsSuccess = true,
            Message = productInfoDTO.ToString(),
            StatusCode = (int)HttpStatusCode.OK
        });
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task);
        
        _mockOrderRepository.VerifyNoOtherCalls();
        
        //act
        var response = await _service.CreateOrder(orderDTO);
        
        //assert
        Assert.IsType<BaseResponse<OrderDTO>>(response);
        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccess);
        Assert.Null(response.Result);
        Assert.Equal("Product stock is not enough.", response.Message);
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
        
        var tcsProductInfo = new TaskCompletionSource<BaseResponse<ProductInfoDTO>>();
        tcsProductInfo.SetResult(new BaseResponse<ProductInfoDTO>
        {
            Result = productInfoDTO,
            IsSuccess = true,
            Message = productInfoDTO.ToString(),
            StatusCode = (int)HttpStatusCode.OK
        });
        _mockProductService.Setup(service => service.GetProduct(productCode))
            .Returns(tcsProductInfo.Task);
        
        var tcsCreateOrder = new TaskCompletionSource<int>();
        tcsCreateOrder.SetResult(0);
        _mockOrderRepository.Setup(repository => repository.CreateOrder(It.IsAny<OrderEntity>()))
            .Returns(tcsCreateOrder.Task);
        
        //act
        async Task Act() => await _service.CreateOrder(orderDTO);

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("There is a technical problem.", exception.Message);
    }
}