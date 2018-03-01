using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Web.UI.XslControls;
using VAPI.Models;

namespace VAPI.Controllers
{
    public class FSOController : Controller
    {
        // GET: FSO
        public ActionResult Index() 
        {
            Item parent = Sitecore.Context.Item.Parent;
            List<Item> yearChildren = parent.GetChildren().ToList();
            List<Item> trims = null;
            List<Item> featuresFolders = null;

            //Item commonDataFolder = yearChildren.FirstOrDefault(x => x.TemplateID.ToString() == "{579820E7-297B-4EFC-AAA2-9AC7FA39B1CD}");
            //if(commonDataFolder != null)
            //{
            //    featuresFolders = commonDataFolder.GetChildren().ToList();
            //}

            Item trimsFolder = yearChildren.FirstOrDefault(x => x.TemplateID.ToString() == "{9EB3892F-F93A-4D30-93A3-0BEA57C7347B}");
            if (trimsFolder != null)
            {
                trims = trimsFolder.GetChildren().ToList();

            }

            featuresFolders = trims.First().GetChildren().Where(x => x.TemplateID.ToString() == "{B1002739-4F8F-405F-B876-C24D402A2841}").ToList();
            FsoModel fsoModel = new FsoModel { Trims = trims, Tabs = featuresFolders };



            

            //var controllerContext = this.ControllerContext;
            //var result = ViewEngines.Engines.FindView(controllerContext, "Test", null);

            //Trim trimsModel = new Trim();
            //List<Trim> trimsList = new List<Trim>();

            ////from Item item in trims select new Models.Trim 
            //var trimsCollection = from Item trimItem in trims                         
            //             select new Trim
            //             {
            //                 Katashiki = trimItem["Katashiki"],
            //                 KatashikiCode = trimItem["Katashiki Code"]                             
            //             };


            return View("Index", fsoModel);
        }
    }

   
}