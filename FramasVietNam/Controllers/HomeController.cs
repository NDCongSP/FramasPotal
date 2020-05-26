using Aspose.Words;
using Aspose.Words.Saving;
using FramasVietNam.Common;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Document = FramasVietNam.Models.Menu.Document;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        #region Global variable

        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();

        //important Variabal cookie in multi language
        private HttpCookie cookie;

        #endregion Global variable

        #region Home

        //View Home
        [HttpGet]
        public ActionResult Index()
        {
            //Remide module seleted
            Session["ModuleID"] = string.Empty;
            Session["GroupID"] = string.Empty;
            PassDataToHome();

            var data = db.Documents.Where(x => x.IsActive == true).ToList();
            if (data.Count() > 0)
            {
                return View(data);
            }
            return View();
        }

        //Pass Data To View
        private void PassDataToHome()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //passing data birthday to view
            int currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
            DateTime sunday = DateTime.Now.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);
            // If we started on Sunday, we should actually have gone *back*
            // 6 days instead of forward 1...
            if (currentDayOfWeek == 0)
            {
                monday = monday.AddDays(-7);
            }
            var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
            var lstBirthday = db.sp_BirthdayEmployeeGet(dates[0].ToString("MMdd"), dates[6].ToString("MMdd")).ToList();
            ViewBag.lstBirthday = lstBirthday;
            //Passing event list data to view
            int id_Lg = mFunction.ToInt(cookie.Value);
            var lstEventList = db.sp_EventList_Get(id_Lg).ToList();
            ViewBag.lstEventList = lstEventList;
            var objNews = db.sp_News_Get(id_Lg).FirstOrDefault(m => m.ID == 1);
            //var objNews = db.sp_News_Get(id_Lg);
            ViewBag.objNews = objNews;
            //Slider list
            List<string> lstSlider = db.sp_SliderImage().ToList();
            ViewBag.lstSlider = lstSlider;
            var objShortNews = db.sp_ShortNews_Get(id_Lg).ToList();
            ViewBag.objShortNews = objShortNews;
        }

        #endregion Home

        #region Event List

        //View Event List
        [HttpGet]
        public ActionResult EventList()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "EventList").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            PassDataToEventList();
            return View();
        }

        //Pass Data To View
        private void PassDataToEventList()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //Passing event list data to view
            var lstEventList = db.EventLists.OrderByDescending(m => m.EventDay).ToList();
            ViewBag.lstEventList = lstEventList;
        }

        //Add event list
        [HttpGet]
        public ActionResult EventListAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "EventList").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            PassDataToEventListAdd();
            return View();
        }

        [HttpPost]
        public ActionResult EventListAdd(FormCollection fc)
        {
            try
            {
                string strEventDay = mFunction.ToString(fc["EventDay"]);
                string strTitle1_EG = mFunction.ToString(fc["Title1_EG"]);
                string strTitle1_GR = mFunction.ToString(fc["Title1_GR"]);
                string strTitle1_VN = mFunction.ToString(fc["Title1_VN"]);
                string strTitle2_EG = mFunction.ToString(fc["Title2_EG"]);
                string strTitle2_GR = mFunction.ToString(fc["Title2_GR"]);
                string strTitle2_VN = mFunction.ToString(fc["Title2_VN"]);
                string strTitle3_EG = mFunction.ToString(fc["Title3_EG"]);
                string strTitle3_GR = mFunction.ToString(fc["Title3_GR"]);
                string strTitle3_VN = mFunction.ToString(fc["Title3_VN"]);

                //Check null data
                if (strEventDay.Trim().Length <= 0 ||
                    strTitle1_EG.Trim().Length <= 0 || strTitle1_GR.Trim().Length <= 0 || strTitle1_VN.Trim().Length <= 0 ||
                    strTitle2_EG.Trim().Length <= 0 || strTitle2_GR.Trim().Length <= 0 || strTitle2_VN.Trim().Length <= 0 ||
                    strTitle3_EG.Trim().Length <= 0 || strTitle3_GR.Trim().Length <= 0 || strTitle3_VN.Trim().Length <= 0)
                {
                    //Passing data to view
                    PassDataToEventListAdd();
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("EventDay") + ", " + Language.label("Title");
                    return View("EventListAdd");
                }

                DateTime oEventDay = DateTime.Parse(strEventDay);
                //prepare data
                EventList obj = new EventList()
                {
                    EventDay = oEventDay,
                    Title1_EG = strTitle1_EG,
                    Title1_GR = strTitle1_GR,
                    Title1_VN = strTitle1_VN,
                    Title2_EG = strTitle2_EG,
                    Title2_GR = strTitle2_GR,
                    Title2_VN = strTitle2_VN,
                    Title3_EG = strTitle3_EG,
                    Title3_GR = strTitle3_GR,
                    Title3_VN = strTitle3_VN,
                    TableName = "EventList",
                    Status = 1,
                    UserAdd = User.Identity.GetUserName(),
                    DateAdd = DateTime.Now
                };

                //Save data
                db.EventLists.Add(obj);
                db.SaveChanges();
                return RedirectToAction("EventList");
            }
            catch
            {
                PassDataToEventListAdd();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View();
            }
        }

        //Pass Data To View
        private void PassDataToEventListAdd()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //Passing event list data to view
            var DateNow = DateTime.Now;
            ViewBag.DateNow = DateNow;
        }

        // Delete MaterialType
        public ActionResult EventListDel(int id)
        {
            try
            {
                //Check permission user
                var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "EventList").ToList();
                if (lst_Check.Count <= 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                EventList obj = db.EventLists.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //Passing data to view
                    PassDataToEventList();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("EventList");
                }
                //Delete data
                db.EventLists.Remove(obj);
                db.SaveChanges();

                //Return view home
                return RedirectToAction("EventList");
            }
            catch
            {
                //Passing data to view
                PassDataToEventList();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("EventList");
            }
        }

        //Edit event list
        [HttpGet]
        public ActionResult EventListEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "EventList").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            EventList obj = db.EventLists.SingleOrDefault(m => m.ID == id);
            if (obj == null)
            {
                //Passing data to view
                PassDataToEventList();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("EventList");
            }
            PassDataToEventListEdit();
            return View(obj);
        }

        //Edit event list
        [HttpPost]
        public ActionResult EventListEdit(EventList obj)
        {
            try
            {
                EventList obj_check = db.EventLists.SingleOrDefault(m => m.ID == obj.ID);
                if (obj_check == null)
                {
                    //Passing data to view
                    PassDataToEventList();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("EventList");
                }

                string strEventDay = mFunction.ToString(obj.EventDay);
                string strTitle1_EG = mFunction.ToString(obj.Title1_EG);
                string strTitle1_GR = mFunction.ToString(obj.Title1_GR);
                string strTitle1_VN = mFunction.ToString(obj.Title1_VN);
                string strTitle2_EG = mFunction.ToString(obj.Title2_EG);
                string strTitle2_GR = mFunction.ToString(obj.Title2_GR);
                string strTitle2_VN = mFunction.ToString(obj.Title2_VN);
                string strTitle3_EG = mFunction.ToString(obj.Title3_EG);
                string strTitle3_GR = mFunction.ToString(obj.Title3_GR);
                string strTitle3_VN = mFunction.ToString(obj.Title3_VN);

                //Check null data
                if (strEventDay.Trim().Length <= 0 ||
                    strTitle1_EG.Trim().Length <= 0 || strTitle1_GR.Trim().Length <= 0 || strTitle1_VN.Trim().Length <= 0 ||
                    strTitle2_EG.Trim().Length <= 0 || strTitle2_GR.Trim().Length <= 0 || strTitle2_VN.Trim().Length <= 0 ||
                    strTitle3_EG.Trim().Length <= 0 || strTitle3_GR.Trim().Length <= 0 || strTitle3_VN.Trim().Length <= 0)
                {
                    //Passing data to view
                    PassDataToEventListAdd();
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("EventDay") + ", " + Language.label("Title");
                    return View(obj);
                }
                //update data
                DateTime oEventDay = DateTime.Parse(strEventDay);
                obj_check.EventDay = oEventDay;
                obj_check.Title1_EG = strTitle1_EG;
                obj_check.Title1_GR = strTitle1_GR;
                obj_check.Title1_VN = strTitle1_VN;
                obj_check.Title2_EG = strTitle2_EG;
                obj_check.Title2_GR = strTitle2_GR;
                obj_check.Title2_VN = strTitle2_VN;
                obj_check.Title3_EG = strTitle3_EG;
                obj_check.Title3_GR = strTitle3_GR;
                obj_check.Title3_VN = strTitle3_VN;
                obj_check.UserAdd = User.Identity.GetUserName();
                obj_check.DateUpd = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("EventList");
            }
            catch
            {
                //Passing data to view
                PassDataToEventListEdit();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }

        //Pass Data To View
        private void PassDataToEventListEdit()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //Passing event list data to view
            var DateNow = DateTime.Now;
            ViewBag.DateNow = DateNow;
        }

        #endregion Event List

        #region News

        //View News
        [HttpGet]
        public ActionResult News()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "News").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            PassDataToNews();
            return View();
        }

        //Pass Data To View
        private void PassDataToNews()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //Passing event list data to view
            var lstNews = db.News.ToList();
            ViewBag.lstNews = lstNews;
        }

        //Add News
        [HttpGet]
        public ActionResult NewsAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "News").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.txtTitle_EG = string.Empty;
            ViewBag.txtTitle_VN = string.Empty;
            ViewBag.txtTitle_GR = string.Empty;
            ViewBag.txtContent_EG = string.Empty;
            ViewBag.txtContent_VN = string.Empty;
            ViewBag.txtContent_GR = string.Empty;
            ViewBag.txtShortContent_EG = string.Empty;
            ViewBag.txtShortContent_VN = string.Empty;
            ViewBag.txtShortContent_GR = string.Empty;
            ViewBag.txtImageAddress = string.Empty;
            ViewBag.txtNewsType = string.Empty;
            PassDataToNews();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewsAdd(FormCollection fc, HttpPostedFileBase file)
        {
            string txtTitle_EG = mFunction.ToString(fc["txtTitle_EG"]);
            string txtTitle_VN = mFunction.ToString(fc["txtTitle_VN"]);
            string txtTitle_GR = mFunction.ToString(fc["txtTitle_GR"]);
            string txtContent_EG = mFunction.ToString(fc["txtContent_EG"]);
            string txtContent_VN = mFunction.ToString(fc["txtContent_VN"]);
            string txtContent_GR = mFunction.ToString(fc["txtContent_GR"]);
            string txtShortContent_EG = mFunction.ToString(fc["txtShortContent_EG"]);
            string txtShortContent_VN = mFunction.ToString(fc["txtShortContent_VN"]);
            string txtShortContent_GR = mFunction.ToString(fc["txtShortContent_GR"]);
            string txtImageAddress = mFunction.ToString(fc["txtImageAddress"]);
            int txtNewsType = mFunction.ToInt(fc["txtNewsType"]);
            string folder = @"/Content/adminlte/dist/img/news/";
            string fileUrl = string.Empty;

            try
            {
                //check null data
                if (txtTitle_EG.IsNullOrEmptyOrWhileSpace()
                    || txtTitle_VN.IsNullOrEmptyOrWhileSpace()
                    || txtTitle_GR.IsNullOrEmptyOrWhileSpace()
                    || txtContent_EG.IsNullOrEmptyOrWhileSpace()
                    || txtContent_VN.IsNullOrEmptyOrWhileSpace()
                    || txtContent_GR.IsNullOrEmptyOrWhileSpace()
                    || txtShortContent_EG.IsNullOrEmptyOrWhileSpace()
                    || txtShortContent_VN.IsNullOrEmptyOrWhileSpace()
                    || txtShortContent_GR.IsNullOrEmptyOrWhileSpace()
                    || txtNewsType == 0)
                {
                    ViewBag.txtTitle_EG = txtTitle_EG;
                    ViewBag.txtTitle_VN = txtTitle_VN;
                    ViewBag.txtTitle_GR = txtTitle_GR;
                    ViewBag.txtContent_EG = txtContent_EG;
                    ViewBag.txtContent_VN = txtContent_VN;
                    ViewBag.txtContent_GR = txtContent_GR;
                    ViewBag.txtShortContent_EG = txtShortContent_EG;
                    ViewBag.txtShortContent_VN = txtShortContent_VN;
                    ViewBag.txtShortContent_GR = txtShortContent_GR;
                    ViewBag.txtImageAddress = txtImageAddress;
                    ViewBag.txtNewsType = txtNewsType;
                    PassDataToNews();
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("Title") + ", " + Language.label("ShortContent") + ", " + Language.label("Content") + ", " + Language.label("ImageAddress") + ", " + Language.label("NewsType");
                    return View();
                }

                //Nếu có upload file mới thì lưu file mới
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath(folder), Path.GetFileName(file.FileName));
                    fileUrl = Path.GetFileName(file.FileName.Trim().ToLower());
                    file.SaveAs(path);
                    txtImageAddress = fileUrl.IsNullOrEmpty() ? "/Content/adminlte/dist/img/no_image.jpg" : folder + fileUrl;
                }
                //Ngược lại thì lấy url cũ
                else
                {
                    if (txtImageAddress != null)
                    {
                        txtImageAddress = txtImageAddress.ToString();
                    }
                    else
                    {
                        txtImageAddress = "/Content/adminlte/dist/img/no_image.jpg";
                    }
                }
                //save data
                News obj_save = new News()
                {
                    Title_EG = txtTitle_EG,
                    Title_VN = txtTitle_VN,
                    Title_GR = txtTitle_GR,
                    Content_EG = txtContent_EG,
                    Content_VN = txtContent_VN,
                    Content_GR = txtContent_GR,
                    ShortContent_EG = txtShortContent_EG,
                    ShortContent_VN = txtShortContent_VN,
                    ShortContent_GR = txtShortContent_GR,
                    Images = txtImageAddress,
                    NewsType = txtNewsType,
                    TableName = "News",
                    Status = 1,
                    UserAdd = User.Identity.GetUserName(),
                    DateAdd = DateTime.Now
                };



                db.News.Add(obj_save);
                db.SaveChanges();
                return RedirectToAction("News");
            }
            catch
            {
                ViewBag.txtTitle_EG = txtTitle_EG;
                ViewBag.txtTitle_VN = txtTitle_VN;
                ViewBag.txtTitle_GR = txtTitle_GR;
                ViewBag.txtContent_EG = txtContent_EG;
                ViewBag.txtContent_VN = txtContent_VN;
                ViewBag.txtContent_GR = txtContent_GR;
                ViewBag.txtShortContent_EG = txtShortContent_EG;
                ViewBag.txtShortContent_VN = txtShortContent_VN;
                ViewBag.txtShortContent_GR = txtShortContent_GR;
                ViewBag.txtImageAddress = txtImageAddress;
                ViewBag.txtNewsType = txtNewsType;
                PassDataToNews();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View();
            }
        }

        public ActionResult NewsDel(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "News").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            int key = mFunction.ToInt(id);
            try
            {
                var objNews = db.News.SingleOrDefault(m => m.ID == key);
                if (objNews == null)
                {
                    //Passing data to view
                    PassDataToNews();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("News");
                }
                db.News.Remove(objNews);
                db.SaveChanges();
                return RedirectToAction("News");
            }
            catch
            {
                PassDataToNews();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("News");
            }
        }

        //Update News
        [HttpGet]
        public ActionResult NewsUpd(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "News").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            int key = mFunction.ToInt(id);
            var objNews = db.News.FirstOrDefault(m => m.ID == key);
            if (objNews == null)
            {
                //Passing data to view
                PassDataToNews();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("News");
            }
            PassDataToNewsUpd();
            return View(objNews);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewsUpd(News objView, FormCollection fc, HttpPostedFileBase file)
        {
            string txtTitle_EG = mFunction.ToString(fc["txtTitle_EG"]);
            string txtTitle_VN = mFunction.ToString(fc["txtTitle_VN"]);
            string txtTitle_GR = mFunction.ToString(fc["txtTitle_GR"]);
            string txtContent_EG = mFunction.ToString(fc["txtContent_EG"]);
            string txtContent_VN = mFunction.ToString(fc["txtContent_VN"]);
            string txtContent_GR = mFunction.ToString(fc["txtContent_GR"]);
            string txtShortContent_EG = mFunction.ToString(fc["txtShortContent_EG"]);
            string txtShortContent_VN = mFunction.ToString(fc["txtShortContent_VN"]);
            string txtShortContent_GR = mFunction.ToString(fc["txtShortContent_GR"]);
            int txtNewsType = mFunction.ToInt(fc["txtNewsType"]);
            string txtImageAddress = mFunction.ToString(fc["txtImageAddress"]);
            string folder = @"/Content/adminlte/dist/img/news/";
            string fileUrl = string.Empty;
            try
            {
                //check Null Or Empty Or WhileSpace data
                if (txtTitle_EG.IsNullOrEmptyOrWhileSpace()
                    || txtTitle_VN.IsNullOrEmptyOrWhileSpace()
                    || txtTitle_GR.IsNullOrEmptyOrWhileSpace()
                    || txtContent_EG.IsNullOrEmptyOrWhileSpace()
                    || txtContent_VN.IsNullOrEmptyOrWhileSpace()
                    || txtContent_GR.IsNullOrEmptyOrWhileSpace()
                    || txtShortContent_EG.IsNullOrEmptyOrWhileSpace()
                    || txtShortContent_VN.IsNullOrEmptyOrWhileSpace()
                    || txtShortContent_GR.IsNullOrEmptyOrWhileSpace()
                    )
                {
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("Title") + ", " + Language.label("ShortContent") + ", " + Language.label("Content") + ", " + Language.label("ImageAddress") + ", " + Language.label("NewsType");
                    //Passing data to view
                    PassDataToNewsUpd();
                    return View(objView);
                }

                //get data by ID
                var obj_Save = db.News.FirstOrDefault(m => m.ID == objView.ID);

                if (obj_Save == null)
                {
                    //Passing data to view
                    PassDataToNewsUpd();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View(objView);
                }

                //Nếu có upload file mới thì lưu file mới
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath(folder), Path.GetFileName(file.FileName));
                    fileUrl = Path.GetFileName(file.FileName.Trim().ToLower());
                    file.SaveAs(path);
                    obj_Save.Images = fileUrl.IsNullOrEmpty() ? "/Content/adminlte/dist/img/no_image.jpg" : folder + fileUrl;
                }
                //Ngược lại thì lấy url cũ
                else
                {
                    if (txtImageAddress != null)
                    {
                        obj_Save.Images = txtImageAddress.ToString();
                    }
                    else
                    {
                        obj_Save.Images = "/Content/adminlte/dist/img/no_image.jpg";
                    }
                }

                //Update data
                obj_Save.Title_EG = txtTitle_EG;
                obj_Save.Title_VN = txtTitle_VN;
                obj_Save.Title_GR = txtTitle_GR;
                obj_Save.Content_EG = txtContent_EG;
                obj_Save.Content_VN = txtContent_VN;
                obj_Save.Content_GR = txtContent_GR;
                obj_Save.ShortContent_EG = txtShortContent_EG;
                obj_Save.ShortContent_VN = txtShortContent_VN;
                obj_Save.ShortContent_GR = txtShortContent_GR;
                obj_Save.NewsType = txtNewsType;
                obj_Save.UserUpd = User.Identity.GetUserName();
                obj_Save.DateUpd = DateTime.Now;
                db.SaveChanges();
                //reload view
                return RedirectToAction("News");
            }
            catch
            {
                PassDataToNewsUpd();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(objView);
            }
        }

        //Pass Data To View
        private void PassDataToNewsUpd()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        //View NewsDetail
        [HttpGet]
        public ActionResult NewsDetail(string id)
        {
            PassDataToNewsDetail();
            //Passing event list data to view
            int id_Lg = mFunction.ToInt(cookie.Value);
            int _key = mFunction.ToInt(id);
            var objNews = db.sp_NewsDetail_Get(id_Lg).SingleOrDefault(m => m.ID == _key);
            ViewBag.objNews = objNews;
            return View();
        }

        //Pass Data To View
        private void PassDataToNewsDetail()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        #endregion News

        #region Slider

        //View News
        [HttpGet]
        public ActionResult Slither()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Slither").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            PassDataToSlither();
            return View();
        }

        //Pass Data To View
        private void PassDataToSlither()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //Passing event list data to view
            List<String> lstSlider = db.sp_SliderImage().ToList();
            ViewBag.lstSlider = lstSlider;
        }

        //Update slither
        [HttpGet]
        public ActionResult SlitherUpd()
        {
            CheckCookie();
            ChangeLanguage(cookie.Value);

            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Slither").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //PassDataToSlitherUpd();
            return View();
        }

        //Update slither
        [HttpPost]
        public ActionResult SlitherUpd(HttpPostedFileBase file)
        {
            CheckCookie();
            ChangeLanguage(cookie.Value);

            //string txtSlider = mFunction.ToString(fc["txtSlider"]);
            string folder = @"/Content/adminlte/dist/img/Sliders/";
            string fileUrl = string.Empty;
            try
            {
                //Check null data
                if (file == null)
                {
                    //Passing data to view
                    //PassDataToSlitherUpd();
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("Image");
                    return View();
                }
                //Nếu có upload file mới thì lưu file mới
                else
                {
                    string path = Path.Combine(Server.MapPath(folder), Path.GetFileName(file.FileName));
                    fileUrl = Path.GetFileName(file.FileName.Trim().ToLower());
                    file.SaveAs(path);
                }

                Slider objNew = db.Sliders.FirstOrDefault(m => m.ID == 1);
                var str_Img = mFunction.ToString(objNew.Images);

                //Check is not exists in image
                if (str_Img.Contains(folder + fileUrl) == true)
                {
                    //Passing data to view
                    PassDataToSlitherUpd();
                    ViewBag.Message = "► " + Language.label("ExistsObject");
                    return View();
                }
                if (str_Img.Trim().IsNullOrEmptyOrWhileSpace())
                {
                    objNew.Images = folder + fileUrl;
                }
                else
                {
                    objNew.Images = str_Img + ";" + folder + fileUrl;
                }
                objNew.UserUpd = User.Identity.GetUserName();
                objNew.DateUpd = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Slither");
            }
            catch
            {
                ViewBag.Message = "► " + Language.label("ErrorSave");
                PassDataToSlitherUpd();
                return View();
            }
        }

        //Pass Data To View
        private void PassDataToSlitherUpd()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //get all file in folder image slider
            string searchFolder = @"/Content/adminlte/dist/img/Sliders/";
            DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
            FileInfo[] Images = di.GetFiles();
            List<string> filesFound = new List<string>();
            foreach (var item in Images)
            {
                filesFound.Add(@"/Content/adminlte/dist/img/Sliders/" + item.Name.ToString());
            }
            ViewBag.lstSlider = filesFound;
        }

        //Delete Image
        public ActionResult SlitherDel(string strimage)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Slither").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                Slider obj = db.Sliders.SingleOrDefault(m => m.ID == 1);
                var str_Image = obj.Images;
                if (str_Image.Contains(strimage) == false)
                {
                    //Passing data to view
                    PassDataToEventList();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("Slither");
                }
                //Delete data
                DataOperation.ExecuteNonQuery(GlobalVariable.DBMenuManager, "exec sp_SliderImageDel '" + strimage + "'");

                //Return view home
                return RedirectToAction("Slither");
            }
            catch
            {
                //Passing data to view
                PassDataToNews();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("Slither");
            }
        }

        // post: Upload File image
        [HttpPost]
        public ActionResult UploadFileImage(HttpPostedFileBase file)
        {
            string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName).ToLower();
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".if" || fileExtension == ".iff" || fileExtension == ".bmp" || fileExtension == ".svg")
            {
                string fileLocation = Server.MapPath(@"~/Content/adminlte/dist/img/Sliders/") + DateTime.Now.ToString("yyyyMMddHHmm") + "_" + Request.Files["file"].FileName;
                Request.Files["file"].SaveAs(fileLocation);
            }
            return RedirectToAction("SlitherUpd");
        }

        #endregion Slider

        #region Short news

        //Short news
        [HttpGet]
        public ActionResult ShortNews()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ShortNews").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            PassDataToShortNews();
            return View();
        }

        //Pass Data To View
        private void PassDataToShortNews()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            int id_Lg = mFunction.ToInt(cookie.Value);
            var lstShortNews = db.sp_ShortNews_Get(id_Lg).ToList();
            ViewBag.lstShortNews = lstShortNews;
        }

        //Update Short news
        [HttpGet]
        public ActionResult ShortNewsUpd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ShortNews").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            PassDataToShortNews();
            return View();
        }

        #endregion Short news

        #region Document upload
        //Get Document
        public ActionResult Document(int? page)
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            int pageNumber = (page ?? 1);
            ViewBag.Page = (pageNumber - 1) * PageConfig.pageSize;
            IPagedList<Document> data;
            data = db.Documents.OrderByDescending(x => x.ID).Skip(0).ToPagedList(pageNumber, PageConfig.pageSize);
            if (data.Count() > 0)
            {
                return View(data);
            }
            return View();
        }

        //Get Document Viewer
        [HttpGet]
        public ActionResult DocumentViewer(int Id)
        {
            var data = db.Documents.FirstOrDefault(x => x.ID == Id);
            if (data != null)
            {
                return PartialView("_DocumentViewer", data);
            }
            return RedirectToAction("Document", "Home");
        }

        //Get Document
        public ActionResult DocumentAdd(DocumentViewModels model, HttpPostedFileBase file)
        {
            string folder = @"/Content/Document/";
            string fileUrl = string.Empty;
            string exPdf = ".pdf";
            try
            {
                if (model.DocName.IsNullOrEmptyOrWhileSpace() || model.Icon.IsNullOrEmptyOrWhileSpace())
                {
                    ViewBag.Message = "► " + Language.label("FieldNotEmpty");
                    return Redirect("Document");
                }
                Document document = new Document();
                document.DocName = model.DocName;
                document.Icon = model.Icon;
                document.Description = model.Description;
                document.CreateBy = User.Identity.GetUserId();
                document.CreateTime = DateTime.Now;
                document.IsActive = model.IsActive;
                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath(folder), Path.GetFileName(DateTime.Now.Minute + "_" + file.FileName));
                    fileUrl = Path.GetFileName(DateTime.Now.Minute + "_" + file.FileName.Trim().ToLower());
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName).ToLower();
                    file.SaveAs(path);

                    //Check is file .doc or .docx
                    if (fileExtension == ".doc" || fileExtension == ".docx")
                    {
                        string pathPdf             = Path.Combine(Server.MapPath(folder), Path.GetFileNameWithoutExtension(DateTime.Now.Minute + "_" + file.FileName));
                        string filePdf             = Path.GetFileNameWithoutExtension(DateTime.Now.Minute + "_" + file.FileName.Trim().ToLower());
                        //Convert file .doc to file pdf
                        Aspose.Words.Document doc  = new Aspose.Words.Document(path);
                        PdfSaveOptions saveOptions = new PdfSaveOptions();
                        saveOptions.Compliance     = PdfCompliance.PdfA1b;
                        doc.Save(pathPdf + exPdf, saveOptions);
                        document.FileUrl           = folder + filePdf + exPdf;
                    }
                    else
                    {
                        document.FileUrl = folder + fileUrl;
                    }
                }
                db.Documents.Add(document);
                db.SaveChanges();
                ViewBag.Message = "► " + Language.label("CommandsCompletedSuccessfully");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "► " + Language.label("OopsSomethingWentWrong");
                ex.ToString();
            }
            return RedirectToAction("Document", "Home");
        }

        //Get Document
        [HttpGet]
        public ActionResult DocumentEdit(int? Id)
        {
            if (!Id.IsNullOrEmptyOrWhileSpace())
            {
                var data = db.Documents.AsQueryable().FirstOrDefault(x => x.ID == Id);
                if (data.IsNullOrEmptyOrWhileSpace())
                {
                    ViewBag.Message = "► " + Language.label("Empty");
                    return RedirectToRoute("Home", "Document");
                }
                return PartialView("_DocumentEdit", data);
            }
            return RedirectToAction("Document", "Home");
        }

        //Post Document Edit
        [HttpPost]
        public ActionResult DocumentEdit(DocumentViewModels model, HttpPostedFileBase file)
        {
            try
            {
                string folder = @"/Content/Document/";
                string fileUrl = string.Empty;
                string exPdf = ".pdf";
                //Check data input
                if (model.DocName.IsNullOrEmptyOrWhileSpace() || model.Icon.IsNullOrEmptyOrWhileSpace())
                {
                    ViewBag.Message = "► " + Language.label("FieldNotEmpty");
                    return Redirect("Document");
                }

                var data = db.Documents.FirstOrDefault(x => x.ID == model.ID);
                //Check data is exist
                if (data != null)
                {
                    data.DocName = model.DocName;
                    data.Icon = model.Icon;
                    data.Description = model.Description;
                    data.CreateBy = User.Identity.GetUserId();
                    data.CreateTime = DateTime.Now;
                    data.IsActive = model.IsActive;
                    //Check data file
                    if (file != null)
                    {
                        string path = Path.Combine(Server.MapPath(folder), Path.GetFileName(DateTime.Now.Minute + "_" + file.FileName));
                        fileUrl = Path.GetFileName(DateTime.Now.Minute + "_" + file.FileName.Trim().ToLower());
                        string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName).ToLower();
                        file.SaveAs(path);
                        //Check is file .doc or .docx
                        if (fileExtension == ".doc" || fileExtension == ".docx")
                        {
                            string pathPdf = Path.Combine(Server.MapPath(folder), Path.GetFileNameWithoutExtension(DateTime.Now.Minute + "_" + file.FileName));
                            string filePdf = Path.GetFileNameWithoutExtension(DateTime.Now.Minute + "_" + file.FileName.Trim().ToLower());
                            //Convert file .doc to file pdf
                            Aspose.Words.Document doc = new Aspose.Words.Document(path);
                            PdfSaveOptions saveOptions = new PdfSaveOptions();
                            saveOptions.Compliance = PdfCompliance.PdfA1b;
                            doc.Save(pathPdf + exPdf, saveOptions);
                            data.FileUrl = folder + filePdf + exPdf;
                        }
                        else
                        {
                            data.FileUrl = folder + fileUrl;
                        }
                    }
                }

                db.SaveChanges();
                ViewBag.Message = "► " + Language.label("CommandsCompletedSuccessfully");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "► " + Language.label("OopsSomethingWentWrong");
                ex.ToString();
            }
            return RedirectToAction("Document", "Home");
        }

        public ActionResult DocumentDel(int? Id)
        {
            var data = db.Documents.AsQueryable().FirstOrDefault(x => x.ID == Id);
            if (data.IsNullOrEmptyOrWhileSpace())
            {
                ViewBag.Message = "► " + Language.label("Empty");
                return RedirectToAction("Document", "Home");
            }
            try
            {
                db.Documents.Remove(data);
                db.SaveChanges();
                ViewBag.Message = "► " + Language.label("CommandsCompletedSuccessfully");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "► " + Language.label("OopsSomethingWentWrong");
                ex.ToString();
            }
            return RedirectToAction("Document", "Home");
        }
        #endregion

        #region Cookie

        //Check cookie
        public void CheckCookie()
        {
            cookie = Request.Cookies["Language"];
            if (cookie == null || cookie.Value == string.Empty)
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

        #endregion Cookie

        #region Change language

        ////change language Sơn không dùng cách này
        //[HttpGet]
        //public ActionResult English()
        //{
        //    Response.Cookies["Language"].Value = "1";
        //    return RedirectToAction("Index");
        //}

        ////change language
        //[HttpGet]
        //public ActionResult VietNam()
        //{
        //    Response.Cookies["Language"].Value = "2";
        //    return RedirectToAction("Index");
        //}

        ////change language
        //[HttpGet]
        //public ActionResult Germany()
        //{
        //    Response.Cookies["Language"].Value = "3";
        //    return RedirectToAction("Index");
        //}

        // Sơn dùng cách này
        public ActionResult ChangeLang(string lang)
        {
            if (lang != null)
            {
                Response.Cookies["Language"].Value = lang;
            }
            HttpCookie cookie = new HttpCookie("Lang");
            cookie.Value = lang;
            Response.Cookies.Add(cookie);
            return Redirect(Request.UrlReferrer.ToString());
        }

        //change language Finger
        [HttpGet]
        public ActionResult English_FingerM1()
        {
            Response.Cookies["Language"].Value = "1";
            return RedirectToAction("MealOrderFingerM1", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult VietNam_FingerM1()
        {
            Response.Cookies["Language"].Value = "2";
            return RedirectToAction("MealOrderFingerM1", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult Germany_FingerM1()
        {
            Response.Cookies["Language"].Value = "3";
            return RedirectToAction("MealOrderFingerM1", "MealControl");
        }

        //change language Finger
        [HttpGet]
        public ActionResult English_FingerM2()
        {
            Response.Cookies["Language"].Value = "1";
            return RedirectToAction("MealOrderFingerM2", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult VietNam_FingerM2()
        {
            Response.Cookies["Language"].Value = "2";
            return RedirectToAction("MealOrderFingerM2", "MealControl");
        }

        //change language
        [HttpGet]
        public ActionResult Germany_FingerM2()
        {
            Response.Cookies["Language"].Value = "3";
            return RedirectToAction("MealOrderFingerM2", "MealControl");
        }

        #endregion Change language

    }
}