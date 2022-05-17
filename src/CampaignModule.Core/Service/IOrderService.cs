using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Service;

public interface IOrderService
{
    Task<BaseResponse<OrderDTO>> CreateOrder(OrderDTO orderDto);
}