using CampaignModule.Domain.DTO;

namespace CampaignModule.Domain.Entity;

public class Campaign : BaseEntity
{
    public string Name { get; set; }
    public string ProductCode { get; set; }
    public int Duration { get; set; }
    public int CurrentDuration { get; set; }
    public int PriceManipulationLimit { get; set; }
    public int TargetSalesCount { get; set; }

    public static Campaign Build(CampaignDTO campaignDto, int currentDuration)
    {
        return new Campaign
        {
            Id = Guid.NewGuid(),
            ProductCode = campaignDto.ProductCode,
            Name = campaignDto.Name,
            Duration = campaignDto.Duration,
            CurrentDuration = currentDuration,
            PriceManipulationLimit = campaignDto.PriceManipulationLimit,
            TargetSalesCount = campaignDto.TargetSalesCount,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
    }
}