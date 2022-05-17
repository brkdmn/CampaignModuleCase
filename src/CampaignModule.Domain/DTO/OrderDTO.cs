namespace CampaignModule.Domain.DTO;

public class OrderDTO
{
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public override string ToString()
    {
        return $"Order created; product {ProductCode}, quantity {Quantity}";
    }
}