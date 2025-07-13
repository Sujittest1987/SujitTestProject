namespace MaidService.API.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public User Client { get; set; }
        public int MaidId { get; set; }
        public MaidProfile Maid { get; set; }
        public DateTime BookingDate { get; set; }
        public int DurationHours { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Completed, Cancelled
        public decimal TotalAmount { get; set; }
        public decimal CommissionAmount { get; set; } // 5% of TotalAmount
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
