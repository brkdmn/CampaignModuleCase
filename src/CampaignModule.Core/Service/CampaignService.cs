using System.Net;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Enum;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Service;

public class CampaignService : ICampaignService
{
    private readonly IUnitOfWork _unitOfWork;

    public CampaignService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<CampaignDTO>> CreateCampaign(CampaignDTO campaignDTO)
    {
        var response = new BaseResponse<CampaignDTO>();
        var product = await _unitOfWork.Product.GetByCodeAsync(campaignDTO.ProductCode);
        if (product == null)
        {
            response.Message = "Product is not found.";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response;
        }

        var campaignEntity = await _unitOfWork.Campaign.GetByCodeAsync(campaignDTO.Name);
        if (campaignEntity == null)
        {
            response.Message = "Campaign is already exist.";
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response;
        }

        campaignEntity = Campaign.Build(campaignDTO, 0);
        var createResult = await _unitOfWork.Campaign.AddAsync(campaignEntity);
        if (createResult != 1) throw new Exception("There is a technical problem.");

        response.IsSuccess = true;
        response.Result = campaignDTO;
        response.Message = campaignDTO.ToString();
        response.StatusCode = (int)HttpStatusCode.Created;
        return response;
    }

    public async Task<BaseResponse<CampaignInfoDTO>> GetCampaign(string name)
    {
        var response = new BaseResponse<CampaignInfoDTO>();

        var campaignEntity = await _unitOfWork.Campaign.GetByCodeAsync(name);
        if (campaignEntity == null)
        {
            response.Message = "Campaign is not found.";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response;
        }

        var totalSale =
            (await _unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(name, campaignEntity.ProductCode))
            ?.Total ?? 0;
        var totalPrice =
            (await _unitOfWork.Order.GetTotalPriceByCampaignNameAndProductCodeAsync(name, campaignEntity.ProductCode))
            ?.Total ?? 0;
        var campaignInfoDTO = new CampaignInfoDTO
        {
            Name = name,
            AvarageItemPrice = totalSale == 0 ? 0 : totalPrice / totalSale,
            Status = await CampaignAvailable(campaignEntity.ProductCode)
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

        var campaignEntity = await _unitOfWork.Campaign.GetByCodeAsync(name);
        var newDuration = campaignEntity.CurrentDuration + time;

        campaignEntity.CurrentDuration = newDuration;
        campaignEntity.UpdatedDate = DateTime.Now;

        var updateResult = await _unitOfWork.Campaign.UpdateAsync(campaignEntity);
        if (updateResult != 1) throw new Exception("Error occurred when update campaign");

        response.Message = $"Time is {newDuration:D2}:00";
        response.Result = $"{newDuration:D2}:00";
        response.IsSuccess = true;
        response.StatusCode = (int)HttpStatusCode.OK;
        return response;
    }

    public async Task<bool> CampaignAvailable(string productCode)
    {
        var campaignEntity = await _unitOfWork.Campaign.GetCampaignByProductCodeAsync(productCode);
        if (campaignEntity == null) return false;

        var totalSalesCount =
            await _unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(campaignEntity.Name,
                campaignEntity.ProductCode);
        return campaignEntity.Duration > campaignEntity.CurrentDuration
               && campaignEntity.CurrentDuration > 0
               && (totalSalesCount?.Total ?? 0) < campaignEntity.TargetSalesCount;
    }
}