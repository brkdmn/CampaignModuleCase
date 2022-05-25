using CampaignModule.Domain.DTO;

namespace CampaignModule.Domain.Entity;

public class Order : BaseEntity
{
    public string ProductCode { get; set; }
    public string CampaignName { get; set; }
    public int Quantity { get; set; }
    public decimal SalePrice { get; set; }

    public static Order Build(OrderDTO orderDto, string campaignName, decimal salePrice)
    {
        return new Order
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