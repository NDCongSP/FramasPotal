using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.HumanResource;
using FramasVietNam.Models.UserManager;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using FramasVietNam.Models;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class HREmployeeController : Controller
    {
        ////////////////////////////////////////////////////////////

        #region Variabal

        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();

        private HumanResourceEntities db_HR = new HumanResourceEntities();
        private UserManagerEntities db_User = new UserManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;

        #endregion Variabal

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        #region Employee

        // GET: Employee
        public ActionResult Employee()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Employee").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass data to view
            PassDataToEmployee();
            //
            return View();
        }

        //pass data to view
        private void PassDataToEmployee()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lstEmployee = db_HR.sp_Employee_Get().ToList();
            ViewBag.lstEmployee = lstEmployee;
        }

        // GET: add new employee
        [HttpGet]
        public ActionResult EmployeeAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Employee").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass data to view
            PassDataToEmployeeAdd();
            //
            return View();
        }

        //pass data to view
        private void PassDataToEmployeeAdd()
        {
            var lstDepartment = db_HR.MaBoPhans.ToList();
            ViewBag.lstDepartment = lstDepartment;
            var lstNationality = db_HR.MaQuocGias.ToList();
            ViewBag.lstNationality = lstNationality;
            var lstReligion = db_HR.MaTonGiaos.ToList();
            ViewBag.lstReligion = lstReligion;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        // Post: add new employee
        [HttpPost]
        public ActionResult EmployeeAdd(FormCollection fc)
        {
            try
            {
                //get data from view
                string _User = mFunction.ToString(fc["User"]);
                string FirstName = mFunction.ToString(fc["FirstName"]);
                string LastName = mFunction.ToString(fc["LastName"]);
                int idsex = mFunction.ToInt(fc["Sex"]);
                Boolean Sex = Convert.ToBoolean(idsex);
                DateTime DateOfBirth = mFunction.ToDateTime(fc["DateOfBirth"]);
                string PlaceOfBirth = mFunction.ToString(fc["PlaceOfBirth"]);
                string Address = mFunction.ToString(fc["Address"]);
                string MobilePhone = mFunction.ToString(fc["MobilePhone"]);
                string Nationality = mFunction.ToString(fc["Nationality"]);
                string Religion = mFunction.ToString(fc["Religion"]);
                string Department = mFunction.ToString(fc["Department"]);
                int office = mFunction.ToInt(fc["isOfficeHours"]);
                Boolean isOfficeHours = Convert.ToBoolean(office);
                //check null data
                if (_User == "" || FirstName == "" || LastName == "" || DateOfBirth.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd") || MobilePhone == "" || Department == "")
                {
                    //pass data to view
                    PassDataToEmployeeAdd();
                    ViewBag.Message = "► " + Language.label("WarningNullData") + ": " + @Language.label("FieldNotEmpty");
                    return View();
                }
                //check key duplication
                var obj = db_HR.tblDanhSachNhanViens.SingleOrDefault(m => m.MaSoNhanVien == _User);
                if (obj != null)
                {
                    //pass date to view
                    PassDataToEmployee();
                    ViewBag.Message = "► " + Language.label("ExistsObject");
                    return View();
                }
                //prepare data for insert
                tblDanhSachNhanVien objNhanVien = new tblDanhSachNhanVien()
                {
                    MaSoNhanVien = _User,
                    HoNhanVien = LastName,
                    TenNhanVien = FirstName,
                    GioiTinh = Sex,
                    NgaySinh = DateOfBirth,
                    NoiSinh = PlaceOfBirth,
                    DiaChi = Address,
                    DienThoaiNR = MobilePhone,
                    QuocTich = Nationality,
                    TonGiao = Religion,
                    Active = true
                };
                tblHoSoLyLichNhanVien objHSNhanVien = new tblHoSoLyLichNhanVien()
                {
                    MaSoNhanVien = _User,
                    BoPhan = Department,
                    HC = isOfficeHours
                };
                UserToDept objUserToDept = new UserToDept()
                {
                    UserName = _User,
                    DeptID = Department,
                    Status = 1,
                    PCIDAdd = User.Identity.GetUserName(),
                    DateAdd = DateTime.Now
                };
                AspNetUser objAspNetUser = new AspNetUser()
                {
                    Id = _User,
                    Email = "nothing" + _User + "@framas.com.vn",
                    EmailConfirmed = true,
                    PasswordHash = "AM3nasno3q/mItJWBfAfXyW6U7OKeAuUUjzngbCgQLPGkEFzX7XzZ5oHE55kgDG8PA==",
                    SecurityStamp = "6e41f859-43f5-40af-bbc1-d8070bd7a640",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    UserName = _User
                };
                //save data
                db_HR.tblDanhSachNhanViens.Add(objNhanVien);
                db_HR.tblHoSoLyLichNhanViens.Add(objHSNhanVien);
                db.UserToDepts.Add(objUserToDept);
                db_User.AspNetUsers.Add(objAspNetUser);
                db_HR.SaveChanges();
                db.SaveChanges();
                db_User.SaveChanges();
                //Save log
                var objLog = db_HR.sp_ActionLog("Employee", "HREmployee", "Insert", "Add new employee", "ID: " + _User, User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Employee");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //pass data to view
                PassDataToEmployeeAdd();
                //
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View();
            }
        }

        // GET: update employee
        [HttpGet]
        public ActionResult EmployeeUpd(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Employee").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //pass data to view
            PassDataToEmployeeUpd();
            var obj = db_HR.sp_Employee_Get().SingleOrDefault(m => m.MaSoNhanVien == id);
            //
            return View(obj);
        }

        [HttpPost]
        public ActionResult EmployeeUpd(sp_Employee_Get_Result obj, FormCollection fc)
        {
            //get data from view
            string _User = mFunction.ToString(fc["User"]);
            string FirstName = mFunction.ToString(fc["FirstName"]);
            string LastName = mFunction.ToString(fc["LastName"]);
            int idsex = mFunction.ToInt(fc["Sex"]);
            Boolean Sex = Convert.ToBoolean(idsex);
            DateTime DateOfBirth = mFunction.ToDateTime(fc["DateOfBirth"]);
            string PlaceOfBirth = mFunction.ToString(fc["PlaceOfBirth"]);
            string Address = mFunction.ToString(fc["Address"]);
            string MobilePhone = mFunction.ToString(fc["MobilePhone"]);
            string Nationality = mFunction.ToString(fc["Nationality"]);
            string Religion = mFunction.ToString(fc["Religion"]);
            string Department = mFunction.ToString(fc["Department"]);
            int office = mFunction.ToInt(fc["isOfficeHours"]);
            Boolean isOfficeHours = Convert.ToBoolean(office);

            var obj_pass = new sp_Employee_Get_Result()
            {
                MaSoNhanVien = _User,
                HoNhanVien = LastName,
                TenNhanVien = FirstName,
                NgaySinh = DateOfBirth,
                GioiTinh = Sex,
                DiaChi = Address,
                NoiSinh = PlaceOfBirth,
                SoCMND = "",
                MaQuocTich = Nationality,
                QuocTich = Nationality,
                MaDanToc = "",
                DanToc = "",
                MaTonGiao = Religion,
                TonGiao = Religion,
                DienThoaiNR = MobilePhone,
                MaBoPhan = Department,
                BoPhan = Department,
                HC = isOfficeHours
            };

            try
            {
                //check null data
                if (_User == "" || FirstName == "" || LastName == "" || DateOfBirth.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd") || MobilePhone == "" || Department == "")
                {
                    //pass data to view
                    PassDataToEmployeeUpd();
                    ViewBag.Message = "► " + Language.label("WarningNullData") + ": " + @Language.label("FieldNotEmpty");
                    return View(obj_pass);
                }
                //prepare data for update
                var objNhanVien = db_HR.tblDanhSachNhanViens.SingleOrDefault(m => m.MaSoNhanVien == _User);
                if (objNhanVien != null)
                {
                    objNhanVien.HoNhanVien = LastName;
                    objNhanVien.TenNhanVien = FirstName;
                    objNhanVien.GioiTinh = Sex;
                    objNhanVien.NgaySinh = DateOfBirth;
                    objNhanVien.NoiSinh = PlaceOfBirth;
                    objNhanVien.DiaChi = Address;
                    objNhanVien.DienThoaiNR = MobilePhone;
                    objNhanVien.QuocTich = Nationality;
                    objNhanVien.TonGiao = Religion;
                }
                var objHSNhanVien = db_HR.tblHoSoLyLichNhanViens.SingleOrDefault(m => m.MaSoNhanVien == _User);
                if (objHSNhanVien != null)
                {
                    objHSNhanVien.BoPhan = Department;
                    objHSNhanVien.HC = isOfficeHours;
                }
                UserToDept objUserToDept = new UserToDept();
                if (objUserToDept != null)
                {
                    objUserToDept.DeptID = Department;
                    objUserToDept.PCIDAdd = User.Identity.GetUserName();
                    objUserToDept.DateAdd = DateTime.Now;
                }
                //check exists object
                if (objUserToDept == null || objHSNhanVien == null || objNhanVien == null)
                {
                    //pass data to view
                    PassDataToEmployee();
                    ViewBag.Message = "► " + Language.label("WarningNullData") + ": " + @Language.label("FieldNotEmpty");
                    return View("Employee");
                }
                //save data
                db_HR.SaveChanges();
                db.SaveChanges();
                //Save log
                var objLog = db_HR.sp_ActionLog("Employee", "HREmployee", "Update", "Update employee", "ID: " + _User, User.Identity.GetUserName(), DateTime.Now);
                //
                return RedirectToAction("Employee");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //pass data to view
                PassDataToEmployeeUpd();
                //
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj_pass);
            }
        }

        //pass data to view
        private void PassDataToEmployeeUpd()
        {
            var lstDepartment = db_HR.MaBoPhans.ToList();
            ViewBag.lstDepartment = lstDepartment;
            var lstNationality = db_HR.MaQuocGias.ToList();
            ViewBag.lstNationality = lstNationality;
            var lstReligion = db_HR.MaTonGiaos.ToList();
            ViewBag.lstReligion = lstReligion;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        //lock employee
        public ActionResult EmployeeLock(string id)
        {
            try
            {
                //Check permission user
                var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Employee").ToList();
                if (lst_Check.Count <= 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                //pass data to view
                PassDataToEmployeeUpd();
                var obj = db_HR.tblDanhSachNhanViens.SingleOrDefault(m => m.MaSoNhanVien == id);
                var obj1 = db_User.AspNetUsers.SingleOrDefault(m => m.Id == id);
                if (obj == null || obj1 == null)
                {
                    //pass date to view
                    PassDataToEmployee();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("Employee");
                }
                obj1.TwoFactorEnabled = true;
                obj.Active = false;
                db_HR.SaveChanges();
                db_User.SaveChanges();
                //Save log
                var objLog = db_HR.sp_ActionLog("Employee", "HREmployee", "Lock", "Lock employee", "ID: " + id, User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("Employee");
            }
            catch
            {
                //pass date to view
                PassDataToEmployee();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("Employee");
            }
        }

        #endregion Employee

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

        #endregion Cookie language

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        #region List Part Code

        //get List Part Code
        public ActionResult ListPartCode()
        {
            #region Check cookie ngôn ngữ

            CheckCookie();
            ChangeLanguage(cookie.Value);

            #endregion Check cookie ngôn ngữ

            var listData = db_HR.MaBoPhans.ToList();
            if (listData != null)
            {
                return View(listData);
            }
            return View();
        }

        //Post Part Code Save
        [HttpPost]
        public JsonResult PartCodeSave(PartCodeViewModels model)
        {
            #region Check cookie ngôn ngữ

            CheckCookie();
            ChangeLanguage(cookie.Value);

            #endregion Check cookie ngôn ngữ

            int result = 0;
            var isExist = db_HR.MaBoPhans.Where(x => x.MaBoPhan1 == model.MaBoPhan1).FirstOrDefault();
            if (isExist != null)
            {
                result = 0;
            }
            else
            {
                MaBoPhan boPhan = new MaBoPhan
                {
                    MaBoPhan1 = model.MaBoPhan1,
                    TenBoPhan = model.TenBoPhan,
                    TenBoPhanA = model.TenBoPhanA,
                    MaPhongBan = model.MaPhongBan,
                    Nghi7 = model.Nghi7
                };
                db_HR.MaBoPhans.Add(boPhan);
                try
                {
                    db_HR.SaveChanges();
                    result = 1;

                    #region Save Log

                    var objLog = db_HR.sp_ActionLog("ListPartCode", "HREmployee", "Update", "Update Part Code", "ID: " + boPhan.MaBoPhan1, User.Identity.GetUserName(), DateTime.Now);

                    #endregion Save Log
                }
                catch (Exception ex)
                {
                    result = -1;
                    ex.ToString();
                    throw;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Post Part Code Delete
        public ActionResult PartCodeDelete(MaBoPhan maBoPhan)
        {
            var data = db_HR.MaBoPhans.FirstOrDefault(x => x.MaBoPhan1 == maBoPhan.MaBoPhan1 && x.MaPhongBan == maBoPhan.MaPhongBan);
            if (data != null)
            {
                db_HR.MaBoPhans.Remove(data);
                try
                {
                    db_HR.SaveChanges();

                    #region Save Log

                    var objLog = db_HR.sp_ActionLog("ListPartCode", "HREmployee", "Delete", "Delete Part Code", "ID: " + data.MaBoPhan1, User.Identity.GetUserName(), DateTime.Now);

                    #endregion Save Log
                }
                catch (Exception)
                {
                    ViewBag.Message = "► " + Language.label("ErrorSave");
                    throw;
                }
            }
            return Redirect("/HREmployee/ListPartCode");
        }

        #endregion List Part Code
    }
}