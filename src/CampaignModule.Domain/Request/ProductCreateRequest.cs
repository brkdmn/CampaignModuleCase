namespace CampaignModule.Domain.Request;

public class ProductCreateRequest
{
    public string ProductCode { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int Stock { get; set; }
}