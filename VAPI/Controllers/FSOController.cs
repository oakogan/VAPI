using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data.Items;
using VAPI.Models;
using System.Text;
using Sitecore.SecurityModel;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Web.Script.Serialization;
using System;

namespace VAPI.Controllers
{
    public class FSOController : Controller
    {
        #region ActionMethods
        public ActionResult LoadFSO()
        {
            CommonModel model = InitializeTrims(new CommonModel() { ContextItemId = Sitecore.Context.Item.ID.ToString(), PreviewUrl = HttpContext.Request.Url.AbsoluteUri });
            Item seriesItem = Helpers.GetCurrentSeriesItem(Sitecore.Context.Item);
            Item yearItem = Helpers.GetCurrentYearItem(Sitecore.Context.Item);

            model.SeriesItemName = seriesItem.Name + " " + yearItem.Name;

            return View("~/Views/FSO/FSONew.cshtml", model);
        }

        /// <summary>
        /// Action method to handle user input
        /// </summary>      
        [HttpPost]
        public void FsoSave(CommonModel model)
        {
            try
            {
                model = InitializeTrims(model);

                Item fsoItem = Sitecore.Context.Database.GetItem(model.ContextItemId);
                Item yearItem = Helpers.GetCurrentYearItem(fsoItem);
                Item seriesItem = Helpers.GetCurrentSeriesItem(fsoItem);
                model.SeriesItemName = seriesItem.Name + " " + yearItem.Name;

                string updatedSpecs = model.UpdatedSpecs;

                List<string> records = updatedSpecs.Split('*').ToList();
                string newFsoString = string.Empty;

                foreach (Item trim in model.Trims)
                {
                    //get all value pairs for this trim
                    List<string> currentPairs = trim[Constants.FieldNames.SOPMatrixGuid_FieldName].Split('/').ToList();
                    List<string> updatedPairs = records.Where(x => x.Contains(trim.ID.ToString())).ToList();

                    List<string> finalPairs = new List<string>();

                    if (updatedPairs != null)
                    {
                        //Update Trim in sitecore
                        StringBuilder sbText = new StringBuilder();
                        StringBuilder sbGuid = new StringBuilder();

                        using (new SecurityDisabler())
                        {
                            trim.Editing.BeginEdit();

                            // Do your edits here
                            foreach (Item featureFolder in model.Tabs)
                            {
                                sbText.Append("<div style='color:blue;'><h1>").Append(featureFolder.Name).Append("</h1></div>").AppendLine();

                                foreach (Item tabSection in featureFolder.Children)
                                {
                                    sbText.Append("<h2>").Append("*" + tabSection.Name).Append("</h2>").AppendLine().AppendLine();

                                    foreach (Item spec in tabSection.Children)
                                    {
                                        bool update = false;
                                        //read currentg spec value
                                        string matchingSpecRecord = currentPairs.FirstOrDefault(x => x.Contains(spec.ID.ToString()));
                                        string specValue = string.Empty;

                                        if (!string.IsNullOrEmpty(matchingSpecRecord))
                                        {
                                            specValue = matchingSpecRecord.Split(':')[1];

                                            foreach (string updatedPair in updatedPairs)
                                            {
                                                if (updatedPair.Contains(spec.ID.ToString()))
                                                {
                                                    //update Matrix fields
                                                    update = true;
                                                    specValue = updatedPair.Split(':')[1];
                                                    break;
                                                }
                                            }
                                        }
                                        else //new spec present
                                        {
                                            string y = updatedPairs.FirstOrDefault(x => x.Contains(spec.ID.ToString()));

                                            if (!string.IsNullOrEmpty(y))
                                            {
                                                string[] pair = y.Split(':');
                                                specValue = pair[1];
                                            }
                                        }

                                        sbText.Append("<div>").Append(spec[Constants.FieldNames.NameMultiline_FieldName]).Append(":").Append(specValue).Append("</div>").AppendLine().AppendLine();
                                        sbGuid.Append(spec.ID).Append(":").Append(specValue).Append("/");
                                    }
                                }
                            }

                            trim[Constants.FieldNames.SOPMatrixText_FieldName] = sbText.ToString();
                            trim[Constants.FieldNames.SOPMatrixGuid_FieldName] = sbGuid.ToString();
                            trim.Editing.EndEdit();
                        } // end of using for trim
                    } //if (updatedPairs != null)

                    newFsoString = newFsoString + "<br><br><div><h1 style='background-color:coral';>       " + trim.Name + "</h1></div>";
                    string currentMatrixFieldValue = trim[Constants.FieldNames.SOPMatrixText_FieldName];
                    newFsoString = newFsoString + currentMatrixFieldValue;
                } //end of foreach (Item trim in model.Trims)

                //updated FSO Matrix field
                using (new SecurityDisabler())
                {
                    fsoItem.Editing.BeginEdit();
                    fsoItem[Constants.FieldNames.SOPMatrixText_FieldName] = newFsoString;
                    fsoItem.Editing.EndEdit();
                }

                //return View("~/Views/FSO/FSONew.cshtml", model);
                //return RedirectToAction("LoadFSO");
                Response.Redirect(model.PreviewUrl);
            }
            catch(Exception ex) { Response.Redirect(model.PreviewUrl); }
        }

      


        #endregion

        #region Private Methods

        private CommonModel InitializeTrims(CommonModel model)
        {
            Item contextItem = null;

            if (Sitecore.Context.Item == null)
            {
                contextItem = Sitecore.Context.Database.GetItem(model.ContextItemId);
            }
            else
            {
                contextItem = Sitecore.Context.Item;
            }

            List<Item> trims = null;
            List<Item> featuresFolders = null;

            Item commonDataFolder = Helpers.GetCurrentDataFolderItem(contextItem);
            if (commonDataFolder != null)
            {
                featuresFolders = commonDataFolder.GetChildren().ToList();
                model.Tabs = featuresFolders;
            }

            Item trimsFolder = Helpers.GetCurrentTrimsFolderItem(contextItem);
            if (trimsFolder != null)
            {
                trims = trimsFolder.GetChildren().ToList();
                model.Trims = trims;
            }

            return model;
        }

        #endregion

        

        
    }
}