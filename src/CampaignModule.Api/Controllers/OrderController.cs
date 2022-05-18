using CampaignModule.Core.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[Route("api/order")]
[ApiController]
public class OrderController : Controller
{
    private readonly IOrderService _orderService; 
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder(OrderCreateRequest orderCreateRequest)
    {
        var result = await _orderService.CreateOrder(orderCreateRequest.Adapt<OrderDTO>());
        return StatusCode(result.StatusCode, result);
    }
}