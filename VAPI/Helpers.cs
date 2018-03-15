using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VAPI
{
    public static class Helpers
    {
        public static Item GetCurrentYearItem(Item item)
        {
            while (item.TemplateID != new ID(Constants.TemplateIDs.YearFolder_TemplateId))
            {
                item = item.Parent;
            }

            return item;
        }

        public static Item GetCurrentFsoItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            Item fsoItem = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.FSO_TemplateId));

            if (fsoItem != null)
                return fsoItem;

            return item;
        }

        public static Item GetCurrentDataFolderItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            Item returnItem = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.CommonDataFolder_TemplateId));

            if (returnItem != null)
                return returnItem;

            return item;
        }

        public static Item GetCurrentTrimsFolderItem(Item item)
        {
            Item yearItem = GetCurrentYearItem(item);

            Item trimsFolderItem = yearItem.GetChildren().FirstOrDefault(x => x.TemplateID == new ID(Constants.TemplateIDs.TrimsFolder_TemplateId));

            if (trimsFolderItem != null)
                return trimsFolderItem;

            return item;
        }
    }
}