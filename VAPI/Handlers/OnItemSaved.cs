using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAPI.Handlers
{
    public class OnItemSaved
    {
        public void VAPIOnItemSaved(object sender, EventArgs args)
        {
            // Extract the item from the event Arguments
            Item savedItem = Event.ExtractParameter(args, 0) as Item;

            if (savedItem.TemplateID.ToString() == "{DD9FBE61-8565-44C8-8283-EC7FF04342E1}" ) // It's a Trim or an FSO
            {
                Item trim = savedItem;

                if (trim == null)
                    return;

                //Custom save 
                List<Item> featuresFolders = trim.GetChildren().ToList();
                if (featuresFolders == null || !featuresFolders.Any())
                    return; 

                StringBuilder sbText = new StringBuilder();
                StringBuilder sbGuid = new StringBuilder();

                //sbText.Append(trim.Name).AppendLine().AppendLine();
                //sbGuid.Append(trim.Name).AppendLine().AppendLine();

                using (new SecurityDisabler())
                {
                    trim.Editing.BeginEdit();

                    // Do your edits here
                    foreach (Item featureFolder in featuresFolders)
                    {
                        sbText.Append("<h1>").Append(featureFolder.Name).Append("</h1>").AppendLine();
                        sbGuid.Append("<h1>").Append(featureFolder.Name).Append("</h1>").AppendLine();

                        foreach (Item tabSection in featureFolder.Children)
                        {
                            sbText.Append("<h2>").Append(tabSection.Name).Append("</h2>").AppendLine().AppendLine();
                            sbGuid.Append("<h2>").Append(tabSection.Name).Append("</h2>").AppendLine().AppendLine();

                            foreach (Item spec in tabSection.Children)
                            {
                                sbText.Append("<div>").Append(spec["Name Multiline"]).Append(" : ").Append(spec["Spec"]).Append("</div>").AppendLine().AppendLine();
                                sbGuid.Append("<div>").Append(spec.ID).Append(" : ").Append(spec["Spec"]).Append("/<div>").AppendLine().AppendLine();
                            }
                        }

                    }

                    //trim["SOP Matrix Text"] = sbText.ToString();
                    //trim["SOP Matrix Guid"] = sbGuid.ToString();
                    trim.Editing.EndEdit();
                }
            }
            //|| savedItem.TemplateID.ToString() == "{97D1B974-7B34-456D-8D59-B21411F160D2}"
        }
    }
}