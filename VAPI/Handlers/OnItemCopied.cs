using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Linq;
using System.Text;

namespace VAPI.Handlers
{
    public class OnItemCopied
    {
        public void VAPIOnItemCopied(object sender, EventArgs args)
        {
            try
            {
                Item originItem = Event.ExtractParameter(args, 0) as Item;
                Item originCommonDataItem = Helpers.GetCurrentDataFolderItem(originItem);

                Item resultItem = Event.ExtractParameter(args, 1) as Item;

                if (resultItem.TemplateID.ToString() == Constants.TemplateIDs.YearFolder_TemplateId) // it's a year
                {
                    Item commonDataItem = Helpers.GetCurrentDataFolderItem(resultItem);
                    Item trimsFolderItem = Helpers.GetCurrentTrimsFolderItem(resultItem);   
                    Sitecore.Data.Database masterDb = Sitecore.Configuration.Factory.GetDatabase("master");

                    using (new SecurityDisabler())
                    {
                        if (resultItem.TemplateID.ToString() == Constants.TemplateIDs.YearFolder_TemplateId)
                        {
                            foreach (Item trim in trimsFolderItem.GetChildren()) 
                            {
                                StringBuilder sbGuid = new StringBuilder();
                                string matrixGuidString = trim[Constants.FieldNames.SOPMatrixGuid_FieldName];
                                string[] pairs = matrixGuidString.Split('/');

                                foreach (string pair in pairs)
                                {
                                    if(!string.IsNullOrEmpty(pair))
                                    {
                                        string guid = pair.Split(':')[0];
                                        string specValue = pair.Split(':')[1];

                                        Item oldSpecitem = masterDb.GetItem(guid);

                                        if (oldSpecitem != null)
                                        {
                                            Item newSpecItem = commonDataItem.Axes.GetDescendants().FirstOrDefault(x => x.Name == oldSpecitem.Name);

                                            if (newSpecItem != null)
                                            {
                                                guid = newSpecItem.ID.ToString();
                                                sbGuid.Append(guid).Append(":").Append(specValue).Append("/");
                                            }
                                        }
                                    }                                   
                                }

                                trim.Editing.BeginEdit();
                                trim[Constants.FieldNames.SOPMatrixGuid_FieldName] = sbGuid.ToString();
                                trim.Editing.EndEdit();
                            }
                        }
                    }// end of using
                }// end of if
            }// end of try
            catch(Exception ex) { }
        } 
    }
}
