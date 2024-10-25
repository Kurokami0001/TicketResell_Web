using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketResell.Services.Services.Mail
{
    public interface IMailService
    {
        Task<ResponseModel> SendEmailAsync(string to, string subject, string body);
        Task<ResponseModel> SendOtpAsync(string to);
        Task<ResponseModel> SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
    }
}