using CampaignModule.Domain.Request;
using FluentValidation;

namespace CampaignModule.Api.Validation;

public class OrderCreateValidator : AbstractValidator<OrderCreateRequest>
{
    public OrderCreateValidator()
    {
        RuleFor(p => p.ProductCode).NotEmpty().MinimumLength(2);
        RuleFor(p => p.Quantity).GreaterThan(0);
    }
}