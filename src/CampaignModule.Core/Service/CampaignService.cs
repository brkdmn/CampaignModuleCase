using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Enum;

namespace CampaignModule.Core.Service;

public class CampaignService : ICampaignService
{
    private readonly IUnitOfWork _unitOfWork;

    public CampaignService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignDTO> CreateCampaign(CampaignDTO campaignDto)
    {
        var product = await _unitOfWork.Product.GetByCodeAsync(campaignDto.ProductCode);
        if (product == null)
            throw new AppException("Product is not found.", HttpStatusCode.NotFound);

        var campaignEntity = await _unitOfWork.Campaign.GetByCodeAsync(campaignDto.Name);
        if (campaignEntity != null)
            throw new AppException("Campaign is already exist.", HttpStatusCode.BadRequest);

        campaignEntity = Campaign.Build(campaignDto, 0);
        var createResult = await _unitOfWork.Campaign.AddAsync(campaignEntity);
        if (createResult != 1)
            throw new Exception("There is a technical problem.");

        return campaignDto;
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

    public async Task<CampaignInfoDTO> GetCampaign(string name)
    {
        var campaignEntity = await _unitOfWork.Campaign.GetByCodeAsync(name);
        if (campaignEntity == null)
            throw new AppException("Campaign is not found.", HttpStatusCode.NotFound);

        var totalSale =
            (await _unitOfWork.Order.GetSalesCountByCampaignNameAndProductCodeAsync(name, campaignEntity.ProductCode))
            ?.Total ?? 0;
        var totalPrice =
            (await _unitOfWork.Order.GetTotalPriceByCampaignNameAndProductCodeAsync(name, campaignEntity.ProductCode))
            ?.Total ?? 0;

        return new CampaignInfoDTO
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
    }

    public async Task<string> IncreaseTime(int time, string name)
    {
        var campaignEntity = await _unitOfWork.Campaign.GetByCodeAsync(name);

        if (campaignEntity == null)
            throw new AppException("Campaign is not found", HttpStatusCode.NotFound);

        var newDuration = campaignEntity.CurrentDuration + time;
        campaignEntity.CurrentDuration = newDuration;
        campaignEntity.UpdatedDate = DateTime.Now;

        var updateResult = await _unitOfWork.Campaign.UpdateAsync(campaignEntity);
        if (updateResult != 1)
            throw new Exception("Error occurred when update campaign");

        return $"Time is {newDuration:D2}:00";
    }
}