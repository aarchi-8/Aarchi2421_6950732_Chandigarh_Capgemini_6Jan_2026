using OrderProcessing.API.Models;

namespace OrderProcessing.API.Services;

public interface IOrderService
{
    Task<bool> PlaceOrderAsync(Order order);
}