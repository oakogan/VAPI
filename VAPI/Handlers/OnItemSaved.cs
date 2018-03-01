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

            if (savedItem.TemplateID.ToString() == "{3DEE5BD4-8ED9-494D-9420-8519C696F78D}"
                || savedItem.TemplateID.ToString() == "{9B79BDE9-EDD6-4214-BBAA-B694C9A81C25}" || savedItem.TemplateID.ToString() == "{8CC4E1AB-2835-4225-929F-29547CE9DF64}") // It's a spec
            {
                Item trim = savedItem.Axes.GetAncestors().FirstOrDefault(x => x.TemplateID.ToString() == "{DD9FBE61-8565-44C8-8283-EC7FF04342E1}");

                if (trim == null)
                    return;

                //Custom save 
                List<Item> featuresFolders = trim.GetChildren().ToList();
                if (featuresFolders == null || !featuresFolders.Any())
                    return; 

                StringBuilder sbText = new StringBuilder();
                StringBuilder sbGuid = new StringBuilder();

                sbText.Append(trim.Name).AppendLine().AppendLine();
                sbGuid.Append(trim.Name).AppendLine().AppendLine();

                using (new SecurityDisabler())
                {
                    trim.Editing.BeginEdit();

                    // Do your edits here
                    foreach (Item featureFolder in featuresFolders)
                    {
                        sbText.Append("===").Append(featureFolder.Name).Append("===").AppendLine();
                        sbGuid.Append("===").Append(featureFolder.Name).Append("===").AppendLine();

                        foreach (Item tabSection in featureFolder.Children)
                        {
                            sbText.Append(tabSection.Name).AppendLine();
                            sbGuid.Append(tabSection.Name).AppendLine();

                            foreach (Item spec in tabSection.Children)
                            {
                                sbText.Append(spec["Name Multiline"]).Append(" : ").Append(spec["Spec"]).AppendLine();
                                sbGuid.Append(spec.ID).Append(" : ").Append(spec["Spec"]).AppendLine();
                            }
                        }

                    }

                    trim["SOP Matrix Text"] = sbText.ToString();
                    trim["SOP Matrix Guid"] = sbGuid.ToString();
                    trim.Editing.EndEdit();
                }

            }
        }
    }
}