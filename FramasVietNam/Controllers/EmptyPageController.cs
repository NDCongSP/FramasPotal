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
    [Authorize]
    public class EmptyPageController : Controller
    {
        ////////////////////////////////////////////////////////////
        //Variable
        #region Variabal
        //important Variable for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region empty page
        // GET: EmptyPage
        public ActionResult EmptyPage()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something


            //
            return View();
        }
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region test page
        // GET: EmptyPage
        [HttpGet]
        public ActionResult TestPage()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something


            //
            return View();
        }
        // post: EmptyPage
        [HttpPost]
        public ActionResult TestPage(FormCollection fc)
        {

            string str1 = fc["Content"];
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
                                   //
            return View();
        }
        //show popup when click detail
        public ActionResult ShowPopup()
        {
            
            var lst_KeyPost = "Hello";
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("TestPopup");
        }
        #endregion
        ////////////////////////////////////////////////////////////



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
        #endregion
        ////////////////////////////////////////////////////////////
    }
}