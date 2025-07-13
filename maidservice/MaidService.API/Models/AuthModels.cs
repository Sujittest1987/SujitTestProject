namespace MaidService.API.Models
{
    public class AuthRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } // "Client" or "Maid"
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
