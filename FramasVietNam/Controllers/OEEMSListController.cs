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

namespace FramasVietNam.Controllers
{
    public class OEEMSListController : Controller
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



        ////////////////////////////////////////////////////////////
        #region TblCategory
        // GET: tblCategory
        public ActionResult TblCategory()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblCategory").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblCategory();
            //
            return View();
        }

        // post TblCategorySave
        [HttpPost]
        public ActionResult TblCategorySave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _id = mFunction.ToString(fc["txtCode"]);
                string _name = mFunction.ToString(fc["txtName"]);
                string _CategoryS = mFunction.ToString(fc["txtCategoryS"]);

                //Check null data
                if (_id == "" || _name == "")
                {
                    //Pass data to view
                    PassdataTotblCategory();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblCategory");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_id);
                var _objCheck = db_OEE.tblCategories.Where(m => m.CategoryID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblCategory();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _id;
                    //
                    return View("TblCategory");
                }

                //save data
                tblCategory obj = new tblCategory
                {
                    CategoryID = _key,
                    CategoryName = _name,
                    CategoryS = mFunction.ToInt(_CategoryS)
                };
                db_OEE.tblCategories.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblCategory", "OEEMSList", "Insert", "Add a New tblCategory", "ID: " + _id, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblCategory");
            }
            catch
            {
                //Pass data to view
                PassdataTotblCategory();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblCategory");
            }
        }

        //Pass data to view
        private void PassdataTotblCategory()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lsttblCategory = db_OEE.tblCategories.ToList();
            ViewBag.lsttblCategory = lsttblCategory;

            var _objLst = db_OEE.tblCategories.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.CategoryID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete TblCategory
        public ActionResult TblCategoryDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblCategory").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblCategory obj = db_OEE.tblCategories.SingleOrDefault(m => m.CategoryID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblCategory();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblCategory");
                }
                db_OEE.tblCategories.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblCategory", "OEEMSList", "Delete", "Del tblCategory", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblCategory");
            }
            catch
            {
                //pass date to view
                PassdataTotblCategory();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblCategory");
            }
        }

        //Edit tblCategory
        [HttpGet]
        public ActionResult TblCategoryEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblCategory").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblCategory _lsttblCategory = db_OEE.tblCategories.SingleOrDefault(n => n.CategoryID == id);
            if (_lsttblCategory == null)
            {
                //pass date to view
                PassdataTotblCategory();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblCategory");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View(_lsttblCategory);
        }
        [HttpPost]
        public ActionResult TblCategoryEdit(tblCategory obj)
        {
            try
            {
                if (mFunction.ToString(obj.CategoryName) == "")
                {
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");

                    // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    return View(obj);
                }

                int _key = obj.CategoryID;
                tblCategory _lsttblCategory = db_OEE.tblCategories.SingleOrDefault(m => m.CategoryID == _key);
                if (_lsttblCategory == null)
                {
                    //pass date to view
                    PassdataTotblCategory();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblCategory");
                }

                //Save
                _lsttblCategory.CategoryName = obj.CategoryName;
                _lsttblCategory.CategoryS = obj.CategoryS;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblCategory", "OEEMSList", "Update", "Update tblCategory", "Id: " + _key.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblCategory");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region TblProject
        // GET: TblProject
        public ActionResult TblProject()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProject").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblProject();
            //
            return View();
        }

        // post tblProjectSave
        [HttpPost]
        public ActionResult TblProjectSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _ProjectID = mFunction.ToString(fc["txtProjectID"]);
                string _ProjectName = mFunction.ToString(fc["txtProjectName"]);
                string _CategoryID = mFunction.ToString(fc["txtCategoryID"]);
                string _CustomerID = mFunction.ToString(fc["txtCustomerID"]);
                //Check null data
                if (_ProjectID == "" || _ProjectName == "" || _CategoryID == "" || _CustomerID == "")
                {
                    //Pass data to view
                    PassdataTotblProject();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblProject");
                }

                //Check id is not exists
                var _objCheck = db_OEE.tblProjects.Where(m => m.ProjectID == _ProjectID).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblProject();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _ProjectID;
                    //
                    return View("TblProject");
                }

                //save data
                tblProject obj = new tblProject
                {
                    ProjectID = _ProjectID,
                    ProjectName = _ProjectName,
                    CategoryID = mFunction.ToInt(_CategoryID),
                    CustomerID = mFunction.ToInt(_CustomerID),
                    CreateUser = User.Identity.GetUserName(),
                    CreateDate = DateTime.Now
                };
                db_OEE.tblProjects.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProject", "OEEMSList", "Insert", "Add a New tblProject", "ID: " + _ProjectID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblProject");
            }
            catch
            {
                //Pass data to view
                PassdataTotblProject();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblProject");
            }
        }

        //Pass data to view
        private void PassdataTotblProject()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblProject = db_OEE.sp_tblProject_Get().ToList();
            ViewBag.lsttblProject = _lsttblProject;

            var _lsttblCustomers = db_OEE.tblCustomers.ToList();
            ViewBag.lsttblCustomers = _lsttblCustomers;

            var _lsttblCategory = db_OEE.tblCategories.ToList();
            ViewBag.lsttblCategory = _lsttblCategory;
        }

        // Delete tblProject
        public ActionResult TblProjectDel(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProject").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblProject obj = db_OEE.tblProjects.SingleOrDefault(m => m.ProjectID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblProject();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblProject");
                }
                db_OEE.tblProjects.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProject", "OEEMSList", "Delete", "Del tblProject", "ID: " + id, User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblProject");
            }
            catch
            {
                //pass date to view
                PassdataTotblProject();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblProject");
            }
        }

        //Edit tblProject
        [HttpGet]
        public ActionResult TblProjectEdit(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProject").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblProject _lsttblProject = db_OEE.tblProjects.SingleOrDefault(n => n.ProjectID == id);
            if (_lsttblProject == null)
            {
                //pass date to view
                PassdataTotblProject();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblProject");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            PassdataTotblProject();

            return View(_lsttblProject);
        }
        [HttpPost]
        public ActionResult TblProjectEdit(tblProject obj, FormCollection fc)
        {
            try
            {
                string _CategoryID = mFunction.ToString(fc["txtCategoryID"]);
                string _CustomerID = mFunction.ToString(fc["txtCustomerID"]);

                //Check null data
                if (obj.ProjectID == "" || obj.ProjectName == "" || _CategoryID == "" || _CustomerID == "")
                {
                    //Pass data to view
                    PassdataTotblProject();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblProject");
                }

                tblProject _lsttblProject = db_OEE.tblProjects.SingleOrDefault(m => m.ProjectID == obj.ProjectID);
                if (_lsttblProject == null)
                {
                    //pass date to view
                    PassdataTotblProject();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblProject");
                }

                //Save
                _lsttblProject.ProjectName = obj.ProjectName;
                _lsttblProject.CategoryID = mFunction.ToInt(_CategoryID);
                _lsttblProject.CustomerID = mFunction.ToInt(_CustomerID);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProject", "OEEMSList", "Update", "Update tblProject", "Id: " + obj.ProjectID, User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblProject");
            }
            catch
            {
                PassdataTotblProject();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region tblSize
        // GET: tblSize
        public ActionResult TblSize()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblSize").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblSize();
            //
            return View();
        }

        // post tblSizeSave
        [HttpPost]
        public ActionResult TblSizeSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _SizeID = mFunction.ToString(fc["txtSizeID"]);
                string _SizeName = mFunction.ToString(fc["txtSizeName"]);
                //Check null data
                if (_SizeID == "" || _SizeName == "")
                {
                    //Pass data to view
                    PassdataTotblSize();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblSize");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_SizeID);
                var _objCheck = db_OEE.tblSizes.Where(m => m.SizeID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblSize();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _SizeID;
                    //
                    return View("TblSize");
                }

                //save data
                tblSize obj = new tblSize
                {
                    SizeID = _key,
                    SizeName = _SizeName
                };
                db_OEE.tblSizes.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblSize", "OEEMSList", "Insert", "Add a New tblSize", "ID: " + _SizeID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblSize");
            }
            catch
            {
                //Pass data to view
                PassdataTotblSize();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblSize");
            }
        }

        //Pass data to view
        private void PassdataTotblSize()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblSize = db_OEE.tblSizes.ToList();
            ViewBag.lsttblSize = _lsttblSize;
            //
            var _objLst = db_OEE.tblSizes.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.SizeID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete tblSize
        public ActionResult TblSizeDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblSize").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblSize obj = db_OEE.tblSizes.SingleOrDefault(m => m.SizeID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblSize();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblSize");
                }
                db_OEE.tblSizes.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblSize", "OEEMSList", "Delete", "Del tblSize", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblSize");
            }
            catch
            {
                //pass date to view
                PassdataTotblSize();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblSize");
            }
        }

        //Edit tblSize
        [HttpGet]
        public ActionResult TblSizeEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblSize").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblSize _lsttblSize = db_OEE.tblSizes.SingleOrDefault(n => n.SizeID == id);
            if (_lsttblSize == null)
            {
                //pass date to view
                PassdataTotblSize();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblSize");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View(_lsttblSize);
        }
        [HttpPost]
        public ActionResult TblSizeEdit(tblSize obj)
        {
            try
            {
                //Check null data
                if (obj.SizeName == "" || obj.SizeName == null)
                {
                    //Pass data to view
                    PassdataTotblSize();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblSize _lsttblSize = db_OEE.tblSizes.SingleOrDefault(m => m.SizeID == obj.SizeID);
                if (_lsttblSize == null)
                {
                    //pass date to view
                    PassdataTotblSize();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblSize");
                }

                //Save
                _lsttblSize.SizeName = obj.SizeName;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblSize", "OEEMSList", "Update", "Update tblSize", "Id: " + obj.SizeID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblSize");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region TblComp
        // GET: TblComp
        public ActionResult TblComp()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblComp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblComp();
            //
            return View();
        }

        // post tblCompSave
        [HttpPost]
        public ActionResult TblCompSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _CompID = mFunction.ToString(fc["txtCompID"]);
                string _CompName = mFunction.ToString(fc["txtCompName"]);
                string _MainComp = mFunction.ToString(fc["txtMainComp"]);
                bool _Actice = Convert.ToBoolean(fc["txtActice"].Split(',')[0]);
                //Boolean _Actice = Convert.ToBoolean(fc["txtActice"]);
                string _Sort = mFunction.ToString(fc["txtSort"]);
                //Check null data
                if (_CompID == "" || _CompName == "")
                {
                    //Pass data to view
                    PassdataTotblComp();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblComp");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_CompID);
                var _objCheck = db_OEE.tblComps.Where(m => m.CompID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblComp();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _CompID;
                    //
                    return View("TblComp");
                }

                //save data
                tblComp obj = new tblComp
                {
                    CompID = _key,
                    CompName = _CompName,
                    MainComp = Convert.ToByte(mFunction.ToInt(_MainComp)),
                    Actice = _Actice,
                    Sort = mFunction.ToInt(_Sort)
                };
                db_OEE.tblComps.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblComp", "OEEMSList", "Insert", "Add a New tblComp", "ID: " + _CompID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblComp");
            }
            catch
            {
                //Pass data to view
                PassdataTotblComp();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblComp");
            }
        }

        //Pass data to view
        private void PassdataTotblComp()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblComp = db_OEE.tblComps.ToList();
            ViewBag.lsttblComp = _lsttblComp;
            //
            var _objLst = db_OEE.tblComps.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.CompID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete tblComp
        public ActionResult TblCompDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblComp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblComp obj = db_OEE.tblComps.SingleOrDefault(m => m.CompID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblComp();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblComp");
                }
                db_OEE.tblComps.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblComp", "OEEMSList", "Delete", "Del tblComp", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblComp");
            }
            catch
            {
                //pass date to view
                PassdataTotblComp();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblComp");
            }
        }

        //Edit tblComp
        [HttpGet]
        public ActionResult TblCompEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblComp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblComp _lsttblComp = db_OEE.tblComps.SingleOrDefault(n => n.CompID == id);
            if (_lsttblComp == null)
            {
                //pass date to view
                PassdataTotblComp();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblComp");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            ViewBag.chkb_Activate = mFunction.ToBool(_lsttblComp.Actice);
            return View(_lsttblComp);
        }
        [HttpPost]
        public ActionResult TblCompEdit(tblComp obj, FormCollection fc)
        {
            try
            {
                bool _Actice = Convert.ToBoolean(fc["txtActice"].Split(',')[0]);
                //Check null data
                if (obj.CompName == "" || obj.CompName == null)
                {
                    //Pass data to view
                    PassdataTotblComp();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblComp _lsttblComp = db_OEE.tblComps.SingleOrDefault(m => m.CompID == obj.CompID);
                if (_lsttblComp == null)
                {
                    //pass date to view
                    PassdataTotblComp();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblComp");
                }

                //Save
                _lsttblComp.CompName = obj.CompName;
                _lsttblComp.MainComp = obj.MainComp;
                _lsttblComp.Actice = _Actice;
                _lsttblComp.Sort = obj.Sort;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblComp", "OEEMSList", "Update", "Update tblComp", "Id: " + obj.CompID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblComp");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region TblCustomers
        // GET: TblCustomers
        public ActionResult TblCustomers()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblCustomers").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblCustomers();
            //
            return View();
        }

        // post tblCustomersSave
        [HttpPost]
        public ActionResult TblCustomersSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _CustID = mFunction.ToString(fc["txtCustID"]);
                string _CustName = mFunction.ToString(fc["txtCustName"]);
                //Check null data
                if (_CustID == "" || _CustName == "")
                {
                    //Pass data to view
                    PassdataTotblCustomers();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblCustomers");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_CustID);
                var _objCheck = db_OEE.tblCustomers.Where(m => m.CustID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblCustomers();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _CustID;
                    //
                    return View("TblCustomers");
                }

                //save data
                tblCustomer obj = new tblCustomer
                {
                    CustID = _key,
                    CustName = _CustName
                };
                db_OEE.tblCustomers.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblCustomers", "OEEMSList", "Insert", "Add a New tblCustomers", "ID: " + _CustID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblCustomers");
            }
            catch
            {
                //Pass data to view
                PassdataTotblCustomers();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblCustomers");
            }
        }

        //Pass data to view
        private void PassdataTotblCustomers()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblCustomers = db_OEE.tblCustomers.ToList();
            ViewBag.lsttblCustomers = _lsttblCustomers;
            //
            var _objLst = db_OEE.tblCustomers.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.CustID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete TblCustomers
        public ActionResult TblCustomersDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblCustomers").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblCustomer obj = db_OEE.tblCustomers.SingleOrDefault(m => m.CustID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblCustomers();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblCustomers");
                }
                db_OEE.tblCustomers.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblCustomers", "OEEMSList", "Delete", "Del tblCustomers", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblCustomers");
            }
            catch
            {
                //pass date to view
                PassdataTotblCustomers();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblCustomers");
            }
        }

        //Edit tblCustomers
        [HttpGet]
        public ActionResult TblCustomersEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblCustomers").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblCustomer _lsttblCustomers = db_OEE.tblCustomers.SingleOrDefault(n => n.CustID == id);
            if (_lsttblCustomers == null)
            {
                //pass date to view
                PassdataTotblCustomers();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblCustomers");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View(_lsttblCustomers);
        }
        [HttpPost]
        public ActionResult TblCustomersEdit(tblCustomer obj)
        {
            try
            {
                //Check null data
                if (obj.CustName == "" || obj.CustName == null)
                {
                    //Pass data to view
                    PassdataTotblCustomers();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblCustomer _lsttblCustomers = db_OEE.tblCustomers.SingleOrDefault(m => m.CustID == obj.CustID);
                if (_lsttblCustomers == null)
                {
                    //pass date to view
                    PassdataTotblCustomers();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblCustomers");
                }

                //Save
                _lsttblCustomers.CustName = obj.CustName;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblCustomers", "OEEMSList", "Update", "Update tblCustomers", "Id: " + obj.CustID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblCustomers");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region TblWorkpieceProcess
        // GET: TblWorkpieceProcess
        public ActionResult TblWorkpieceProcess()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblWorkpieceProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblWorkpieceProcess();
            //
            return View();
        }

        // post tblWorkpieceProcessSave
        [HttpPost]
        public ActionResult TblWorkpieceProcessSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _WorkpieceID = mFunction.ToString(fc["txtWorkpieceID"]);
                string _ProcessID = mFunction.ToString(fc["txtProcessID"]);
                //Check null data
                if (_WorkpieceID == "" || _ProcessID == "")
                {
                    //Pass data to view
                    PassdataTotblWorkpieceProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblWorkpieceProcess");
                }

                //Check id is not exists
                int _key1 = mFunction.ToInt(_WorkpieceID);
                int _key2 = mFunction.ToInt(_ProcessID);
                var _objCheck = db_OEE.tblWorkpieceProcesses.Where(m => (m.WorkpieceID == _key1) && (m.ProcessID == _key2)).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblWorkpieceProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + "WorkpieceID: " + _WorkpieceID + ", ProcessID: " + _ProcessID;
                    //
                    return View("TblWorkpieceProcess");
                }

                //save data
                tblWorkpieceProcess obj = new tblWorkpieceProcess
                {
                    WorkpieceID = _key1,
                    ProcessID = _key2
                };
                db_OEE.tblWorkpieceProcesses.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblWorkpieceProcess", "OEEMSList", "Insert", "Add a New tblWorkpieceProcess", "WorkpieceID: " + _WorkpieceID + ", ProcessID: " + _ProcessID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblWorkpieceProcess");
            }
            catch
            {
                //Pass data to view
                PassdataTotblWorkpieceProcess();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblWorkpieceProcess");
            }
        }

        //Pass data to view
        private void PassdataTotblWorkpieceProcess()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblWorkpieceProcess = db_OEE.sp_tblWorkpieceProcess_Get().ToList();
            ViewBag.lsttblWorkpieceProcess = _lsttblWorkpieceProcess;
            //
            ViewBag.lsttblWorkpiece = db_OEE.tblWorkpieces.ToList();
            ViewBag.lsttblProcess = db_OEE.tblProcesses.ToList();
        }

        // Delete tblWorkpieceProcess
        public ActionResult TblWorkpieceProcessDel(int _ProcessID, int _WorkpieceID)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblWorkpieceProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblWorkpieceProcess obj = db_OEE.tblWorkpieceProcesses.SingleOrDefault(m => (m.ProcessID == _ProcessID) && (m.WorkpieceID == _WorkpieceID));
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblWorkpieceProcess();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblWorkpieceProcess");
                }
                db_OEE.tblWorkpieceProcesses.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblWorkpieceProcess", "OEEMSList", "Delete", "Del tblWorkpieceProcess", "WorkpieceID: " + _WorkpieceID.ToString() + ", ProcessID: " + _ProcessID.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblWorkpieceProcess");
            }
            catch
            {
                //pass date to view
                PassdataTotblWorkpieceProcess();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblWorkpieceProcess");
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region TblWorkpiece
        // GET: TblWorkpiece
        public ActionResult TblWorkpiece()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblWorkpiece").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblWorkpiece();
            //
            return View();
        }


        // post tblWorkpieceSave
        [HttpPost]
        public ActionResult TblWorkpieceSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _WorkpieceID = mFunction.ToString(fc["txtWorkpieceID"]);
                string _WorkpieceName = mFunction.ToString(fc["txtWorkpieceName"]);
                //Check null data
                if (_WorkpieceID == "" || _WorkpieceName == "")
                {
                    //Pass data to view
                    PassdataTotblWorkpiece();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblWorkpiece");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_WorkpieceID);
                var _objCheck = db_OEE.tblWorkpieces.Where(m => m.WorkpieceID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblWorkpiece();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _WorkpieceID;
                    //
                    return View("TblWorkpiece");
                }

                //save data
                tblWorkpiece obj = new tblWorkpiece
                {
                    WorkpieceID = _key,
                    WorkpieceName = _WorkpieceName
                };
                db_OEE.tblWorkpieces.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblWorkpiece", "OEEMSList", "Insert", "Add a New tblWorkpiece", "ID: " + _WorkpieceID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblWorkpiece");
            }
            catch
            {
                //Pass data to view
                PassdataTotblWorkpiece();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblWorkpiece");
            }
        }

        //Pass data to view
        private void PassdataTotblWorkpiece()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblWorkpiece = db_OEE.tblWorkpieces.ToList();
            ViewBag.lsttblWorkpiece = _lsttblWorkpiece;
            //
            var _objLst = db_OEE.tblWorkpieces.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.WorkpieceID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete tblWorkpiece
        public ActionResult TblWorkpieceDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblWorkpiece").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblWorkpiece obj = db_OEE.tblWorkpieces.SingleOrDefault(m => m.WorkpieceID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblWorkpiece();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblWorkpiece");
                }
                db_OEE.tblWorkpieces.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblWorkpiece", "OEEMSList", "Delete", "Del tblWorkpiece", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblWorkpiece");
            }
            catch
            {
                //pass date to view
                PassdataTotblWorkpiece();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblWorkpiece");
            }
        }

        //Edit tblWorkpiece
        [HttpGet]
        public ActionResult TblWorkpieceEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblWorkpiece").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblWorkpiece _lsttblWorkpiece = db_OEE.tblWorkpieces.SingleOrDefault(n => n.WorkpieceID == id);
            if (_lsttblWorkpiece == null)
            {
                //pass date to view
                PassdataTotblWorkpiece();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblWorkpiece");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View(_lsttblWorkpiece);
        }
        [HttpPost]
        public ActionResult TblWorkpieceEdit(tblWorkpiece obj)
        {
            try
            {
                //Check null data
                if (obj.WorkpieceName == "" || obj.WorkpieceName == null)
                {
                    //Pass data to view
                    PassdataTotblWorkpiece();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblWorkpiece _lsttblWorkpiece = db_OEE.tblWorkpieces.SingleOrDefault(m => m.WorkpieceID == obj.WorkpieceID);
                if (_lsttblWorkpiece == null)
                {
                    //pass date to view
                    PassdataTotblWorkpiece();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblWorkpiece");
                }

                //Save
                _lsttblWorkpiece.WorkpieceName = obj.WorkpieceName;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblWorkpiece", "OEEMSList", "Update", "Update tblWorkpiece", "Id: " + obj.WorkpieceID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblWorkpiece");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////
        

        ////////////////////////////////////////////////////////////
        #region TblGroupDT
        // GET: TblGroupDT
        public ActionResult TblGroupDT()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblGroupDT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblGroupDT();
            //
            return View();
        }

        // post tblGroupDTSave
        [HttpPost]
        public ActionResult TblGroupDTSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _GDTID = mFunction.ToString(fc["txtGDTID"]);
                string _GroupName = mFunction.ToString(fc["txtGroupName"]);
                //Check null data
                if (_GDTID == "" || _GroupName == "")
                {
                    //Pass data to view
                    PassdataTotblGroupDT();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblGroupDT");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_GDTID);
                var _objCheck = db_OEE.tblGroupDTs.Where(m => m.GDTID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblGroupDT();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _GDTID;
                    //
                    return View("TblGroupDT");
                }

                //save data
                tblGroupDT obj = new tblGroupDT
                {
                    GDTID = _key,
                    GroupName = _GroupName
                };
                db_OEE.tblGroupDTs.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblGroupDT", "OEEMSList", "Insert", "Add a New tblGroupDT", "ID: " + _GDTID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblGroupDT");
            }
            catch
            {
                //Pass data to view
                PassdataTotblGroupDT();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblGroupDT");
            }
        }

        //Pass data to view
        private void PassdataTotblGroupDT()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblGroupDT = db_OEE.tblGroupDTs.ToList();
            ViewBag.lsttblGroupDT = _lsttblGroupDT;
            //
            var _objLst = db_OEE.tblGroupDTs.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.GDTID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete tblGroupDT
        public ActionResult TblGroupDTDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblGroupDT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblGroupDT obj = db_OEE.tblGroupDTs.SingleOrDefault(m => m.GDTID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblGroupDT();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblGroupDT");
                }
                db_OEE.tblGroupDTs.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblGroupDT", "OEEMSList", "Delete", "Del tblGroupDT", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblGroupDT");
            }
            catch
            {
                //pass date to view
                PassdataTotblGroupDT();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblGroupDT");
            }
        }

        //Edit tblGroupDT
        [HttpGet]
        public ActionResult TblGroupDTEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblGroupDT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblGroupDT _lsttblGroupDT = db_OEE.tblGroupDTs.SingleOrDefault(n => n.GDTID == id);
            if (_lsttblGroupDT == null)
            {
                //pass date to view
                PassdataTotblGroupDT();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblGroupDT");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            return View(_lsttblGroupDT);
        }
        [HttpPost]
        public ActionResult TblGroupDTEdit(tblGroupDT obj)
        {
            try
            {
                //Check null data
                if (obj.GroupName == "" || obj.GroupName == null)
                {
                    //Pass data to view
                    PassdataTotblGroupDT();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblGroupDT _lsttblGroupDT = db_OEE.tblGroupDTs.SingleOrDefault(m => m.GDTID == obj.GDTID);
                if (_lsttblGroupDT == null)
                {
                    //pass date to view
                    PassdataTotblGroupDT();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblGroupDT");
                }

                //Save
                _lsttblGroupDT.GroupName = obj.GroupName;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblGroupDT", "OEEMSList", "Update", "Update tblGroupDT", "Id: " + obj.GDTID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblGroupDT");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region TblDownTime
        // GET: TblDownTime
        public ActionResult TblDownTime()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblDownTime").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblDownTime();
            //
            return View();
        }

        // post tblDownTimeSave
        [HttpPost]
        public ActionResult TblDownTimeSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _DTID = mFunction.ToString(fc["txtDTID"]);
                string _DTName = mFunction.ToString(fc["txtDTName"]);
                string _Note = mFunction.ToString(fc["txtNote"]);
                string _GroupID = mFunction.ToString(fc["txtGroupID"]);
                //Check null data
                if (_DTID == "" || _DTName == "" || _GroupID == "")
                {
                    //Pass data to view
                    PassdataTotblDownTime();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblDownTime");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_DTID);
                var _objCheck = db_OEE.tblDownTimes.Where(m => m.DTID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblDownTime();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _DTID;
                    //
                    return View("TblDownTime");
                }

                //save data
                tblDownTime obj = new tblDownTime
                {
                    DTID = _key,
                    DTName = _DTName,
                    Note = _Note,
                    GroupID = mFunction.ToInt(_GroupID)
                };
                db_OEE.tblDownTimes.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblDownTime", "OEEMSList", "Insert", "Add a New tblDownTime", "ID: " + _DTID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblDownTime");
            }
            catch
            {
                //Pass data to view
                PassdataTotblDownTime();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblDownTime");
            }
        }

        //Pass data to view
        private void PassdataTotblDownTime()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblDownTime = db_OEE.tblDownTimes.ToList();
            ViewBag.lsttblDownTime = _lsttblDownTime;
            //
            ViewBag.lsttblGroupDT = db_OEE.tblProcesses.ToList();
            //
            var _objLst = db_OEE.tblDownTimes.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.DTID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete tblDownTime
        public ActionResult TblDownTimeDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblDownTime").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblDownTime obj = db_OEE.tblDownTimes.SingleOrDefault(m => m.DTID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblDownTime();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblDownTime");
                }
                db_OEE.tblDownTimes.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblDownTime", "OEEMSList", "Delete", "Del tblDownTime", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblDownTime");
            }
            catch
            {
                //pass date to view
                PassdataTotblDownTime();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblDownTime");
            }
        }

        //Edit tblDownTime
        [HttpGet]
        public ActionResult TblDownTimeEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblDownTime").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblDownTime _lsttblDownTime = db_OEE.tblDownTimes.SingleOrDefault(n => n.DTID == id);
            if (_lsttblDownTime == null)
            {
                //pass date to view
                PassdataTotblDownTime();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblDownTime");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //
            ViewBag.lsttblProcess = db_OEE.tblProcesses.ToList();

            return View(_lsttblDownTime);
        }
        [HttpPost]
        public ActionResult TblDownTimeEdit(tblDownTime obj, FormCollection fc)
        {
            try
            {
                string _GroupID = mFunction.ToString(fc["txtGroupID"]);
                obj.GroupID = mFunction.ToInt(_GroupID);
                //Check null data
                if (obj.DTName == "" || obj.DTName == null || _GroupID == "")
                {
                    //Pass data to view
                    PassdataTotblDownTime();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblDownTime _lsttblDownTime = db_OEE.tblDownTimes.SingleOrDefault(m => m.DTID == obj.DTID);
                if (_lsttblDownTime == null)
                {
                    //pass date to view
                    PassdataTotblDownTime();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblDownTime");
                }

                //Save
                _lsttblDownTime.DTName = obj.DTName;
                _lsttblDownTime.Note = obj.Note;
                _lsttblDownTime.GroupID = obj.GroupID;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblDownTime", "OEEMSList", "Update", "Update tblDownTime", "Id: " + obj.DTID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblDownTime");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                //
                ViewBag.lsttblProcess = db_OEE.tblProcesses.ToList();

                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////
        

        ////////////////////////////////////////////////////////////
        #region TblProcess
        // GET: TblProcess

        public ActionResult TblProcess()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblProcess();
            //
            return View();
        }

        // post tblProcessSave
        [HttpPost]
        public ActionResult TblProcessSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _ProcessID = mFunction.ToString(fc["txtProcessID"]);
                string _ProcessName = mFunction.ToString(fc["txtProcessName"]);
                bool _EndofProcess = Convert.ToBoolean(fc["txtEndofProcess"].Split(',')[0]);
                //Check null data
                if (_ProcessID == "" || _ProcessName == "")
                {
                    //Pass data to view
                    PassdataTotblProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblProcess");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_ProcessID);
                var _objCheck = db_OEE.tblProcesses.Where(m => m.ProcessID == _key).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ID: " + _ProcessID;
                    //
                    return View("TblProcess");
                }

                //save data
                tblProcess obj = new tblProcess
                {
                    ProcessID = _key,
                    ProcessName = _ProcessName,
                    EndofProcess = _EndofProcess
                };
                db_OEE.tblProcesses.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProcess", "OEEMSList", "Insert", "Add a New tblProcess", "ID: " + _ProcessID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblProcess");
            }
            catch
            {
                //Pass data to view
                PassdataTotblProcess();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblProcess");
            }
        }

        //Pass data to view
        private void PassdataTotblProcess()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblProcess = db_OEE.tblProcesses.ToList();
            ViewBag.lsttblProcess = _lsttblProcess;
            //
            var _objLst = db_OEE.tblProcesses.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = _objLst.Max(z => z.ProcessID);
            }
            else
            {
                _Key = 0;
            }
            ViewBag._Key = _Key + 1;
        }

        // Delete tblProcess
        public ActionResult TblProcessDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblProcess obj = db_OEE.tblProcesses.SingleOrDefault(m => m.ProcessID == id);
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblProcess();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblProcess");
                }
                db_OEE.tblProcesses.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProcess", "OEEMSList", "Delete", "Del tblProcess", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblProcess");
            }
            catch
            {
                //pass date to view
                PassdataTotblProcess();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblProcess");
            }
        }

        //Edit tblProcess
        [HttpGet]
        public ActionResult TblProcessEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            tblProcess _lsttblProcess = db_OEE.tblProcesses.SingleOrDefault(n => n.ProcessID == id);
            if (_lsttblProcess == null)
            {
                //pass date to view
                PassdataTotblProcess();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("TblProcess");
            }

            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            ViewBag.chkb_EndofProcess = mFunction.ToBool(_lsttblProcess.EndofProcess);
            return View(_lsttblProcess);
        }
        [HttpPost]
        public ActionResult TblProcessEdit(tblProcess obj, FormCollection fc)
        {
            try
            {
                bool _EndofProcess = Convert.ToBoolean(fc["txtEndofProcess"].Split(',')[0]);
                //Check null data
                if (obj.ProcessName == "" || obj.ProcessName == null)
                {
                    //Pass data to view
                    PassdataTotblProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View(obj);
                }

                tblProcess _lsttblProcess = db_OEE.tblProcesses.SingleOrDefault(m => m.ProcessID == obj.ProcessID);
                if (_lsttblProcess == null)
                {
                    //pass date to view
                    PassdataTotblProcess();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblProcess");
                }

                //Save
                _lsttblProcess.ProcessName = obj.ProcessName;
                _lsttblProcess.EndofProcess = _EndofProcess;
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProcess", "OEEMSList", "Update", "Update tblProcess", "Id: " + obj.ProcessID.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("TblProcess");
            }
            catch
            {
                CheckCookie();
                ChangeLanguage(cookie.Value);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////
        

        ////////////////////////////////////////////////////////////
        #region TblDepartmentProcess
        // GET: TblDepartmentProcess
        public ActionResult TblDepartmentProcess()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblDepartmentProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataTotblDepartmentProcess();
            //
            return View();
        }

        // post tblDepartmentProcessSave
        [HttpPost]
        public ActionResult TblDepartmentProcessSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _ProcessID = mFunction.ToString(fc["txtProcessID"]);
                string _DepartmentID = mFunction.ToString(fc["txtDepartmentID"]);
                //Check null data
                if (_ProcessID == "" || _DepartmentID == "")
                {
                    //Pass data to view
                    PassdataTotblDepartmentProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblDepartmentProcess");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_ProcessID);
                var _objCheck = db_OEE.tblDepartmentProcesses.Where(m => (m.DepartmentID == _DepartmentID) && (m.ProcessID == _key)).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataTotblDepartmentProcess();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ProcessID: " + _ProcessID + ", DepartmentID: " + _DepartmentID;
                    //
                    return View("TblDepartmentProcess");
                }

                //save data
                tblDepartmentProcess obj = new tblDepartmentProcess
                {
                    ProcessID = _key,
                    DepartmentID = _DepartmentID
                };
                db_OEE.tblDepartmentProcesses.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblDepartmentProcess", "OEEMSList", "Insert", "Add a New tblDepartmentProcess", "ProcessID: " + _ProcessID + ", DepartmentID: " + _DepartmentID, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblDepartmentProcess");
            }
            catch
            {
                //Pass data to view
                PassdataTotblDepartmentProcess();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblDepartmentProcess");
            }
        }

        //Pass data to view
        private void PassdataTotblDepartmentProcess()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lsttblDepartmentProcess = db_OEE.sp_tblDepartmentProcess_Get().ToList();
            ViewBag.lsttblDepartmentProcess = _lsttblDepartmentProcess;

            ViewBag.lstMaBoPhan = db_HR.MaBoPhans.ToList();
            ViewBag.lsttblProcess = db_OEE.tblProcesses.ToList();
        }

        // Delete tblDepartmentProcess
        public ActionResult TblDepartmentProcessDel(int _ProcessID, string _DepartmentID)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblDepartmentProcess").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblDepartmentProcess obj = db_OEE.tblDepartmentProcesses.SingleOrDefault(m => (m.ProcessID == _ProcessID) && (m.DepartmentID == _DepartmentID));
                if (obj == null)
                {
                    //pass date to view
                    PassdataTotblDepartmentProcess();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblDepartmentProcess");
                }
                db_OEE.tblDepartmentProcesses.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblDepartmentProcess", "OEEMSList", "Delete", "Del tblDepartmentProcess", "ProcessID: " + _ProcessID + ", DepartmentID: " + _DepartmentID, User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblDepartmentProcess");
            }
            catch
            {
                //pass date to view
                PassdataTotblDepartmentProcess();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblDepartmentProcess");
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region TblProjectComp
        // GET: TblProjectComp
        public ActionResult TblProjectComp()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProjectComp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataToTblProjectComp();
            //
            return View();
        }

        // post tblDepartmentProcessSave
        [HttpPost]
        public ActionResult TblProjectCompSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _ProjectID = mFunction.ToString(fc["txtProjectID"]);
                string _CompID = mFunction.ToString(fc["txtCompID"]);
                //Check null data
                if (_CompID == "" || _ProjectID == "")
                {
                    //Pass data to view
                    TblProjectComp();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblProjectComp");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_CompID);
                var _objCheck = db_OEE.tblProjectComps.Where(m => (m.ProjectID == _ProjectID) && (m.CompID == _key)).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataToTblProjectComp();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ProjectID: "  + _ProjectID + ", CompID: " + _key;
                    //
                    return View("TblProjectComp");
                }

                //save data
                tblProjectComp obj = new tblProjectComp
                {
                    CompID = _key,
                    ProjectID = _ProjectID
                };
                db_OEE.tblProjectComps.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("tblProjectComps", "OEEMSList", "Insert", "Add a New tblProjectComps", "ProjectID: " + _ProjectID + ", CompID: " + _key, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblProjectComp");
            }
            catch
            {
                //Pass data to view
                PassdataToTblProjectComp();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblProjectComp");
            }
        }

        //Pass data to view
        private void PassdataToTblProjectComp()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lstTblProjectComp = db_OEE.sp_tblProjectComp_Get().ToList();
            ViewBag.lstTblProjectComp = _lstTblProjectComp;

            ViewBag.lsttblComp = db_OEE.tblComps.ToList();
            ViewBag.lsttblProject = db_OEE.tblProjects.ToList();
        }

        // Delete tblDepartmentProcess
        public ActionResult TblProjectCompDel(int _CompID, string _ProjectID)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProjectComp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblProjectComp obj = db_OEE.tblProjectComps.SingleOrDefault(m => (m.ProjectID == _ProjectID) && (m.CompID == _CompID));
                if (obj == null)
                {
                    //pass date to view
                    PassdataToTblProjectComp();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblProjectComp");
                }
                db_OEE.tblProjectComps.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProjectComp", "OEEMSList", "Delete", "Del TblProjectComp", "ProjectID: " + _ProjectID + ", CompID: " + _CompID, User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblProjectComp");
            }
            catch
            {
                //pass date to view
                PassdataToTblProjectComp();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblProjectComp");
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        #region TblProjectSize
        // GET: TblProjectComp
        public ActionResult TblProjectSize()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProjectSize").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //Pass data to view
            PassdataToTblProjectSize();
            //
            return View();
        }

        // post tblDepartmentProcessSave
        [HttpPost]
        public ActionResult TblProjectSizeSave(FormCollection fc)
        {
            try
            {
                //Prepare data 
                string _ProjectID = mFunction.ToString(fc["txtProjectID"]);
                string _SizeID = mFunction.ToString(fc["txtSizeID"]);
                //Check null data
                if (_SizeID == "" || _ProjectID == "")
                {
                    //Pass data to view
                    TblProjectSize();
                    //pass error
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    //
                    return View("TblProjectSize");
                }

                //Check id is not exists
                int _key = mFunction.ToInt(_SizeID);
                var _objCheck = db_OEE.tblProjectSizes.Where(m => (m.ProjectID == _ProjectID) && (m.SizeID == _key)).ToList();
                if (_objCheck.Count > 0)
                {
                    //Pass data to view
                    PassdataToTblProjectSize();
                    //pass error
                    ViewBag.Message = "► " + Language.label("ExistsObject") + ". ProjectID: " + _ProjectID + ", SizeID: " + _key;
                    //
                    return View("TblProjectSize");
                }

                //save data
                tblProjectSize obj = new tblProjectSize
                {
                    SizeID = _key,
                    ProjectID = _ProjectID
                };
                db_OEE.tblProjectSizes.Add(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProjectSize", "OEEMSList", "Insert", "Add a New TblProjectSize", "ProjectID: " + _ProjectID + ", SizeID: " + _key, User.Identity.GetUserName(), DateTime.Now);

                //
                return RedirectToAction("TblProjectSize");
            }
            catch
            {
                //Pass data to view
                PassdataToTblProjectSize();
                //pass error
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //
                return View("TblProjectSize");
            }
        }

        //Pass data to view
        private void PassdataToTblProjectSize()
        {
            // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var _lstTblProjectSize = db_OEE.sp_tblProjectSize_Get().ToList();
            ViewBag.lstTblProjectSize = _lstTblProjectSize;

            ViewBag.lsttblSize = db_OEE.tblSizes.ToList();
            ViewBag.lsttblProject = db_OEE.tblProjects.ToList();
        }

        // Delete tblDepartmentProcess
        public ActionResult TblProjectSizeDel(int _SizeID, string _ProjectID)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TblProjectSize").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                tblProjectSize obj = db_OEE.tblProjectSizes.SingleOrDefault(m => (m.ProjectID == _ProjectID) && (m.SizeID == _SizeID));
                if (obj == null)
                {
                    //pass date to view
                    PassdataToTblProjectComp();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("TblProjectSize");
                }
                db_OEE.tblProjectSizes.Remove(obj);
                db_OEE.SaveChanges();
                //Save log
                var objLog = db_OEE.sp_ActionLog("TblProjectSize", "OEEMSList", "Delete", "Del TblProjectSize", "ProjectID: " + _ProjectID + ", SizeID: " + _SizeID, User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("TblProjectSize");
            }
            catch
            {
                //pass date to view
                PassdataToTblProjectComp();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("TblProjectSize");
            }
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