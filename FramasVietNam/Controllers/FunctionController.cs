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
    public class FunctionController : Controller
    {
        ////////////////////////////////////////////////////////////////////////////////
        #region globar variable
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        private object ds_Functions;
        #endregion
        ////////////////////////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////////////////////////
        #region Function
        // GET: Function
        [HttpGet]
        public ActionResult Index()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            try
            {
                string moduleID = Session["ModuleID"].ToString();
                string groupID = Session["GroupID"].ToString();
                //passing data to view
                ds_Functions = db.sp_Function_Get(mFunction.ToInt(cookie.Value), mFunction.ToInt(groupID)).ToList();
                ViewBag.ds_Functions = ds_Functions;

                var objMlGrp = db.sp_ModuleGroupGet(mFunction.ToInt(cookie.Value), mFunction.ToInt(moduleID), mFunction.ToInt(groupID)).ToList();
                    var strGroup = "";
                var strModule = "";
                if (objMlGrp != null)
                {
                    strGroup = objMlGrp[0].GroupName;
                    strModule = objMlGrp[0].ModuleName;
                }
                ViewBag.strGroup = strGroup;
                ViewBag.strModule = strModule;
                return View("Index");
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                ds_Functions = db.sp_Function_Get(mFunction.ToInt(cookie.Value), mFunction.ToInt(0)).ToList();
                ViewBag.ds_Functions = ds_Functions;
                return View("Index");
            }
        }

        // GET: Function
        [HttpPost]
        public ActionResult Index(string ModuleID, string GroupID)
        {
            Session["ModuleID"] = ModuleID;
            Session["GroupID"] = GroupID;
            return RedirectToAction("Index");
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////////


            
        ////////////////////////////////////////////////////////////////////////////////
        #region Cookie
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
        ////////////////////////////////////////////////////////////////////////////////
    }
}