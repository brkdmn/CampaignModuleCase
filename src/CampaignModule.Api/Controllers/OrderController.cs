using System.Net;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Request;
using CampaignModule.Domain.Response;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CampaignModule.Api.Controllers;

[Route("api/order")]
public class OrderController : BaseController<OrderController>
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder(OrderCreateRequest orderCreateRequest)
    {
        try
        {
            var orderDto = await _orderService.CreateOrder(orderCreateRequest.Adapt<OrderDTO>());
            var response = new BaseResponse<OrderDTO>
            {
                IsSuccess = true,
                Result = orderDto,
                Message = orderDto.ToString(),
                StatusCode = (int)HttpStatusCode.OK
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Error,
                "Error occurred when creating order, productCode:{}, Ex: {}", orderCreateRequest.ProductCode,
                e.Message);
            throw new Exception("Error occurred when creating order.");
        }
    }
}