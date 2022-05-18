using System.Net;
using CampaignModule.Core.Repository;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Enum;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Service;

public class CampaignService : ICampaignService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    
    public CampaignService(
        ICampaignRepository campaignRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _campaignRepository = campaignRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }
    public async Task<BaseResponse<CampaignDTO>> CreateCampaign(CampaignDTO campaignDTO)
    {
        var response = new BaseResponse<CampaignDTO>();
        var product = await _productRepository.GetProduct(campaignDTO.ProductCode);
        if (product == null)
        {
            response.Message = "Product is not found.";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response;
        }
        
        var campaignEntity = await _campaignRepository.GetCampaignByCampaignName(campaignDTO.Name);
        if (campaignEntity == null)
        {
            response.Message = "Campaign is already exist.";
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response;
        }

        campaignEntity = CampaignEntity.Build(campaignDTO, 0);
        var createResult = await _campaignRepository.CreateCampaign(campaignEntity);
        if (createResult != 1)
        {
            throw new Exception("There is a technical problem.");
        }

        response.IsSuccess = true;
        response.Result = campaignDTO;
        response.Message = campaignDTO.ToString();
        response.StatusCode = (int)HttpStatusCode.Created;
        return response;
    }

    public async Task<BaseResponse<CampaignInfoDTO>> GetCampaign(string name)
    {
        var response = new BaseResponse<CampaignInfoDTO>();
        
        var campaignEntity = await _campaignRepository.GetCampaignByCampaignName(name);
        if (campaignEntity == null)
        {
            response.Message = "Campaign is not found.";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response;
        }
        
        var totalSale =
            (await _orderRepository.GetSalesCountByCampaignNameAndProductCode(name, campaignEntity.ProductCode))?.Total ?? 0;
        var totalPrice =
            (await _orderRepository.GetTotalPriceByCampaignNameAndProductCode(name, campaignEntity.ProductCode))?.Total ?? 0;
        var campaignInfoDTO = new CampaignInfoDTO
        {
            Name = name,
            AvarageItemPrice = totalSale == 0 ? 0 : totalPrice / totalSale,
            Status = (await CampaignAvailable(campaignEntity.ProductCode))
                ? CampaignStatus.Active
                : CampaignStatus.Passive,
            TargetSales = campaignEntity.TargetSalesCount,
            TotalSales = totalSale,
            Turnover = totalPrice
        };

        response.Message = campaignInfoDTO.ToString();
        response.IsSuccess = true;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.Result = campaignInfoDTO;
        return response;
    }

    public async Task<BaseResponse<string>> IncreaseTime(int time, string name)
    {
        var response = new BaseResponse<string>();
        
        var campaignEntity = await _campaignRepository.GetCampaignByCampaignName(name);
        var newDuration = campaignEntity.CurrentDuration + time;

        campaignEntity.CurrentDuration = newDuration;
        campaignEntity.UpdatedDate = DateTime.Now;

        var updateResult = await _campaignRepository.UpdateCampaign(campaignEntity);
        if (updateResult != 1)
        {
            throw new Exception("Error occurred when update campaign");
        }

        response.Message = $"Time is {newDuration:D2}:00";
        response.Result = $"{newDuration:D2}:00";
        response.IsSuccess = true;
        response.StatusCode = (int)HttpStatusCode.OK;
        return response;
    }
    
    public async Task<bool> CampaignAvailable(string productCode)
    {
        var campaignEntity = await _campaignRepository.GetCampaignByProductCode(productCode);
        if (campaignEntity == null) return false;
        
        var totalSalesCount =
            await _orderRepository.GetSalesCountByCampaignNameAndProductCode(campaignEntity.Name, campaignEntity.ProductCode);
        return campaignEntity.Duration > campaignEntity.CurrentDuration
               && campaignEntity.CurrentDuration > 0
               && (totalSalesCount?.Total ?? 0) < campaignEntity.TargetSalesCount;
    }
}