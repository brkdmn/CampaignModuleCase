using CampaignModule.Domain.Request;
using FluentValidation;

namespace CampaignModule.Api.Validation;

public class CampaignCreateValidator : AbstractValidator<CampaignCreateRequest>
{
    public CampaignCreateValidator()
    {
        RuleFor(p => p.ProductCode).NotEmpty().MinimumLength(2);
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2);
        RuleFor(p => p.Duration).GreaterThan(0);
        RuleFor(p => p.PriceManipulationLimit).GreaterThan(0);
        RuleFor(p => p.TargetSalesCount).GreaterThan(0);
    }
}