using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MaidService.API.Data;
using MaidService.API.Models;
using System.ComponentModel.DataAnnotations;

namespace MaidService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookings()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            var bookings = userRole == "Maid"
                ? await _context.Bookings
                    .Include(b => b.Client)
                    .Include(b => b.Maid)
                    .Where(b => b.MaidId == userId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync()
                : await _context.Bookings
                    .Include(b => b.Client)
                    .Include(b => b.Maid)
                    .Where(b => b.ClientId == userId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();

            return bookings;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<Booking>> CreateBooking(BookingRequest request)
        {
            var clientId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var maid = await _context.MaidProfiles
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == request.MaidId);

            if (maid == null)
                return NotFound("Maid not found");

            if (!maid.IsAvailable)
                return BadRequest("Maid is not available");

            decimal totalAmount = maid.HourlyRate * request.DurationHours;
            decimal commissionAmount = totalAmount * 0.05m; // 5% commission

            var booking = new Booking
            {
                ClientId = clientId,
                MaidId = request.MaidId,
                BookingDate = request.BookingDate,
                DurationHours = request.DurationHours,
                Status = "Pending",
                TotalAmount = totalAmount,
                CommissionAmount = commissionAmount,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Maid)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            if (userRole == "Client" && booking.ClientId != userId)
                return Forbid();

            if (userRole == "Maid" && booking.MaidId != userId)
                return Forbid();

            return booking;
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            // Only allow maid to update status
            if (userRole != "Maid" || booking.MaidId != userId)
                return Forbid();

            // Validate status transition
            if (!IsValidStatusTransition(booking.Status, status))
                return BadRequest("Invalid status transition");

            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                ("Pending", "Confirmed") => true,
                ("Pending", "Cancelled") => true,
                ("Confirmed", "Completed") => true,
                ("Confirmed", "Cancelled") => true,
                _ => false
            };
        }
    }

    public class BookingRequest
    {
        [Required]
        public int MaidId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        [Range(1, 12)]
        public int DurationHours { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
