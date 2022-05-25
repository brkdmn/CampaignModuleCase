using CampaignModule.Domain.Request;
using FluentValidation;

namespace CampaignModule.Api.Validation;

public class ProductCreateValidator : AbstractValidator<ProductCreateRequest>
{
    public ProductCreateValidator()
    {
        RuleFor(p => p.ProductCode).NotEmpty().MinimumLength(2);
        RuleFor(p => p.Stock).GreaterThanOrEqualTo(0);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}