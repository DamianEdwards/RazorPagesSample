using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RazorPagesWebApplication.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<SendGridOptions> options)
        {
            Options = options.Value;
        }

        public SendGridOptions Options { get; }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("no-reply@razorpages.asp.net", "Razor Pages"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            var result = await client.SendEmailAsync(msg);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
