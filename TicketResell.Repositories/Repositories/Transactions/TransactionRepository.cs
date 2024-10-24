using System.Transactions;
using Repositories.Core.Context;
using Repositories.Core.Entities;
using Repositories.Core.Helper;
using Microsoft.EntityFrameworkCore;
using TicketResell.Repositories.Logger;
using Microsoft.Extensions.Logging;
using Repositories.Core.Dtos.OrderDetail;

namespace Repositories.Repositories;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public readonly TicketResellManagementContext _context;
    public readonly IAppLogger _logger;

    public TransactionRepository(IAppLogger logger, TicketResellManagementContext context) : base(context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderDetail>> GetTransactionsByDateAsync(string sellerId, DateRange dateRange)
    {
        return (await _context.OrderDetails.Include(od => od.Ticket)
            .Include(od => od.Order)
            .ThenInclude(o => o.Buyer).Where(od => od != null &&
                                                   od.Ticket != null &&
                                                   od.Ticket.SellerId == sellerId &&
                                                   od.Order != null &&
                                                   od.Order.Date >= dateRange.StartDate &&
                                                   od.Order.Date <= dateRange.EndDate).ToListAsync())!;
    }

    public async Task<double?> CalculatorTotal(string sellerId, DateRange dateRange)
    {
        return await _context.OrderDetails.Where(od => od != null &&
                                                       od.Ticket != null &&
                                                       od.Ticket.SellerId == sellerId &&
                                                       od.Order != null &&
                                                       od.Order.Date >= dateRange.StartDate &&
                                                       od.Order.Date <= dateRange.EndDate)
            .SumAsync(od => od.Price * od.Quantity);
    }

    public async Task<List<OrderDetail>> GetTicketOrderDetailsBySeller(string sellerId)
    {
        var result = await _context.OrderDetails
            .Include(od => od.Ticket)
            .Include(od => od.Order)
            .ThenInclude(o => o.Buyer)
            .Where(od => od.Ticket.SellerId == sellerId && od.Order.Status == 0)
            .ToListAsync();

        return result;
    }
}