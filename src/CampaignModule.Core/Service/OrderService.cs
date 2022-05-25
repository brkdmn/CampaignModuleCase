using System.Net;
using CampaignModule.Core.Helper;
using CampaignModule.Core.Interfaces.Infrastructer;
using CampaignModule.Core.Interfaces.Service;
using CampaignModule.Domain.DTO;
using CampaignModule.Domain.Entity;

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

    public async Task<OrderDTO> CreateOrder(OrderDTO orderDto)
    {
        var product = await _productService.GetProduct(orderDto.ProductCode);
        if (product == null)
            throw new AppException("Product is not found.", HttpStatusCode.NotFound);


        if (product.Stock < orderDto.Quantity)
            throw new AppException("Product is stock out.", HttpStatusCode.BadRequest);

        var orderEntity = Order.Build(orderDto, product.CampaignName, product.Price);
        var createResult = await _unitOfWork.Order.AddAsync(orderEntity);
        if (createResult != 1)
            throw new Exception("There is a technical problem.");

        return orderDto;
    }
}