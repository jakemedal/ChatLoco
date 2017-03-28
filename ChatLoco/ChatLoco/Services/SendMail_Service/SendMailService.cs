using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatLoco.Models.Email_Model;
using System.Net.Mail;
using ChatLoco.Models.Error_Model;

namespace ChatLoco.Services.SendMail_Service
{
    public static class SendMailService
    {
        //TODO make way to NOT store these as plaintext
        private static string sender = "chatlocodummy@gmail.com";
        private static string pass = "chatloco123";

        public static List<ErrorModel> SendMail(SendRequestModel request)
        {
            var errors = new List<ErrorModel>();
            var mail = new MailMessage();
            var SmtpServer = new SmtpClient("smtp.gmail.com", 587);
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(sender, pass);
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.EnableSsl = true;
            try
            {
                mail.From = new MailAddress(request.fromEmail);
                mail.To.Add(request.toEmail);
                mail.Subject = request.subject;
                mail.Body = request.Message;
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                errors.Add(new ErrorModel(e.ToString()));
            }
            //email sent
            return errors;
        }
    }
}