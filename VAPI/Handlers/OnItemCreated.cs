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

            if (scItem == null)
                return;

            Item fsoItem = Helpers.GetCurrentFsoItem(scItem);

            if (fsoItem == null)
                return;

            Item commonDataItem = Helpers.GetCurrentDataFolderItem(scItem);

            if (commonDataItem == null)
                return;

            StringBuilder sbText = new StringBuilder();
            StringBuilder sbGuid = new StringBuilder();

            List<Item> featuresFolders = commonDataItem.GetChildren().ToList();
            if (featuresFolders == null || !featuresFolders.Any())
                return;

            if (scItem.TemplateID.ToString() == Constants.TemplateIDs.Trim_TemplateId && string.IsNullOrEmpty(scItem[Constants.FieldNames.SOPMatrixText_FieldName]))// It's a Trim 
            {    
                foreach (Item featureFolder in featuresFolders)
                {
                    sbText.Append("<div><h1 style='color:blue';>").Append(featureFolder.Name).Append("</h1></div>").AppendLine();

                    foreach (Item tabSection in featureFolder.Children)
                    {
                        sbText.Append("<h2>").Append("*" + tabSection.Name).Append("</h2>").AppendLine().AppendLine();

                        foreach (Item spec in tabSection.Children)
                        {
                            sbText.Append("<div>").Append(spec[Constants.FieldNames.NameMultiline_FieldName]).Append(":").Append("</div>").AppendLine().AppendLine();
                            sbGuid.Append(spec.ID).Append(":").Append(spec[Constants.FieldNames.Spec_FieldName]).Append("/");
                        }
                    }
                }

                using (new SecurityDisabler())
                {
                    scItem.Editing.BeginEdit(); //update Trim item
                    scItem[Constants.FieldNames.SOPMatrixText_FieldName] = sbText.ToString();
                    scItem[Constants.FieldNames.SOPMatrixGuid_FieldName] = sbGuid.ToString();
                    scItem.Editing.EndEdit();

                    fsoItem.Editing.BeginEdit(); //update FSO item
                    string currentFsoValueString = fsoItem[Constants.FieldNames.SOPMatrixText_FieldName];
                    fsoItem[Constants.FieldNames.SOPMatrixText_FieldName] = currentFsoValueString + "<div style=' background-color: coral;'><h1>     " + scItem.Name + "</h1></div>" + sbText.ToString();                  
                    fsoItem.Editing.EndEdit();
                }    
            }
            //If a new spec is created it gets written to Matrix fields upon save in the FSO vew



        }
    }
}