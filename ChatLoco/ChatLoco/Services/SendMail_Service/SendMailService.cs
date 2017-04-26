using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatLoco.Models.Email_Model;
using System.Net.Mail;
using ChatLoco.Models.Error_Model;
using ChatLoco.Models.User_Model;
using ChatLoco.Services.Security_Service;
using ChatLoco.Services.User_Service;

namespace ChatLoco.Services.SendMail_Service
{
    public static class SendMailService
    {
        //TODO make way to NOT store these as plaintext
        private static string sender = "chatlocodummy@gmail.com";
        private static string pass = "chatloco123";

        //Maps user names to creation code
        private static Dictionary<int, string> creationCodes = new Dictionary<int, string>();
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
        public static List<ErrorModel> sendCreationCode(string email, int uid) 
        {
            Random rand = new Random();
            long r = rand.Next(0, 200000000);
            string longcode = SecurityService.GetStringSha256Hash(r.ToString());
            string code = longcode.Substring(0, 8);
            creationCodes.Add(uid, code);
            SendRequestModel mod = new SendRequestModel();
            mod.toEmail = email;
            mod.subject = "Welcome to ChatLoco";
            mod.Message = "Your verification code is: "+code;
            var errors = new List<ErrorModel>();
            var mail = new MailMessage();
            var SmtpServer = new SmtpClient("smtp.gmail.com", 587);
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(sender, pass);
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.EnableSsl = true;
            try
            {
                mail.From = new MailAddress(sender);
                mail.To.Add(email);
                mail.Subject = mod.subject;
                mail.Body = mod.Message;
                SmtpServer.Send(mail);
            }
            catch (Exception e)
            {
                errors.Add(new ErrorModel(e.ToString()));
            }
            return errors;
        }
        
        public static List<ErrorModel> verifyUserCreationCode(ActivateUserRequestModel model)
        {
            var errors = new List<ErrorModel>();
            string val;
            if(creationCodes.TryGetValue(model.UserId, out val)) {
                if(!val.Equals(model.ActivationCode)){
                    errors.Add(new ErrorModel("Verification Code is Incorrect"));
                    return errors;
                }
            }
            else
            {
                errors.Add(new ErrorModel("Could not find user Id."));
                return errors;
            }
            UserService.MakeActivated(model.UserId);

            return errors;
        }
    }
}