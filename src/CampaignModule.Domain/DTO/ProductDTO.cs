namespace CampaignModule.Domain.DTO;

public class ProductDTO
{
    public string? ProductCode { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public ProductDTO(string? productCode, decimal price, int stock)
    {
        ProductCode = productCode;
        Price = price;
        Stock = stock;
    }
}