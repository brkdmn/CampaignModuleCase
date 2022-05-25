using CampaignModule.Core.Configuration;
using FluentValidation.AspNetCore;

namespace CampaignModule.Api.RegisterProgram;

public class ApplicationServiceRegister : IWebApplicationBuilderRegister
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddControllers()
            .AddFluentValidation(fvc =>
                fvc.RegisterValidatorsFromAssemblyContaining<Program>());

        builder.Services.AddSingleton<IPostgresSqlConfiguration, PostgresSqlConfiguration>();
        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();
    }
}