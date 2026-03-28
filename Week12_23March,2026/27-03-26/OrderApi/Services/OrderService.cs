using AutoMapper;
using log4net;
using OrderApi.DTOs;
using OrderApi.Models;
namespace OrderApi.Services
{
   

    public interface IOrderService
    {
        Task<string> CreateOrder(OrderDto dto);
    }

    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private static readonly ILog log = LogManager.GetLogger(typeof(OrderService));

        public OrderService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<string> CreateOrder(OrderDto dto)
        {
            log.Info("Order creation started");

            if (dto.Quantity <= 0)
            {
                log.Warn("Invalid quantity");
                return "Invalid quantity";
            }

            try
            {
                var order = _mapper.Map<Order>(dto);

                // Simulate DB Save
                await Task.Delay(100);

                log.Info("Order created successfully");

                return "Order Created";
            }
            catch (Exception ex)
            {
                log.Error("Order failed", ex);
                throw;
            }
        }
    }
}
