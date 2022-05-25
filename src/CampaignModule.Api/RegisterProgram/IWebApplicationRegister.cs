namespace CampaignModule.Api.RegisterProgram;

public interface IWebApplicationRegister : IRegister
{
    public void RegisterPipelineComponents(WebApplication app);
}