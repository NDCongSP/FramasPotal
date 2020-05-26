using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using System.Data;
using FramasVietNam.Reports;
using FramasVietNam.Reports.BarCode;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {

        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;

        // GET: Report
        public ActionResult Index(string strQr)
        {
            rpt_SaturdayOffPlanning rpt = new rpt_SaturdayOffPlanning();
            DataTable dt = DataOperation.getDataTable(GlobalVariable.DBMenuManager, strQr);
            rpt.DataSource = dt;
            rpt.DataMember = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            return View(rpt);
        }

        // GET: Report Work Load Print
        public ActionResult WorkLoadPrint(string strQr)
        {
            rpt_WorkLoad rpt = new rpt_WorkLoad();
            DataTable dt = DataOperation.getDataTable(GlobalVariable.DBMenuManager, strQr);
            rpt.DataSource = dt;
            rpt.DataMember = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();        
            ChangeLanguage(cookie.Value);
            return View(rpt);
        }


        public ActionResult BarcodeSalesVNT_OC(string strQr)
        {
            //rpt_BarCodeSalesNVT_OC rpt = new rpt_BarCodeSalesNVT_OC();
            rpt_BarCodeSalesVNT_OS rpt = new rpt_BarCodeSalesVNT_OS();
            DataTable dt                 = DataOperation.getDataTable(GlobalVariable.DBVNT86, strQr);
            rpt.DataSource               = dt;
            rpt.DataMember               = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View("Index",rpt);
        }

        public ActionResult BarcodeSalesVNT_HC(string strQr)
        {
            //rpt_BarCodeSalesNVT_OC rpt = new rpt_BarCodeSalesNVT_OC();
            rpt_BarCodeSalesVNT_HC rpt = new rpt_BarCodeSalesVNT_HC();
            DataTable dt = DataOperation.getDataTable(GlobalVariable.DBVNT86, strQr);
            rpt.DataSource = dt;
            rpt.DataMember = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View("Index", rpt);
        }

        public ActionResult BarcodeSalesVNT_HCLR(string strQr)
        {
            //rpt_BarCodeSalesNVT_OC rpt = new rpt_BarCodeSalesNVT_OC();
            rpt_BarCodeSalesVNT_HC_LR rpt = new rpt_BarCodeSalesVNT_HC_LR();
            DataTable dt = DataOperation.getDataTable(GlobalVariable.DBVNT86, strQr);
            rpt.DataSource = dt;
            rpt.DataMember = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View("Index", rpt);
        }
 

        public ActionResult BarcodeSalesVNT_Sample(string strQr)
        {
            rpt_BarCodeSalesVNT_Sample rpt = new rpt_BarCodeSalesVNT_Sample();
            DataTable dt = DataOperation.getDataTable(GlobalVariable.DBVNT86, strQr);
            rpt.DataSource = dt;
            rpt.DataMember = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View("Index", rpt);
        }

        public ActionResult BarcodeSalesVNL_OC(string strQr)
        {
            rpt_BarCodeSalesVNL_OC_1 rpt = new rpt_BarCodeSalesVNL_OC_1();
            //rpt_BarCodeSalesVNT_Sample rpt = new rpt_BarCodeSalesVNT_Sample();
            DataTable dt = DataOperation.getDataTable(GlobalVariable.DBVNL86, strQr);
            rpt.DataSource = dt;
            rpt.DataMember = "dt";
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View("Index", rpt);
        }

        // GET: Report
        public ActionResult ListReport()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            return View();
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