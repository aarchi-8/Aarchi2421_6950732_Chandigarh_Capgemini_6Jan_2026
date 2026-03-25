namespace E_Commerce_Order_Management.Models
{
    public class ShippingDetail
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public string Status { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
