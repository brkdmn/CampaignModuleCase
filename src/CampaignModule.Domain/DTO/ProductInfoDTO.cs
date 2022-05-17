namespace CampaignModule.Domain.DTO;

public class ProductInfoDTO
{
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public override string ToString()
    {
        return $"Product Info; price {Price}, stock {Stock}";
    }
}