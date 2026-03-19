using System.Collections.Generic;

namespace ShoppingCartApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<CartItem> Items { get; set; }

        public string Address { get; set; }
        public string PaymentMethod { get; set; }
    }
}