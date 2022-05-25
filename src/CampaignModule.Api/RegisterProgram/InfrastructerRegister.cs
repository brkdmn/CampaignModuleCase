using CampaignModule.Core.Infrastructer;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Repository;
using CampaignModule.Core.Repository;

namespace CampaignModule.Api.RegisterProgram;

public class RepositoryRegister : IWebApplicationBuilderRegister
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
    }
}