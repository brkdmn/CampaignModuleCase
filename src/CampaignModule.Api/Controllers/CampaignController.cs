using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[Route("api/campaign")]
[ApiController]
public class CampaignController : Controller
{
    private readonly ICampaignService _campaignService; 
    public CampaignController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }
    
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign(CampaignCreateRequest campaignCreateRequest)
    {
        var result = await _campaignService.CreateCampaign(campaignCreateRequest.Adapt<CampaignDTO>());
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet("info")]
    public async Task<IActionResult> GetCampaignInfo(string name)
    {
        var result = await _campaignService.GetCampaign(name);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPatch("increase-time")]
    public async Task<IActionResult> IncreaseTime(int time, string name)
    {
        var result = await _campaignService.IncreaseTime(time, name);
        return StatusCode(result.StatusCode, result);
    }
}