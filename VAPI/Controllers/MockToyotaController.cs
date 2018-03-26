using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VAPI.Controllers
{
    public class MockToyotaController : Controller
    {
        // GET: MockToyota
        public ActionResult Index()
        {
            return View();
        }

        #region Mock Toyota Site

        public ActionResult LoadBuildAndPrice()
        {

            return View("~/Views/MockToyotaSite/BuildAndPrice.cshtml");
        }

        #endregion
    }
}