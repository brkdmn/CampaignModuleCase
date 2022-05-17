namespace CampaignModule.Core.Repository;

public interface IOrderRepository
{
    Task<int> GetSalesCountByCampaignNameAndProductCode(string? campaignName, string? productCode);
    Task<decimal> GetTotalPriceByCampaignNameAndProductCode(string? campaignName, string? productCode);
}