using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAPI.Models;

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

            if (scItem.Paths.FullPath.Contains("Standard Values"))
                return;

            if ((scItem.TemplateID.ToString() == Constants.TemplateIDs.Trim_TemplateId && string.IsNullOrEmpty(scItem[Constants.FieldNames.SOPMatrixGuid_FieldName]))
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.SOPSpec_TemplateId
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.PredefinedSpec_TemplateId
                || scItem.TemplateID.ToString() == Constants.TemplateIDs.FreeFormSpec_TemplateId)// It's a Trim or a Spec 
            {
                try
                {
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

                    foreach (Item featureFolder in featuresFolders)
                    {
                        sbText.Append("<div><h1 style='color:blue';>").Append(featureFolder.Name).Append("</h1></div>").AppendLine();

                        foreach (Item tabSection in featureFolder.Children)
                        {
                            sbText.Append("<h2>").Append("*" + tabSection.Name).Append("</h2>").AppendLine().AppendLine();

                            foreach (Item spec in tabSection.Children)
                            {
                                sbText.Append("<div>").Append(spec[Constants.FieldNames.NameMultiline_FieldName]).Append(":").Append(spec[Constants.FieldNames.Spec_FieldName]).Append("</div>").AppendLine().AppendLine();
                                sbGuid.Append(spec.ID).Append(":").Append(spec[Constants.FieldNames.Spec_FieldName]).Append("/");
                            }
                        }
                    }

                    using (new SecurityDisabler())
                    {
                        if(scItem.TemplateID.ToString() == Constants.TemplateIDs.Trim_TemplateId)
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
                        else
                        {
                            Item trimsFolderItem = Helpers.GetCurrentTrimsFolderItem(scItem);
                            //Item previousItem = scItem.Axes.GetPreviousSibling();

                            foreach(Item trim in trimsFolderItem.GetChildren())//add new spec to the trims' Matrix fields
                            {
                                string currentGuildValue = trim[Constants.FieldNames.SOPMatrixGuid_FieldName];
                                //string[] pairs = currentGuildValue.Split('/');

                                string newPair = string.Empty;

                               
                                    if (scItem.TemplateID.ToString() == Constants.TemplateIDs.PredefinedSpec_TemplateId)
                                    {
                                        newPair += scItem.ID.ToString() + ":" + scItem.GetChildren().First()["Name"];
                                    }
                                    else
                                    {
                                        newPair += scItem.ID.ToString() + ":" + scItem["Spec"];
                                    }


                                string newValue = currentGuildValue += newPair;

                                trim.Editing.BeginEdit();
                                trim[Constants.FieldNames.SOPMatrixGuid_FieldName] = newValue;
                                trim.Editing.EndEdit();
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }
            //If a new spec ToDo: add other specs
            //else if(scItem.TemplateID.ToString() == Constants.TemplateIDs.SOPSpec_TemplateId)
            //{
            //    CommonModel model = new CommonModel();

            //    Item fsoItem = Helpers.GetCurrentFsoItem(scItem);
            //    Item yearItem = Helpers.GetCurrentYearItem(fsoItem);
            //    Item seriesItem = Helpers.GetCurrentSeriesItem(fsoItem);
            //    //model.SeriesItemName = seriesItem.Name + " " + yearItem.Name;

            //    //string updatedSpecs = model.UpdatedSpecs;

            //    //List<string> records = updatedSpecs.Split('*').ToList();
            //    string newFsoString = string.Empty;

            //    foreach (Item trim in model.Trims)
            //    {
            //        //get all value pairs for this trim
            //        List<string> currentPairs = trim[Constants.FieldNames.SOPMatrixGuid_FieldName].Split('/').ToList();
            //        List<string> updatedPairs = records.Where(x => x.Contains(trim.ID.ToString())).ToList();

            //        List<string> finalPairs = new List<string>();

            //        if (updatedPairs != null)
            //        {
            //            //Update Trim in sitecore
            //            StringBuilder sbText = new StringBuilder();
            //            StringBuilder sbGuid = new StringBuilder();

            //            using (new SecurityDisabler())
            //            {
            //                trim.Editing.BeginEdit();

            //                // Do your edits here
            //                foreach (Item featureFolder in model.Tabs)
            //                {
            //                    sbText.Append("<div style='color:blue;'><h1>").Append(featureFolder.Name).Append("</h1></div>").AppendLine();

            //                    foreach (Item tabSection in featureFolder.Children)
            //                    {
            //                        sbText.Append("<h2>").Append("*" + tabSection.Name).Append("</h2>").AppendLine().AppendLine();

            //                        foreach (Item spec in tabSection.Children)
            //                        {
            //                            bool update = false;
            //                            //read currentg spec value
            //                            string matchingSpecRecord = currentPairs.FirstOrDefault(x => x.Contains(spec.ID.ToString()));
            //                            string specValue = string.Empty;

            //                            if (!string.IsNullOrEmpty(matchingSpecRecord))
            //                            {
            //                                specValue = matchingSpecRecord.Split(':')[1];

            //                                foreach (string updatedPair in updatedPairs)
            //                                {
            //                                    if (updatedPair.Contains(spec.ID.ToString()))
            //                                    {
            //                                        //update Matrix fields
            //                                        update = true;
            //                                        specValue = updatedPair.Split(':')[1];
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                            else //new spec present
            //                            {
            //                                string y = updatedPairs.FirstOrDefault(x => x.Contains(spec.ID.ToString()));

            //                                if (!string.IsNullOrEmpty(y))
            //                                {
            //                                    string[] pair = y.Split(':');
            //                                    specValue = pair[1];
            //                                }
            //                            }

            //                            sbText.Append("<div>").Append(spec[Constants.FieldNames.NameMultiline_FieldName]).Append(":").Append(specValue).Append("</div>").AppendLine().AppendLine();
            //                            sbGuid.Append(spec.ID).Append(":").Append(specValue).Append("/");
            //                        }
            //                    }
            //                }

            //                trim[Constants.FieldNames.SOPMatrixText_FieldName] = sbText.ToString();
            //                trim[Constants.FieldNames.SOPMatrixGuid_FieldName] = sbGuid.ToString();
            //                trim.Editing.EndEdit();
            //            } // end of using for trim
            //        } //if (updatedPairs != null)

            //        newFsoString = newFsoString + "<br><br><div><h1 style='background-color:coral';>       " + trim.Name + "</h1></div>";
            //        string currentMatrixFieldValue = trim[Constants.FieldNames.SOPMatrixText_FieldName];
            //        newFsoString = newFsoString + currentMatrixFieldValue;
            //    } //end of foreach (Item trim in model.Trims)

            //    //updated FSO Matrix field
            //    using (new SecurityDisabler())
            //    {
            //        fsoItem.Editing.BeginEdit();
            //        fsoItem[Constants.FieldNames.SOPMatrixText_FieldName] = newFsoString;
            //        fsoItem.Editing.EndEdit();
            //    }
            //}
        }
    }
}