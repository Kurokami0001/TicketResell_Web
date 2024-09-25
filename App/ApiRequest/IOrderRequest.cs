using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Core.Helper;

namespace App.ApiRequest;

public interface IOrderRequest
{
    Task<Message?> CreateOrderAsync(Order? order);

    Task<Order?> GetOrderByIdAsync(string orderId);
    Task<IEnumerable<Order>?> GetAllOrdersAsync();
    Task<IEnumerable<Order>?> GetOrdersByBuyerIdAsync(string buyerId);
    Task<IEnumerable<Order>?> GetOrdersByDateRangeAsync(DateRange? dateRange);
    Task<IEnumerable<Order>?> GetOrdersByTotalPriceRangeAsync(DoubleRange? priceDoubleRange);

    Task<Message?> UpdateOrderAsync(Order? order);

    Task<Message?> DeleteOrderAsync(string orderId);
    
    Task<double?> CalculateTotalPriceForOrderAsync(string orderId);
    
    Task<bool?> HasOrder(string orderId);
}