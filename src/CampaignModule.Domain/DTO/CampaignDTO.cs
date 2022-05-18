namespace CampaignModule.Domain.DTO;

public class CampaignDTO
{
    public string Name { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int PriceManipulationLimit { get; set; }
    public int TargetSalesCount { get; set; }

    public override string ToString()
    {
        return @$"Campaign created; name {Name}, 
                    product {ProductCode}, 
                    duration {Duration},
                    limit {PriceManipulationLimit}, 
                    target sales count {TargetSalesCount}";
    }
}