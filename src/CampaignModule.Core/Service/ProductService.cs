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
        var response = new BaseResponse<ProductDTO>();
        var product = await _productRepository.GetProduct(productDTO.ProductCode);

        if (product != null)
        {
            response.Message = "Product is already exist.";
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response;
        }

        product = ProductEntity.Build(productDTO);
        var createResult = await _productRepository.CreateProduct(product);
        if (createResult != 1)
        {
            throw new Exception("There is a technical problem.");
        }

        response.IsSuccess = true;
        response.Result = productDTO;
        response.Message = productDTO.ToString();
        response.StatusCode = (int)HttpStatusCode.OK;
        return response;
    }

    public async Task<BaseResponse<ProductInfoDTO>> GetProduct(string productCode)
    {
        var response = new BaseResponse<ProductInfoDTO>();
        var productEntity = await _productRepository.GetProduct(productCode);

        if (productEntity != null)
        {
            var productInfoDTO = new ProductInfoDTO
            {
                Price = await CalculateProductPrice(productEntity),
                Stock = await CalculateProductStock(productEntity.Stock, productEntity.ProductCode)
            };
            
            response.IsSuccess = true;
            response.Result = productInfoDTO;
            response.Message = productInfoDTO.ToString();
            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        response.Message = $"Product {productCode} is not found.";
        response.StatusCode = (int)HttpStatusCode.NotFound;
        return response;
    }

    private async Task<decimal> CalculateProductPrice(ProductEntity productEntity)
    {
        var campaign = await _campaignRepository.GetCampaignByProductCode(productEntity.ProductCode);
        if (campaign == null) return productEntity.Price;
        
        var saleCount =
            await _orderRepository.GetSalesCountByCampaignNameAndProductCode(campaign.Name, productEntity.ProductCode);
        if (CampaignAvailable(campaign, saleCount?.Total ?? 0))
        {
            return (campaign.PriceManipulationLimit / productEntity.Price * 100) / campaign.CurrentDuration;
        }

        return productEntity.Price;
    }

    private async Task<int> CalculateProductStock(int totalStock, string productCode)
    {
        var saleCount = await _orderRepository.GetSalesCountByProductCode(productCode);
        return totalStock - (saleCount?.Total ?? 0);
    }

    private bool CampaignAvailable(CampaignEntity campaignEntity, int totalSalesCount)
    {
        return campaignEntity.Duration > campaignEntity.CurrentDuration
               && totalSalesCount < campaignEntity.TargetSalesCount;
    }
}