using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Repository;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Response;
using Mapster;

namespace CampaignModule.Core.Service;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IOrderRepository _orderRepository;
    
    public ProductService(
        IProductRepository productRepository, 
        ICampaignRepository campaignRepository,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _campaignRepository = campaignRepository;
        _orderRepository = orderRepository;
    }
    public async Task<BaseResponse<ProductDTO>> CreateProduct(ProductDTO productDTO)
    {
        var product = await _productRepository.GetProduct(productDTO.ProductCode);

        if (product != null)
        {
            throw new AppException("Product is already exist.", HttpStatusCode.BadRequest);
        }

        product = ProductEntity.Build(productDTO);
        var createResult = await _productRepository.CreateProduct(product);
        if (createResult != 1)
        {
            throw new Exception("There is a technical problem.");
        }
        
        return new BaseResponse<ProductDTO>
        {
            IsSuccess = true,
            Result = productDTO
        };
    }

    private async Task<decimal> CalculateProductPrice(ProductEntity productEntity)
    {
        var campaign = await _campaignRepository.GetCampaignByProductCode(productEntity.ProductCode);
        var totalSalesCount =
            await _orderRepository.GetSalesCountByCampaignNameAndProductCode(campaign.Name, productEntity.ProductCode);

        if (CampaignAvailable(campaign, totalSalesCount))
        {
            return (campaign.PriceManipulationLimit / productEntity.Price * 100) / campaign.CurrentDuration;
        }

        return productEntity.Price;
    }

    private bool CampaignAvailable(CampaignEntity campaignEntity, int totalSalesCount)
    {
        return campaignEntity.Duration > campaignEntity.CurrentDuration
               && totalSalesCount < campaignEntity.TargetSalesCount;
    }
}