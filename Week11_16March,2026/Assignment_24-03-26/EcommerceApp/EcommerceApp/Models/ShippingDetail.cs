using System.ComponentModel.DataAnnotations;

public class ShippingDetail
{
    public int Id { get; set; }

    [Required]
    public string Address { get; set; }

    public string Status { get; set; } // Pending / Shipped

    public int OrderId { get; set; }
    public Order Order { get; set; }
}