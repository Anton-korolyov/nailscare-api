namespace NailsCare.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Phone { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Admin";
    }
}
