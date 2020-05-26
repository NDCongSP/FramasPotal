using FramasVietNam.Common;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FramasVietNam.Controllers
{
    public class BaseController : Controller
    {
        #region Global variable

        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();

        //important Variabal cookie in multi language
        private HttpCookie cookie;

        #endregion Global variable

        // GET: Base        
        #region Cookie

        //Check cookie
        public void CheckCookie()
        {
            cookie = Request.Cookies["Language"];
            if (cookie == null || cookie.Value == string.Empty)
            {
                Response.Cookies["Language"].Value = "1";
                cookie = Response.Cookies["Language"];
                cookie.Value = "1";
            }
        }

        //get data from entity and push to master view
        public void ChangeLanguage(string textInput)
        {
            Response.Cookies["Language"].Value = textInput;
            object ds_Module = db.sp_Module_Get(mFunction.ToInt(textInput), User.Identity.GetUserName()).OrderBy(m => m.Code).ToList();
            object ds_Group = db.sp_Group_Get(mFunction.ToInt(textInput)).OrderBy(m => m.Code).ToList();
            ViewBag.ds_Module = ds_Module;
            ViewBag.ds_Group = ds_Group;
            string strModuleID = mFunction.ToString(Session["ModuleID"]);
            ViewBag.strModuleID = strModuleID;
            string strGroupID = mFunction.ToString(Session["GroupID"]);
            ViewBag.strGroupID = strGroupID;
        }

        #endregion Cookie

        #region Change language

        public ActionResult ChangeLang(string lang)
        {
            if (lang != null)
            {
                Response.Cookies["Language"].Value = lang;
            }
            HttpCookie cookie = new HttpCookie("Lang");
            cookie.Value = lang;
            Response.Cookies.Add(cookie);
            return Redirect(Request.UrlReferrer.ToString());
        }

        //change language Finger
        [HttpGet]
        public ActionResult English_FingerM1()
        {
            Response.Cookies["Language"].Value = "1";
            return RedirectToAction("MealOrderFingerM1", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult VietNam_FingerM1()
        {
            Response.Cookies["Language"].Value = "2";
            return RedirectToAction("MealOrderFingerM1", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult Germany_FingerM1()
        {
            Response.Cookies["Language"].Value = "3";
            return RedirectToAction("MealOrderFingerM1", "MealControl");
        }

        //change language Finger
        [HttpGet]
        public ActionResult English_FingerM2()
        {
            Response.Cookies["Language"].Value = "1";
            return RedirectToAction("MealOrderFingerM2", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult VietNam_FingerM2()
        {
            Response.Cookies["Language"].Value = "2";
            return RedirectToAction("MealOrderFingerM2", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult Germany_FingerM2()
        {
            Response.Cookies["Language"].Value = "3";
            return RedirectToAction("MealOrderFingerM2", "MealControl");
        }

        #endregion Change language
    }
}