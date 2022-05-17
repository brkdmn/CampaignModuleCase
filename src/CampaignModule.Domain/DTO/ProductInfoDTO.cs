namespace CampaignModule.Domain.DTO;

public class ProductInfoDTO
{
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"Product Info; price {Price}, stock {Stock}";
    }
}