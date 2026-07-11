namespace RedBerryCorporate.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int? EmpId { get; set; }

        public string? EmployeeName { get; set; }

        public string? Role { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }
    }
}