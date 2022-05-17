namespace CampaignModule.Domain.Response;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}