using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Repository;

namespace CampaignModule.Core.Infrastructer;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IProductRepository product, ICampaignRepository campaign, IOrderRepository order)
    {
        Product = product;
        Campaign = campaign;
        Order = order;
    }

    public IProductRepository Product { get; }
    public ICampaignRepository Campaign { get; }
    public IOrderRepository Order { get; }
}