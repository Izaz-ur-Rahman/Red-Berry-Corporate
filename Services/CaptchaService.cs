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

        public CaptchaService(
            HttpClient httpClient,
            IOptions<RecaptchaSettings> settings)
        {
            _httpClient = httpClient;

            _settings = settings.Value;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            //----------------------------------------
            // Skip captcha in development if enabled
            //----------------------------------------

            if (_settings.BypassCaptcha)
                return true;

            //----------------------------------------
            // Invalid Token
            //----------------------------------------

            if (string.IsNullOrWhiteSpace(token))
                return false;

            //----------------------------------------
            // Google Verification URL
            //----------------------------------------

            var url =
                $"{_settings.VerifyUrl}" +
                $"?secret={_settings.SecretKey}" +
                $"&response={token}";

            //----------------------------------------
            // Verify with Google
            //----------------------------------------

            var response =
                await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
                return false;

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
                return false;

            //----------------------------------------
            // Validation
            //----------------------------------------

            if (!result.success)
                return false;

            if (result.score < _settings.MinimumScore)
                return false;

            return true;
        }
    }
}