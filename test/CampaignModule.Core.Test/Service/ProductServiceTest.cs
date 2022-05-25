using System.Net;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Response;
using Moq;

namespace CampaignModule.Core.Test.Service;

public class ProductServiceTest
{
    private readonly Mock<ICampaignService> _mockCampaignService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProductService _service;

    public ProductServiceTest(Mock<IUnitOfWork> mockUnitOfWork)
    {
        _mockUnitOfWork = mockUnitOfWork;
        _mockCampaignService = new Mock<ICampaignService>();
        _service = new ProductService(_mockCampaignService.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateProduct_ReturnsProductDtoResponse()
    {
        //arrange
        const string productCode = "P1";
        var productDTO = new ProductDTO(productCode, 100, 1000);

        var tcsProductDTO = new TaskCompletionSource<Product?>();
        tcsProductDTO.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDTO.Task);

        var tcsCreateProduct = new TaskCompletionSource<int>();
        tcsCreateProduct.SetResult(1);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.AddAsync(It.IsAny<Product>()))
            .Returns(tcsCreateProduct.Task);

        //act
        var response = await _service.CreateProduct(productDTO);

        //assert
        Assert.IsType<BaseResponse<ProductDTO>>(response);
        Assert.Equal((int)HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productCode, response.Result!.ProductCode);
        Assert.Equal(100, response.Result!.Price);
        Assert.Equal(1000, response.Result!.Stock);
        Assert.Equal(productDTO.ToString(), response.Message);
    }

    [Fact]
    public async Task CreateProduct_ProductCodeIsAlreadyExist_ReturnsNullAndBadRequestCode()
    {
        //arrange
        const string productCode = "P1";
        var productDTO = new ProductDTO(productCode, 100, 1000);

        var tcsProductDTO = new TaskCompletionSource<Product?>();
        tcsProductDTO.SetResult(new Product());
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDTO.Task);

        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.Product.AddAsync(It.IsAny<Product>()), Times.Never);

        //act
        var response = await _service.CreateProduct(productDTO);

        //assert
        Assert.IsType<BaseResponse<ProductDTO>>(response);
        Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(response.IsSuccess);
        Assert.Null(response.Result);
        Assert.Equal("Product is already exist.", response.Message);
    }

    [Fact]
    public async Task CreateProduct_CreateProductIsNotSuccess_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        var productDTO = new ProductDTO(productCode, 100, 1000);

        var tcsProductDTO = new TaskCompletionSource<Product?>();
        tcsProductDTO.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDTO.Task);

        var tcsCreateProduct = new TaskCompletionSource<int>();
        tcsCreateProduct.SetResult(0);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.AddAsync(It.IsAny<Product>()))
            .Returns(tcsCreateProduct.Task);

        //act
        async Task Act()
        {
            await _service.CreateProduct(productDTO);
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("There is a technical problem.", exception.Message);
    }

    [Fact]
    public async Task GetProduct_ReturnsProductInfoDTo()
    {
        //arrange
        const string productCode = "P1";
        var productInfoDTO = new ProductInfoDTO
        {
            CampaignName = "C1",
            Price = 100,
            Stock = 1000
        };

        var tcsProductEntity = new TaskCompletionSource<Product?>();
        tcsProductEntity.SetResult(new Product
        {
            ProductCode = productCode,
            Price = 100,
            Stock = 1000
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductEntity.Task);

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(new Campaign
        {
            ProductCode = productCode,
            Name = "C1"
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode))
            .Returns(tcsCampaignEntity.Task);


        var tcsCampaignAvailable = new TaskCompletionSource<bool>();
        tcsCampaignAvailable.SetResult(false);
        _mockCampaignService.Setup(service => service.CampaignAvailable(productCode))
            .Returns(tcsCampaignAvailable.Task);

        var tcsSalesCountByProductCod = new TaskCompletionSource<TotalQuantity>();
        tcsSalesCountByProductCod.SetResult(new TotalQuantity
        {
            Total = 0
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Order.GetSalesCountByProductCodeAsync(productCode))
            .Returns(tcsSalesCountByProductCod.Task!);
        //act
        var response = await _service.GetProduct(productCode);

        //assert
        Assert.IsType<BaseResponse<ProductInfoDTO>>(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productInfoDTO.ToString(), response.Message);
        Assert.Equal(productInfoDTO.CampaignName, response.Result?.CampaignName);
        Assert.Equal(productInfoDTO.Price, response.Result?.Price);
        Assert.Equal(productInfoDTO.Stock, response.Result?.Stock);
    }

    [Fact]
    public async Task GetProduct_CampaignIsAvailable_ReturnsProductInfoDTOWithManipulatedPrice()
    {
        //arrange
        const string productCode = "P1";
        var productInfoDTO = new ProductInfoDTO
        {
            CampaignName = "C1",
            Price = 95.0M,
            Stock = 1000
        };

        var tcsProductEntity = new TaskCompletionSource<Product?>();
        tcsProductEntity.SetResult(new Product
        {
            ProductCode = productCode,
            Price = 100,
            Stock = 1000
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductEntity.Task);

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(new Campaign
        {
            ProductCode = productCode,
            Name = "C1",
            PriceManipulationLimit = 20,
            Duration = 5,
            CurrentDuration = 1
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode))
            .Returns(tcsCampaignEntity.Task);


        var tcsCampaignAvailable = new TaskCompletionSource<bool>();
        tcsCampaignAvailable.SetResult(true);
        _mockCampaignService.Setup(service => service.CampaignAvailable(productCode))
            .Returns(tcsCampaignAvailable.Task);

        var tcsSalesCountByProductCod = new TaskCompletionSource<TotalQuantity>();
        tcsSalesCountByProductCod.SetResult(new TotalQuantity
        {
            Total = 0
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Order.GetSalesCountByProductCodeAsync(productCode))
            .Returns(tcsSalesCountByProductCod.Task!);
        //act
        var response = await _service.GetProduct(productCode);

        //assert
        Assert.IsType<BaseResponse<ProductInfoDTO>>(response);
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Result);
        Assert.Equal(productInfoDTO.ToString(), response.Message);
        Assert.Equal(productInfoDTO.CampaignName, response.Result?.CampaignName);
        Assert.Equal(productInfoDTO.Price, response.Result?.Price);
        Assert.Equal(productInfoDTO.Stock, response.Result?.Stock);
    }

    [Fact]
    public async Task GetProduct_InvalidProductCode_ReturnsNullAndNotFoundCode()
    {
        //arrange
        const string productCode = "P1";
        var tcsProductDTO = new TaskCompletionSource<Product?>();
        tcsProductDTO.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDTO.Task);

        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode),
            Times.Never);

        //act
        var response = await _service.GetProduct(productCode);

        //assert
        Assert.IsType<BaseResponse<ProductInfoDTO>>(response);
        Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.IsSuccess);
        Assert.Null(response.Result);
        Assert.Equal($"Product {productCode} is not found.", response.Message);
    }
}