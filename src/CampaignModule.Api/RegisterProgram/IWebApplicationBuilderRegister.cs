namespace CampaignModule.Api.RegisterProgram;

public interface IWebApplicationBuilderRegister : IRegister
{
    void RegisterServices(WebApplicationBuilder builder);
}