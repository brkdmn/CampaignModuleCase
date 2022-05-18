using CampaignModule.Domain.DTO;

namespace CampaignModule.Domain.Entity;

public class OrderEntity : BaseEntity
{
    public string ProductCode { get; set; }
    public string CampaignName { get; set; }
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }
    
    public static OrderEntity Build(OrderDTO orderDto, string campaignName, decimal salePrice)
    {
        return new OrderEntity
        {
            Id = Guid.NewGuid(),
            ProductCode = orderDto.ProductCode,
            Quantity = orderDto.Quantity,
            CampaignName = campaignName,
            SalePrice = salePrice,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
    }
 }