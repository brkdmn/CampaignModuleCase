using CampaignModule.Domain.DTO;

namespace CampaignModule.Core.Interfaces.Service;

public interface IOrderService
{
    Task<OrderDTO> CreateOrder(OrderDTO orderDto);
}