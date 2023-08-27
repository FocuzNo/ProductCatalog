using System.Text.Json.Serialization;

namespace ProductCatalog.DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();
    }
}