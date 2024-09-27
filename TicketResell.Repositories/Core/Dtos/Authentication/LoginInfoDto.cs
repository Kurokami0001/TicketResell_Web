using Repositories.Core.Dtos.User;
using Repositories.Core.Entities;

namespace TicketResell.Repositories.Core.Dtos.Authentication;

public class LoginInfoDto
{
    public UserReadDto User { get; set; }
    public string AccessKey { get; set; }
}