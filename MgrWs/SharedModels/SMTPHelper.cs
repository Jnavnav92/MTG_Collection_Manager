using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Shared.Models;
using System.Drawing;
using System.Xml;

namespace Shared
{
    public static class SMTPHelper
    {
        public static async Task SendEmailVerificationCodeAsync(BaseAccountModel account, Guid AuthorizationToken, string SMTPEmail, string SMTPPassword, string sVerifyEmailCallbackURL)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MtgCollectionMgrAdmin", "mtgcollectionmgr@gmail.com"));
            message.To.Add(new MailboxAddress(string.Empty, account.EmailAddress));
            message.Subject = "MtgCollectionMgr - Verify Email";

            BodyBuilder msgBody = new BodyBuilder();
            msgBody.HtmlBody = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\View_VerificationEmail.html");

            message.Body = msgBody.ToMessageBody();

            await SendEmailAsync(SMTPEmail, SMTPPassword, message);
        }

        public static async Task SendEmailForgotPasswordAsync(BaseAccountModel account, string SMTPEmail, string SMTPPassword, string sVerifyEmailCallbackURL)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MtgCollectionMgrAdmin", "mtgcollectionmgr@gmail.com"));
            message.To.Add(new MailboxAddress(string.Empty, account.EmailAddress));
            message.Subject = "MtgCollectionMgr - Forgot Password";

            BodyBuilder msgBody = new BodyBuilder();
            msgBody.HtmlBody = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\View_ForgotPassword.html");

            message.Body = msgBody.ToMessageBody();


            await SendEmailAsync(SMTPEmail, SMTPPassword, message);
        }

        private static async Task SendEmailAsync(string SMTPEmail, string SMTPPassword, MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(SMTPEmail, SMTPPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
