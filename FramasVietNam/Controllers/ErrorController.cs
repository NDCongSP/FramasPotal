using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;

namespace FramasVietNam.Controllers
{
    public class ErrorController : Controller
    {
        ////////////////////////////////////////////////////////////
        //Variabal
        #region Variabal
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region Error
        // GET: Error
        public ActionResult ErrorDefault()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something


            //
            return View();
        }
        public ActionResult Error500()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something


            //
            return View();
        }
        public ActionResult Error404()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something


            //
            return View();
        }
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        //Cookie allway exists in controller
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
        #endregion
        ////////////////////////////////////////////////////////////
    }
}