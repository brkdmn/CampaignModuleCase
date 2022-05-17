using CampaignModule.Domain.Entity;

namespace CampaignModule.Core.Repository;

public interface ICampaignRepository
{
    Task<CampaignEntity?> GetCampaignByProductCode(string productCode);
    Task<CampaignEntity?> GetCampaignByCampaignName(string campaignName);
}