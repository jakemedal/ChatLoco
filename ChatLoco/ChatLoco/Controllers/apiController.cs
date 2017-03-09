using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatLoco.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        public ActionResult GetRevisionString()
        {
            return Json(Labels.RevisionString, JsonRequestBehavior.AllowGet);
        }
    }
}