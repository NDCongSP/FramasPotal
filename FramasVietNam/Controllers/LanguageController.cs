using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class LanguageController : Controller
    {
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;

        // GET: Language
        [HttpGet]
        public ActionResult Index()
        {
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));
            var ds_Language = db.sys_Language.ToList();
            ViewBag.ds_Language = ds_Language; 
            ViewBag.IDLanguage = cookie.Value;
            return View();
        }

        //get data from entity
        [HttpPost]
        public ActionResult Index(string dropLanguage)
        {
            if (dropLanguage == "0")
            {
                ViewBag.Message = "► " + Language.label("PleaseInputData") + ": Language";
                //Passing data to View
                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));
                var ds_Language = db.sys_Language.ToList();
                ViewBag.ds_Language = ds_Language;
                ViewBag.IDLanguage = cookie.Value;
                return View();
            }

            Response.Cookies["Language"].Value = dropLanguage;
            return RedirectToAction("Index");
        }

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
    }
}