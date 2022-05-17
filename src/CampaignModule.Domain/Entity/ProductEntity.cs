using CampaignModule.Domain.DTO;

namespace CampaignModule.Domain.Entity;

public class ProductEntity : BaseEntity
{
    public string ProductCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public static ProductEntity Build(ProductDTO productDTO)
    {
        return new ProductEntity
        {
            Id = Guid.NewGuid(),
            ProductCode = productDTO.ProductCode,
            Price = productDTO.Price,
            Stock = productDTO.Stock,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
    }
}