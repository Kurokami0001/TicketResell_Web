using Backend.Core.Entities;
using Backend.Core.Helper;

namespace Backend.Repositories;

public interface ITransactionRepository
{
    Task<IEnumerable<OrderDetail?>> GetTransactionsByDateAsync(string sellerId, DateRange dateRange);
    Task<double?> CalculatorTotal(string sellerId, DateRange dateRange);
    Task<IEnumerable<User?>> GetUserBuyTicket(string sellerId);
}