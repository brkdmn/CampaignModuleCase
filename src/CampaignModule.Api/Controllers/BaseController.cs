using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[ApiController]
public abstract class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private ILogger<T>? _logger;

    protected ILogger<T> Logger => _logger ??= HttpContext?.RequestServices.GetService<ILogger<T>>()!;
}