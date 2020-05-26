using FramasVietNam.Common;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FramasVietNam.Controllers
{
    public class BarcodePPICVNTController : Controller
    {
        ////////////////////////////////////////////////////////////
        //Variabal

        #region Variabal

        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();

        //important Variabal cookie in multi language
        private HttpCookie cookie;

        #endregion Variabal

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        // GET: BarcodePPICVNT

        public ActionResult BarcodePPICVNT()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            return View();
        }

        public ActionResult BarcodePPICVNT_OS(FormCollection fc)
        {
            return RedirectToAction("BarcodePPICVNT", "Report", new { strQr = "exec SP_T024HK10_GetAll" });
        }

        ////////////////////////////////////////////////////////////
        //Cookie always exists in controller

        #region Cookie language

        //Check cookie
        public void CheckCookie()
        {
            cookie = Request.Cookies["Language"];
            if (cookie == null || cookie.Value == "")
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

        #endregion Cookie language

        ////////////////////////////////////////////////////////////
    }
}