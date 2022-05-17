namespace CampaignModule.Domain.Request;

public class OrderCreateRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
}