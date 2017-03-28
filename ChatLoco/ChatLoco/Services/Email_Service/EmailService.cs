using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatLoco.Models.Email_Model;
using System.Net.Mail;

namespace ChatLoco.Services.Email_Service
{
    public static class EmailService
    {
        //TODO make way to NOT store these as plaintext
        private static string sender = "chatlocodummy@gmail.com";
        private static string pass = "chatloco123";

        public static bool sendMail(EmailModel contact)
        {
            var mail = new MailMessage();
            var SmtpServer = new SmtpClient("smtp.gmail.com", 587);
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(sender, pass);
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.EnableSsl = true;
            try
            {
                mail.From = new MailAddress(contact.fromEmail);
                mail.To.Add(contact.toEmail);
                mail.Subject = contact.subject;
                mail.Body = contact.Message;
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Console.Write(e);
                //email failed
                return false;
            }
            //email sent
            return true;
        }
    }
}