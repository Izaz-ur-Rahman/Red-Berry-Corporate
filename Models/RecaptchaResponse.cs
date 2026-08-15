using System.Text.Json.Serialization;

namespace RedBerryCorporate.Models
{
    public class RecaptchaResponse
    {
        public bool success { get; set; }

        public double score { get; set; }

        public string action { get; set; }

        public DateTime challenge_ts { get; set; }

        public string hostname { get; set; }

        //public string[] error_codes { get; set; }
        [JsonPropertyName("error-codes")]
        public string[]? error_codes { get; set; }
    }
}