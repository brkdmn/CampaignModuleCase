using CampaignModule.Api.Validation;
using CampaignModule.Domain.Request;

namespace CampaignModule.Api.Test.Validation;

public class CampaignCreateValidatorTest
{
    private readonly CampaignCreateValidator _validator;

    public CampaignCreateValidatorTest()
    {
        _validator = new CampaignCreateValidator();
    }

    [Fact]
    public void AllowValidCampaignRequest()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "C1",
            PriceManipulationLimit = 20,
            ProductCode = "P1",
            TargetSalesCount = 100
        };
        Assert.True(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowEmptyProductCode()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "C1",
            PriceManipulationLimit = 20,
            ProductCode = "",
            TargetSalesCount = 100
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowLessThen2CharacterProductCode()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "C1",
            PriceManipulationLimit = 20,
            ProductCode = "P",
            TargetSalesCount = 100
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowZeroValueDuration()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 0,
            Name = "C1",
            PriceManipulationLimit = 20,
            ProductCode = "P1",
            TargetSalesCount = 100
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowZeroValuePriceManipulationLimit()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "C1",
            PriceManipulationLimit = 0,
            ProductCode = "P1",
            TargetSalesCount = 100
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowZeroValueTargetSalesCount()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "C1",
            PriceManipulationLimit = 20,
            ProductCode = "P1",
            TargetSalesCount = 0
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowEmptyCampaignName()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "",
            PriceManipulationLimit = 20,
            ProductCode = "P1",
            TargetSalesCount = 100
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }

    [Fact]
    public void NotAllowLessThen2CharacterCampaignName()
    {
        var campaignCreateRequest = new CampaignCreateRequest
        {
            Duration = 10,
            Name = "C",
            PriceManipulationLimit = 20,
            ProductCode = "P",
            TargetSalesCount = 100
        };
        Assert.False(_validator.Validate(campaignCreateRequest).IsValid);
    }
}