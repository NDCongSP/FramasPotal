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
    public class BarcodeSalesController : Controller
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
        #region empty page
        // GET: EmptyPage
        public ActionResult BarcodeSalesVNT()
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
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult BarcodeSalesVNT_OC(FormCollection fc)
        {
            string strOC = fc["OcDnInput"];
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "BarcodeSalesVNT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
                
            }
            
            return RedirectToAction("BarcodeSalesVNT_OC", "Report", new { strQr = "exec SP_DRPLabel_New_List_QRBarCode '" + strOC + "'" });

            //return RedirectToAction("Index", "Report");
        }

        public ActionResult BarcodeSalesVNT_HC(FormCollection fc)
        {
            string strOC = fc["OcDnInputHC"];
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "BarcodeSalesVNT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string str = Request.Params["command"];
            if (str == "Print HC")
            {
                return RedirectToAction("BarcodeSalesVNT_HC", "Report", new { strQr = "exec SP_DRPLabel_WH_QR_HC_List_Barcode '" + strOC + "'" });
            }
            else if (str == "Print HC (L/R)")
            {
                return RedirectToAction("BarcodeSalesVNT_HCLR", "Report", new { strQr = "exec SP_DRPLabel_New_HC_List_QRBarcode '" + strOC + "'" });
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult BarcodeSalesVNT_HCSample(FormCollection fc)
        {
            string strOC = fc["OcDnInputSample"];
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "BarcodeSalesVNT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("BarcodeSalesVNT_Sample", "Report", new { strQr = "exec SP_DRPLabel_New_QRSample_List '" + strOC + "'" });

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