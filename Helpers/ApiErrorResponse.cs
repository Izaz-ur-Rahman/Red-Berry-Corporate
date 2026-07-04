namespace RedBerryCorporate.Helpers
{
    public class ApiErrorResponse
    {
        public bool Success => false;

        public string Message { get; set; } = string.Empty;

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public string? TraceId { get; set; }
    }
}