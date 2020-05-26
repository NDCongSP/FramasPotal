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
    public class BarcodeSalesVNLController : Controller
    {
        // GET: BarcodeSalesVNL
        ////////////////////////////////////////////////////////////
        //Variabal
        #region Variabal
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion
        ////////////////////////////////////////////////////////////


        public ActionResult BarcodeSalesVNL()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something


            //
            return View();
        }

        public ActionResult BarcodeSalesVNL_OC(FormCollection fc)
        {
            string strOC = fc["OcInput"];
            string strDN = fc["DnInput"];
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "BarcodeSalesVNL").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");

            }
            string str = Request.Params["command"];
            if (str == "Print")
            {
                return RedirectToAction("BarcodeSalesVNL_OC", "Report", new { strQr = "exec SP_DRPLabel_New_DN_QR_Barcode '" + strOC + "', '" + strDN + "'" });
            }
            else if (str == "Print DN")
            {
                return RedirectToAction("BarcodeSalesVNL_OCDN", "Report", new { strQr = "exec SP_DRPLabel_New_DN_QR_Barcode '" + strOC + "', '" + strDN + "'" });
            }
            return RedirectToAction("Index", "Home");

            //return RedirectToAction("Index", "Report");
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
        #endregion
        ////////////////////////////////////////////////////////////
    }
}