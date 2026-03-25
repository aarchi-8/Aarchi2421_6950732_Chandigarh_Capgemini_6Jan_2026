using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Range(1, 100000)]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    // 🔥 FIX
    public Category? Category { get; set; }

    // 🔥 FIX
    public List<OrderItem>? OrderItems { get; set; }
}