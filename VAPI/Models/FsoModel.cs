using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VAPI.Models
{
    public class FsoModel
    {
        public List<Item> Trims { get; set; }
        public List<Item> Tabs { get; set; }
    }
}