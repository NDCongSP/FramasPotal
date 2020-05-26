using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.UserManager;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using FramasVietNam.Models;
using FramasVietNam.Models.Meal;
using FramasVietNam.Models.HumanResource;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class AdministratorController : Controller
    {

        ////////////////////////////////////////////////////////////
        //Variabal
        #region Variabal
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        private UserManagerEntities db_user = new UserManagerEntities();
        private HumanResourceEntities db_HR = new HumanResourceEntities();

        //important Variabal cookie in multi language
        private HttpCookie cookie;

        private ApplicationDbContext UsersContext = new ApplicationDbContext();
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


        ////////////////////////////////////////////////////////////
        //add,delete,edit Department
        #region Department
        // GET: Department
        public ActionResult Dept()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Dept").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //pass date to view
            SendDataDept();
            return View();
        }

        //pass date to view
        private void SendDataDept()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstDept = db.Departments.Where(m => m.Status == 1).ToList();
            ViewBag.lstDept = lstDept;
        }

        // Delete Department
        public ActionResult DeptDel(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Dept").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Department obj = db.Departments.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //pass date to view
                    SendDataDept();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("Dept");
                }
                db.Departments.Remove(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Dept", "Administrator", "Delete", "Delete Dept", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("Dept");
            }
            catch
            {
                //pass date to view
                SendDataDept();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Dept");
            }
        }

        //Save department
        [HttpPost]
        public ActionResult DeptSave(FormCollection fc)
        {
            try
            {
                var a = mFunction.ToString(fc["Code"]).Trim();
                var b = mFunction.ToString(fc["Name"]);
                if (a == "" || b == "")
                {
                    //pass date to view
                    SendDataDept();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    ViewBag.Message = mess;
                    return View("Dept");
                }
                if (db.Departments.Where(m => m.Code == a).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataDept();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("Dept");
                }
                Department obj = new Department
                {
                    ID = a,
                    Code = a,
                    Name = b,
                    TableName = "Department",
                    Image = "fa fa-android",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.Departments.Add(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Dept", "Administrator", "Insert", "Insert A New Dept", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("Dept");
            }
            catch
            {
                //pass date to view
                SendDataDept();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Dept");
            }
        }

        //Edit department
        [HttpGet]
        public ActionResult DeptEdit(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Dept").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            Department lstDept = db.Departments.SingleOrDefault(n => n.ID == id);
            if (lstDept == null)
            {
                SendDataDept();
                string mess = "► " + Language.label("NotExistsObject");
                ViewBag.Message = mess;
                return View("Dept");
            }
            //pass data lstMenuItem to view
            return View(lstDept);
        }
        [HttpPost]
        public ActionResult DeptEdit(Department obj)
        {
            try
            {
                if (mFunction.ToString(obj.Code) == "" || mFunction.ToString(obj.Name) == "")
                {
                    CheckCookie();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    ChangeLanguage(mFunction.ToString(cookie.Value));

                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    ViewBag.Message = mess;
                    return View(obj);
                }
                Department objDept = db.Departments.SingleOrDefault(m => m.ID == obj.ID);
                objDept.Code = obj.Code;
                objDept.Name = obj.Name;
                objDept.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                objDept.DateAdd = DateTime.Now;
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Dept", "Administrator", "Update", "Update Dept", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Dept");
            }
            catch
            {
                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));

                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit Module
        #region Module

        // GET: Module
        public ActionResult Module()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Module").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataModule();
            return View();
        }

        //pass date to view
        private void SendDataModule()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var lstModule = db.Modules.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstModule = lstModule;
            int idLanguage = mFunction.ToInt(cookie.Value);
            var lstLanguageModule = db.sys_Translation.Where(m => (m.TableName == "Module") && (m.LanguageID == idLanguage)).ToList();
            ViewBag.lstLanguageModule = lstLanguageModule;
        }

        // Delete Module
        public ActionResult ModuleDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Module").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Module obj = db.Modules.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    SendDataModule();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("Module");
                }
                db.Modules.Remove(obj);
                List<sys_Translation> lst_Translate = db.sys_Translation.Where(n => (n.TableName == "Module") && (n.NameID == id)).ToList();
                foreach (var item in lst_Translate)
                {
                    db.sys_Translation.Remove(item);
                }
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Module", "Administrator", "Delete", "Delete Module", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Module");
            }
            catch
            {
                SendDataModule();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Module");
            }
        }

        //Save Group
        [HttpPost]
        public ActionResult ModuleSave(FormCollection fc)
        {
            try
            {
                CheckCookie();
                string code = mFunction.ToString(fc["Code"]);
                string titleEG = mFunction.ToString(fc["Title-EG"]);
                string descriptionEG = mFunction.ToString(fc["Description-EG"]);
                string titleVN = mFunction.ToString(fc["Title-VN"]);
                string descriptionVN = mFunction.ToString(fc["Description-VN"]);
                string titleGR = mFunction.ToString(fc["Title-GR"]);
                string descriptionGR = mFunction.ToString(fc["Description-GR"]);
                //check null
                if (code == "" || titleEG == "" || titleVN == "" || titleGR == "")
                {
                    //pass date to view
                    SendDataModule();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Title") + "," + Language.label("Description");
                    ViewBag.Message = mess;
                    return View("Module");
                }
                //check already exists in DB
                if (db.Modules.Where(m => m.Code == code).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataModule();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("Module");
                }

                //install Module
                Module obj = new Module
                {
                    Code = code,
                    TableName = "Module",
                    Image = "fa fa-edit",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.Modules.Add(obj);
                db.SaveChanges();
                //install sys_Translation
                foreach (var item in db.sys_Language.ToList())
                {
                    sys_Translation obj_Tr = new sys_Translation
                    {
                        LanguageID = item.ID,
                        LanguageCode = item.Code,
                        NameID = obj.ID,
                        TableName = "Module"
                    };
                    if (item.ID == 1)
                    {
                        obj_Tr.Title = titleEG;
                        obj_Tr.Description = descriptionEG;
                    }
                    else if (item.ID == 2)
                    {
                        obj_Tr.Title = titleVN;
                        obj_Tr.Description = descriptionVN;
                    }
                    else if (item.ID == 3)
                    {
                        obj_Tr.Title = titleGR;
                        obj_Tr.Description = descriptionGR;
                    }
                    else
                    {
                        obj_Tr.Title = titleEG;
                        obj_Tr.Description = descriptionEG;
                    }
                    obj_Tr.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                    obj_Tr.DateAdd = DateTime.Now;
                    db.sys_Translation.Add(obj_Tr);
                }
                //Save data
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Module", "Administrator", "Insert", "Insert A New Module", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Module");
            }
            catch
            {
                //pass date to view
                SendDataModule();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Module");
            }
        }

        //Edit Group
        [HttpGet]
        public ActionResult ModuleEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Module").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            Module lstModule = db.Modules.SingleOrDefault(n => n.ID == id);
            if (lstModule == null)
            {
                SendDataModule();
                string mess = "► " + Language.label("NotExistsObject");
                ViewBag.Message = mess;
                return View("Module");
            }
            //pass data lstMenuItem to view
            List<sys_Translation> lstLanguageModule = db.sys_Translation.Where(m => (m.TableName == "Module" && m.NameID == id)).ToList();
            ViewBag.lstLanguageModule = lstLanguageModule;
            return View(lstModule);
        }
        [HttpPost]
        public ActionResult ModuleEdit(Module obj, FormCollection fc)
        {
            try
            {
                //prepare data
                string code = mFunction.ToString(fc["Code"]);
                string titleEG = mFunction.ToString(fc["Title-EG"]);
                string descriptionEG = mFunction.ToString(fc["Description-EG"]);
                string titleVN = mFunction.ToString(fc["Title-VN"]);
                string descriptionVN = mFunction.ToString(fc["Description-VN"]);
                string titleGR = mFunction.ToString(fc["Title-GR"]);
                string descriptionGR = mFunction.ToString(fc["Description-GR"]);
                //check null
                if (code == "" || titleEG == "" || titleVN == "" || titleGR == "")
                {
                    CheckCookie();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    ChangeLanguage(mFunction.ToString(cookie.Value));

                    var lstModule = db.Modules.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
                    ViewBag.lstModule = lstModule;
                    int idLanguage = mFunction.ToInt(cookie.Value);
                    var lstLanguageModule = db.sys_Translation.Where(m => (m.TableName == "Module") && (m.NameID == obj.ID)).ToList();
                    ViewBag.lstLanguageModule = lstLanguageModule;
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Title") + "," + Language.label("Description");
                    ViewBag.Message = mess;
                    return View(obj);
                }

                //update module
                Module obj_Upd = db.Modules.SingleOrDefault(m => m.ID == obj.ID);
                if (obj_Upd == null)
                {
                    SendDataModule();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("Module");
                }
                obj_Upd.Code = code;
                obj_Upd.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                obj_Upd.DateAdd = DateTime.Now;
                //update translation
                List<sys_Translation> lst_Trn_Up = db.sys_Translation.Where(m => (m.TableName == "Module" && m.NameID == obj.ID)).ToList();
                foreach (var item in lst_Trn_Up)
                {
                    if (item.LanguageID == 1)
                    {
                        item.Title = titleEG;
                        item.Description = descriptionEG;
                    }
                    else if (item.LanguageID == 2)
                    {
                        item.Title = titleVN;
                        item.Description = descriptionVN;
                    }
                    else if (item.LanguageID == 3)
                    {
                        item.Title = titleGR;
                        item.Description = descriptionGR;
                    }
                    else
                    {
                        item.Title = titleEG;
                        item.Description = descriptionEG;
                    }
                    item.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                    item.DateAdd = DateTime.Now;
                }
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Module", "Administrator", "Update", "Update Module", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Module");
            }
            catch
            {
                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));

                var lstModule = db.Modules.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
                ViewBag.lstModule = lstModule;
                int idLanguage = mFunction.ToInt(cookie.Value);
                var lstLanguageModule = db.sys_Translation.Where(m => (m.TableName == "Module") && (m.NameID == obj.ID)).ToList();
                ViewBag.lstLanguageModule = lstLanguageModule;
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View(obj);
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit Group
        #region Group

        // GET: Group
        public ActionResult Group()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Group").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataGroup();
            return View();
        }

        //pass date to view
        private void SendDataGroup()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstGroup = db.Groups.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstGroup = lstGroup;
            int idLanguage = mFunction.ToInt(cookie.Value);
            var lstLanguageGroup = db.sys_Translation.Where(m => (m.TableName == "Group") && (m.LanguageID == idLanguage)).ToList();
            ViewBag.lstLanguageGroup = lstLanguageGroup;
        }

        // Delete Group
        public ActionResult GroupDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Group").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Group obj = db.Groups.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //pass date to view
                    SendDataGroup();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("Group");
                }
                db.Groups.Remove(obj);
                List<sys_Translation> lst_Translate = db.sys_Translation.Where(n => (n.TableName == "Group") && (n.NameID == id)).ToList();
                foreach (var item in lst_Translate)
                {
                    db.sys_Translation.Remove(item);
                }
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Group", "Administrator", "Delete", "Delete Group", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Group");
            }
            catch
            {
                //pass date to view
                SendDataGroup();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Group");
            }
        }

        //Save Group
        [HttpPost]
        public ActionResult GroupSave(FormCollection fc)
        {
            try
            {
                CheckCookie();
                string code = mFunction.ToString(fc["Code"]);
                string titleEG = mFunction.ToString(fc["Title-EG"]);
                string descriptionEG = mFunction.ToString(fc["Description-EG"]);
                string titleVN = mFunction.ToString(fc["Title-VN"]);
                string descriptionVN = mFunction.ToString(fc["Description-VN"]);
                string titleGR = mFunction.ToString(fc["Title-GR"]);
                string descriptionGR = mFunction.ToString(fc["Description-GR"]);
                //check null
                if (code == "" || titleEG == "" || titleVN == "" || titleGR == "")
                {
                    //pass date to view
                    SendDataGroup();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Title") + "," + Language.label("Description");
                    ViewBag.Message = mess;
                    return View("Group");
                }
                //check already exists in DB
                if (db.Groups.Where(m => m.Code == code).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataGroup();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("Group");
                }

                //install Group
                Group obj = new Group
                {
                    Code = code,
                    TableName = "Group",
                    Image = "fa fa-caret-right",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.Groups.Add(obj);
                db.SaveChanges();
                //install sys_Translation
                foreach (var item in db.sys_Language.ToList())
                {
                    sys_Translation obj_Tr = new sys_Translation
                    {
                        LanguageID = item.ID,
                        LanguageCode = item.Code,
                        NameID = obj.ID,
                        TableName = "Group"
                    };
                    if (item.ID == 1)
                    {
                        obj_Tr.Title = titleEG;
                        obj_Tr.Description = descriptionEG;
                    }
                    else if (item.ID == 2)
                    {
                        obj_Tr.Title = titleVN;
                        obj_Tr.Description = descriptionVN;
                    }
                    else if (item.ID == 3)
                    {
                        obj_Tr.Title = titleGR;
                        obj_Tr.Description = descriptionGR;
                    }
                    else
                    {
                        obj_Tr.Title = titleEG;
                        obj_Tr.Description = descriptionEG;
                    }
                    obj_Tr.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                    obj_Tr.DateAdd = DateTime.Now;
                    db.sys_Translation.Add(obj_Tr);
                }
                //Save data
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Group", "Administrator", "Insert", "Insert A New Group", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Group");
            }
            catch
            {
                //pass date to view
                SendDataGroup();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Group");
            }
        }

        //Edit Group
        [HttpGet]
        public ActionResult GroupEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Group").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            Group lstGroup = db.Groups.SingleOrDefault(n => n.ID == id);
            if (lstGroup == null)
            {
                //pass date to view
                SendDataGroup();
                string mess = "► " + Language.label("NotExistsObject");
                ViewBag.Message = mess;
                return View("Group");
            }
            //pass data lstMenuItem to view
            List<sys_Translation> lstLanguageGroup = db.sys_Translation.Where(m => (m.TableName == "Group" && m.NameID == id)).ToList();
            ViewBag.lstLanguageGroup = lstLanguageGroup;
            return View(lstGroup);
        }
        [HttpPost]
        public ActionResult GroupEdit(Group obj, FormCollection fc)
        {
            try
            {
                //prepare data
                string code = mFunction.ToString(fc["Code"]);
                string titleEG = mFunction.ToString(fc["Title-EG"]);
                string descriptionEG = mFunction.ToString(fc["Description-EG"]);
                string titleVN = mFunction.ToString(fc["Title-VN"]);
                string descriptionVN = mFunction.ToString(fc["Description-VN"]);
                string titleGR = mFunction.ToString(fc["Title-GR"]);
                string descriptionGR = mFunction.ToString(fc["Description-GR"]);
                //check null
                if (code == "" || titleEG == "" || titleVN == "" || titleGR == "")
                {
                    CheckCookie();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    ChangeLanguage(mFunction.ToString(cookie.Value));

                    var lstGroup = db.Groups.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
                    ViewBag.lstGroup = lstGroup;
                    int idLanguage = mFunction.ToInt(cookie.Value);
                    var lstLanguageGroup = db.sys_Translation.Where(m => (m.TableName == "Group") && (m.NameID == obj.ID)).ToList();
                    ViewBag.lstLanguageGroup = lstLanguageGroup;
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Title") + "," + Language.label("Description");
                    ViewBag.Message = mess;
                    return View(obj);
                }

                //update Group
                Group obj_Upd = db.Groups.SingleOrDefault(m => m.ID == obj.ID);
                if (obj_Upd == null)
                {
                    //pass date to view
                    SendDataGroup();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("Group");
                }
                obj_Upd.Code = code;
                obj_Upd.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                obj_Upd.DateAdd = DateTime.Now;
                //update translation
                List<sys_Translation> lst_Trn_Up = db.sys_Translation.Where(m => (m.TableName == "Group" && m.NameID == obj.ID)).ToList();
                foreach (var item in lst_Trn_Up)
                {
                    if (item.LanguageID == 1)
                    {
                        item.Title = titleEG;
                        item.Description = descriptionEG;
                    }
                    else if (item.LanguageID == 2)
                    {
                        item.Title = titleVN;
                        item.Description = descriptionVN;
                    }
                    else if (item.LanguageID == 3)
                    {
                        item.Title = titleGR;
                        item.Description = descriptionGR;
                    }
                    else
                    {
                        item.Title = titleEG;
                        item.Description = descriptionEG;
                    }
                    item.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                    item.DateAdd = DateTime.Now;
                }
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Group", "Administrator", "Update", "Update Group", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Group");
            }
            catch
            {
                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));

                var lstGroup = db.Groups.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
                ViewBag.lstGroup = lstGroup;
                int idLanguage = mFunction.ToInt(cookie.Value);
                var lstLanguageGroup = db.sys_Translation.Where(m => (m.TableName == "Group") && (m.NameID == obj.ID)).ToList();
                ViewBag.lstLanguageGroup = lstLanguageGroup;
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Group");
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit Function
        #region Function

        // GET: Function
        public ActionResult Function()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Function").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataFunction();
            return View();
        }

        //pass date to view
        private void SendDataFunction()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstFunction = db.Functions.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstFunction = lstFunction;
            int idLanguage = mFunction.ToInt(cookie.Value);
            var lstLanguageFunction = db.sys_Translation.Where(m => (m.TableName == "Function") && (m.LanguageID == idLanguage)).ToList();
            ViewBag.lstLanguageFunction = lstLanguageFunction;
        }

        // Delete Function
        public ActionResult FunctionDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Function").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                Function obj = db.Functions.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    SendDataFunction();
                    return View("Function");
                }
                db.Functions.Remove(obj);
                List<sys_Translation> lst_Translate = db.sys_Translation.Where(n => (n.TableName == "Function") && (n.NameID == id)).ToList();
                foreach (var item in lst_Translate)
                {
                    db.sys_Translation.Remove(item);
                }
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Function", "Administrator", "Delete", "Delete Function", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Function");
            }
            catch
            {
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                SendDataFunction();
                return View("Function");
            }
        }

        //Save Group
        [HttpPost]
        public ActionResult FunctionSave(FormCollection fc)
        {
            try
            {
                CheckCookie();
                string code = mFunction.ToString(fc["Code"]);

                string action = mFunction.ToString(fc["Action"]);
                string controller = mFunction.ToString(fc["Controller"]);
                string image = mFunction.ToString(fc["Image"]);
                string color = mFunction.ToString(fc["Color"]);

                string titleEG = mFunction.ToString(fc["Title-EG"]);
                string descriptionEG = mFunction.ToString(fc["Description-EG"]);
                string titleVN = mFunction.ToString(fc["Title-VN"]);
                string descriptionVN = mFunction.ToString(fc["Description-VN"]);
                string titleGR = mFunction.ToString(fc["Title-GR"]);
                string descriptionGR = mFunction.ToString(fc["Description-GR"]);

                //check null
                if (code == "" || titleEG == "" || titleVN == "" || titleGR == "")
                {
                    //pass date to view
                    SendDataFunction();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Title") + "," + Language.label("Description");
                    ViewBag.Message = mess;
                    return View("Function");
                }
                //check already exists in DB
                if (db.Functions.Where(m => m.Code == code).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataFunction();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("Function");
                }

                //install Function
                Function obj = new Function
                {
                    Code = code,
                    TableName = "Function",
                    URL = "/",
                    Action = action,
                    Controller = controller,
                    Image = image,
                    Color = color,
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.Functions.Add(obj);
                db.SaveChanges();
                //install sys_Translation
                foreach (var item in db.sys_Language.ToList())
                {
                    sys_Translation obj_Tr = new sys_Translation
                    {
                        LanguageID = item.ID,
                        LanguageCode = item.Code,
                        NameID = obj.ID,
                        TableName = "Function"
                    };
                    if (item.ID == 1)
                    {
                        obj_Tr.Title = titleEG;
                        obj_Tr.Description = descriptionEG;
                    }
                    else if (item.ID == 2)
                    {
                        obj_Tr.Title = titleVN;
                        obj_Tr.Description = descriptionVN;
                    }
                    else if (item.ID == 3)
                    {
                        obj_Tr.Title = titleGR;
                        obj_Tr.Description = descriptionGR;
                    }
                    else
                    {
                        obj_Tr.Title = titleEG;
                        obj_Tr.Description = descriptionEG;
                    }
                    obj_Tr.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                    obj_Tr.DateAdd = DateTime.Now;
                    db.sys_Translation.Add(obj_Tr);
                }
                //Save data
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Function", "Administrator", "Insert", "Insert A New Function", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Function");
            }
            catch
            {
                //pass date to view
                SendDataFunction();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Function");
            }
        }

        //Edit Group
        [HttpGet]
        public ActionResult FunctionEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Function").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            Function lstFunction = db.Functions.SingleOrDefault(n => n.ID == id);
            if (lstFunction == null)
            {
                //pass date to view
                SendDataFunction();
                string mess = "► " + Language.label("NotExistsObject");
                ViewBag.Message = mess;
                return View("Function");
            }
            //pass data lstMenuItem to view
            List<sys_Translation> lstLanguageFunction = db.sys_Translation.Where(m => (m.TableName == "Function" && m.NameID == id)).ToList();
            ViewBag.lstLanguageFunction = lstLanguageFunction;
            return View(lstFunction);
        }
        [HttpPost]
        public ActionResult FunctionEdit(Function obj, FormCollection fc)
        {
            try
            {
                //prepare data
                string code = mFunction.ToString(fc["Code"]);

                string action = mFunction.ToString(fc["Action"]);
                string controller = mFunction.ToString(fc["Controller"]);
                string image = mFunction.ToString(fc["Image"]);
                string color = mFunction.ToString(fc["Color"]);

                string titleEG = mFunction.ToString(fc["Title-EG"]);
                string descriptionEG = mFunction.ToString(fc["Description-EG"]);
                string titleVN = mFunction.ToString(fc["Title-VN"]);
                string descriptionVN = mFunction.ToString(fc["Description-VN"]);
                string titleGR = mFunction.ToString(fc["Title-GR"]);
                string descriptionGR = mFunction.ToString(fc["Description-GR"]);
                //check null
                if (code == "" || titleEG == "" || titleVN == "" || titleGR == "")
                {
                    CheckCookie();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    ChangeLanguage(mFunction.ToString(cookie.Value));

                    var lstFunction = db.Functions.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
                    ViewBag.lstFunction = lstFunction;
                    var lstLanguageFunction = db.sys_Translation.Where(m => (m.TableName == "Function") && (m.NameID == obj.ID)).ToList();
                    ViewBag.lstLanguageFunction = lstLanguageFunction;
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Title") + "," + Language.label("Description");
                    ViewBag.Message = mess;
                    return View(obj);
                }

                //update Function
                Function obj_Upd = db.Functions.SingleOrDefault(m => m.ID == obj.ID);
                if (obj_Upd == null)
                {
                    //pass date to view
                    SendDataFunction();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("Function");
                }
                obj_Upd.Code = code;
                obj_Upd.Action = action;
                obj_Upd.Controller = controller;
                obj_Upd.Image = image;
                obj_Upd.Color = color;
                obj_Upd.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                obj_Upd.DateAdd = DateTime.Now;
                //update translation
                List<sys_Translation> lst_Trn_Up = db.sys_Translation.Where(m => (m.TableName == "Function" && m.NameID == obj.ID)).ToList();
                foreach (var item in lst_Trn_Up)
                {
                    if (item.LanguageID == 1)
                    {
                        item.Title = titleEG;
                        item.Description = descriptionEG;
                    }
                    else if (item.LanguageID == 2)
                    {
                        item.Title = titleVN;
                        item.Description = descriptionVN;
                    }
                    else if (item.LanguageID == 3)
                    {
                        item.Title = titleGR;
                        item.Description = descriptionGR;
                    }
                    else
                    {
                        item.Title = titleEG;
                        item.Description = descriptionEG;
                    }
                    item.PCIDAdd = mFunction.ToString(User.Identity.GetUserName());
                    item.DateAdd = DateTime.Now;
                }
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Function", "Administrator", "Update", "Update Function", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Function");
            }
            catch
            {
                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));

                var lstFunction = db.Functions.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
                ViewBag.lstFunction = lstFunction;
                int idLanguage = mFunction.ToInt(cookie.Value);
                var lstLanguageFunction = db.sys_Translation.Where(m => (m.TableName == "Function") && (m.NameID == obj.ID)).ToList();
                ViewBag.lstLanguageFunction = lstLanguageFunction;
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("Function");
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit GroupToFunction
        #region GroupToFunction

        // GET: GroupToFunction
        public ActionResult GrpToFn()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "GrpToFn").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataGrpToFn();
            return View();
        }

        //pass date to view
        private void SendDataGrpToFn()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstGrpToFn = db.GroupToFunctions.Where(m => m.Status == 1).ToList();
            ViewBag.lstGrpToFn = lstGrpToFn;
            var lstGrp = db.Groups.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstGrp = lstGrp;
            var lstFn = db.Functions.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstFn = lstFn;
        }

        // Delete GroupToFunction
        public ActionResult GrpToFnDel(int grpid, int fnid)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "GrpToFn").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                GroupToFunction obj = db.GroupToFunctions.SingleOrDefault(m => (m.GroupID == grpid) && (m.FunctionalID == fnid));
                if (obj == null)
                {
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    //pass date to view
                    SendDataGrpToFn();
                    return View("GrpToFn");
                }
                db.GroupToFunctions.Remove(obj);
                db.SaveChanges();

                //pass date to view
                SendDataGrpToFn();
                //Save log
                var objLog = db.sp_ActionLog("GrpToFn", "Administrator", "Delete", "Delete GrpToFn", "grpid: " + grpid.ToString() + " ; fnid: " + fnid.ToString(), User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("GrpToFn");
            }
            catch
            {
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //pass date to view
                SendDataGrpToFn();
                return View("GrpToFn");
            }
        }

        //Save GroupToFunction
        [HttpPost]
        public ActionResult GrpToFnSave(FormCollection fc)
        {
            try
            {
                var codeGrp = mFunction.ToString(fc["Group"]);
                var codeFn = mFunction.ToString(fc["Function1"]);
                //Check code group and function already exists in table Group and Function
                Group grp = db.Groups.SingleOrDefault(m => m.Code == codeGrp);
                Function fn = db.Functions.SingleOrDefault(m => m.Code == codeFn);
                if (grp == null || fn == null)
                {
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + Language.label("Group") + "," + Language.label("Code") + Language.label("Function");
                    ViewBag.Message = mess;
                    //pass date to view
                    SendDataGrpToFn();
                    return View("GrpToFn");
                }
                int idgrp = mFunction.ToInt(db.Groups.SingleOrDefault(m => m.Code == codeGrp).ID);
                int idfn = mFunction.ToInt(db.Functions.SingleOrDefault(m => m.Code == codeFn).ID);
                //Check code group and function not exists in table GroupToFunctions
                if (db.GroupToFunctions.Where(m => (m.GroupID == idgrp) && (m.FunctionalID == idfn)).ToList().Count > 0)
                {
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    //pass date to view
                    SendDataGrpToFn();
                    return View("GrpToFn");
                }
                //Save data
                GroupToFunction obj = new GroupToFunction
                {
                    GroupID = idgrp,
                    FunctionalID = idfn,
                    TableName = "GroupToFunction",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.GroupToFunctions.Add(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("GrpToFn", "Administrator", "Insert", "Insert GrpToFn", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("GrpToFn");
            }
            catch
            {
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //pass date to view
                SendDataGrpToFn();
                return View("GrpToFn");
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit ModuleToGroup
        #region ModuleToGroup

        // GET: ModuleToGroup
        public ActionResult MlToGrp()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MlToGrp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataMlToGrp();
            return View();
        }

        //pass date to view
        private void SendDataMlToGrp()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstMlToGrp = db.ModuleToGroups.Where(m => m.Status == 1).ToList();
            ViewBag.lstMlToGrp = lstMlToGrp;
            var lstMl = db.Modules.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstMl = lstMl;
            var lstGrp = db.Groups.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstGrp = lstGrp;
        }

        // Delete ModuleToGroup
        public ActionResult MlToGrpDel(int mlid, int grpid)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MlToGrp").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                ModuleToGroup obj = db.ModuleToGroups.SingleOrDefault(m => (m.ModuleID == mlid) && (m.GroupID == grpid));
                if (obj == null)
                {
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    //pass date to view
                    SendDataMlToGrp();
                    return View("MlToGrp");
                }
                db.ModuleToGroups.Remove(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("MlToGrp", "Administrator", "Delete", "Delete MlToGrp", "mlid: " + mlid.ToString() + " ; grpid: " + grpid, User.Identity.GetUserName(), DateTime.Now);
                //
                //pass date to view
                return RedirectToAction("MlToGrp");
            }
            catch
            {
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                //pass date to view
                SendDataMlToGrp();
                return View("MlToGrp");
            }
        }

        //Save ModuleToGroup
        [HttpPost]
        public ActionResult MlToGrpSave(FormCollection fc)
        {
            try
            {
                var codeMl = mFunction.ToString(fc["Module"]);
                var codeGrp = mFunction.ToString(fc["Group1"]);
                //Check code group and function already exists in table Group and Function
                Module ml = db.Modules.SingleOrDefault(m => m.Code == codeMl);
                Group grp = db.Groups.SingleOrDefault(m => m.Code == codeGrp);
                if (ml == null || grp == null)
                {
                    //pass date to view
                    SendDataMlToGrp();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + Language.label("Module") + "," + Language.label("Code") + Language.label("Group");
                    ViewBag.Message = mess;
                    return View("MlToGrp");
                }
                int idMl = mFunction.ToInt(db.Modules.SingleOrDefault(m => m.Code == codeMl).ID);
                int idgrp = mFunction.ToInt(db.Groups.SingleOrDefault(m => m.Code == codeGrp).ID);
                //Check code group and function not exists in table ModuleToGroups
                if (db.ModuleToGroups.Where(m => (m.ModuleID == idMl) && (m.GroupID == idgrp)).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataMlToGrp();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("MlToGrp");
                }
                //Save data
                ModuleToGroup obj = new ModuleToGroup
                {
                    ModuleID = idMl,
                    GroupID = idgrp,
                    TableName = "ModuleToGroup",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.ModuleToGroups.Add(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("MlToGrp", "Administrator", "Insert", "Insert MlToGrp", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("MlToGrp");
            }
            catch
            {
                //pass date to view
                SendDataMlToGrp();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View();
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit DeptToModule
        #region DeptToModule

        // GET: DeptToModule
        public ActionResult DeptToMl()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "DeptToMl").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataDeptToMl();
            return View();
        }

        //pass date to view
        private void SendDataDeptToMl()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstDeptToMl = db.DeptToModules.Where(m => m.Status == 1).ToList();
            ViewBag.lstDeptToMl = lstDeptToMl;
            var lstDept = db.Departments.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstDept = lstDept;
            var lstMl = db.Modules.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstMl = lstMl;
        }

        // Delete DeptToModule
        public ActionResult DeptToMlDel(string deptid, int mlid)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "DeptToMl").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                DeptToModule obj = db.DeptToModules.SingleOrDefault(m => (m.DeptID == deptid) && (m.ModuleID == mlid));
                if (obj == null)
                {
                    //pass date to view
                    SendDataDeptToMl();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("DeptToMl");
                }
                db.DeptToModules.Remove(obj);
                db.SaveChanges();

                //Save log
                var objLog = db.sp_ActionLog("DeptToMl", "Administrator", "Delete", "Delete DeptToMl", "deptid: " + deptid.ToString() + "; mlid: " + mlid.ToString(), User.Identity.GetUserName(), DateTime.Now);
                //

                //pass date to view
                SendDataDeptToMl();
                return RedirectToAction("DeptToMl");
            }
            catch
            {
                //pass date to view
                SendDataDeptToMl();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("DeptToMl");
            }
        }

        //Save DeptToModule
        [HttpPost]
        public ActionResult DeptToMlSave(FormCollection fc)
        {
            try
            {
                var codeDept = mFunction.ToString(fc["Dept"]);
                var codeMl = mFunction.ToString(fc["Module1"]);
                //Check code group and function already exists in table Group and Function
                Department dept = db.Departments.SingleOrDefault(m => m.Code == codeDept);
                Module ml = db.Modules.SingleOrDefault(m => m.Code == codeMl);
                if (dept == null || ml == null)
                {
                    //pass date to view
                    SendDataDeptToMl();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + Language.label("Department") + "," + Language.label("Code") + Language.label("Module");
                    ViewBag.Message = mess;
                    return View("DeptToMl");
                }
                string idDept = mFunction.ToString(dept.ID);
                int idMl = mFunction.ToInt(ml.ID);
                //Check code group and function not exists in table DeptToModules
                if (db.DeptToModules.Where(m => (m.DeptID == idDept) && (m.ModuleID == idMl)).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataDeptToMl();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("DeptToMl");
                }
                //Save data
                DeptToModule obj = new DeptToModule
                {
                    DeptID = idDept,
                    ModuleID = idMl,
                    TableName = "DeptToModule",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.DeptToModules.Add(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("DeptToMl", "Administrator", "Insert", "Insert DeptToMl", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("DeptToMl");
            }
            catch
            {
                //pass date to view
                SendDataDeptToMl();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View();
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //add,delete,edit UserToDept
        #region UserToDept

        // GET: UserToDept
        public ActionResult UserToDept()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UserToDept").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            SendDataUserToDept();
            return View();
        }

        //pass date to view
        private void SendDataUserToDept()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            var lstUserToDept = db.UserToDepts.Where(m => m.Status == 1).ToList();
            ViewBag.lstUserToDept = lstUserToDept;

            var lstUser = UsersContext.Users.OrderBy(m => m.UserName).ToList(); // Exception
            ViewBag.lstUser = lstUser;
            var lstDept = db.Departments.Where(m => m.Status == 1).OrderBy(m => m.Code).ToList();
            ViewBag.lstDept = lstDept;
        }

        // Delete UserToDept
        public ActionResult UserToDeptDel(string UserId, string deptid)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UserToDept").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                UserToDept obj = db.UserToDepts.SingleOrDefault(m => (m.DeptID == deptid) && (m.UserName == UserId));
                if (obj == null)
                {
                    //pass date to view
                    SendDataUserToDept();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("UserToDept");
                }
                db.UserToDepts.Remove(obj);
                db.SaveChanges();

                //pass date to view
                SendDataUserToDept();
                //Save log
                var objLog = db.sp_ActionLog("UserToDept", "Administrator", "Delete", "Delete UserToDept", "UserId: " + UserId.ToString() + "; deptid: " + deptid.ToString(), User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("UserToDept");
            }
            catch
            {
                //pass date to view
                SendDataUserToDept();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("UserToDept");
            }
        }

        //Save UserToDept
        [HttpPost]
        public ActionResult UserToDeptSave(FormCollection fc)
        {
            try
            {
                string codeDept = mFunction.ToString(fc["Dept"]);
                string UserID = mFunction.ToString(fc["UserID"]);
                //Check code group and function already exists in table Group and Function
                Department dept = db.Departments.SingleOrDefault(m => m.Code == codeDept);

                var lstUser = UsersContext.Users.SingleOrDefault(m => m.UserName == UserID);

                if (dept == null || lstUser == null)
                {
                    //pass date to view
                    SendDataUserToDept();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + Language.label("Department") + "," + Language.label("Code") + Language.label("User");
                    ViewBag.Message = mess;
                    return View("UserToDept");
                }
                string idDept = mFunction.ToString(dept.ID);
                //Check code group and function not exists in table UserToDepts
                if (db.UserToDepts.Where(m => (m.DeptID == idDept) && (m.UserName == UserID)).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataUserToDept();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("UserToDept");
                }
                //Save data
                UserToDept obj = new UserToDept
                {
                    DeptID = idDept,
                    UserName = UserID,
                    TableName = "UserToDept",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db.UserToDepts.Add(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("UserToDept", "Administrator", "Insert", "Insert UserToDept", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("UserToDept");
            }
            catch
            {
                //pass date to view
                SendDataUserToDept();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View();
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //User roles
        #region UserRoles
        // GET: UserToDept
        public ActionResult UserRoles()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UserRoles").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            PassingDataToUserRoles();
            return View();
        }

        //pass date to view
        private void PassingDataToUserRoles()
        {
            var lstUser = db_user.sp_User_Get().ToList();
            ViewBag.lstUser = lstUser;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        [HttpGet]
        public ActionResult UserRolesEdit(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UserRoles").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            PassingDataToUserRolesEdit(id);

            var lstUserRols = db_user.sp_UserRoles_Get(id).ToList();

            var lstRoles = db_user.AspNetRoles.ToList();
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            foreach (var i in lstRoles)
            {
                var sltCount = lstUserRols.Where(m => m.RoleId == i.Id).ToList();
                if (sltCount.Count > 0)
                {
                    obj.Add(new CheckBoxModel { Code = i.Id, Name = i.Id, IsChecked = true });
                }
                else
                {
                    obj.Add(new CheckBoxModel { Code = i.Id, Name = i.Id, IsChecked = false });
                }
            }
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            return View(objBind);
        }

        [HttpPost]
        public ActionResult UserRolesEdit(CheckBoxList Obj, FormCollection fc)
        {
            string key = mFunction.ToString(fc["Code"]);
            try
            {
                DataOperation.ExecuteNonQuery(GlobalVariable.DBUserManager, "exec sp_UserRoles_Del '" + key + "'");
                foreach (var item in Obj.CheckBox)
                {
                    if (item.IsChecked == true)
                    {
                        DataOperation.ExecuteNonQuery(GlobalVariable.DBUserManager, "exec sp_UserRoles_Ins '" + key + "','" + item.Code + "'");
                        //Save log
                        var objLog = db.sp_ActionLog("UserRoles", "Administrator", "Insert", "Insert UserRoles", "exec sp_UserRoles_Ins '" + key + "','" + item.Code + "'", User.Identity.GetUserName(), DateTime.Now);
                        //
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                //pass date to view
                PassingDataToUserRolesEdit(key);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(Obj);
            }
            return RedirectToAction("UserRoles");
        }

        [HttpPost]
        public ActionResult UserRolesEditPass(CheckBoxList Obj, FormCollection fc)
        {
            string key = mFunction.ToString(fc["UserName"]);
            try
            {
                var user = db_user.AspNetUsers.SingleOrDefault(m => m.UserName == key);
                user.PasswordHash = "AM3nasno3q/mItJWBfAfXyW6U7OKeAuUUjzngbCgQLPGkEFzX7XzZ5oHE55kgDG8PA==";
                user.SecurityStamp = "6e41f859-43f5-40af-bbc1-d8070bd7a640";
                db_user.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("UserRoles", "Administrator", "Update", "Reset Password", "ID: " + key, User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("UserRoles");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return RedirectToAction("UserRoles");
            }
        }

        //pass date to view
        private void PassingDataToUserRolesEdit(string id)
        {
            sp_User_Get_Result objUser = db_user.sp_User_Get().SingleOrDefault(m => m.UserName == id);
            ViewBag.objUser = objUser;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }
        #endregion
        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        #region UserModuleRole
        //Get UserModuleRole
        public ActionResult UserModuleRole(string maBoPhan, int? moduleId)
        {
            #region Check cookie ngôn ngữ
            CheckCookie();
            ChangeLanguage(cookie.Value);
            #endregion
            //Get data Module User Module Role
            int languageId = mFunction.ToInt(cookie.Value);
            ViewBag.DataModule = db.sp_Module_Get(languageId, mFunction.ToString(User.Identity.Name));

            //Get data show modal Add User Module Role
            ViewBag.ListModule = db.sp_Module_Get(languageId, mFunction.ToString(User.Identity.Name));
            ViewBag.ListRoles = db_user.AspNetRolesForApps.ToList();
            ViewBag.ListPart = db_HR.MaBoPhans.ToList();

            //Lấy toàn thông tin nhân viên khi show modal add roles
            ViewBag.AllUser = db_HR.tblDanhSachNhanViens
            .Where(x => x.Active == true)
            .Select(x => new UserModuleRoleViewModels
            {
                MaSoNhanVien = x.MaSoNhanVien,
                Ho = x.HoNhanVien,
                Ten = x.TenNhanVien
            }).ToList();


            var listUser = db_user.sp_AspNetUserModuleRole_Get(languageId, maBoPhan, moduleId).ToList();
            if (listUser != null)
            {
                ViewBag.MaBoPhan = maBoPhan;
                ViewBag.ModuleId = moduleId;
                return View(listUser);
            }
            return View();
        }

        //Post Add User Module Role
        [HttpPost]
        public JsonResult UserModuleRoleAdd(AspNetUserModuleRole moduleRole)
        {
            bool result = false;
            AspNetUserModuleRole module = new AspNetUserModuleRole
            {
                UserID = moduleRole.UserID,
                RoleID = moduleRole.RoleID,
                ModuleID = moduleRole.ModuleID,
                Description = moduleRole.Description
            };
            try
            {
                db_user.AspNetUserModuleRoles.Add(module);
                db_user.SaveChanges();
                result = true;
                #region Save Log
                var objLog = db_user.sp_ActionLog("UserModuleRole", "Administrator", "Add", "Add Module Role", "UserID: " + moduleRole.UserID, User.Identity.GetUserName(), DateTime.Now);
                #endregion
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Get Edit UserModuleRole
        [HttpGet]
        public ActionResult UserModuleRoleEdit(string Id, int moduleId)
        {
            //ViewBag.ListGroup = db.Groups.ToList();
            #region Check cookie ngôn ngữ
            CheckCookie();
            ChangeLanguage(cookie.Value);
            #endregion
            int languageId = mFunction.ToInt(cookie.Value);
            ViewBag.ListModule = db.sp_Module_Get(languageId, mFunction.ToString(User.Identity.Name));
            ViewBag.ListRoles = db_user.AspNetRolesForApps.ToList();
            ViewBag.ModuleId = moduleId;
            var data = db_user.AspNetUserModuleRoles.FirstOrDefault(x => x.UserID == Id && x.ModuleID == moduleId);
            if (data != null)
            {
                return PartialView("/Views/Administrator/_ModuleRoleEdit.cshtml", data);
            }
            return View();
        }
        //Post User Module Role Edit
        [HttpPost]
        public JsonResult UserModuleRoleEdit(string userId, string roleId, string desc, int moduleId)
        {
            bool result      = false;
            var data         = db_user.AspNetUserModuleRoles.FirstOrDefault(x => x.UserID == userId && x.ModuleID == moduleId);
            data.RoleID      = roleId;
            data.Description = desc;
            try
            {
                db_user.SaveChanges();
                result = true;
                #region Save Log
                var objLog = db_user.sp_ActionLog("UserModuleRole", "Administrator", "Edit", "Edit Module Role", "UserID: " + userId, User.Identity.GetUserName(), DateTime.Now);
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Post Delete ModuleRole
        public ActionResult UserModuleRoleDelete(int moduleId, string userId)
        {
            if (moduleId != 0 && userId != null)
            {
                var data = db_user.AspNetUserModuleRoles.FirstOrDefault(x => x.ModuleID == moduleId && x.UserID == userId);
                db_user.AspNetUserModuleRoles.Remove(data);
                try
                {
                    db_user.SaveChanges();
                    #region Save Log
                    var objLog = db_user.sp_ActionLog("UserModuleRole", "Administrator", "Delete", "Delete Part Code", "UserId & ModuleId: " + userId + "&" + moduleId, User.Identity.GetUserName(), DateTime.Now);
                    #endregion
                }
                catch (Exception)
                {
                    ViewBag.Message = "► " + Language.label("ErrorSave");
                    throw;
                }
            }
            return Redirect("/Administrator/UserModuleRole");
        }
        #endregion

        #region RoleButton

        //Get view RoleButton
        public ActionResult RoleButton()
        {
            #region Check cookie ngôn ngữ
            CheckCookie();
            ChangeLanguage(cookie.Value);
            #endregion

            //get list button
            ViewBag.Button = db_user.AspNetButtons.ToList();
            //get list role
            ViewBag.Role = db_user.AspNetRolesDevs.ToList();

            //get data
            var data = db_user.AspNetRoleButtons.ToList();
            if (data != null)
            {
                return View(data);
            }
            return View();
        }

        //Get View RoleButtonEdit
        public ActionResult RoleButtonEdit()
        {
            var data = db_user.AspNetRoleButtons.ToList();
            if (data != null)
            {
                return View(data);
            }
            return View();
        }

        // Post RoleButtonEdit
        [HttpGet]
        public ActionResult RoleButtonEdit(AspNetRoleButton model)
        {
            var data = db_user.AspNetRoleButtons.FirstOrDefault(x => x.RoleID == model.RoleID && x.ButtonID == model.ButtonID);
            ViewBag.Button = db_user.AspNetButtons.ToList();
            ViewBag.Role = db_user.AspNetRolesDevs.ToList();
            if (data != null)
            {
                return PartialView("/Views/Administrator/_RoleButtonEdit.cshtml", data);
            }
            return View();
        }

        // Post RoleButtonAdd
        [HttpPost]
        public JsonResult RoleButtonAdd(AspNetRoleButton model)
        {
            int status = 0;
            var isData = db_user.AspNetRoleButtons.FirstOrDefault(x => x.RoleID == model.RoleID && x.ButtonID == model.ButtonID);
            if (isData != null)
            {
                status = -1;
            }
            else
            {
                AspNetRoleButton roleButton = new AspNetRoleButton
                {
                    ButtonID = model.ButtonID,
                    RoleID = model.RoleID,
                    Description = model.Description
                };
                db_user.AspNetRoleButtons.Add(roleButton);
                try
                {
                    db_user.SaveChanges();
                    status = 1;
                    #region Save Log
                    var objLog = db_user.sp_ActionLog("UserModuleRole", "Administrator", "Add", "Add  Module Role", "RoleID & ButtonID: " + model.RoleID + " & " + model.ButtonID, User.Identity.GetUserName(), DateTime.Now);
                    #endregion
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "► " + Language.label("ErrorSave");
                    ex.ToString();
                    throw;
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //Get RoleButtonDelete
        public ActionResult RoleButtonDelete(string roleId, string buttonId)
        {
            if (roleId != null && buttonId != null)
            {
                var data = db_user.AspNetRoleButtons.FirstOrDefault(x => x.RoleID == roleId && x.ButtonID == buttonId);
                db_user.AspNetRoleButtons.Remove(data);
                try
                {
                    db_user.SaveChanges();
                    var objLog = db_user.sp_ActionLog("RoleButton", "Administrator", "Delete", "Delete  Role Button", "RoleID & ButtonID: " + roleId + " & " + buttonId, User.Identity.GetUserName(), DateTime.Now);
                }
                catch (Exception)
                {
                    ViewBag.Message = "► " + Language.label("ErrorSave");
                    throw;
                }
            }
            return Redirect("/Administrator/RoleButton");
        }

        //Get ButtonRoleUpdate
        public ActionResult ButtonRoleUpdate(AspNetRoleButton model)
        {
            if (model != null)
            {
                var data = db_user.AspNetRoleButtons.FirstOrDefault(x => x.RoleID == model.RoleID && x.ButtonID == model.ButtonID);
                data.ButtonID = model.ButtonID;
                data.RoleID = model.RoleID;
                data.Description = model.Description;
                try
                {
                    db_user.SaveChanges();
                    var objLog = db_user.sp_ActionLog("RoleButton", "Administrator", "Update", "Update  Role Button", "RoleID & ButtonID: " + model.RoleID + " & " + model.ButtonID, User.Identity.GetUserName(), DateTime.Now);
                }
                catch (Exception)
                {
                    ViewBag.Message = "► " + Language.label("ErrorSave");
                    throw;
                }
            }
            return Redirect("/Administrator/RoleButton");
        }
        #endregion

        #region Roles Dev
        //Get list Roles
        public ActionResult Roles()
        {
            #region Check cookie ngôn ngữ
            CheckCookie();
            ChangeLanguage(cookie.Value);
            #endregion
            var data = db_user.AspNetRolesDevs.ToList();
            if (data != null)
            {
                return View(data);
            }
            return View();
        }

        //Add Roles
        public JsonResult RoleAdd(AspNetRolesDev model)
        {
            int status = 0;
            var isData = db_user.AspNetRolesDevs.FirstOrDefault(x => x.Id == model.Id);
            if (isData != null)
            {
                status = -1;
            }
            else
            {
                AspNetRolesDev aspNetRolesDev = new AspNetRolesDev
                {
                    Id = model.Id,
                    Name = model.Name
                };
                try
                {
                    db_user.AspNetRolesDevs.Add(aspNetRolesDev);
                    db_user.SaveChanges();
                    status = 1;
                    var objLog = db_user.sp_ActionLog("Roles", "Administrator", "RoleAdd", "RoleAdd  Role", "Id & Name: " + model.Id + " & " + model.Name, User.Identity.GetUserName(), DateTime.Now);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    throw;
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //Post Delete Role 
        [HttpPost]
        public JsonResult RoleDelete(string Id)
        {
            int status = 0;
            var data = db_user.AspNetRolesDevs.FirstOrDefault(x=>x.Id ==Id);
            db_user.AspNetRolesDevs.Remove(data);
            try
            {
                db_user.SaveChanges();
                status = 1;
                var objLog = db_user.sp_ActionLog("Roles", "Administrator", "RoleDelete", "RoleDelete  Role", "Id: " + Id, User.Identity.GetUserName(), DateTime.Now);
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                status = -1;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //Get Edit Roles
        [HttpGet]
        public ActionResult RoleEdit(AspNetRolesDev model)
        {
            var data   = db_user.AspNetRolesDevs.FirstOrDefault(x => x.Id == model.Id);
            if (data != null)
            {
                return PartialView("/Views/Administrator/_RoleEdit.cshtml", data);
            }
            return View();
        }

        // Post Role Update
        [HttpPost]
        public JsonResult RoleUpdate(AspNetRolesDev model)
        {
            int status = 0;
            var data = db_user.AspNetRolesDevs.FirstOrDefault(x => x.Id == model.Id);
            data.Name = model.Name;
            try
            {
                db_user.SaveChanges();
                status = 1;
            }
            catch (Exception ex)
            {
                ex.ToString();
                status = -1;
                throw;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}