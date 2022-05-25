using CampaignModule.Core.Interfaces.Repository;

namespace CampaignModule.Core.Interfaces.Infrastructer;

public interface IUnitOfWork
{
    IProductRepository Product { get; }
    ICampaignRepository Campaign { get; }
    IOrderRepository Order { get; }
}