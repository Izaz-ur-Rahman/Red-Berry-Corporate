using Microsoft.Extensions.Options;
using RedBerryCorporate.Interfaces;
using RedBerryCorporate.Interfaces.EmailTemplate;
using RedBerryCorporate.Models;
using System.Net;
using System.Net.Mail;

namespace RedBerryCorporate.Services
{
    public class EmailService : IEmailService
    {
        /*private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }*/
        private readonly EmailSettings _settings;
        private readonly IEmailTemplateService _templateService;

        public EmailService(
            IOptions<EmailSettings> options,
            IEmailTemplateService templateService)
        {
            _settings = options.Value;
            _templateService = templateService;
        }

        public async Task SendBlueprintEmailsAsync(BlueprintSubmission submission)
        {
            // Email to Supervisor
            await SendSupervisorEmail(submission);

            // Email to Client
            await SendClientEmail(submission);
        }
        private async Task SendSupervisorEmail(
    BlueprintSubmission submission)
        {
            var template =
                await _templateService.RenderAsync(
                    "BLUEPRINT_ADMIN",
                    new
                    {
                        submission.Name,
                        submission.Company,
                        submission.Email,
                        submission.Whatsapp,
                        submission.Location,
                        submission.TotalScore,
                        submission.OverallStatus,
                        submission.RecommendedPathway,
                        submission.StrongestLayer,
                        submission.ExposedLayer
                    });

            await SendEmailAsync(
                _settings.SupervisorEmail,
                template.Subject,
                template.Body);
        }
        //private async Task SendSupervisorEmail(BlueprintSubmission submission)
        //{
        //    var body = BuildSupervisorBody(submission);

        //    await SendEmailAsync(
        //        _settings.SupervisorEmail,
        //        "New Blueprint Submission",
        //        body);
        //}
        private async Task SendClientEmail(
    BlueprintSubmission submission)
        {
            var template =
                await _templateService.RenderAsync(
                    "BLUEPRINT_CLIENT",
                    new
                    {
                        submission.Name,
                        submission.Company,
                        submission.Email,
                        submission.TotalScore,
                        submission.OverallStatus,
                        submission.RecommendedPathway
                    });

            await SendEmailAsync(
                submission.Email,
                template.Subject,
                template.Body);
        }
        //private async Task SendClientEmail(BlueprintSubmission submission)
        //{
        //    var body = BuildClientBody(submission);

        //    await SendEmailAsync(
        //        submission.Email,
        //        "Your Blueprint Has Been Received",
        //        body);
        //}

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
                //Console.WriteLine("STEP 7 AFTER SEND");
                //Console.WriteLine("EMAIL SENT");
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);

                //throw;
                //throw new Exception($"SMTP Error: {ex.Message}", ex);
            }
        }

//        private string BuildSupervisorBody(BlueprintSubmission s)
//        {
//            return $@"
//<!DOCTYPE html>
//<html>
//<head>
//    <meta charset='UTF-8'>
//</head>

//<body style='margin:0;padding:0;background:#f4f6f9;font-family:Arial,Helvetica,sans-serif;'>

//<table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f6f9;padding:30px 0;'>

//<tr>
//<td align='center'>

//<table width='700' cellpadding='0' cellspacing='0'
//style='background:#ffffff;border-radius:8px;overflow:hidden;border:1px solid #e5e5e5;'>

//<!-- Header -->
//<tr>
//<td style='background:#b22222;color:#ffffff;padding:20px;text-align:center;'>

//<h2 style='margin:0;'>New Blueprint Submission</h2>

//<p style='margin:8px 0 0;font-size:14px;'>
//A new Ambition Infrastructure Blueprint has been submitted.
//</p>

//</td>
//</tr>

//<!-- Body -->
//<tr>
//<td style='padding:30px;'>

//<p style='margin-top:0;font-size:15px;color:#444;'>
//Dear Team,
//</p>

//<p style='font-size:15px;color:#444;line-height:24px;'>
//A new Blueprint Assessment has been successfully submitted.
//Below are the submission details.
//</p>

//<table width='100%' cellpadding='10' cellspacing='0'
//style='border-collapse:collapse;border:1px solid #dddddd;font-size:14px;'>

//<tr style='background:#f8f9fa;'>
//<td width='35%' style='border:1px solid #dddddd;'><strong>Name</strong></td>
//<td style='border:1px solid #dddddd;'>{s.Name}</td>
//</tr>

//<tr>
//<td style='border:1px solid #dddddd;'><strong>Company</strong></td>
//<td style='border:1px solid #dddddd;'>{s.Company}</td>
//</tr>

//<tr style='background:#f8f9fa;'>
//<td style='border:1px solid #dddddd;'><strong>Email</strong></td>
//<td style='border:1px solid #dddddd;'>{s.Email}</td>
//</tr>

//<tr>
//<td style='border:1px solid #dddddd;'><strong>WhatsApp</strong></td>
//<td style='border:1px solid #dddddd;'>{s.Whatsapp}</td>
//</tr>

//<tr style='background:#f8f9fa;'>
//<td style='border:1px solid #dddddd;'><strong>Location</strong></td>
//<td style='border:1px solid #dddddd;'>{s.Location}</td>
//</tr>

//<tr>
//<td style='border:1px solid #dddddd;'><strong>Total Score</strong></td>
//<td style='border:1px solid #dddddd;'><strong>{s.TotalScore}</strong></td>
//</tr>

//<tr style='background:#f8f9fa;'>
//<td style='border:1px solid #dddddd;'><strong>Overall Status</strong></td>
//<td style='border:1px solid #dddddd;'>
//<span style='color:#28a745;font-weight:bold;'>
//{s.OverallStatus}
//</span>
//</td>
//</tr>

//<tr>
//<td style='border:1px solid #dddddd;'><strong>Recommended Pathway</strong></td>
//<td style='border:1px solid #dddddd;'>
//{s.RecommendedPathway}
//</td>
//</tr>

//<tr style='background:#f8f9fa;'>
//<td style='border:1px solid #dddddd;'><strong>Strongest Layer</strong></td>
//<td style='border:1px solid #dddddd;'>
//{s.StrongestLayer}
//</td>
//</tr>

//<tr>
//<td style='border:1px solid #dddddd;'><strong>Exposed Layer</strong></td>
//<td style='border:1px solid #dddddd;'>
//{s.ExposedLayer}
//</td>
//</tr>

//</table>

//<p style='margin-top:25px;font-size:15px;color:#444;'>
//Please log in to the CMS dashboard to review the complete Blueprint assessment and follow up with the client.
//</p>

//</td>
//</tr>

//<!-- Footer -->
//<tr>
//<td style='background:#f8f9fa;padding:18px;text-align:center;
//font-size:13px;color:#777;border-top:1px solid #e5e5e5;'>

//This is an automated notification generated by
//<strong>RedBerry Corporate CMS</strong>.

//</td>
//</tr>

//</table>

//</td>
//</tr>

//</table>

//</body>
//</html>";
//        }

//        private string BuildClientBody(BlueprintSubmission s)
//        {
//            return $@"
//<h2>Hello {s.Name},</h2>

//<p>
//Thank you for completing the
//<b>Ambition Infrastructure Blueprint.</b>
//</p>

//<p>
//Your submission has been received successfully.
//</p>

//<p>
//Our team will carefully review your Blueprint and contact you shortly.
//</p>

//<br/>

//Regards,

//<br/>

//<b>RedBerry Corporate</b>
//";
//        }
    }
}