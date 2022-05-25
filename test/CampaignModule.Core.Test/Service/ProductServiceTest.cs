using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using Moq;

namespace CampaignModule.Core.Test.Service;

public class ProductServiceTest
{
    private readonly Mock<ICampaignService> _mockCampaignService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProductService _service;

    public ProductServiceTest()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCampaignService = new Mock<ICampaignService>();
        _service = new ProductService(_mockCampaignService.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateProduct_ReturnsProductDtoResponse()
    {
        //arrange
        const string productCode = "P1";
        var productDto = new ProductDTO(productCode, 100, 1000);

        var tcsProductDto = new TaskCompletionSource<Product?>();
        tcsProductDto.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDto.Task);

        var tcsCreateProduct = new TaskCompletionSource<int>();
        tcsCreateProduct.SetResult(1);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.AddAsync(It.IsAny<Product>()))
            .Returns(tcsCreateProduct.Task);

        //act
        var response = await _service.CreateProduct(productDto);

        //assert
        Assert.IsType<ProductDTO>(response);
        Assert.Equal(productCode, response.ProductCode);
        Assert.Equal(100, response.Price);
        Assert.Equal(1000, response.Stock);
    }

    [Fact]
    public async Task CreateProduct_ProductCodeIsAlreadyExist_ReturnsNullAndBadRequestCode()
    {
        //arrange
        const string productCode = "P1";
        var productDto = new ProductDTO(productCode, 100, 1000);

        var tcsProductDto = new TaskCompletionSource<Product?>();
        tcsProductDto.SetResult(new Product());
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDto.Task);

        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.Product.AddAsync(It.IsAny<Product>()), Times.Never);

        //act
        async Task Act()
        {
            await _service.CreateProduct(productDto);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Product is already exist.", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_CreateProductIsNotSuccess_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        var productDto = new ProductDTO(productCode, 100, 1000);

        var tcsProductDto = new TaskCompletionSource<Product?>();
        tcsProductDto.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDto.Task);

        var tcsCreateProduct = new TaskCompletionSource<int>();
        tcsCreateProduct.SetResult(0);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.AddAsync(It.IsAny<Product>()))
            .Returns(tcsCreateProduct.Task);

        //act
        async Task Act()
        {
            await _service.CreateProduct(productDto);
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
        var productInfoDto = new ProductInfoDTO
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
        Assert.IsType<ProductInfoDTO>(response);
        Assert.Equal(productInfoDto.CampaignName, response.CampaignName);
        Assert.Equal(productInfoDto.Price, response.Price);
        Assert.Equal(productInfoDto.Stock, response.Stock);
    }

    [Fact]
    public async Task GetProduct_CampaignIsAvailable_ReturnsProductInfoDTOWithManipulatedPrice()
    {
        //arrange
        const string productCode = "P1";
        var productInfoDto = new ProductInfoDTO
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
        Assert.IsType<ProductInfoDTO>(response);
        Assert.Equal(productInfoDto.CampaignName, response.CampaignName);
        Assert.Equal(productInfoDto.Price, response.Price);
        Assert.Equal(productInfoDto.Stock, response.Stock);
    }

    [Fact]
    public async Task GetProduct_InvalidProductCode_ReturnsNullAndNotFoundCode()
    {
        //arrange
        const string productCode = "P1";
        var tcsProductDto = new TaskCompletionSource<Product?>();
        tcsProductDto.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductDto.Task);

        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode),
            Times.Never);

        //act
        async Task Act()
        {
            await _service.GetProduct(productCode);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Product P1 is not found.", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }
}