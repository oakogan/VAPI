using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Text;

namespace VAPI.Handlers
{
    public class OnItemDeleted
    {
        public void VAPIOnItemDeleted(object sender, EventArgs args)
        {
            // Extract the item from the event Arguments
            Item scItem = Event.ExtractParameter(args, 0) as Item;

            if (scItem == null)
                return;

            Item fsoItem = Helpers.GetCurrentFsoItem(scItem);

            if (fsoItem == null)
                return;

            if (scItem.TemplateID.ToString() == Constants.TemplateIDs.Trim_TemplateId)// It's a Trim 
            {
                Item commonDataItem = Helpers.GetCurrentDataFolderItem(scItem);

                if (commonDataItem == null)
                    return;

                StringBuilder sbText = new StringBuilder();                
                Item trimsFolder = Helpers.GetCurrentTrimsFolderItem(scItem);

                if (trimsFolder == null)
                    return;                

                foreach (Item trim in trimsFolder.GetChildren())
                {
                    if (trim.ID != scItem.ID)
                    {
                        sbText.Append("<div style=' background-color: coral;'><h1>     " + trim.Name + "</h1></div>")
                            .Append(trim[Constants.FieldNames.SOPMatrixText_FieldName]);
                    }
                }
              
                using (new SecurityDisabler())
                {
                    fsoItem.Editing.BeginEdit(); //update FSO item                    
                    fsoItem[Constants.FieldNames.SOPMatrixText_FieldName] = sbText.ToString();
                    fsoItem.Editing.EndEdit();
                }
            }            
        }
    }
}