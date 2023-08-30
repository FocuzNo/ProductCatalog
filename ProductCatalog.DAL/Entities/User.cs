using System.Text.Json.Serialization;

namespace ProductCatalog.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime TokenCreated { get; set; }
        [JsonIgnore]
        public DateTime TokenExpires { get; set; }
        public bool Blocked { get; set; } = false;
        public string Role { get; set; } = string.Empty;
    }
}