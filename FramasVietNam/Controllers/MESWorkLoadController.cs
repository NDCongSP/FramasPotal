using FramasVietNam.Common;
using FramasVietNam.Models.HumanResource;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.MESWorkLoad;
using FramasVietNam.Models.UserManager;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FramasVietNam.Controllers
{
    public class MESWorkLoadController : Controller
    {
        ////////////////////////////////////////////////////////////
        #region Variabal
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        private HumanResourceEntities db_HR = new HumanResourceEntities();
        private UserManagerEntities db_User = new UserManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////

        // GET: MESWorkLoad
        public ActionResult MESWorkLoad(FormCollection fc)
        {
            //CheckCookie and ChangeLanguage always have in action(it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            //checkDataDepartment
            string strUserID   = User.Identity.GetUserId();
            int strModuleID = mFunction.ToInt(Session["ModuleID"]);
            var objRole = db_User.AspNetUserModuleRoles.FirstOrDefault(x => x.UserID == strUserID && x.ModuleID == strModuleID);

            if (objRole != null)
            {
                ViewBag.strRole = objRole.RoleID;
            }

            if (strUserID != null)
            {
                ViewBag.strUserID = strUserID;
            }

            //Check permission user
            var lst_Check         = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MESWorkLoad").ToList();
            string strDeptId      = mFunction.ToString(fc["Department"]);
            string strDateWork    = mFunction.ToString(fc["dtDateSearch"]);
            DateTime dateWork     = mFunction.ToDateTime(strDateWork);
            object lstMesWorkLoad = string.Empty;
            if (!strDeptId.IsNullOrEmptyOrWhileSpace() && !dateWork.IsNullOrEmptyOrWhileSpace())
            {
                lstMesWorkLoad = db.sp_MesListWorkLoad(dateWork, strDeptId).ToList();
                ViewBag.DeptId = strDeptId;
            }
            else if (!strDeptId.IsNullOrEmptyOrWhileSpace())
            {
                
                ViewBag.AllDept = db_HR.MaBoPhans.FirstOrDefault(m => m.MaBoPhan1 == strDeptId);
            }
            else if (!dateWork.IsNullOrEmptyOrWhileSpace())
            {
                //Lấy ra mặc định department của user đang login
              var objDept = db_HR.tblHoSoLyLichNhanViens.FirstOrDefault(m => m.MaSoNhanVien == strUserID);
                if (objDept != null)
                {
                    strDeptId = mFunction.ToString(objDept.BoPhan).Trim();
                }
                lstMesWorkLoad = db.sp_MesListWorkLoad(dateWork, strDeptId).ToList();
                ViewBag.DeptId = strDeptId;
            }
            var lstDepartment = db_HR.MaBoPhans.OrderBy(m => m.TenBoPhanA).ToList();
            
            if (lstDepartment != null)
            {
                ViewBag.AllDept = lstDepartment;
            }

            if (!dateWork.IsNullOrEmptyOrWhileSpace())
            {
                ViewBag.dtDateSearch = dateWork;
            }

            ViewBag.AllUser = db_HR.tblDanhSachNhanViens
                .Where(x => x.Active == true)
                .Select(x => new UserModuleRoleViewModels
                {
                    MaSoNhanVien = x.MaSoNhanVien,
                    Ho = x.HoNhanVien,
                    Ten = x.TenNhanVien,
                }).ToList();

            return View(lstMesWorkLoad);
        }

        //Choose user when departmentId change
        public JsonResult UserByDeptId(string deptId)
        {
            //Lấy toàn thông tin nhân viên 
            List<UserModuleRoleViewModels> ltsUser = new List<UserModuleRoleViewModels>();
            if (deptId != null && deptId != string.Empty)
            {
                ltsUser = (from a in db_HR.tblDanhSachNhanViens
                           join b in db_HR.tblHoSoLyLichNhanViens
                           on a.MaSoNhanVien equals b.MaSoNhanVien
                           where (a.Active == true && b.BoPhan == deptId)
                           select new UserModuleRoleViewModels
                           {
                               MaSoNhanVien = a.MaSoNhanVien,
                               Ho = a.HoNhanVien,
                               Ten = a.TenNhanVien,
                           }).ToList();
            }
            else
            {
                ViewBag.AllUser = db_HR.tblDanhSachNhanViens
                .Where(x => x.Active == true)
                .Select(x => new UserModuleRoleViewModels
                {
                    MaSoNhanVien = x.MaSoNhanVien,
                    Ho = x.HoNhanVien,
                    Ten = x.TenNhanVien,
                }).ToList();
            }
            if (ltsUser != null)
            {
                ViewBag.AllUser = ltsUser;
            }
            return Json(ltsUser, JsonRequestBehavior.AllowGet);
        }

        //POST Add MESWorkLoad
        [HttpPost]
        public JsonResult MESWorkLoadAdd(ListWorkLoad model)
        {
            //Bien tam
            int status = 0;
            //Check list data
            var checkData = db.ListWorkLoads.Where(m => m.EmployeeId == model.EmployeeId
            && m.DateWork.Value.Month == DateTime.Now.Month
            && m.DateWork.Value.Year == DateTime.Now.Year).ToList();
            //data sum 
            var totalWorkingHour = checkData.Select(x => x.WorkingHour).Sum();
            int targetTime = 208 - mFunction.ToInt(totalWorkingHour);
            //check Total Time Can Not Greater Than The Target Time
            if (model.WorkingHour > targetTime)
            {
                status = -1;
                ViewBag.Message = "► " + "false";
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Set data ListWorkLoad
                ListWorkLoad list = new ListWorkLoad();
                list.DepartmentId = model.DepartmentId;
                list.DepartmentName = model.DepartmentName;
                list.EmployeeId = model.EmployeeId.Trim();
                list.EmployeeName = model.EmployeeName;
                list.TotalWorking = model.TotalWorking;
                list.WorkingHour = model.WorkingHour;
                list.Percen = model.Percen;
                list.Description = model.Description;
                list.DateWork = DateTime.Now;
                list.DateAdd = DateTime.Now;
                list.Active = true;
                list.UserAdd = User.Identity.GetUserName();
                try
                {
                    db.ListWorkLoads.Add(list);
                    db.SaveChanges();
                    status = 1;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    throw;
                }
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }


        public ActionResult MESWorkLoadPrint(DateTime dtSearch, string dept)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MESWorkLoad").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            if (dept.IsNullOrEmptyOrWhileSpace())
            {
                dept = "All";
            }

            var lstMesWorkLoadPlanning = db.sp_MesListWorkLoad(dtSearch, dept).ToList();
            ViewBag.lstMesWorkLoadPlanning = lstMesWorkLoadPlanning;
            return RedirectToAction("WorkLoadPrint", "Report", new { strQr = "exec sp_MesListWorkLoad '" + dtSearch.ToString("yyyy/MM/dd") + "','" + dept + "'" });
        }


        //POST Delete MESWorkLoad
        [HttpPost]
        public JsonResult MESWorkLoadDel(int? Id)
        {
            int status = 0;
            var data = db.ListWorkLoads.FirstOrDefault(x => x.Id == Id);
            db.ListWorkLoads.Remove(data);
            try
            {
                db.SaveChanges();
                status = 1;
                //Save log
                //var objLog = db_MES.sp_ActionLog("Roles", "MESWorkLoa", "MESWorkLoadDel", "MESWorkLoadDel  Index", "Id: " + Id, User.Identity.GetUserName(), DateTime.Now);
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                status = -1;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //Get Edit MESWorkLoad
        [HttpGet]
        public ActionResult MESWorkLoadEdit(int? Id)
        {
            var data = db.ListWorkLoads.FirstOrDefault(x => x.Id == Id);
            if (data != null)
            {
                return PartialView("/Views/MESWorkLoad/_MESWorkLoadEdit.cshtml", data);
            }
            return PartialView();
        }

        // Post Update MESWorkLoad 
        [HttpPost]
        public JsonResult MESWorkLoadUpdate(int? Id, double WorkingHour, double Percen, string Description)
        {
            int status = 0;
            var data = db.ListWorkLoads.FirstOrDefault(x => x.Id == Id);
            data.WorkingHour = WorkingHour;
            data.Percen = Percen;
            data.Description = Description;
            try
            {
                db.SaveChanges();
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
    }
}