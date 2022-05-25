using System.Net;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;
using CampaignModule.Domain.Response;

namespace CampaignModule.Core.Service;

public class OrderService : IOrderService
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IProductService productService, IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<OrderDTO>> CreateOrder(OrderDTO orderDto)
    {
        var response = new BaseResponse<OrderDTO>();
        var product = (await _productService.GetProduct(orderDto.ProductCode)).Result;
        if (product == null)
        {
            response.Message = "Product is not found.";
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response;
        }

        if (product.Stock < orderDto.Quantity)
        {
            response.Message = "Product stock is not enough.";
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response;
        }

        var orderEntity = Order.Build(orderDto, product.CampaignName, product.Price);
        var createResult = await _unitOfWork.Order.AddAsync(orderEntity);
        if (createResult != 1) throw new Exception("There is a technical problem.");

        response.IsSuccess = true;
        response.Result = orderDto;
        response.Message = orderDto.ToString();
        response.StatusCode = (int)HttpStatusCode.Created;
        return response;
    }
}