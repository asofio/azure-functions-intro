using System.Collections.Generic;

namespace FunctionsIntro.Models
{
    public class ProductResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public int Id { get; set; }
    }
}