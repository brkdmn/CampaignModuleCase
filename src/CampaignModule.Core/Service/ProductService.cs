using System.Net;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Service;

public class ProductService : IProductService
{
    private readonly ICampaignService _campaignService;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        ICampaignService campaignService, IUnitOfWork unitOfWork)
    {
        _campaignService = campaignService;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<ProductDTO>> CreateProduct(ProductDTO productDTO)
    {
        var response = new BaseResponse<ProductDTO>();
        var product = await _unitOfWork.Product.GetByCodeAsync(productDTO.ProductCode);

        if (product != null)
        {
            response.Message = "Product is already exist.";
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response;
        }

        product = Product.Build(productDTO);
        var createResult = await _unitOfWork.Product.AddAsync(product);
        if (createResult != 1) throw new Exception("There is a technical problem.");

        response.IsSuccess = true;
        response.Result = productDTO;
        response.Message = productDTO.ToString();
        response.StatusCode = (int)HttpStatusCode.Created;
        return response;
    }

    public async Task<BaseResponse<ProductInfoDTO>> GetProduct(string productCode)
    {
        var response = new BaseResponse<ProductInfoDTO>();
        var productEntity = await _unitOfWork.Product.GetByCodeAsync(productCode);

        if (productEntity != null)
        {
            var campaign = await _unitOfWork.Campaign.GetCampaignByProductCodeAsync(productEntity.ProductCode);
            var productInfoDTO = new ProductInfoDTO
            {
                CampaignName = campaign?.Name ?? string.Empty,
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

    private async Task<decimal> CalculateProductPrice(Product product)
    {
        if (!await _campaignService.CampaignAvailable(product.ProductCode)) return product.Price;
        var campaign = await _unitOfWork.Campaign.GetCampaignByProductCodeAsync(product.ProductCode);
        return product.Price
               - campaign.PriceManipulationLimit / product.Price * 100 /
               (campaign.Duration - campaign.CurrentDuration);
    }

    private async Task<int> CalculateProductStock(int totalStock, string productCode)
    {
        var saleCount = await _unitOfWork.Order.GetSalesCountByProductCodeAsync(productCode);
        return totalStock - (saleCount?.Total ?? 0);
    }
}