using OrderProcessing.API.Models;

namespace OrderProcessing.API.Services;

public class OrderService : IOrderService
{
    public async Task<bool> PlaceOrderAsync(Order order)
    {
        // Real business logic would go here
        // e.g., validate, persist, call payment gateway
        await Task.Delay(10); // simulate async work
        return order.Quantity > 0 && order.Price > 0;
    }
}