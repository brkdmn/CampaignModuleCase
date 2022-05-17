namespace CampaignModule.Domain.Entity;

public class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}