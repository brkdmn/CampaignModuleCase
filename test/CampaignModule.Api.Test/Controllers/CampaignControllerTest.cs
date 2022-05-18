using System.Net;
using CampaignModule.Api.Controllers;
using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Enum;
using CampaignModule.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CampaignModule.Api.Test.Controllers;

public class CampaignControllerTest
{
    private readonly Mock<ICampaignService> _mockService;
    private readonly CampaignController _controller;
    public CampaignControllerTest()
    {
        _mockService = new Mock<ICampaignService>();
        _controller = new CampaignController(_mockService.Object);
    }
    
    [Fact]
    public async Task Index_ActionExecutes_ReturnsViewForIndex()
    {
        
        var campaignName = "C1";
        var campaignDTO = new CampaignInfoDTO
        {
            AvarageItemPrice = 100,
            Name = "C1",
            Status = CampaignStatus.Active,
            TargetSales = 10,
            TotalSales = 1,
            Turnover = 100
        };
        var tcs = new TaskCompletionSource<BaseResponse<CampaignInfoDTO>>();
        tcs.SetResult(new BaseResponse<CampaignInfoDTO>
        {
                Result = campaignDTO,
                IsSuccess = true,
                Message = campaignDTO.ToString(),
                StatusCode = (int)HttpStatusCode.OK
        });
        
        _mockService.Setup(service => service.GetCampaign(campaignName))
            .Returns(tcs.Task);
        
        var result = await _controller.GetCampaignInfo(campaignName);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<BaseResponse<CampaignInfoDTO>>(objectResult.Value);
    }
}