using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAPI.Models;

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
            //read query string for Series           
            if (string.IsNullOrEmpty(Request.QueryString["seriesId"]))
                return new EmptyResult();

            string seriesId = Request.QueryString["seriesId"];

            //read query string for Year           
            string yearParam = Request.QueryString["year"];
            if (string.IsNullOrEmpty(yearParam))
            {
                yearParam = DateTime.Now.Year.ToString();
            }

            SeriesModel model = Helpers.InitializeSeriesModel(seriesId, yearParam);


            return View("~/Views/MockToyotaSite/BuildAndPrice.cshtml", model.Years.First());
        }

        private string GetImageUrl(Item item)
        {
            Sitecore.Data.Fields.ImageField imageField = item.Fields["Image"];
            var imageUrl = string.Empty;

            if (imageField?.MediaItem != null)
            {
                var image = new MediaItem(imageField.MediaItem);
                imageUrl = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
            }

            return imageUrl;
        }

        private IEnumerable<Trim> InitializeTrims(Item contextItem)
        {
            //Item contextItem = null;

            //if (Sitecore.Context.Item == null)
            //{
            //    contextItem = Sitecore.Context.Database.GetItem(model.ContextItemId);
            //}
            //else
            //{
            //    contextItem = Sitecore.Context.Item;
            //}

            List<Item> trimItems = null;
            //List<Trim> trims = new List<Trim>();


            Item trimsFolder = Helpers.GetCurrentTrimsFolderItem(contextItem);
            if (trimsFolder != null)
            {
                trimItems = trimsFolder.GetChildren().ToList();

                var test = trimItems.Select(e => new Trim
                {
                    Katashiki = e["Katashiki"],
                    Description = e["Description"],
                    ImageUrl = GetImageUrl(e)
                });

                return test;
            }

            return null;
        }

        #endregion
    }
}