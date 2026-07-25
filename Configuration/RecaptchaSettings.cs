namespace RedBerryCorporate.Configuration
{
    public class RecaptchaSettings
    {
        public string SiteKey { get; set; } = "";

        public string SecretKey { get; set; } = "";

        public string VerifyUrl { get; set; } =
            "https://www.google.com/recaptcha/api/siteverify";

        public double MinimumScore { get; set; } = 0.5;

        public bool BypassCaptcha { get; set; }
    }
}