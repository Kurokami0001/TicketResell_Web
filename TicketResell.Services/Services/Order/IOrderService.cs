﻿using Repositories.Core.Dtos.Order;
using Repositories.Core.Helper;
using Repositories.Core.Entities;

namespace TicketResell.Services.Services;

public interface IOrderService
{
    public Task<ResponseModel> CreateOrder(OrderDto dto, bool saveAll = true);
    public Task<ResponseModel> GetOrderById(string id);
    public Task<ResponseModel> GetAllOrders();
    public Task<ResponseModel> GetOrdersByBuyerId(string buyerId);
    public Task<ResponseModel> GetOrdersByDateRange(DateRange dateRange);
    public Task<ResponseModel> GetOrdersByTotalPriceRange(DoubleRange priceDoubleRange);
    public Task<ResponseModel> CalculateTotalPriceForOrder(string orderId);
    public Task<ResponseModel> UpdateOrder(Order? order, bool saveAll = true);
    public Task<ResponseModel> DeleteOrder(string orderId, bool saveAll = true);
    Task<ResponseModel> SetOrderStatus(string orderId, int status);
}