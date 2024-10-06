using TicketResell.Repositories.Core.Dtos.Cart;

namespace TicketResell.Services.Services.Carts;

public interface ICartService
{
    Task<ResponseModel> GetCart(string userId);
    Task<ResponseModel> AddToCart(CartItemDto cartItem);
    Task<ResponseModel> UpdateCartItem(CartItemDto cartItem);
    Task<ResponseModel> RemoveFromCart(string userId, string ticketId);
    Task<ResponseModel> ClearCart(string userId);
    Task<ResponseModel> GetCartTotal(string userId);
}