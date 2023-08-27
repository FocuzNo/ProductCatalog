using System.Text.Json.Serialization;

namespace ProductCatalog.DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string SpecialNote { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string GeneralNote { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
    }
}