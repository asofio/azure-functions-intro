using System.Collections.Generic;

namespace FunctionsIntro.Models
{
    public class OrderResult
    {
        public OrderResult()
        {
            Items = new List<OrderResultItem>();
        }

        public decimal TotalPrice { get; set; }
        public List<OrderResultItem> Items { get; set; }

    }

    public class OrderResultItem {
        public int ItemId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}