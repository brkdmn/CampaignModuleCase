namespace CampaignModule.Domain.Request;

public class CampaignCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int PriceManipulationLimit { get; set; }
    public int TargetSalesCount { get; set; }
}