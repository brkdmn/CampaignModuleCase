using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Core.Service;

namespace CampaignModule.Api.RegisterProgram;

public class BusinessServiceRegister : IWebApplicationBuilderRegister
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<ICampaignService, CampaignService>();
    }
}