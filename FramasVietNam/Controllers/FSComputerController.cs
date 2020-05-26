using FramasVietNam.Common;
using FramasVietNam.Models.HumanResource;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.UserManager;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class FSComputerController : Controller
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
        // GET: FivesComputer
        [HttpGet]
        public ActionResult FSComputer()
        {
            //CheckCookie and ChangeLanguage always exist in action(it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "FSComputer").ToList();
            string strUserID = User.Identity.GetUserId();
            string strDeptId = string.Empty;
            object lstFSComputer = string.Empty;
            var lstDepartment = db_HR.MaBoPhans.OrderBy(m => m.TenBoPhanA).ToList();
            if (lstDepartment != null)
            {
                ViewBag.AllDept = lstDepartment;
            }
            ViewBag.AllUser = db_HR.tblDanhSachNhanViens
                .Where(x => x.Active == true)
                .Select(x => new UserModuleRoleViewModels
                {
                    MaSoNhanVien = x.MaSoNhanVien,
                    Ho = x.HoNhanVien,
                    Ten = x.TenNhanVien,
                }).ToList();

            UserByDeptId(strDeptId.ToString());

            lstFSComputer = db.FSComputers.Where(x => x.Active == true).AsQueryable().ToList();
            return View(lstFSComputer);
        }

        [HttpPost]
        public ActionResult FSComputer(FormCollection fc)
        {
            //CheckCookie and ChangeLanguage always exist in action(it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "FSComputer").ToList();
            string strDeptId = mFunction.ToString(fc["select_DeptId"]);
            var strUserID = mFunction.ToString(fc["select_UserId"]);
            string strDateMark = mFunction.ToString(fc["dtDateAdd"]);
            DateTime dateMark = mFunction.ToDateTime(strDateMark);
            object lstFSComputer = string.Empty;

            //Nếu chỉ chọn department 
            if (!strDeptId.IsNullOrEmptyOrWhileSpace() && !strDeptId.Contains("All"))
            {
                //Nếu có chọn userid theo department 
                if (!strUserID.IsNullOrEmptyOrWhileSpace() && !strDateMark.IsNullOrEmptyOrWhileSpace())
                {
                    lstFSComputer = db.FSComputers.Where(x => x.DepartmentId == strDeptId
                    && x.EmployeeId == strUserID
                    && (x.DateMark.Value.Month == dateMark.Month && x.DateMark.Value.Year == dateMark.Year)).AsQueryable().ToList();
                    ViewBag.UserId = strUserID.ToString();
                }
                //Nếu có chọn userid theo department 
                else if (!strUserID.IsNullOrEmptyOrWhileSpace())
                {
                    lstFSComputer = db.FSComputers.Where(x => x.DepartmentId == strDeptId && x.EmployeeId == strUserID).AsQueryable().ToList();
                    ViewBag.UserId = strUserID.ToString();
                }
                else
                {
                    //Ngược lại nếu không chọn userId
                    lstFSComputer = db.FSComputers.Where(x => x.DepartmentId == strDeptId).AsQueryable().ToList();
                }
                //ViewBag.AllDept = db_HR.MaBoPhans.FirstOrDefault(m => m.MaBoPhan1 == strDeptId);
                ViewBag.DeptId = strDeptId.ToString();
                UserByDeptId(strDeptId.ToString());
            }


            //Nếu chỉ chọn date
            else if (!dateMark.IsNullOrEmptyOrWhileSpace())
            {
                lstFSComputer = db.FSComputers.Where(x => x.DateMark.Value.Month == dateMark.Month && x.DateMark.Value.Year == dateMark.Year).ToList();
            }

            //Nếu chọn cả department và date
            else if (!strDeptId.IsNullOrEmptyOrWhileSpace() && !strUserID.IsNullOrEmptyOrWhileSpace())
            {
                //Nễu có chọn userId theo department 
                if (!dateMark.IsNullOrEmptyOrWhileSpace())
                {
                    lstFSComputer = db.FSComputers.Where(x => x.DepartmentId == strDeptId && x.EmployeeId == strUserID && x.DateMark == dateMark).ToList();
                }
                lstFSComputer = db.FSComputers.Where(x => x.DepartmentId == strDeptId && x.DateMark == dateMark).ToList();
                ViewBag.DeptId = strDeptId;
            }

            var lstDepartment = db_HR.MaBoPhans.OrderBy(m => m.TenBoPhanA).ToList();

            if (lstDepartment != null)
            {
                ViewBag.AllDept = lstDepartment;
            }

            if (!dateMark.IsNullOrEmptyOrWhileSpace())
            {
                ViewBag.dtDateSearch = dateMark;
            }
            return View(lstFSComputer);
        }

        //FSComputerView
        [HttpGet]
        public ActionResult FSComputerView(int? Id)
        {
            var data = db.FSComputers.FirstOrDefault(x => x.Id == Id);
            ViewBag.objView = data;
            if (data != null)
            {
                return PartialView("/Views/FSComputer/_FSComputerView.cshtml");
            }
            return PartialView();
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

        //FSComputerAdd
        [HttpPost]
        public JsonResult FSComputerAdd(FSComputerViewModels model)
        {
            int status = 0;
            //ConvertYear
            DateTime convertDate = mFunction.ToDateTime(model.DateMark);
            if (model != null)
            {
                var checkData = db.FSComputers.Where(x => x.EmployeeId == model.EmployeeId && x.DateMark == model.DateMark).ToList();
                if (checkData.Count() > 2)
                {
                    status = -1;
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //data sum 
                    FSComputer list = new FSComputer();
                    list.DepartmentId = model.DepartmentId;
                    list.DepartmentName = model.DepartmentName;
                    list.EmployeeId = model.EmployeeId;
                    list.EmployeeName = model.EmployeeName;
                    list.DateMark = model.DateMark;
                    list.TotalScore = model.TotalScore;
                    list.TotalPercent = model.TotalPercent;
                    list.Mark1 = model.Mark1;
                    list.Factor1 = model.Factor1;
                    list.Note1 = model.Note1;
                    list.Mark2 = model.Mark2;
                    list.Factor2 = model.Factor2;
                    list.Note2 = model.Note2;
                    list.Mark3 = model.Mark3;
                    list.Factor3 = model.Factor3;
                    list.Note3 = model.Note3;
                    list.Mark4 = model.Mark4;
                    list.Factor4 = model.Factor4;
                    list.Note4 = model.Note4;
                    list.Mark5 = model.Mark5;
                    list.Factor5 = model.Factor5;
                    list.Note5 = model.Note5;
                    list.Mark6 = model.Mark6;
                    list.Factor6 = model.Factor6;
                    list.Note6 = model.Note6;
                    list.Mark7 = model.Mark7;
                    list.Factor7 = model.Factor7;
                    list.Note7 = model.Note7;
                    list.Mark8 = model.Mark8;
                    list.Factor8 = model.Factor8;
                    list.Note8 = model.Note8;
                    list.Mark9 = model.Mark9;
                    list.Factor9 = model.Factor9;
                    list.Note9 = model.Note9;
                    list.Mark10 = model.Mark10;
                    list.Factor10 = model.Factor10;
                    list.Note10 = model.Note1;
                    list.Mark11 = model.Mark10;
                    list.Factor11 = model.Factor11;
                    list.Note11 = model.Note11;
                    list.Mark12 = model.Mark11;
                    list.Factor12 = model.Factor1;
                    list.Note12 = model.Note1;
                    list.Mark13 = model.Mark1;
                    list.Factor13 = model.Factor1;
                    list.Note13 = model.Note1;
                    list.Mark14 = model.Mark1;
                    list.Factor14 = model.Factor1;
                    list.Note14 = model.Note1;
                    list.DateAdd = DateTime.Now;
                    list.Active = true;
                    list.UserAdd = User.Identity.GetUserName();
                    list.UserUpd = User.Identity.GetUserName();
                    list.DateUpd = DateTime.Now;
                    try
                    {
                        db.FSComputers.Add(list);
                        db.SaveChanges();
                        status = 1;
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }

            return Json(status, JsonRequestBehavior.AllowGet);

        }

        //Print FSComputer
        public ActionResult FSComputerPrint(DateTime dtSearch, string dept)
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


        //POST Delete FSComputer
        [HttpPost]
        public JsonResult FSComputerDel(int? Id)
        {
            int status = 0;
            var data = db.FSComputers.SingleOrDefault(x => x.Id == Id);
            db.FSComputers.Remove(data);
            try
            {
                db.SaveChanges();
                status = 1;
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                status = -1;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }


        //Get FSComputerEdit
        [HttpGet]
        public ActionResult FSComputerEdit(int? Id)
        {
            var data = db.FSComputers.FirstOrDefault(x => x.Id == Id);
            if (data != null)
            {
                return PartialView("_FSComputerEdit", data);
            }
            return RedirectToAction("FSComputer");
        }


        #region  Post Update FSComputer
        // Post Update FSComputer
        //[HttpPost]
        //public JsonResult FSComputerUpdate(int? Id, int Month, DateTime Year, float TotalScore, float TotalPercent,
        //    int Mark1, string Factor1, string Note1, int Mark2, string Factor2, string Note2, int Mark3, string Factor3, string Note3,
        //    int Mark4, string Factor4, string Note4, int Mark5, string Factor5, string Note5, int Mark6, string Factor6, string Note6,
        //    int Mark7, string Factor7, string Note7, int Mark8, string Factor8, string Note8, int Mark9, string Factor9, string Note9,
        //    int Mark11, string Factor11, string Note11, int Mark12, string Factor12, string Note12, int Mark13, string Factor13, string Note13,
        //    int Mark14, string Factor14, string Note14, int Mark15, string Factor15, string Note15)
        //{
        //    int status = 0;
        //    var data = db.FSComputers.FirstOrDefault(x => x.Id == Id);
        //    data.Month = Month;
        //    data.Year = Year;
        //    data.TotalScore = TotalScore;
        //    data.TotalPercent = TotalPercent;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    data.Mark1 = Mark1;
        //    data.Factor1 = Factor1;
        //    data.Note1 = Note1;
        //    try
        //    {
        //        db.SaveChanges();
        //        status = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        status = -1;
        //        throw;
        //    }
        //    return Json(status, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult FSComputerUpdate(FSComputerViewModels models)
        {
            int status = 0;
            try
            {
                var data = db.FSComputers.FirstOrDefault(x => x.Id == models.Id);
                if (data != null)
                {
                   data.TotalScore = models.TotalScore;
                   data.TotalPercent = models.TotalPercent;
                   data.Mark1 = models.Mark1;
                   data.Factor1 = models.Factor1;
                   data.Note1 = models.Note1;
                   data.Mark2 = models.Mark2;
                   data.Factor2 = models.Factor2;
                   data.Note2 = models.Note2;
                   data.Mark3 = models.Mark3;
                   data.Factor3 = models.Factor3;
                   data.Note3 = models.Note3;
                   data.Mark4 = models.Mark4;
                   data.Factor4 = models.Factor4;
                   data.Note4 = models.Note4;
                   data.Mark5 = models.Mark5;
                   data.Factor5 = models.Factor5;
                   data.Note5 = models.Note5;
                   data.Mark6 = models.Mark6;
                   data.Factor6 = models.Factor6;
                   data.Note6 = models.Note6;
                   data.Mark7 = models.Mark7;
                   data.Factor7 = models.Factor7;
                   data.Note7 = models.Note7;
                   data.Mark8 = models.Mark8;
                   data.Factor8 = models.Factor8;
                   data.Note8 = models.Note8;
                   data.Mark9 = models.Mark9;
                   data.Factor9 = models.Factor9;
                   data.Note9 = models.Note9;
                   data.Mark10 = models.Mark10;
                   data.Factor10 = models.Factor10;
                   data.Note10 = models.Note1;
                   data.Mark11 = models.Mark10;
                   data.Factor11 = models.Factor11;
                   data.Note11 = models.Note11;
                   data.Mark12 = models.Mark11;
                   data.Factor12 = models.Factor1;
                   data.Note12 = models.Note1;
                   data.Mark13 = models.Mark1;
                   data.Factor13 = models.Factor1;
                   data.Note13 = models.Note1;
                   data.Mark14 = models.Mark1;
                   data.Factor14 = models.Factor1;
                   data.Note14 = models.Note1;
                   data.DateAdd = DateTime.Now;
                   data.Active = true;
                   data.UserAdd = User.Identity.GetUserName();
                   data.UserUpd = User.Identity.GetUserName();
                   data.DateUpd = DateTime.Now;
                }
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
    }
}