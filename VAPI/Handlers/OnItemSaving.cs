using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;

namespace VAPI.Handlers
{
    public class OnItemSaving
    {
        public void VAPIOnItemSaving(object sender, EventArgs args)
        {
            // Extract the item from the event Arguments
            Item scItem = Event.ExtractParameter(args, 0) as Item;

            if (scItem.Paths.FullPath.Contains("Standard Values"))
                return;
            

            if (scItem.TemplateID.ToString() == Constants.TemplateIDs.FreeFormSpec_TemplateId // It's a spec
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.PredefinedSpec_TemplateId
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.SOPSpec_TemplateId)
            {
                Item fsoItem = Helpers.GetCurrentFsoItem(scItem);
                Item existingItem = scItem.Database.GetItem(scItem.ID, scItem.Language, scItem.Version);
                Sitecore.Diagnostics.Assert.IsNotNull(existingItem, "existingItem");
                string oldSpecName = existingItem[Constants.FieldNames.NameMultiline_FieldName];

                if (string.IsNullOrEmpty(oldSpecName)) // to prevent publishing error
                    return;

                Item trimsFolder = Helpers.GetCurrentTrimsFolderItem(scItem);

                //replace for all trims 
                foreach (Item trim in trimsFolder.GetChildren())
                {                                    
                    string oldMatrixText = trim[Constants.FieldNames.SOPMatrixText_FieldName];

                    using (new SecurityDisabler())
                    {     
                        trim.Editing.BeginEdit();

                        trim[Constants.FieldNames.SOPMatrixText_FieldName] = oldMatrixText.Replace(oldSpecName, scItem[Constants.FieldNames.NameMultiline_FieldName]);

                        trim.Editing.EndEdit();
                    }
                }

                //update FSO matrix field 
                if (fsoItem == null)
                    return;

                string fsoMatrixText = fsoItem[Constants.FieldNames.SOPMatrixText_FieldName];

                using (new SecurityDisabler())
                {
                    fsoItem.Editing.BeginEdit();

                    fsoItem[Constants.FieldNames.SOPMatrixText_FieldName] = fsoMatrixText.Replace(oldSpecName, scItem[Constants.FieldNames.NameMultiline_FieldName]);

                    fsoItem.Editing.EndEdit();
                }
            }          
        }
    }
}