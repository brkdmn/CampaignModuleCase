using CampaignModule.Api.Validation;
using CampaignModule.Domain.Request;

namespace CampaignModule.Api.Test.Validation;

public class ProductCreateValidatorTest
{
    private readonly ProductCreateValidator _validator;

    public ProductCreateValidatorTest()
    {
        _validator = new ProductCreateValidator();
    }

    [Fact]
    public void AllowValidProductRequest()
    {
        var productCreateRequest = new ProductCreateRequest
        {
            Stock = 10,
            ProductCode = "P1",
            Price = 100
        };
        Assert.True(_validator.Validate(productCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowEmptyProductCode()
    {
        var productCreateRequest = new ProductCreateRequest
        {
            Stock = 10,
            ProductCode = "",
            Price = 100
        };
        Assert.False(_validator.Validate(productCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowLessThen2CharacterProductCode()
    {
        var productCreateRequest = new ProductCreateRequest
        {
            Stock = 10,
            ProductCode = "1",
            Price = 100
        };
        Assert.False(_validator.Validate(productCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowNegativeValueStock()
    {
        var productCreateRequest = new ProductCreateRequest
        {
            Stock = -10,
            ProductCode = "P1",
            Price = 100
        };
        Assert.False(_validator.Validate(productCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowZeroValuePrice()
    {
        var productCreateRequest = new ProductCreateRequest
        {
            Stock = 10,
            ProductCode = "P1",
            Price = 0
        };
        Assert.False(_validator.Validate(productCreateRequest).IsValid);
    }
}