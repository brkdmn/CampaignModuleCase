using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Interfaces.Service;

public interface IProductService
{
    Task<BaseResponse<ProductDTO>> CreateProduct(ProductDTO productDto);
    Task<BaseResponse<ProductInfoDTO>> GetProduct(string productCode);
}