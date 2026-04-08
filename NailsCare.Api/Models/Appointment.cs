namespace NailsCare.Api.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
