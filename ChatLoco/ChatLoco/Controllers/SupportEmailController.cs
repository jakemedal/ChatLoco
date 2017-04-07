using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Text;

namespace ChatLoco.Controllers
{
    public class SupportEmailController : Controller
    {
        // GET: SupportEmail
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult sendEmail(string name, string email, string message)
        {
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("user@gmail.com", "password");

            MailMessage mm = new MailMessage("donotreply@domain.com", "felipeuno67@gmail.com", "test", "test");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            return null;
        }

    }
}