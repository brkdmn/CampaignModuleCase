using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Service;

public interface ICampaignService
{
    Task<BaseResponse<CampaignDTO>> CreateCampaign(CampaignDTO campaignDTO);
    Task<BaseResponse<CampaignInfoDTO>> GetCampaign(string name);
    Task<BaseResponse<string>> IncreaseTime(int time, string name);
    Task<bool> CampaignAvailable(string productCode);
}