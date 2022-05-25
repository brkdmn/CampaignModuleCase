using CampaignModule.Domain.Entity;

namespace CampaignModule.Core.Interfaces.Repository;

public interface ICampaignRepository : IGenericRepository<Campaign>
{
    Task<Campaign?> GetCampaignByProductCodeAsync(string productCode);
}