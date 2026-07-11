namespace RedBerryCorporate.DTOs.Auth
{
    public class JwtTokenResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }
    }
}