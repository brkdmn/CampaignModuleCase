namespace CampaignModule.Domain.Response;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
}