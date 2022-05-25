using CampaignModule.Api.Validation;
using CampaignModule.Domain.Request;

namespace CampaignModule.Api.Test.Validation;

public class OrderCreateValidatorTest
{
    private readonly OrderCreateValidator _validator;

    public OrderCreateValidatorTest()
    {
        _validator = new OrderCreateValidator();
    }

    [Fact]
    public void AllowValidOrderRequest()
    {
        var orderCreateRequest = new OrderCreateRequest
        {
            Quantity = 10,
            ProductCode = "P1"
        };
        Assert.True(_validator.Validate(orderCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowEmptyProductCode()
    {
        var orderCreateRequest = new OrderCreateRequest
        {
            Quantity = 10,
            ProductCode = ""
        };
        Assert.False(_validator.Validate(orderCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowLessThen2CharacterProductCode()
    {
        var orderCreateRequest = new OrderCreateRequest
        {
            Quantity = 10,
            ProductCode = "P"
        };
        Assert.False(_validator.Validate(orderCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowZeroValueQuantity()
    {
        var orderCreateRequest = new OrderCreateRequest
        {
            Quantity = 0,
            ProductCode = "P1"
        };
        Assert.False(_validator.Validate(orderCreateRequest).IsValid);
    }
}