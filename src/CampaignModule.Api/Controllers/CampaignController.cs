using System.Net;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using CampaignModule.Domain.Response;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[Route("api/campaign")]
public class CampaignController : BaseController<CampaignController>
{
    private readonly ICampaignService _campaignService;

    public CampaignController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign(CampaignCreateRequest campaignCreateRequest)
    {
        try
        {
            var campaignDto = await _campaignService.CreateCampaign(campaignCreateRequest.Adapt<CampaignDTO>());

            var response = new BaseResponse<CampaignDTO>
            {
                IsSuccess = true,
                Result = campaignDto,
                Message = campaignDto.ToString(),
                StatusCode = (int)HttpStatusCode.Created
            };
            return CreatedAtAction(nameof(GetCampaignInfo), new { name = campaignDto.Name }, response);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error,
                "Error occurred when creating campaign with campaign name{}:, Ex: {}", campaignCreateRequest.Name,
                e.Message);
            throw new Exception("Error occurred when creating campaign with campaign.");
        }
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetCampaignInfo(string name)
    {
        try
        {
            var campaignInfo = await _campaignService.GetCampaign(name);
            var response = new BaseResponse<CampaignInfoDTO>
            {
                IsSuccess = true,
                Result = campaignInfo,
                Message = campaignInfo.ToString(),
                StatusCode = (int)HttpStatusCode.OK
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error,
                "Error occurred when getting campaign info, campaign name:{}, Ex: {}", name,
                e.Message);
            throw new Exception("Error occurred when getting campaign info.");
        }
    }

    [HttpPatch("increase-time")]
    public async Task<IActionResult> IncreaseTime(int time, string name)
    {
        try
        {
            var result = await _campaignService.IncreaseTime(time, name);
            var response = new BaseResponse<string>
            {
                IsSuccess = true,
                Result = result,
                Message = result,
                StatusCode = (int)HttpStatusCode.OK
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error,
                "Error occurred when increase time. Ex: {}", e.Message);
            throw new Exception("Error occurred when increase time.");
        }
    }
}