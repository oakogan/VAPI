using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Web.UI.XslControls;
using VAPI.Models;
using Sitecore.Data.Fields;

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

            List<Item> test = featuresFolders.First().GetChildren().ToList();

            foreach (Item subsection in test)
            {
                //if (FieldTypeManager.GetField(Sitecore.Context.Item.Fields["Spec"]) is Sitecore.Data.Fields.ValueLookupField)
                //{
                //    Sitecore.Context.Item.Fields["Spec"].Source
                //}

                string name = subsection.Name;
                Item spec = trims.First().Axes.GetDescendants().FirstOrDefault(x => x["Name Multiline"] == "");
            }

            return View("Index", fsoModel);
        }

        public ActionResult Common()
        {
            Item parent = Sitecore.Context.Item.Parent;
            List<Item> yearChildren = parent.GetChildren().ToList();
            List<Item> trims = null;
            List<Item> featuresFolders = null;

            Item commonDataFolder = yearChildren.FirstOrDefault(x => x.TemplateID.ToString() == "{579820E7-297B-4EFC-AAA2-9AC7FA39B1CD}");
            if (commonDataFolder != null)
            {
                featuresFolders = commonDataFolder.GetChildren().ToList();
            }

            Item trimsFolder = yearChildren.FirstOrDefault(x => x.TemplateID.ToString() == "{9EB3892F-F93A-4D30-93A3-0BEA57C7347B}");
            if (trimsFolder != null)
            {
                trims = trimsFolder.GetChildren().ToList();

            }

            //featuresFolders = trims.First().GetChildren().Where(x => x.TemplateID.ToString() == "{B1002739-4F8F-405F-B876-C24D402A2841}").ToList();
            FsoModel fsoModel = new FsoModel { Trims = trims, Tabs = featuresFolders };


            //CommonModel model = new CommonModel { Tabs = featuresFolders, FullTrims = new List<FullTrim>() };


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

            List<Item> test = featuresFolders.First().GetChildren().ToList();

            foreach (Item subsection in test)
            {
                string name = subsection.Name;
                Item spec = trims.First().Axes.GetDescendants().FirstOrDefault(x => x["Name Multiline"] == "");

            }

            var controllerContext = this.ControllerContext;
            var result = ViewEngines.Engines.FindView(controllerContext, "CommonFSO", null);

            return View("~/Views/FSO/FSO.cshtml", fsoModel);
        }

        public ActionResult LoadFSO()
        {
            Item parent = Sitecore.Context.Item.Parent;
            List<Item> yearChildren = parent.GetChildren().ToList();
            List<Item> trims = null;
            List<Item> featuresFolders = null;

            Item commonDataFolder = yearChildren.FirstOrDefault(x => x.TemplateID.ToString() == "{579820E7-297B-4EFC-AAA2-9AC7FA39B1CD}");
            if (commonDataFolder != null)
            {
                featuresFolders = commonDataFolder.GetChildren().ToList();
            }

            Item trimsFolder = yearChildren.FirstOrDefault(x => x.TemplateID.ToString() == "{9EB3892F-F93A-4D30-93A3-0BEA57C7347B}");
            if (trimsFolder != null)
            {
                trims = trimsFolder.GetChildren().ToList();

            }

            CommonModel model = new CommonModel();
            model.Tabs = featuresFolders;
            model.FullTrims = new List<FullTrim>();

            //Build full trims
            foreach (Item trim in trims)
            {
                FullTrim fullTrim = new FullTrim();
                fullTrim.SitecoreTrim = trim;
                fullTrim.SopMatrixGuid = trim["SOP Matrix Guid"];
                fullTrim.FullTabs = new List<FullTab>();

                foreach (Item tab in model.Tabs) 
                {
                    FullTab fullTab = new FullTab();
                    fullTab.Name = tab.Name;
                    fullTab.FullSectionTabs = new List<FullSectionTab>();

                    foreach (Item sectionTab in tab.GetChildren().Where(x => x.TemplateID.ToString() == "{A07350B4-FF45-4F4D-BDA0-B2160880C510}"))
                    {
                        FullSectionTab fullSectionTab = new FullSectionTab();
                        fullSectionTab.Name = sectionTab.Name;
                        fullSectionTab.FullSpecs = new List<FullSpec>();

                        foreach (Item spec in sectionTab.GetChildren())
                        {
                            FullSpec fullSpec = new FullSpec();
                            fullSpec.Name = spec.Name;
                            fullSpec.Value = spec["Spec"];
                            fullSpec.DropDownOptions = null;

                            if (FieldTypeManager.GetField(spec.Fields["Spec"]) is ValueLookupField)
                            {
                                Item sourceItem = Sitecore.Context.Database.GetItem(spec.Fields["Spec"].Source);
                                fullSpec.DropDownOptions = new List<string>();

                                var options = from Item option in sourceItem.GetChildren()
                                              select option.Name;

                                fullSpec.DropDownOptions = options.ToList();
                            }
                            fullSectionTab.FullSpecs.Add(fullSpec);

                        }
                        fullTab.FullSectionTabs.Add(fullSectionTab);
                    }
                    fullTrim.FullTabs.Add(fullTab);
                }

                model.FullTrims.Add(fullTrim);

            }

            foreach (FullTrim trim in model.FullTrims)
            {
                FullSpec spec = new FullSpec();
                //string y = trim.FullTabs.FirstOrDefault(x => x.Name == model.Tabs.First().Name).FullSectionTabs.FirstOrDefault(x => x.Name == trim.FullTabs.First().Name).FullSpecs.FirstOrDefault(x => x.Name == spec.Name).Name;
            }

            return View("~/Views/FSO/FSONew.cshtml", model);
        }

        /// <summary>
        /// Action method to handle user input
        /// </summary>      
        [HttpPost]
        public ActionResult FsoSave(CommonModel model)
        {
            return View("~/Views/FSO/FSONew.cshtml", model);
        }
    }


}