using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatLoco.Controllers;
using System.Web.Mvc;
using ChatLoco.Models.Email_Model;

namespace ChatLocoTest
{
    [TestClass]
    public class ContactControllerTest
    {
        /*Call the Send method: an email will be sent to the admin chatloco account */
        [TestMethod]
        public void TestSend()
        {
            ContactController contactControllerTest = new ContactController();
            SendRequestModel model = new SendRequestModel()
            {
                fromEmail = "test@test.com",
                toEmail = "chatloco.contact@gmail.com",
                subject = "Email Sent Via Unit Tesing",
                Message = "This is a test email. Please do not respond."
            };

            //Test seccussful message sent
            var result = contactControllerTest.Send(model) as JsonResult;
            Assert.AreEqual(0, ((SendResponseModel)result.Data).Errors.Count);
            Assert.AreEqual("Email sent successfully. We will respond ASAP!</p><br><p>You will be redirected to the home page shortly.", ((SendResponseModel)result.Data).Message);
        }
    }
}
