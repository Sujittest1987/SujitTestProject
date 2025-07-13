namespace MaidService.API.Models
{
    public class MaidProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Description { get; set; }
        public decimal HourlyRate { get; set; }
        public string Skills { get; set; }
        public decimal Rating { get; set; }
        public int TotalBookings { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
