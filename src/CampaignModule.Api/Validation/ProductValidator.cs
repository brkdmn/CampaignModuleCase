using CampaignModule.Domain.Request;
using FluentValidation;

namespace CampaignModule.Api.Validation;

public class ProductValidator : AbstractValidator<ProductCreateRequest>
{
    public ProductValidator()
    {
        RuleFor(p => p.ProductCode).Length(2, 200);
    }
}