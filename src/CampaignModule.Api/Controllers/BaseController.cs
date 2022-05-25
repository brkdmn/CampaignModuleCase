using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[ApiController]
public abstract class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private IConfiguration _configuration;
    private ILogger<T> _logger;

    protected ILogger<T> Logger => _logger = HttpContext?.RequestServices.GetService<ILogger<T>>()!;

    protected IConfiguration Configuration =>
        _configuration = HttpContext?.RequestServices.GetService<IConfiguration>()!;
}