using Microsoft.EntityFrameworkCore;
using TicketResell.Repository.Core.Context;
using TicketResell.Repository.Core.Entities;

namespace TicketResell.Repository.Repositories
{
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly TicketResellManagementContext _context;

        public OrderDetailRepository(TicketResellManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByUsernameAsync(string username)
        {
            return await _context.OrderDetails.Where(od =>
                    od.Order != null && od.Order.Buyer != null && od.Order.Buyer.Username == username)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByBuyerIdAsync(string userId)
        {
            return await _context.OrderDetails.Where(od => od.Order != null && od.Order.BuyerId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsBySellerIdAsync(string sellerId)
        {
            return await _context.OrderDetails
                .Where(od => od.Ticket != null && od.Ticket.SellerId == sellerId)
                .ToListAsync();
        }
    }
}