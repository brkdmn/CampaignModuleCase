using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using Moq;

namespace CampaignModule.Core.Test.Service;

public class CampaignServiceTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CampaignService _service;

    public CampaignServiceTest()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new CampaignService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateCampaign_ReturnsCampaignDtoResponse()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";
        var campaignDto = new CampaignDTO
        {
            Name = campaignName,
            ProductCode = productCode,
            Duration = 10,
            PriceManipulationLimit = 20,
            TargetSalesCount = 100
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<Product?>();
        tcsProductEntity.SetResult(new Product
        {
            ProductCode = productCode,
            Price = 100,
            Stock = 1000
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductEntity.Task);

        var tcsCreateCampaign = new TaskCompletionSource<int>();
        tcsCreateCampaign.SetResult(1);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.AddAsync(It.IsAny<Campaign>()))
            .Returns(tcsCreateCampaign.Task);

        //act
        var response = await _service.CreateCampaign(campaignDto);

        //assert
        Assert.IsType<CampaignDTO>(response);
        Assert.Equal(productCode, response.ProductCode);
        Assert.Equal(20, response.PriceManipulationLimit);
        Assert.Equal(10, response.Duration);
        Assert.Equal(100, response.TargetSalesCount);
    }

    [Fact]
    public async Task CreateCampaign_ProductIsNotFound_ThrowNotFoundException()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";
        var campaignDto = new CampaignDTO
        {
            Name = campaignName,
            ProductCode = productCode,
            Duration = 10,
            PriceManipulationLimit = 20,
            TargetSalesCount = 100
        };

        var tcsProductEntity = new TaskCompletionSource<Product?>();
        tcsProductEntity.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductEntity.Task);

        //act
        async Task Act()
        {
            await _service.CreateCampaign(campaignDto);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Product is not found.", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task CreateCampaign_CampaignAlreadyExist_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";
        var campaignDto = new CampaignDTO
        {
            Name = campaignName,
            ProductCode = productCode,
            Duration = 10,
            PriceManipulationLimit = 20,
            TargetSalesCount = 100
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(new Campaign());
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<Product?>();
        tcsProductEntity.SetResult(new Product
        {
            ProductCode = productCode,
            Price = 100,
            Stock = 1000
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductEntity.Task);

        //act
        async Task Act()
        {
            await _service.CreateCampaign(campaignDto);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Campaign is already exist.", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task CreateCampaign_AddCampaignCountIsNot1_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";
        var campaignDto = new CampaignDTO
        {
            Name = campaignName,
            ProductCode = productCode,
            Duration = 10,
            PriceManipulationLimit = 20,
            TargetSalesCount = 100
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<Product?>();
        tcsProductEntity.SetResult(new Product
        {
            ProductCode = productCode,
            Price = 100,
            Stock = 1000
        });
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Product.GetByCodeAsync(productCode))
            .Returns(tcsProductEntity.Task);

        var tcsCreateCampaign = new TaskCompletionSource<int>();
        tcsCreateCampaign.SetResult(0);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.AddAsync(It.IsAny<Campaign>()))
            .Returns(tcsCreateCampaign.Task);

        //act
        async Task Act()
        {
            await _service.CreateCampaign(campaignDto);
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("There is a technical problem.", exception.Message);
    }

    [Fact]
    public async Task CampaignAvailable_ReturnsAvailableIsTrue()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var campaign = new Campaign
        {
            Duration = 10,
            PriceManipulationLimit = 20,
            CurrentDuration = 1,
            ProductCode = productCode,
            TargetSalesCount = 100,
            Name = campaignName
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(campaign);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<TotalQuantity?>();
        tcsProductEntity.SetResult(new TotalQuantity
        {
            Total = 10
        });
        _mockUnitOfWork.Setup(unitOfWork =>
                unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(campaignName, productCode))
            .Returns(tcsProductEntity.Task);

        //act
        var response = await _service.CampaignAvailable(productCode);

        //assert
        Assert.IsType<bool>(response);
        Assert.True(response);
    }

    [Fact]
    public async Task CampaignAvailable_CampaignNotFound_ReturnsAvailableIsFalse()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<TotalQuantity?>();
        tcsProductEntity.SetResult(new TotalQuantity
        {
            Total = 10
        });
        _mockUnitOfWork.Setup(unitOfWork =>
                unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(campaignName, productCode))
            .Returns(tcsProductEntity.Task);

        //act
        var response = await _service.CampaignAvailable(productCode);

        //assert
        Assert.IsType<bool>(response);
        Assert.False(response);
    }

    [Fact]
    public async Task CampaignAvailable_DurationLteCurrentDuration_ReturnsAvailableIsFalse()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var campaign = new Campaign
        {
            Duration = 10,
            PriceManipulationLimit = 20,
            CurrentDuration = 10,
            ProductCode = productCode,
            TargetSalesCount = 100,
            Name = campaignName
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(campaign);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<TotalQuantity?>();
        tcsProductEntity.SetResult(new TotalQuantity
        {
            Total = 10
        });
        _mockUnitOfWork.Setup(unitOfWork =>
                unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(campaignName, productCode))
            .Returns(tcsProductEntity.Task);

        //act
        var response = await _service.CampaignAvailable(productCode);

        //assert
        Assert.IsType<bool>(response);
        Assert.False(response);
    }

    [Fact]
    public async Task CampaignAvailable_SalesCountGteTargetSalesCount_ReturnsAvailable()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var campaign = new Campaign
        {
            Duration = 10,
            PriceManipulationLimit = 20,
            CurrentDuration = 1,
            ProductCode = productCode,
            TargetSalesCount = 100,
            Name = campaignName
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(campaign);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode))
            .Returns(tcsCampaignEntity.Task);

        var tcsProductEntity = new TaskCompletionSource<TotalQuantity?>();
        tcsProductEntity.SetResult(new TotalQuantity
        {
            Total = 100
        });
        _mockUnitOfWork.Setup(unitOfWork =>
                unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(campaignName, productCode))
            .Returns(tcsProductEntity.Task);

        //act
        var response = await _service.CampaignAvailable(productCode);

        //assert
        Assert.IsType<bool>(response);
        Assert.False(response);
    }

    [Fact]
    public async Task GetCampaign_ReturnsCampaignInfoDTO()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var campaign = new Campaign
        {
            Duration = 10,
            PriceManipulationLimit = 20,
            CurrentDuration = 1,
            ProductCode = productCode,
            TargetSalesCount = 100,
            Name = campaignName
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(campaign);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        var tcsTotalQuantity = new TaskCompletionSource<TotalQuantity?>();
        tcsTotalQuantity.SetResult(new TotalQuantity
        {
            Total = 100
        });
        _mockUnitOfWork.Setup(unitOfWork =>
                unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(campaignName, productCode))
            .Returns(tcsTotalQuantity.Task);


        var tcsTotalPrice = new TaskCompletionSource<TotalPrice?>();
        tcsTotalPrice.SetResult(new TotalPrice
        {
            Total = 1000
        });
        _mockUnitOfWork.Setup(unitOfWork =>
                unitOfWork.Order.GetTotalPriceByCampaignNameAndProductCodeAsync(campaignName, productCode))
            .Returns(tcsTotalPrice.Task);

        //act
        var response = await _service.GetCampaign(campaignName);

        //assert
        Assert.IsType<CampaignInfoDTO>(response);
        Assert.Equal(campaignName, response.Name);
        Assert.Equal(10, response.AvarageItemPrice);
        Assert.Equal(100, response.TargetSales);
        Assert.Equal(100, response.TotalSales);
        Assert.Equal(1000, response.Turnover);
    }

    [Fact]
    public async Task GetCampaign_CampaignIsNotFound_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        //act
        async Task Act()
        {
            await _service.GetCampaign(campaignName);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Campaign is not found.", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task IncreaseTime_ReturnsIncreasedTime()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var campaign = new Campaign
        {
            Duration = 10,
            PriceManipulationLimit = 20,
            CurrentDuration = 1,
            ProductCode = productCode,
            TargetSalesCount = 100,
            Name = campaignName
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(campaign);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        var tcsCreateCampaign = new TaskCompletionSource<int>();
        tcsCreateCampaign.SetResult(1);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.UpdateAsync(It.IsAny<Campaign>()))
            .Returns(tcsCreateCampaign.Task);

        //act
        var response = await _service.IncreaseTime(1, campaignName);

        //assert
        Assert.IsType<string>(response);
        Assert.Equal("Time is 02:00", response);
    }

    [Fact]
    public async Task IncreaseTime_CampaignIsNotFound_ThrowException()
    {
        //arrange
        const string campaignName = "C1";

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(null);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        //act
        async Task Act()
        {
            await _service.IncreaseTime(1, campaignName);
        }

        //assert
        var exception = await Assert.ThrowsAsync<AppException>((Func<Task>)Act);
        Assert.Equal("Campaign is not found", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task IncreaseTime_AddCampaignCountIsNot1_ThrowException()
    {
        //arrange
        const string productCode = "P1";
        const string campaignName = "C1";

        var campaign = new Campaign
        {
            Duration = 10,
            PriceManipulationLimit = 20,
            CurrentDuration = 1,
            ProductCode = productCode,
            TargetSalesCount = 100,
            Name = campaignName
        };

        var tcsCampaignEntity = new TaskCompletionSource<Campaign?>();
        tcsCampaignEntity.SetResult(campaign);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.GetByCodeAsync(campaignName))
            .Returns(tcsCampaignEntity.Task);

        var tcsCreateCampaign = new TaskCompletionSource<int>();
        tcsCreateCampaign.SetResult(0);
        _mockUnitOfWork.Setup(unitOfWork => unitOfWork.Campaign.UpdateAsync(It.IsAny<Campaign>()))
            .Returns(tcsCreateCampaign.Task);

        //act
        async Task Act()
        {
            await _service.IncreaseTime(1, campaignName);
        }

        //assert
        var exception = await Assert.ThrowsAsync<Exception>((Func<Task>)Act);
        Assert.Equal("Error occurred when update campaign", exception.Message);
    }
}