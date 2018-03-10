using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAPI.Handlers
{
    public class OnItemCreated
    {
        public void VAPIOnItemCreated(object sender, EventArgs args)
        {
            // Extract the item from the event Arguments
            Item createdItem = Event.ExtractParameter(args, 0) as Item;

            if (createdItem.TemplateID.ToString() == "{DD9FBE61-8565-44C8-8283-EC7FF04342E1}" && string.IsNullOrEmpty(createdItem["SOP Matrix Text"])) // It's a Trim or an FSO
            {
                Item commonDataItem = createdItem.Parent.Parent.GetChildren().FirstOrDefault(x => x.TemplateID.ToString() == "{579820E7-297B-4EFC-AAA2-9AC7FA39B1CD}");

                if (commonDataItem == null)
                    return;
                 
                StringBuilder sbText = new StringBuilder();
                StringBuilder sbGuid = new StringBuilder();

                //Custom save 
                List<Item> featuresFolders = commonDataItem.GetChildren().ToList();
                if (featuresFolders == null || !featuresFolders.Any())
                    return;
            
                using (new SecurityDisabler())
                {
                    createdItem.Editing.BeginEdit();

                    // Do your edits here
                    foreach (Item featureFolder in featuresFolders)
                    {
                        sbText.Append("<h1>").Append(featureFolder.Name).Append("</h1>").AppendLine();
                        //sbGuid.Append(featureFolder.Name).AppendLine();

                        foreach (Item tabSection in featureFolder.Children)
                        {
                            sbText.Append("<h2>").Append(tabSection.Name).Append("</h2>").AppendLine().AppendLine();
                            //sbGuid.Append(tabSection.Name).AppendLine().AppendLine();

                            foreach (Item spec in tabSection.Children)
                            {
                                sbText.Append("<div>").Append(spec["Name Multiline"]).Append(" : ").Append(spec["Spec"]).Append("</div>").AppendLine().AppendLine();
                                sbGuid.Append(spec.ID).Append(" : ").Append(spec["Spec"]).Append("/");
                            }
                        }

                    }

                    createdItem["SOP Matrix Text"] = sbText.ToString();
                    createdItem["SOP Matrix Guid"] = sbGuid.ToString();
                    createdItem.Editing.EndEdit();
                }
            }
            else
            {


            }
        }
    }
}