using FramasVietNam.Models.FingerScan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using FramasVietNam.Common;

namespace FramasVietNam.Controllers
{
    public class LoginFingerController : Controller
    {
        // GET: LoginFinger
        public ActionResult FingerM1()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MealOrderFingerM1", "MealControl");
            }
            //Delete data table tbl_LogFinger_Login_M1
            DataOperation.ExecuteNonQuery(GlobalVariable.DBFingerScan, "Delete from tbl_LogFinger_Login_M1");
            return View();
        }
        public JsonResult GetNotificationLoginM1()
        {
            List<tbl_LogFinger_Login_M1> obj = SendNotificationsLoginM1.GetNotificationLoginM1();
            Session["IDUserM1"] = "";
            if (obj.Count > 0)
            {
                Session["IDUserM1"] = obj[0].IndRegID;
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(obj);
            var result = Json(json, JsonRequestBehavior.AllowGet);
            return result;
        }
        // GET: LoginFinger
        public ActionResult FingerM2()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MealOrderFingerM2", "MealControl");
            }
            //Delete data table tbl_LogFinger_Login_M1
            DataOperation.ExecuteNonQuery(GlobalVariable.DBFingerScan, "Delete from tbl_LogFinger_Login_M2");
            return View();
        }
        public JsonResult GetNotificationLoginM2()
        {
            //return Json(SendNotificationsLoginM2.GetNotificationLoginM2(), JsonRequestBehavior.AllowGet);
            List<tbl_LogFinger_Login_M1> obj = SendNotificationsLoginM2.GetNotificationLoginM2();
            Session["IDUserM2"] = "";
            if (obj.Count > 0)
            {
                Session["IDUserM2"] = obj[0].IndRegID;
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(obj);
            var result = Json(json, JsonRequestBehavior.AllowGet);
            return result;
        }
    }
}