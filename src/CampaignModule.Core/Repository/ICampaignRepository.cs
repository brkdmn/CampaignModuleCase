using CampaignModule.Domain.Entity;

namespace CampaignModule.Core.Repository;

public interface ICampaignRepository
{
    Task<int> CreateCampaign(CampaignEntity campaignEntity);
    Task<CampaignEntity?> GetCampaignByProductCode(string productCode);
    Task<CampaignEntity?> GetCampaignByCampaignName(string campaignName);
    Task<int> UpdateCampaign(CampaignEntity campaignEntity);
}