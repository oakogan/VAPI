using Sitecore.Data.Items;
using System.Collections.Generic;

namespace VAPI.Models
{
    public class CommonModel
    {
        public List<Item> Trims { get; set; }
        public List<Item> Tabs { get; set; }
        public string ContextItemId { get; set; }
        public string UpdatedSpecs { get; set; }
        public string PreviewUrl { get; set; }

        public string SeriesItemName { get; set; }
    }
}