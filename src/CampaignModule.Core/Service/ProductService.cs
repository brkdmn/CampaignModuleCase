using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;

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

    public async Task<ProductDTO> CreateProduct(ProductDTO productDto)
    {
        var product = await _unitOfWork.Product.GetByCodeAsync(productDto.ProductCode!);

        if (product != null)
            throw new AppException("Product is already exist.", HttpStatusCode.BadRequest);

        product = Product.Build(productDto);
        var createResult = await _unitOfWork.Product.AddAsync(product);

        if (createResult != 1)
            throw new Exception("There is a technical problem.");

        return productDto;
    }

    public async Task<ProductInfoDTO> GetProduct(string productCode)
    {
        var product = await _unitOfWork.Product.GetByCodeAsync(productCode);

        if (product == null)
            throw new AppException($"Product {productCode} is not found.", HttpStatusCode.NotFound);

        var campaign = await _unitOfWork.Campaign.GetCampaignByProductCodeAsync(product.ProductCode);
        var productInfoDto = new ProductInfoDTO
        {
            CampaignName = campaign?.Name ?? string.Empty,
            Price = await CalculateProductPrice(product),
            Stock = await CalculateProductStock(product.Stock, product.ProductCode)
        };
        return productInfoDto;
    }

    private async Task<decimal> CalculateProductPrice(Product product)
    {
        if (!await _campaignService.CampaignAvailable(product.ProductCode)) return product.Price;
        var campaign = await _unitOfWork.Campaign.GetCampaignByProductCodeAsync(product.ProductCode);

        if (campaign != null)
            return product.Price
                   - campaign.PriceManipulationLimit / product.Price * 100 /
                   (campaign.Duration - campaign.CurrentDuration);

        return product.Price;
    }

    private async Task<int> CalculateProductStock(int totalStock, string productCode)
    {
        var saleCount = await _unitOfWork.Order.GetSalesCountByProductCodeAsync(productCode);
        return totalStock - (saleCount?.Total ?? 0);
    }
}