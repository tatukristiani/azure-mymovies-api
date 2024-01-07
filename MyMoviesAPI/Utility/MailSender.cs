using MyMoviesAPI.Controllers;
using System.Net;
using System.Net.Mail;

namespace MyMoviesAPI.Utility
{
    public static class MailSender
    {
        public static Task SendPasswordResetMail(string email, string passwordResetLink)
        {
            var senderEmail = Environment.GetEnvironmentVariable("MYMOVIE_EMAIL");
            var senderEmailPass = Environment.GetEnvironmentVariable("MYMOVIE_EMAIL_PASS");
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable(senderEmail), senderEmailPass);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail);
            mailMessage.To.Add(email);
            mailMessage.Subject = "Hello from MyMovies";
            mailMessage.Body = $"You've reqeusted to reset your password and this is why you've received this email.\nFollow this link {passwordResetLink} to reset your password.\n\nIf you did not make this request then we highly recommend to remove this email.";
            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
