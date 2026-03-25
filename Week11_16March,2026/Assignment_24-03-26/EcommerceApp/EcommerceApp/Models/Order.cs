using System.ComponentModel.DataAnnotations;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem>? OrderItems { get; set; }
    public ShippingDetail? ShippingDetail { get; set; }
}