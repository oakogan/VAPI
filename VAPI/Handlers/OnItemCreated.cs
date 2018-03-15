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
            Item effectedItem = null;

            if (scItem.TemplateID.ToString() == Constants.TemplateIDs.Trim_TemplateId && string.IsNullOrEmpty(scItem[Constants.FieldNames.SOPMatrixText_FieldName])// It's a Trim 
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.FreeFormSpec_TemplateId // It's a spec
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.PredefinedSpec_TemplateId
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.SOPSpec_TemplateId)
            {
                Item commonDataItem = Helpers.GetCurrentDataFolderItem(scItem);

                if (commonDataItem == null)
                    return;

                StringBuilder sbText = new StringBuilder();
                StringBuilder sbGuid = new StringBuilder();

                //Custom save 
                List<Item> featuresFolders = commonDataItem.GetChildren().ToList();
                if (featuresFolders == null || !featuresFolders.Any())
                    return;


                foreach (Item featureFolder in featuresFolders)
                {
                    sbText.Append("<div style=' background-color: coral;'><h1 style='color:blue';>").Append(featureFolder.Name).Append("</h1></div>").AppendLine();

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

                if (scItem.TemplateID.ToString() == Constants.TemplateIDs.Trim_TemplateId) //update Trim being saved
                {
                    using (new SecurityDisabler())
                    {
                        scItem.Editing.BeginEdit();

                        scItem["SOP Matrix Text"] = sbText.ToString();
                        scItem["SOP Matrix Guid"] = sbGuid.ToString();

                        scItem.Editing.EndEdit();
                    }
                }
                else // update all Trims
                {
                    Item trimsFolder = Helpers.GetCurrentTrimsFolderItem(scItem);

                    foreach (Item trim in trimsFolder.GetChildren())
                    {
                        // string matrixTextFieldValue = trim[Constants.FieldNames.SOPMatrixText_FieldName];
                        //matrixTextFieldValue.Replace(scItem[Constants.FieldNames.NameMultiline_FieldName])

                        using (new SecurityDisabler())
                        {
                            scItem.Editing.BeginEdit();

                            //trim[Constants.FieldNames.SOPMatrixText_FieldName] = sbText.ToString();

                            scItem.Editing.EndEdit();
                        }     
                    }
                }
            }
        }
    }
}