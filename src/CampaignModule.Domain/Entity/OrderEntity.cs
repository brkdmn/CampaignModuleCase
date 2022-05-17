namespace CampaignModule.Domain.Entity;

public class OrderEntity : BaseEntity
{
    public string? ProductCount { get; set; }
    public string? CampaignName { get; set; }
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }
 }