namespace RedBerryCorporate.Interfaces
{
    public interface ICaptchaService
    {
        Task<bool> VerifyTokenAsync(string token);
    }
}