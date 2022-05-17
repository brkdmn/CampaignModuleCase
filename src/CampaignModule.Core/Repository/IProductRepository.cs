using CampaignModule.Domain.Entity;

namespace CampaignModule.Core.Repository;

public interface IProductRepository
{
    Task<int> CreateProduct(ProductEntity productEntity);
    Task<ProductEntity?> GetProduct(string productCode);
}