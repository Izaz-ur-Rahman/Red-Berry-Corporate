//using Microsoft.Extensions.Options;
//using RedBerryCorporate.Configuration;
//using RedBerryCorporate.Interfaces;
//using RedBerryCorporate.Models;
//using System.Text.Json;

//namespace RedBerryCorporate.Services
//{
//    public class CaptchaService : ICaptchaService
//    {
//        private readonly HttpClient _httpClient;

//        private readonly RecaptchaSettings _settings;
//        private readonly ILogger<CaptchaService> _logger;
//        public CaptchaService(
//     HttpClient httpClient,
//     IOptions<RecaptchaSettings> settings,
//     ILogger<CaptchaService> logger)
//        {
//            _httpClient = httpClient;
//            _settings = settings.Value;
//            _logger = logger;
//        }

//        public async Task<bool> VerifyTokenAsync(string token)
//        {
//            //----------------------------------------
//            // Skip captcha in development if enabled
//            //----------------------------------------

//            if (_settings.BypassCaptcha)
//                return true;

//            //----------------------------------------
//            // Invalid Token
//            //----------------------------------------

//            if (string.IsNullOrWhiteSpace(token))
//                return false;

//            //----------------------------------------
//            // Google Verification URL
//            //----------------------------------------

//            var url =
//                $"{_settings.VerifyUrl}" +
//                $"?secret={_settings.SecretKey}" +
//                $"&response={token}";

//            //----------------------------------------
//            // Verify with Google
//            //----------------------------------------

//            var response =
//                await _httpClient.PostAsync(url, null);

//            if (!response.IsSuccessStatusCode)
//                return false;

//            //----------------------------------------
//            // Read Response
//            //----------------------------------------

//            var json =
//                await response.Content.ReadAsStringAsync();

//            var result =
//                JsonSerializer.Deserialize<RecaptchaResponse>(
//                    json,
//                    new JsonSerializerOptions
//                    {
//                        PropertyNameCaseInsensitive = true
//                    });

//            if (result == null)
//                return false;

//            //----------------------------------------
//            // Validation
//            //----------------------------------------

//            if (!result.success)
//                return false;

//            if (result.score < _settings.MinimumScore)
//                return false;

//            return true;
//        }
//    }
//}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedBerryCorporate.Configuration;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;
using System.Text.Json;

namespace RedBerryCorporate.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly RecaptchaSettings _settings;
        private readonly ILogger<CaptchaService> _logger;

        public CaptchaService(
            HttpClient httpClient,
            IOptions<RecaptchaSettings> settings,
            ILogger<CaptchaService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            //----------------------------------------
            // Skip Captcha (Development)
            //----------------------------------------

            if (_settings.BypassCaptcha)
            {
                _logger.LogInformation("Captcha verification bypassed because BypassCaptcha is enabled.");

                return true;
            }

            //----------------------------------------
            // Empty Token
            //----------------------------------------

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Captcha verification failed because no token was provided.");

                return false;
            }

            //----------------------------------------
            // Google Verification URL
            //----------------------------------------

            var url =
                $"{_settings.VerifyUrl}" +
                $"?secret={_settings.SecretKey}" +
                $"&response={token}";

            try
            {
                //----------------------------------------
                // Verify with Google
                //----------------------------------------

                _logger.LogInformation("Sending captcha verification request to Google.");

                var response =
                    await _httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Google reCAPTCHA returned HTTP Status Code {StatusCode}.",
                        response.StatusCode);

                    return false;
                }

                //----------------------------------------
                // Read Response
                //----------------------------------------

                var json =
                    await response.Content.ReadAsStringAsync();

                var result =
                    JsonSerializer.Deserialize<RecaptchaResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result == null)
                {
                    _logger.LogError("Unable to deserialize Google reCAPTCHA response.");

                    return false;
                }

                //----------------------------------------
                // Google Validation
                //----------------------------------------

                if (!result.success)
                {
                    _logger.LogWarning(
                        "Captcha verification failed. Error Codes: {Errors}",
                        result.error_codes == null
                            ? "None"
                            : string.Join(", ", result.error_codes));

                    return false;
                }

                //----------------------------------------
                // Score Validation
                //----------------------------------------

                if (result.score < _settings.MinimumScore)
                {
                    _logger.LogWarning(
                        "Captcha rejected because score {Score} is below minimum score {MinimumScore}.",
                        result.score,
                        _settings.MinimumScore);

                    return false;
                }

                //----------------------------------------
                // Success
                //----------------------------------------

                _logger.LogInformation(
                    "Captcha verified successfully. Score: {Score}",
                    result.score);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error occurred while verifying Google reCAPTCHA.");

                return false;
            }
        }
    }
}