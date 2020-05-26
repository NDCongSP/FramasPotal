using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.UserManager;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using FramasVietNam.Models.Meal;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class OEEMSUserRolesController : Controller
    {
        ////////////////////////////////////////////////////////////
        //Variable
        #region Variabal
        //important Variable for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        private UserManagerEntities db_User = new UserManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region OEEMS Roles

        // GET: OEEMSRoles
        public ActionResult OEEMSRoles()
        {
            //
            SendDataOEEMSRoles();
            //
            return View();
        }

        //pass date to view
        private void SendDataOEEMSRoles()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            // to do something            
            var _lstAspNetRolesForApp = db_User.AspNetRolesForApps.ToList();
            ViewBag.lstAspNetRolesForApp = _lstAspNetRolesForApp;

            //Create key
            var _objLst = db_User.AspNetRolesForApps.ToList();
            int _Key;
            if (_objLst.Count > 0)
            {
                _Key = mFunction.ToInt(_objLst.Max(z => z.Id));
            }
            else
            {
                _Key = 0;
            }
            ViewBag.Key = _Key + 1;
        }

        // GET: OEEMSRoles Save
        public ActionResult OEEMSRolesSave(FormCollection fc)
        {
            try
            {
                var _a = mFunction.ToString(fc["Code"]).Trim();
                var _b = mFunction.ToString(fc["Name"]);
                if (_a == "" || _b == "")
                {
                    //pass date to view
                    SendDataOEEMSRoles();
                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    ViewBag.Message = mess;
                    return View("OEEMSRoles");
                }
                if (db_User.AspNetRolesForApps.Where(m => m.Id == _a).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataOEEMSRoles();
                    string mess = "► " + Language.label("ExistsObject");
                    ViewBag.Message = mess;
                    return View("OEEMSRoles");
                }
                AspNetRolesForApp obj = new AspNetRolesForApp
                {
                    Id = _a,
                    Name = _b
                };
                db_User.AspNetRolesForApps.Add(obj);
                db_User.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("OEEMSRoles", "OEEMSUserRoles", "Insert", "Insert A New Roles", "ID: " + _a, User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("OEEMSRoles");
            }
            catch
            {
                //pass date to view
                SendDataOEEMSRoles();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("OEEMSRoles");
            }
        }

        // Delete Department
        public ActionResult OEEMSRolesDel(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSRoles").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                AspNetRolesForApp obj = db_User.AspNetRolesForApps.SingleOrDefault(m => m.Id == id);
                if (obj == null)
                {
                    //pass date to view
                    SendDataOEEMSRoles();
                    string mess = "► " + Language.label("NotExistsObject");
                    ViewBag.Message = mess;
                    return View("OEEMSRoles");
                }
                db_User.AspNetRolesForApps.Remove(obj);
                db_User.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("OEEMSRoles", "OEEMSUserRoles", "Delete", "Delete Dept", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("OEEMSRoles");
            }
            catch
            {
                //pass date to view
                SendDataOEEMSRoles();
                string mess = "► " + Language.label("ErrorSave");
                ViewBag.Message = mess;
                return View("OEEMSRoles");
            }
        }

        //Edit department
        [HttpGet]
        public ActionResult OEEMSRolesEdit(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OEEMSRoles").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            AspNetRolesForApp lstobj = db_User.AspNetRolesForApps.SingleOrDefault(n => n.Id == id);
            if (lstobj == null)
            {
                SendDataOEEMSRoles();
                string mess = "► " + Language.label("NotExistsObject");
                ViewBag.Message = mess;
                return View("OEEMSRoles");
            }
            //pass data lstMenuItem to view
            return View(lstobj);
        }
        [HttpPost]
        public ActionResult OEEMSRolesEdit(AspNetRolesForApp obj)
        {
            try
            {
                if (mFunction.ToString(obj.Id) == "" || mFunction.ToString(obj.Name) == "")
                {
                    CheckCookie();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    ChangeLanguage(mFunction.ToString(cookie.Value));

                    string mess = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    ViewBag.Message = mess;
                    return View(obj);
                }
                AspNetRolesForApp lstobj = db_User.AspNetRolesForApps.SingleOrDefault(m => m.Id == obj.Id);
                lstobj.Id = obj.Id;
                lstobj.Name = obj.Name;
                db_User.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("OEEMSRoles", "OEEMSUserRoles", "Update", "Update Dept", "", User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("OEEMSRoles");
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
        #region OEEMS User Roles

        // GET: OEEMSUserRoles
        public ActionResult OEEMSUserRoles()
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
            var lstUser = db_User.sp_User_Get().ToList();
            ViewBag.lstUser = lstUser;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }


        [HttpGet]
        public ActionResult OEEMSUserRolesEdit(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UserRoles").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass date to view
            PassingDataToUserRolesEdit(id);

            var lstUserRols = db_User.AspNetUserRolesForApps.Where(m=>m.UserId == id).ToList();

            var lstRoles = db_User.AspNetRolesForApps.ToList();
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            foreach (var i in lstRoles)
            {
                var sltCount = lstUserRols.Where(m => m.RoleId == i.Id).ToList();
                if (sltCount.Count > 0)
                {
                    obj.Add(new CheckBoxModel { Code = i.Id, Name = i.Name, IsChecked = true });
                }
                else
                {
                    obj.Add(new CheckBoxModel { Code = i.Id, Name = i.Name, IsChecked = false });
                }
            }
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            return View(objBind);
        }

        [HttpPost]
        public ActionResult OEEMSUserRolesEdit(CheckBoxList Obj, FormCollection fc)
        {
            string key = mFunction.ToString(fc["Code"]);
            try
            {
                List<AspNetUserRolesForApp> _lstobj = db_User.AspNetUserRolesForApps.Where(m => m.UserId == key).ToList();
                if (_lstobj.Count > 0)
                {
                    foreach(var item in _lstobj)
                    {
                        db_User.AspNetUserRolesForApps.Remove(item);
                    }
                }
                if (Obj.CheckBox.Count > 0)
                {
                    foreach (var item in Obj.CheckBox)
                    {
                        if (item.IsChecked == true)
                        {
                            AspNetUserRolesForApp objAdd = new AspNetUserRolesForApp()
                            {
                                RoleId = item.Code,
                                UserId = key,
                                Description = "OEEMS"
                            };
                            db_User.AspNetUserRolesForApps.Add(objAdd);
                        }
                    }
                }
                db_User.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("OEEMSUserRolesEdit", "OEEMSUserRoles", "Update", "Update role", "ID: " + key, User.Identity.GetUserName(), DateTime.Now);
                
            }

            catch (Exception ex)
            {
                ex.ToString();
                //pass date to view
                PassingDataToUserRolesEdit(key);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(Obj);
            }
            return RedirectToAction("OEEMSUserRoles");
        }

        //pass date to view
        private void PassingDataToUserRolesEdit(string id)
        {
            List<sp_UserRolesForApp_Get_Result> lstObj = db_User.sp_UserRolesForApp_Get(id).ToList();
            if (lstObj.Count > 0)
            {
                sp_UserRolesForApp_Get_Result objUserRolesForApp = lstObj[0];
                ViewBag.objUserRolesForApp = objUserRolesForApp;
            }
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
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