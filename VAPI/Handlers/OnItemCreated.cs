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
            Item scItem = Event.ExtractParameter(args, 0) as Item;

            if (scItem.TemplateID.ToString() == Constants.TemplateIDs.TrimTemplateId && string.IsNullOrEmpty(scItem[Constants.FieldNames.SOPMatrixText_FieldName])) // It's a Trim 
            {
                Item commonDataItem = scItem.Parent.Parent.GetChildren().FirstOrDefault(x => x.TemplateID.ToString() == Constants.TemplateIDs.CommonDataFolderId);

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
                    scItem.Editing.BeginEdit();
                  
                    foreach (Item featureFolder in featuresFolders)
                    {
                        sbText.Append("<div style=' background-color: coral;'><h1 style='color:blue';>").Append(featureFolder.Name).Append("</h1></div>").AppendLine();
                        //sbGuid.Append(featureFolder.Name).AppendLine();

                        foreach (Item tabSection in featureFolder.Children)
                        {
                            sbText.Append("<h2>").Append("*" + tabSection.Name).Append("</h2>").AppendLine().AppendLine();
                            //sbGuid.Append(tabSection.Name).AppendLine().AppendLine();

                            foreach (Item spec in tabSection.Children)
                            {
                                sbText.Append("<div>").Append(spec[Constants.FieldNames.NameMultiline_FieldName]).Append(":").Append("</div>").AppendLine().AppendLine();
                                sbGuid.Append(spec.ID).Append(":").Append(spec["Spec"]).Append("/");
                            }
                        }

                    }

                    scItem["SOP Matrix Text"] = sbText.ToString();
                    scItem["SOP Matrix Guid"] = sbGuid.ToString();
                    scItem.Editing.EndEdit();
                }
            }
            else if(scItem.TemplateID.ToString() == Constants.TemplateIDs.FreeFormSpecTemplateId // It's a spec
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.PredefinedSpecTemplateId 
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.SOPSpecTemplateId)
            {


            }
        }
    }
}