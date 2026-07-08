using RedBerryCorporate.Models;

namespace RedBerryCorporate.Interfaces
{
    public interface IEmailService
    {
        Task SendBlueprintEmailsAsync(BlueprintSubmission submission);
    }
}