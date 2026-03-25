using E_Commerce_Order_Management.Models;

namespace E_Commerce_Order_Management.ViewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public ShippingDetail ShippingDetail { get; set; }
    }
}
