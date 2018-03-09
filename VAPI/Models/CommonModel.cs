using Sitecore.Data.Items;
using System.Collections.Generic;

namespace VAPI.Models
{
    public class CommonModel
    {
        public List<FullTrim> FullTrims { get; set; }
        public List<Item> Tabs { get; set; }
    }

    public class FullTrim
    {
        public List<FullTab> FullTabs { get; set; }
        public Item SitecoreTrim { get; set; }
    }

    public class FullTab
    {
        public string Name { get; set; }
        public List<FullSectionTab> FullSectionTabs { get; set; }
    }

    public class FullSectionTab
    {
        public string Name { get; set; }
        public List<FullSpec> FullSpecs { get; set; }
    }

    public class FullSpec
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<string> DropDownOptions { get; set; }
    }
}