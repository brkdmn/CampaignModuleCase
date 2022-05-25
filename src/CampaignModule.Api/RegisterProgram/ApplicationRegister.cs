using CampaignModule.Api.Middleware;

namespace CampaignModule.Api.RegisterProgram;

public class ApplicationRegister : IWebApplicationRegister
{
    public void RegisterPipelineComponents(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction()) app.UseHttpsRedirection();

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.MapControllers();
    }
}