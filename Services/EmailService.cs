using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendBlueprintEmailsAsync(BlueprintSubmission submission)
        {
            // Email to Supervisor
            await SendSupervisorEmail(submission);

            // Email to Client
            await SendClientEmail(submission);
        }

        private async Task SendSupervisorEmail(BlueprintSubmission submission)
        {
            var body = BuildSupervisorBody(submission);

            await SendEmailAsync(
                _settings.SupervisorEmail,
                "New Blueprint Submission",
                body);
        }

        private async Task SendClientEmail(BlueprintSubmission submission)
        {
            var body = BuildClientBody(submission);

            await SendEmailAsync(
                submission.Email,
                "Your Blueprint Has Been Received",
                body);
        }

        private async Task SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            try
            {
                using var client = new SmtpClient(
                    _settings.SmtpServer,
                    _settings.Port);

                client.Credentials = new NetworkCredential(
                    _settings.SenderEmail,
                    _settings.Password);

                client.EnableSsl = _settings.EnableSSL;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 30000;

                var mail = new MailMessage
                {
                    From = new MailAddress(
                        _settings.SenderEmail,
                        _settings.SenderName),

                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception($"SMTP Error: {ex.Message}", ex);
            }
        }

        private string BuildSupervisorBody(BlueprintSubmission s)
        {
            return $@"
<h2>New Blueprint Submission</h2>

<table border='1' cellpadding='8' cellspacing='0'>

<tr>
<td><b>Name</b></td>
<td>{s.Name}</td>
</tr>

<tr>
<td><b>Company</b></td>
<td>{s.Company}</td>
</tr>

<tr>
<td><b>Email</b></td>
<td>{s.Email}</td>
</tr>

<tr>
<td><b>WhatsApp</b></td>
<td>{s.Whatsapp}</td>
</tr>

<tr>
<td><b>Total Score</b></td>
<td>{s.TotalScore}</td>
</tr>

<tr>
<td><b>Status</b></td>
<td>{s.OverallStatus}</td>
</tr>

<tr>
<td><b>Recommended Pathway</b></td>
<td>{s.RecommendedPathway}</td>
</tr>

</table>

<br/>

Please login to CMS to review this submission.
";
        }

        private string BuildClientBody(BlueprintSubmission s)
        {
            return $@"
<h2>Hello {s.Name},</h2>

<p>
Thank you for completing the
<b>Ambition Infrastructure Blueprint.</b>
</p>

<p>
Your submission has been received successfully.
</p>

<p>
Our team will carefully review your Blueprint and contact you shortly.
</p>

<br/>

Regards,

<br/>

<b>RedBerry Corporate</b>
";
        }
    }
}