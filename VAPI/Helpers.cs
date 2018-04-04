using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Linq;
using VAPI.Models;
using System.Web.Script.Serialization;

namespace VAPI
{
    public static class Helpers
    {
        public static string GetSpecValue(string matrixValue, string specId)
        {
            if(specId == "{A6B5F769-B6F2-4AB3-9C47-73EC51AB31DB}")
            {
                int x = 0;
            }

            List<string> matches = matrixValue.Split('/').ToList();
            string match = matches.FirstOrDefault(x => x.Contains(specId));

            if (match != null)
            {
                string[] a = match.Split(':');
                return a[1];
            }

            return "not found";
        }
        public static string GetJson(string seriesId, string yearParam)
        {
            if (string.IsNullOrEmpty(seriesId))
                return null;

            //read query string for Year    
            if (string.IsNullOrEmpty(yearParam))
            {
                yearParam = System.DateTime.Now.Year.ToString();
            }

            SeriesModel model = Helpers.InitializeSeriesModel(seriesId, yearParam);

            if (model == null)
                return null;

            var serializer = new JavaScriptSerializer();
            string VAPI = serializer.Serialize(model.Years.First());         

            try
            {
                Year yearNew = serializer.Deserialize<Year>(VAPI);
            }
            catch
            {
                return null;
            }


            return VAPI;
        }

        public static Item GetCurrentYearItem(Item item)
        {
            try
            {
                while (item.TemplateID != new ID(Constants.TemplateIDs.YearFolder_TemplateId) && item.Parent != null)
                {
                    item = item.Parent;
                }

                return item;
            }
            catch { return null; }
        }

        public static Item GetCurrentFsoItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            if (yearItem == null)
                return null;

            Item fsoItem = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.FSO_TemplateId));

            if (fsoItem != null)
                return fsoItem;

            return item;
        }

        public static Item GetCurrentDataFolderItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            if (yearItem == null)
                return null;

            Item returnItem = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.CommonDataFolder_TemplateId));

            if (returnItem != null)
                return returnItem;

            return item;
        }

        public static Item GetCurrentTrimsFolderItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            if (yearItem == null)
                return null;

            Item trimsFolderItem = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.TrimsFolder_TemplateId));

            if (trimsFolderItem != null)
                return trimsFolderItem;

            return item;
        }

        public static Item GetCurrentSeriesItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            if (yearItem == null)
                return null;

            Item seriesItem = yearItem.Parent;

            if (seriesItem != null)
                return seriesItem;

            return null;
        }

        public static IEnumerable<Trim> InitializeTrims(Item contextItem)
        {
            List<Item> trimItems = null;

            Item trimsFolder = Helpers.GetCurrentTrimsFolderItem(contextItem);
            if (trimsFolder != null)
            {
                trimItems = trimsFolder.GetChildren().ToList();

                var test = trimItems.Select(e => new Trim
                {
                    Katashiki = e["Katashiki"],
                    Description = e["Description"],
                    ImageUrl = GetImageUrl(e),
                    MatrixGuid = e[Constants.FieldNames.SOPMatrixGuid_FieldName]
                });

                return test;
            }

            return null;
        }

        public static IEnumerable<Feature> InitializeFeatures(Item yearItem)
        {
            Item dataFolder = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.CommonDataFolder_TemplateId));

            if (dataFolder == null)
                return null;


            var features = dataFolder.GetChildren().Select(e => new Feature
            {
                Name = e.Name,
                Guid = e.ID.ToString(),
                Subsections = e.GetChildren()
                    .Select(x => new Subsection
                    {
                        Name = x.Name,
                        Specs = x.GetChildren().Select(y => new Spec
                        {
                            Name = y[Constants.FieldNames.NameMultiline_FieldName],
                            Guid = y.ID.ToString(),
                            Code = y["code"]
                        }).ToList()
                    }).ToList()
            });

            if(features == null)
                return null;

            return features;
        }

        public static string GetImageUrl(Item item)
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

        public static SeriesModel InitializeSeriesModel(string seriesId, string yearParam)
        { 
            //Find Series item      
            Item seriesItem = Context.Database.GetItem(seriesId);

            if (seriesItem == null)
                return null;

            Item yearItem = seriesItem.GetChildren().FirstOrDefault(x => x.Name == yearParam);

            if (yearItem == null)
                return null;

            //Process request
            Year year = new Year() { Number = seriesItem.Name + " " + yearItem.Name, Trims = Helpers.InitializeTrims(yearItem).ToList(), Features = Helpers.InitializeFeatures(yearItem).ToList() };
            SeriesModel model = new SeriesModel();
            model.SeriesName = seriesItem.Name;
            model.Years = new List<Year>() { year };

            return model;
        }
    }
}