using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Core.Dtos.Payment;

namespace TicketResell.Services.Services.Payments
{
    public interface IMomoService
    {
        public Task<ResponseModel> CreatePaymentAsync(PaymentDto paymentRequest, double amount);
        public Task<ResponseModel> CheckTransactionStatus(string orderId);
    }
}