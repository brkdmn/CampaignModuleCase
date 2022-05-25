using CampaignModule.Domain.Entity;

namespace CampaignModule.Core.Interfaces.Repository;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<TotalQuantity?> GetSalesCountByCampaignNameAndProductCodeAsync(string? campaignName, string? productCode);
    Task<TotalPrice?> GetTotalPriceByCampaignNameAndProductCodeAsync(string? campaignName, string? productCode);
    Task<TotalQuantity?> GetSalesCountByProductCodeAsync(string? productCode);
}