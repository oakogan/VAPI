using System.Linq;
using System.Web.Mvc;
using VAPI.Models;
using System.Web.Script.Serialization;

namespace VAPI.Controllers
{
    public class ApiController : Controller
    {
        //http://nine1.local/api/sitecore/API/GetSeriesById?seriesId={7D5179AD-845C-4A76-A074-4F3A2999AE38}

        // GET: Api
        #region API_Calls
        public ActionResult GetSeriesById(string seriesId)
        {
            //read query string for Series           
            if (string.IsNullOrEmpty(seriesId))
                return Json(new
                {
                    message = "Invalid Series ID"
                }, JsonRequestBehavior.AllowGet);

            //read query string for Year           
            string yearParam = Request.QueryString["year"];
            if (string.IsNullOrEmpty(yearParam))
            {
                yearParam = System.DateTime.Now.Year.ToString();
            }

            

            SeriesModel model = Helpers.InitializeSeriesModel(seriesId, yearParam);



            if(model == null)
                return Json(new
                {
                    message = "Invalid input"
                }, JsonRequestBehavior.AllowGet);

            var serializer = new JavaScriptSerializer();
            string VAPI = serializer.Serialize(model.Years.First());

            try
            {
                Year yearNew = serializer.Deserialize<Year>(VAPI);
            }
            catch
            {
                return Json(new
                {
                    message = "No result"
                }, JsonRequestBehavior.AllowGet);
            }
            

            return Json(new
            {
                VAPI
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}