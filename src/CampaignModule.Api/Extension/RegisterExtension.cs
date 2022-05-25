using CampaignModule.Api.RegisterProgram;

namespace CampaignModule.Api.Extension;

public static class RegisterExtension
{
    public static void RegisterServices(this WebApplicationBuilder builder, Type scanningType)
    {
        var registers = GetRegisters<IWebApplicationBuilderRegister>(scanningType);
        foreach (var register in registers) register.RegisterServices(builder);
    }

    public static void RegisterPipelineComponents(this WebApplication app, Type scanningType)
    {
        var registers = GetRegisters<IWebApplicationRegister>(scanningType);
        foreach (var register in registers) register.RegisterPipelineComponents(app);
    }

    private static IEnumerable<T> GetRegisters<T>(Type scanningType) where T : IRegister
    {
        return scanningType.Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(T)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<T>();
    }
}