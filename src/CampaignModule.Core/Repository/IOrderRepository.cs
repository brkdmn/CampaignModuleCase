using CampaignModule.Domain.Entity;

namespace CampaignModule.Core.Repository;

public interface IOrderRepository
{
    Task<TotalQuantity?> GetSalesCountByCampaignNameAndProductCode(string? campaignName, string? productCode);
    Task<TotalPrice?> GetTotalPriceByCampaignNameAndProductCode(string? campaignName, string? productCode);
    Task<TotalQuantity?> GetSalesCountByProductCode(string? productCode);
}