using System.Collections.Generic;

namespace FunctionsIntro.Models
{
    public class Order
    {
        public string StateAbbreviation { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem {
        public int ItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}