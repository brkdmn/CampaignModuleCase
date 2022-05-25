using CampaignModule.Domain.DTO;

namespace CampaignModule.Core.Interfaces.Service;

public interface ICampaignService
{
    Task<CampaignDTO> CreateCampaign(CampaignDTO campaignDto);
    Task<CampaignInfoDTO> GetCampaign(string name);
    Task<string> IncreaseTime(int time, string name);
    Task<bool> CampaignAvailable(string productCode);
}