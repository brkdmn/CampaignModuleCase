using CampaignModule.Domain.Enum;

namespace CampaignModule.Domain.DTO;

public class CampaignInfoDTO
{
    public string Name { get; set; } = string.Empty;
    public CampaignStatus Status { get; set; }
    public int TargetSales { get; set; }
    public int TotalSales { get; set; }
    public decimal Turnover { get; set; }
    public decimal AvarageItemPrice { get; set; }

    public override string ToString()
    {
        return @$"Campaign {Name} info; Status {Status}, Target Sales {TargetSales},Total Sales {TotalSales}, Turnover {Turnover}, Average Item Price {AvarageItemPrice}";
    }
}