using CampaignModule.Domain.DTO;

namespace CampaignModule.Core.Interfaces.Service;

public interface IProductService
{
    Task<ProductDTO> CreateProduct(ProductDTO productDto);
    Task<ProductInfoDTO> GetProduct(string productCode);
}