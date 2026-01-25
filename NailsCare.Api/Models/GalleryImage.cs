namespace NailsCare.Api.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = "";
        public string Category { get; set; } = ""; // manicure | pedicure
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
