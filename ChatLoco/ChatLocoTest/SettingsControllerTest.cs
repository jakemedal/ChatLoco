using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatLoco.Controllers;
using System.Web.Mvc;

namespace ChatLocoTest
{
    [TestClass]
    public class SettingsControllerTest
    {
        //The controller has literally one method that returns the default View

        /*Call the Index method: The Index view*/
        [TestMethod]
        public void TestIndexView()
        {
            SettingsController settingsControllerTest = new SettingsController();
            var result = settingsControllerTest.Index() as ViewResult;
            Assert.AreEqual("", result.ViewName);
        }
    }
}
