using CampaignModule.Domain.DTO;

namespace CampaignModule.Domain.Entity;

public class Product : BaseEntity
{
    public string ProductCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public static Product Build(ProductDTO productDTO)
    {
        return new Product
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