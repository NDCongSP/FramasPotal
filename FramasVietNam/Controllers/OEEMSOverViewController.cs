using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.OEEMS;
using FramasVietNam.Models.HumanResource;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using System.Text;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Globalization;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class OEEMSOverViewController : Controller
    {
        ////////////////////////////////////////////////////////////
        //Variable
        #region Variabal
        //important Variable for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        private OEEMSEntities db_OEE = new OEEMSEntities();
        private HumanResourceEntities db_HR = new HumanResourceEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion

        // GET: OEEMSOverView
        public ActionResult Index()
        {
            return View();
        }

        #region OEEMS Overview
        public ActionResult OEEMSOverView(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string dtdate = mFunction.ToString(fc["reservation"]);
            string _ToDate = DateTime.Now.ToString("MM/d/yyyy");
            string _fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/d/yyyy");

            if (dtdate != null && dtdate.Count() > 0)
            {
                _fromDate = dtdate.Substring(0, 10);
                _ToDate = dtdate.Substring(13, 10);
            }
            else
            {
                string _dtdate = mFunction.ToString(fc["reservation"]);
                if (_dtdate != null && _dtdate.Count() > 0)
                {
                    _fromDate = _dtdate.Substring(0, 10);
                    _ToDate = _dtdate.Substring(13, 10);
                }
            }

            ViewBag.dtDateSearch = _fromDate + " - " + _ToDate;
            ViewBag.titleview = "List Project of " + _fromDate + " - " + _ToDate; ;
            PassdataOverViewNew(_fromDate, _ToDate);
            return View();

        }

        public ActionResult OEEMSOverViewWeed(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string dtdate = mFunction.ToString(fc["datepicker"]);
            int chk_rd = mFunction.ToInt(fc["r1"]);

            if (dtdate != null && dtdate == "")
            {
                dtdate = DateTime.Now.ToString("MM/d/yyyy");
            }
            if (chk_rd == 0)
            {
                CultureInfo ciCurr = CultureInfo.CurrentCulture;
                int weekNum = ciCurr.Calendar.GetWeekOfYear(mFunction.ToDateTime(dtdate), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                ViewBag.titleview = "List Project with Week " + mFunction.ToString(weekNum);
            }
            else
            {
                ViewBag.titleview = "List Project with Month " + mFunction.ToString(mFunction.ToDateTime(dtdate).Month);
            }

            ViewBag.dtDateSearch = dtdate;
            ViewBag.rc = chk_rd;

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            // to do something
            if (chk_rd == 0)
            {
                var lstOverView = db_OEE.get_OEEMSWeekByDate(dtdate, chk_rd).ToList();
                ViewBag.lstOverView = lstOverView;
            }
            else if (chk_rd == 1)
            {
                var lstOverView1 = db_OEE.get_OEEMSWeekByDate_1(dtdate, chk_rd).ToList();
                ViewBag.lstOverView1 = lstOverView1;
            }

            return View();

        }

        public ActionResult OEEMSOverViewWorkPiece(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string dtdate = mFunction.ToString(fc["datepicker"]);
            int chk_rd = mFunction.ToInt(fc["r1"]);

            if (dtdate != null && dtdate == "")
            {
                dtdate = DateTime.Now.ToString("MM/d/yyyy");
            }
            if (chk_rd == 0)
            {
                CultureInfo ciCurr = CultureInfo.CurrentCulture;
                int weekNum = ciCurr.Calendar.GetWeekOfYear(mFunction.ToDateTime(dtdate), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                ViewBag.titleview = "List workpiece with Week " + mFunction.ToString(weekNum);
            }
            else
            {
                ViewBag.titleview = "List workpiece with Month " + mFunction.ToString(mFunction.ToDateTime(dtdate).Month);
            }

            ViewBag.dtDateSearch = dtdate;
            ViewBag.rc = chk_rd;

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            // var lstOverView = db_OEE.Database.ExecuteSqlCommand("[get_OEEMSWeekByDate]  '" + _date+ "', " +chk_rd );

            // to do something
            if (chk_rd == 0)
            {
                var lstOverView = db_OEE.get_OEEMSWeekWorkpieceByDate(dtdate, chk_rd).ToList();
                ViewBag.lstOverView = lstOverView;
            }
            else if (chk_rd == 1)
            {
                var lstOverView1 = db_OEE.get_OEEMSWeekWorkpieceByDate_1(dtdate, chk_rd).ToList();
                ViewBag.lstOverView1 = lstOverView1;
            }

            return View();

        }

        public ActionResult OEEMSOverViewProces(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string dtdate = mFunction.ToString(fc["datepicker"]);
            int chk_rd = mFunction.ToInt(fc["r1"]);

            if (dtdate != null && dtdate == "")
            {
                dtdate = DateTime.Now.ToString("MM/d/yyyy");
            }
            if (chk_rd == 0)
            {
                CultureInfo ciCurr = CultureInfo.CurrentCulture;
                int weekNum = ciCurr.Calendar.GetWeekOfYear(mFunction.ToDateTime(dtdate), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                ViewBag.titleview = "List Process with Week " + mFunction.ToString(weekNum);
            }
            else
            {
                ViewBag.titleview = "List Process with Month " + mFunction.ToString(mFunction.ToDateTime(dtdate).Month);
            }

            ViewBag.dtDateSearch = dtdate;
            ViewBag.rc = chk_rd;

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            // var lstOverView = db_OEE.Database.ExecuteSqlCommand("[get_OEEMSWeekByDate]  '" + _date+ "', " +chk_rd );

            // to do something
            if (chk_rd == 0)
            {
                var lstOverView = db_OEE.get_OEEMSWeekProcessByDate(dtdate, chk_rd).ToList();
                ViewBag.lstOverView = lstOverView;
            }
            else if (chk_rd == 1)
            {
                var lstOverView1 = db_OEE.get_OEEMSWeekProcessByDate_1(dtdate, chk_rd).ToList();
                ViewBag.lstOverView1 = lstOverView1;
            }

            return View();

        }

        public ActionResult OEEMSOverViewDT(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string dtdate = mFunction.ToString(fc["reservation"]);
            string _ToDate = DateTime.Now.ToString("MM/d/yyyy");
            string _fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/d/yyyy");

            if (dtdate != null && dtdate.Count() > 0)
            {
                _fromDate = dtdate.Substring(0, 10);
                _ToDate = dtdate.Substring(13, 10);
            }
            else
            {
                string _dtdate = mFunction.ToString(fc["reservation"]);
                if (_dtdate != null && _dtdate.Count() > 0)
                {
                    _fromDate = _dtdate.Substring(0, 10);
                    _ToDate = _dtdate.Substring(13, 10);
                }
            }

            ViewBag.dtDateSearch = _fromDate + " - " + _ToDate;
            ViewBag.titleview = "List DownTime of " + _fromDate + " - " + _ToDate; ;
            PassdataOverViewDT(_fromDate, _ToDate);
            return View();

        }

        public void PassdataOverView(string fromdate, string todate)
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lstOverView = db_OEE.get_OEEMSHeaderByDistanceDate(fromdate, todate).ToList();
            ViewBag.lstOverView = lstOverView;
        }

        public void PassdataOverViewNew(string fromdate, string todate)
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lstOverView = db_OEE.get_OEEMSHeaderByDistanceDate_new(fromdate, todate).ToList();
            ViewBag.lstOverView = lstOverView;
        }

        public void PassdataOverVieweekProcess(string _date, int chk_rd)
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            // var lstOverView = db_OEE.Database.ExecuteSqlCommand("[get_OEEMSWeekByDate]  '" + _date+ "', " +chk_rd );
            var lstOverView = db_OEE.get_OEEMSWeekProcessByDate(_date, chk_rd).ToList();
            ViewBag.lstOverView = lstOverView;
        }

        public void PassdataOverViewDT(string fromdate, string todate)
        {

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lstOverView = db_OEE.get_OEEMS_ResultDTDistanceDate(fromdate, todate).ToList();
            ViewBag.lstOverView = lstOverView;
        }
        #endregion

        #region OEEMS Input Load data
        public ActionResult OEEMSInput(FormCollection fc, string dtdate, string MessageComplete)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _dtdate = mFunction.ToString(fc["reservation"]);
            string _ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            string _fromDate = DateTime.Now.AddDays(-6).ToString("MM/dd/yyyy");

            if (_dtdate != null && _dtdate != "")
            {
                _fromDate = _dtdate.Substring(0, 10);
                _ToDate = _dtdate.Substring(13, 10);
            }
            else if (dtdate != null && dtdate != "")
            {
                _fromDate = dtdate.Substring(0, 10);
                _ToDate = dtdate.Substring(13, 10);
            }
            else
            {
                if (_dtdate != null && _dtdate.Count() > 0)
                {
                    _fromDate = _dtdate.Substring(0, 10);
                    _ToDate = _dtdate.Substring(13, 10);
                }
            }

            ViewBag.dtDateSearch = _fromDate + " - " + _ToDate;
            ViewBag.titleview = "List Project of " + _fromDate + " - " + _ToDate; ;
            PassdataOEEMSInput(_fromDate, _ToDate);
            ViewBag.MessageComplete = MessageComplete;
            return View();

        }

        public void PassdataOEEMSInput(string fromdate, string todate)
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstOverView = db_OEE.get_OEEMSHeaderByDistanceDate(fromdate, todate).ToList();
            ViewBag.lstOverView = lstOverView;
        }

        [HttpPost]
        public JsonResult PassdataOEEMSInputPN(string ProjectName)
        {
            if (ProjectName.Trim() == "")
            {
                var _lsttblProject = db_OEE.sp_tblProject_Get().ToList();
                return Json(_lsttblProject, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _lsttblProject = db_OEE.sp_tblProject_Get().Where(m => m.ProjectName.Contains(ProjectName)).ToList();
                return Json(_lsttblProject, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult PassdataOEEMSInputCN(string ProjectID, string CompName)
        {
            if ((ProjectID.Trim()) == "")
            {
                var _lsttblProjectComp = db_OEE.sp_tblProjectComp_Get().ToList();
                return Json(_lsttblProjectComp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _lsttblProjectComp = db_OEE.sp_tblProjectComp_Get().Where(m => m.ProjectID.Equals(ProjectID) && m.CompName.Contains(CompName.Trim())).ToList();
                return Json(_lsttblProjectComp, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult PassdataOEEMSInputCNNew(string ProjectID)
        {
            var _lsttblProjectComp = db_OEE.sp_tblProjectComp_Get().Where(m => m.ProjectID.Equals(ProjectID)).ToList();
            return Json(_lsttblProjectComp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PassdataOEEMSInputSN(string ProjectID, string SizeName)
        {
            if ((ProjectID.Trim()) == "")
            {
                var _lsttblProjectSize = db_OEE.sp_tblProjectSize_Get().ToList();
                return Json(_lsttblProjectSize, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var _lsttblProjectSize = db_OEE.sp_tblProjectSize_Get().Where(m => m.ProjectID.Equals(ProjectID) && m.SizeName.Contains(SizeName.Trim())).ToList();
                return Json(_lsttblProjectSize, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult PassdataOEEMSInputSNNew(string ProjectID)
        {
            var _lsttblProjectSize = db_OEE.sp_tblProjectSize_Get().Where(m => m.ProjectID.Equals(ProjectID)).ToList();
            return Json(_lsttblProjectSize, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region OEEMS InputAdd
        public ActionResult OEEMSAddPopup(string dtdate)
        {
            var _lsttblProject = db_OEE.tblProjects.ToList();
            ViewBag.lsttblProject = _lsttblProject;

            var _lsttblProjectComp = db_OEE.sp_tblProjectComp_Get().ToList();
            ViewBag.lsttblProjectComp = _lsttblProjectComp;

            var _lsttblProjectSize = db_OEE.sp_tblProjectSize_Get().ToList();
            ViewBag.lsttblProjectSize = _lsttblProjectSize;

            var _lsttblWorkPiece = db_OEE.tblWorkpieces.ToList();
            ViewBag.lsttblWorkPiece = _lsttblWorkPiece;

            var _tblWorkpieceProcess = db_OEE.sp_tblWorkpieceProcess_Get().ToList();
            ViewBag.tblWorkpieceProcess = _tblWorkpieceProcess;

            var _lsttblComp = db_OEE.tblComps.ToList();
            ViewBag.lsttblComp = _lsttblComp;


            //Add Records into generic list
            List<tblWorkPieceModel> obj = new List<tblWorkPieceModel>();
            //

            foreach (var item in _tblWorkpieceProcess)
            {
                obj.Add(new tblWorkPieceModel
                {
                    WorkpieceID = mFunction.ToInt(item.WorkpieceID),
                    ProcessID = mFunction.ToInt(item.ProcessID),
                    WorkpieceName = item.WorkpieceName.ToString(),
                    ProcessName = item.ProcessName.ToString(),
                    plantime = 0,
                    hour = 0,
                    munite = 0
                });
            }

            tblWorkPieceList objBind = new tblWorkPieceList
            {
                tblWorkPiece = obj
            };

            ViewBag.dtDateSearch = dtdate;
            return PartialView("OEEMSPopupAdd", objBind);
        }

        [HttpPost]
        public ActionResult OEEMSInputAdd(FormCollection fc, get_OEEMSHeaderByDistanceDate_Result tblProject, tblWorkPieceList Obj)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();

            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _OEEID = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            DateTime _Date = mFunction.ToDateTime(fc["datepicker"]);

            string _ProjectID = mFunction.ToString(fc["ProjectID"]);
            string _CompID = mFunction.ToString(fc["CompID"]);
            string _SizeID = mFunction.ToString(fc["SizeID"]);
            string _UserID = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {
                try
                {
                    var sp_InsOEEMS_Header = db_OEE.sp_InsOEEMS_Header(_OEEID, _Date, _ProjectID, _CompID, _SizeID, _UserID);

                    foreach (var item in Obj.tblWorkPiece)
                    {
                        if (mFunction.ToInt(item.hour) + mFunction.ToInt(item.munite) > 0)
                        {
                            var ins = db_OEE.sp_InsOEEMS_Process(0, _OEEID, mFunction.ToInt(item.ProcessID), mFunction.ToInt(item.WorkpieceID), mFunction.ToInt(item.hour * 60) + mFunction.ToInt(item.munite), _UserID);
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs  
                    ex.ToString();
                }
            }

            string _id = mFunction.ToString(fc["reservation"]);
            return RedirectToAction("OEEMSInput", new { @dtdate = _id, @MessageComplete = "Insert data complete" });
        }
        #endregion

        #region OEEMS InputEdit
        public ActionResult OEEMSInputEditLoad(string OEEID, string dtdate)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var _lsttblProject = db_OEE.tblProjects.ToList();
            ViewBag.lsttblProject = _lsttblProject;

            var _lsttblWorkPiece = db_OEE.tblWorkpieces.ToList();
            ViewBag.lsttblWorkPiece = _lsttblWorkPiece;

            var lstOverView = db_OEE.tblOEEMS_Header.Where(m => m.OEEID.Equals(OEEID)).ToList();
            ViewBag.lstOverView = lstOverView;

            var _lsttblComp = db_OEE.tblComps.ToList();
            ViewBag.lsttblComp = _lsttblComp;

            foreach (var item in lstOverView)
            {
                var _lstProjectComp = db_OEE.tblProjectComps.Where(m => m.ProjectID.Equals(item.ProjectID)).ToList();
                ViewBag.lstProjectComp = _lstProjectComp;

                var _lstProjectSize = db_OEE.tblProjectSizes.Where(m => m.ProjectID.Equals(item.ProjectID)).ToList();
                ViewBag.lstProjectSize = _lstProjectSize;
            }

            //Add Records into generic list    Where(m => m.o.Contains(ProjectName)).ToList();
            List<tblWorkPieceModel> obj = new List<tblWorkPieceModel>();
            //
            var _tblWorkpieceProcess = db_OEE.sp_tblWorkpieceProcess_edit_Get(OEEID).ToList();
            foreach (var item in _tblWorkpieceProcess)
            {
                obj.Add(new tblWorkPieceModel
                {
                    ID = item.ID,
                    WorkpieceID = mFunction.ToInt(item.WorkpieceID),
                    ProcessID = mFunction.ToInt(item.ProcessID),
                    WorkpieceName = item.WorkpieceName.ToString(),
                    ProcessName = item.ProcessName.ToString(),
                    plantime = mFunction.ToInt(item.PlanTime),
                    hour = mFunction.ToInt(item.PlanTime / 60),
                    munite = mFunction.ToInt(item.PlanTime) % 60,
                    Realtime = mFunction.ToInt(item.RealTime),
                    Realhour = mFunction.ToInt(item.RealTime / 60),
                    Realmunite = mFunction.ToInt(item.RealTime) % 60
                });
            }

            tblWorkPieceList objBind = new tblWorkPieceList
            {
                tblWorkPiece = obj
            };
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            //CheckCookie();
            //ChangeLanguage(cookie.Value);
            ViewBag.dtDateSearch = dtdate;
            return PartialView("OEEMSPopupEdit", objBind);
        }

        public ActionResult OEEMSInputEdit(FormCollection fc, tblWorkPieceList Obj)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();

            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _OEEID = mFunction.ToString(fc["OEEIDedit"]);
            DateTime _Date = mFunction.ToDateTime(fc["datepickeredit"]);

            string _ProjectID = mFunction.ToString(fc["ProjectIDedit"]);
            string _CompID = mFunction.ToString(fc["CompIDedit"]);
            string _SizeID = mFunction.ToString(fc["SizeIDedit"]);

            string _UserID = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

                try
                {
                    var sp_InsOEEMS_Header = db_OEE.sp_InsOEEMS_Header(_OEEID, _Date, _ProjectID, _CompID, _SizeID, _UserID);

                    foreach (var item in Obj.tblWorkPiece)
                    {
                        if (mFunction.ToInt(item.hour) + mFunction.ToInt(item.munite) > 0)
                        {
                            var ins = db_OEE.sp_InsOEEMS_Process(mFunction.ToInt(item.ID), _OEEID, mFunction.ToInt(item.ProcessID), mFunction.ToInt(item.WorkpieceID), mFunction.ToInt(item.hour * 60) + mFunction.ToInt(item.munite), _UserID);
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs  
                    ex.ToString();

                }

            }

            string _id = mFunction.ToString(fc["reservation"]);
            return RedirectToAction("OEEMSInput", new { @dtdate = _id, @MessageComplete = "Update data complete" });
        }

        public ActionResult OEEMSInputAddComLoad(string OEEID, string dtdate)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var lstOverView = db_OEE.tblOEEMS_Header.Where(m => m.OEEID.Equals(OEEID)).ToList();
            ViewBag.lstOverView = lstOverView;

            foreach (var item in lstOverView)
            {
                var _lstProjectComp = db_OEE.tblProjectComps.Where(m => m.ProjectID.Equals(item.ProjectID)).ToList();
                ViewBag.lstProjectComp = _lstProjectComp;

                var _lstProjectSize = db_OEE.tblProjectSizes.Where(m => m.ProjectID.Equals(item.ProjectID)).ToList();
                ViewBag.lstProjectSize = _lstProjectSize;
            }

            //Add Records into generic list    Where(m => m.o.Contains(ProjectName)).ToList();
            List<tblOEEMS_PercentComplete> obj = new List<tblOEEMS_PercentComplete>();
            //
            var _tblOEEMS_PercentComplete = db_OEE.tblOEEMS_PercentComplete.Where(m => m.OEEID.Equals(OEEID)).ToList();

            foreach (var item in _tblOEEMS_PercentComplete)
            {
                obj.Add(new tblOEEMS_PercentComplete
                {
                    ID = item.ID,
                    OEEID = mFunction.ToString(item.OEEID),
                    DateOfWeek = item.DateOfWeek,
                    WeedOfYear = mFunction.ToInt(item.WeedOfYear),
                    PercentCom = mFunction.ToDouble(item.PercentCom)
                });
            }

            tblOEEMS_PercentCompleteList objBind = new tblOEEMS_PercentCompleteList
            {
                tblOEEMS_PercentComplete = obj
            };

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            //CheckCookie();
            //ChangeLanguage(cookie.Value);
            ViewBag.dtDateSearch = dtdate;
            return PartialView("OEEMSPopupAddCom", objBind);
        }

        public ActionResult OEEMSInputChangeProcess(string OEEID, string dtdate)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var lstOverView = db_OEE.tblOEEMS_Header.Where(m => m.OEEID.Equals(OEEID)).ToList();
            ViewBag.lstOverView = lstOverView;

            foreach (var item in lstOverView)
            {
                var _lstProjectComp = db_OEE.tblProjectComps.Where(m => m.ProjectID.Equals(item.ProjectID)).ToList();
                ViewBag.lstProjectComp = _lstProjectComp;

                var _lstProjectSize = db_OEE.tblProjectSizes.Where(m => m.ProjectID.Equals(item.ProjectID)).ToList();
                ViewBag.lstProjectSize = _lstProjectSize;
            }

            //
            var _tblWorkpieceProcess = db_OEE.sp_tblWorkpieceProcess_edit_Get(OEEID).ToList();
            ViewBag.tblWorkpieceProcess = _tblWorkpieceProcess;
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            //CheckCookie();
            //ChangeLanguage(cookie.Value);
            ViewBag.dtDateSearch = dtdate;
            return PartialView("OEEMSPopupChangeProcess", null);
        }

        public ActionResult OEEMSInputChangeProcessEdit(FormCollection fc)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();

            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _OEEID = mFunction.ToString(fc["OEEIDCP"]);


            int _WorkPieceID = mFunction.ToInt(fc["WorkPieceIDCP"]);
            int _ProcessIDOld = mFunction.ToInt(fc["ProcessIDOld"]);
            int _ProcessIDNew = mFunction.ToInt(fc["ProcessIDNew"]);

            string _UserID = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

                try
                {
                    var sp_InsOEEMS_Header = db_OEE.sp_udpOEEMS_ChangeProcess(_OEEID, _WorkPieceID, _ProcessIDOld, _ProcessIDNew, _UserID);

                }
                catch (Exception ex)
                {
                    ex.ToString();
                    //Rollback transaction if exception occurs                          ex.ToString();
                }

            }
            string _id = mFunction.ToString(fc["reservation"]);
            return RedirectToAction("OEEMSInput", new { @dtdate = _id, @MessageComplete = "Update data complete" });
        }

        [HttpPost]
        public JsonResult PassdataOEEMSProcess(string OEEID, int WorkpieceID)
        {
            var _tblProcessOld = db_OEE.sp_tblWorkpieceProcess_edit_Get(OEEID).Where(m => m.WorkpieceID.Equals(WorkpieceID)).ToList();
            return Json(_tblProcessOld, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PassdataOEEMSProcessOld(string OEEID, int WorkpieceID)
        {
            var _tblProcessOld = db_OEE.sp_tblWorkpieceProcess_edit_Get(OEEID).Where(m => m.WorkpieceID.Equals(WorkpieceID) && m.OEEID != null).ToList();
            return Json(_tblProcessOld, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OEEMSInputAddCom(FormCollection fc, get_OEEMSHeaderByDistanceDate_Result tblProject, tblOEEMS_PercentCompleteList Obj)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();

            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _OEEID = mFunction.ToString(fc["OEEIDAddCom"]);
            // DateTime _Date = mFunction.ToDateTime(fc["datepicker"]);

            string _ProjectID = tblProject.ProjectID;
            string _CompID = mFunction.ToString(tblProject.CompID);
            string _SizeID = mFunction.ToString(tblProject.SizeID);

            string _UserID = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

                try
                {
                    foreach (var item in Obj.tblOEEMS_PercentComplete)
                    {
                        if (mFunction.ToDouble(item.PercentCom) > 0)
                        {
                            var ins = db_OEE.sp_InsOEEMS_PercentComplete(mFunction.ToInt(item.ID), _OEEID, mFunction.ToInt(item.WeedOfYear), mFunction.ToDouble(item.PercentCom), _UserID);
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs  
                    ex.ToString();

                }

            }

            string _id = mFunction.ToString(fc["reservation"]);
            return RedirectToAction("OEEMSInput", new { @dtdate = _id, @MessageComplete = "Update data complete" });
        }
        #endregion

        #region OEEMS Input Delete & Stop
        public ActionResult OEEMSInputDelete(string OEEID, string dtdate)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _ToDate = DateTime.Now.ToString("MM/d/yyyy");
            string _fromDate = DateTime.Now.AddDays(-7).ToString("MM/d/yyyy");
            string _UserID = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

                try
                {
                    var sp_InsOEEMS_Header = db_OEE.sp_DelOEEMS_Header(OEEID, null, "", "", "", _UserID);

                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs  
                    ex.ToString();
                }

            }

            return RedirectToAction("OEEMSInput", new { @dtdate = dtdate });
        }

        public ActionResult OEEMSInputStop(string OEEID, string dtdate)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSOverView").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _ToDate = DateTime.Now.ToString("MM/d/yyyy");
            string _fromDate = DateTime.Now.AddDays(-7).ToString("MM/d/yyyy");
            string _UserID = User.Identity.GetUserName();

            if (ModelState.IsValid)
            {

                try
                {
                    var sp_InsOEEMS_Header = db_OEE.sp_EndOEEMS_Header(OEEID, _UserID);
                }
                catch (Exception ex)
                {
                    //Rollback transaction if exception occurs  
                    ex.ToString();
                }

            }

            return RedirectToAction("OEEMSInput", new { @dtdate = dtdate });
        }
        #endregion

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