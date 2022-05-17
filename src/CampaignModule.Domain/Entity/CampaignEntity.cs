namespace CampaignModule.Domain.Entity;

public class CampaignEntity : BaseEntity
{
    public string? Name { get; set; }
    public string? ProductCode { get; set; }
    public int Duration { get; set; }
    public int CurrentDuration { get; set; }
    public int PriceManipulationLimit { get; set; }
    public int TargetSalesCount { get; set; }
}
