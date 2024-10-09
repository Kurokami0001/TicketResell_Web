﻿using AutoMapper;
using Repositories.Core.Dtos.Order;
using Repositories.Core.Entities;
using Repositories.Core.Helper;
using Repositories.Core.Validators;
using TicketResell.Repositories.UnitOfWork;

namespace TicketResell.Services.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private IValidatorFactory _validatorFactory;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidatorFactory validatorFactory)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validatorFactory = validatorFactory;
    }

    public async Task<ResponseModel> CreateOrder(OrderDto dto, bool saveAll = true)
    {
        var order = _mapper.Map<Order>(dto);
        order.Date = DateTime.UtcNow;
        order.Total = 0;
        order.Status = (int)OrderStatus.Pending;

        var validator = _validatorFactory.GetValidator<Order>();
        var validationResult = await validator.ValidateAsync(order);
        if (!validationResult.IsValid)
        {
            return ResponseModel.BadRequest("Validation Error", validationResult.Errors);
        }

        await _unitOfWork.OrderRepository.CreateAsync(order);

        if (saveAll)
            await _unitOfWork.CompleteAsync();

        return ResponseModel.Success($"Successfully created order: {dto.OrderId}", order);
    }

    public async Task<ResponseModel> GetOrderById(string id)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
        return ResponseModel.Success($"Successfully get order: {order?.OrderId}", order);
    }

    public async Task<ResponseModel> GetAllOrders()
    {
        var orders = await _unitOfWork.OrderRepository.GetAllAsync();
        return ResponseModel.Success($"Successfully get all order", orders);
    }

    public async Task<ResponseModel> GetOrdersByBuyerId(string buyerId)
    {
        var orders = await _unitOfWork.OrderRepository.GetOrdersByBuyerIdAsync(buyerId);
        return ResponseModel.Success($"Successfully get order by buyer id: {buyerId}", orders);
    }

    public async Task<ResponseModel> GetOrdersByDateRange(DateRange dateRange)
    {
        dateRange.StartDate ??= DateTime.MinValue;
        dateRange.EndDate ??= DateTime.UtcNow;
        var orders = await _unitOfWork.OrderRepository.GetOrdersByDateRangeAsync(dateRange);
        return ResponseModel.Success($"Successfully get order from {dateRange.StartDate} to {dateRange.EndDate}",
            orders);
    }

    public async Task<ResponseModel> GetOrdersByTotalPriceRange(DoubleRange priceDoubleRange)
    {
        priceDoubleRange.Min ??= 0;
        priceDoubleRange.Max ??= double.MaxValue;
        var orders = await _unitOfWork.OrderRepository.GetOrdersByTotalPriceRangeAsync(priceDoubleRange);
        return ResponseModel.Success(
            $"Successfully get order with price from {priceDoubleRange.Min} to {priceDoubleRange.Max}", orders);
    }

    public async Task<ResponseModel> CalculateTotalPriceForOrder(string orderId)
    {
        var total = await _unitOfWork.OrderRepository.CalculateTotalPriceForOrderAsync(orderId);
        return ResponseModel.Success($"Successfully get total price for order: {orderId}", total);
    }

    public async Task<ResponseModel> UpdateOrder(Order? order, bool saveAll = true)
    {
        var validator = _validatorFactory.GetValidator<Order>();
        if (order != null)
        {
            var validationResult = await validator.ValidateAsync(order);
            if (!validationResult.IsValid)
            {
                return ResponseModel.BadRequest("Validation Error", validationResult.Errors);
            }
        }
        else
        {
            return ResponseModel.BadRequest("Validation Error", "No data");
        }

        _unitOfWork.OrderRepository.Update(order);

        if (saveAll)
            await _unitOfWork.CompleteAsync();

        return ResponseModel.Success($"Successfully updated order: {order.OrderId}", order);
    }

    public async Task<ResponseModel> DeleteOrder(string orderId, bool saveAll = true)
    {
        Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            return ResponseModel.NotFound($"Order not found");
        }
        
        _unitOfWork.OrderRepository.Delete(order);

        if (saveAll)
            await _unitOfWork.CompleteAsync();
        
        return ResponseModel.Success($"Successfully deleted: {order.OrderId}");
    }
}