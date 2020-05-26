using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using FramasVietNam.Models.Meal;
using FramasVietNam.Models.FingerScan;
using FramasVietNam.Models.HumanResource;
using System.IO;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class MealControlCanteenController : Controller
    {

        /// //////////////////////////////////////////////////////////////////
        #region global variable
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        //Add db meal menu
        private MealControlEntities db_Meal = new MealControlEntities();
        //Add db HR menu
        private HumanResourceEntities db_Hr = new HumanResourceEntities();
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region menu meal
        // GET: upload image to meal function
        public ActionResult MealUploadImage()
        {
            //passing data to view
            PassingDataToMealUploadImage();
            return View();
        }
        //passing data to view
        private void PassingDataToMealUploadImage()
        {
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get all file in folder image slider
            string searchFolder = @"/Content/adminlte/dist/img/meals/";
            DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
            FileInfo[] Images = di.GetFiles();
            List<string> filesFound = new List<string>();
            foreach (var item in Images)
            {
                filesFound.Add(searchFolder + item.Name.ToString());
            }
            ViewBag.lstSlider = filesFound;
        }
        // post: Upload File image
        [HttpPost]
        public ActionResult MealUploadImageAdd(HttpPostedFileBase file)
        {
            try
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName).ToLower();
                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".if" || fileExtension == ".iff" || fileExtension == ".bmp" || fileExtension == ".svg")
                {
                    string fileLocation = Server.MapPath(@"/Content/adminlte/dist/img/meals/") + Request.Files["file"].FileName;
                    Request.Files["file"].SaveAs(fileLocation);
                }
                return RedirectToAction("MealUploadImage");
            }
            catch
            {
                //passing data to view
                PassingDataToMealUploadImage();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("MealUploadImage");
            }
        }

        // GET: MealMenuItem
        public ActionResult MenuItem()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenuItem").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //pass data to menu
            object ds_MenuItem = db_Meal.MenuMeals.ToList();
            object ds_MenuItemsDetails = db_Meal.MenuMealDetails.ToList();
            ViewBag.ds_MenuItem = ds_MenuItem;
            ViewBag.ds_MenuItemsDetails = ds_MenuItemsDetails;
            return View();
        }

        // Add new menu item
        [HttpGet]
        public ActionResult AddItem()
        {

            //get all file in folder image slider
            string searchFolder = @"/Content/adminlte/dist/img/meals/";
            DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
            FileInfo[] Images = di.GetFiles();
            List<string> filesFound = new List<string>();
            foreach (var item in Images)
            {
                filesFound.Add(searchFolder + item.Name.ToString());
            }
            ViewBag.lstSlider = filesFound;

            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenuItem").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //Pass ID, code
            var lstMenu = db_Meal.MenuMeals.ToList();
            int key = 1;
            if (lstMenu.Count > 0)
            {
                key = mFunction.ToInt(lstMenu.Select(i => i.ID).Max()) + 1;
            }

            //Get list/material
            var lstMaterial = db_Meal.Materials.ToList();
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            foreach (var i in lstMaterial)
            {
                if (i.Status == 1)
                {
                    obj.Add(new CheckBoxModel { Value = i.ID, Code = i.Code, Name = i.Name, IsChecked = false });
                }
            }
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            ViewBag.id = key.ToString();
            return View(objBind);
        }
        [HttpPost]
        public ActionResult AddItem(CheckBoxList Obj, FormCollection fc)
        {
            int key = mFunction.ToInt(fc["ID"]);
            //create data parent
            MenuMeal objMenu = new MenuMeal
            {
                ID          = key,
                Code        = fc["Code"],
                Name        = fc["Name"],
                Description = fc["Description"],
                Image       = fc["Image"],
                TableName   = "MenuMeal",
                Status      = 1,
                PCIDAdd     = Request.Url.Host,
                UserAdd     = User.Identity.GetUserName().ToString(),
                DateAdd     = DateTime.Now
            };
            db_Meal.MenuMeals.Add(objMenu);

            //create data children=
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    MenuMealDetail obj_Child = new MenuMealDetail
                    {
                        ID = key,
                        MaterialID = mFunction.ToInt(item.Value),
                        MaterialCode = mFunction.ToString(item.Code),
                        MaterialName = mFunction.ToString(item.Name),
                        TableName = "MenuMealDetail",
                        Status = 1
                    };
                    db_Meal.MenuMealDetails.Add(obj_Child);
                }
            }
            bool isError = false;
            //Check exists item in children
            if (Obj.CheckBox.Where(m => m.IsChecked == true).ToList().Count == 0)
            {
                isError = true;
                ViewBag.Message = "► " + Language.label("PleaseChooseItem");
            }
            //Check ID not exists in DB
            var existsId = db_Meal.MenuMeals.Where(i => i.ID == objMenu.ID);
            if (existsId.ToList().Count > 0)
            {
                isError = true;
                ViewBag.Message = "► " + Language.label("ExistsObject");
            }
            //Check code not exists in DB
            var existsCode = db_Meal.MenuMeals.Where(i => i.Code == objMenu.Code);
            if (existsCode.ToList().Count > 0)
            {
                isError = true;
                ViewBag.Message = "► " + Language.label("ExistsObject");
            }
            //Check error
            if (isError == true)
            {

                //get all file in folder image slider
                string searchFolder = @"/Content/adminlte/dist/img/meals/";
                DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
                FileInfo[] Images = di.GetFiles();
                List<string> filesFound = new List<string>();
                foreach (var item in Images)
                {
                    filesFound.Add(searchFolder + item.Name.ToString());
                }
                ViewBag.lstSlider = filesFound;

                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                CheckCookie();
                ChangeLanguage(mFunction.ToString(cookie.Value));

                ViewBag.id = key.ToString();
                return View(Obj);
            }
            //Insert data to Database
            try
            {
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MenuItem", "MealControlCanteen", "Insert", "Add New Item", "", User.Identity.GetUserName(), DateTime.Now);
            }
            catch (Exception ex)
            {
                //get all file in folder image slider
                string searchFolder = @"/Content/adminlte/dist/img/meals/";
                DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
                FileInfo[] Images = di.GetFiles();
                List<string> filesFound = new List<string>();
                foreach (var item in Images)
                {
                    filesFound.Add(searchFolder + item.Name.ToString());
                }
                ViewBag.lstSlider = filesFound;

                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                CheckCookie();
                ChangeLanguage(mFunction.ToString(cookie.Value));

                ex.ToString();
                ViewBag.id = key.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(Obj);
            }
            return RedirectToAction("MenuItem");
        }

        //Delete Menu item
        public ActionResult DelItem(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenuItem").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //delete data from Database
            List<MenuMealDetail> lst_Detail = db_Meal.MenuMealDetails.Where(m => m.ID.Equals(id)).ToList();
            foreach (var item in lst_Detail)
            {
                db_Meal.MenuMealDetails.Remove(item);
            }
            MenuMeal mn = db_Meal.MenuMeals.FirstOrDefault(m => m.ID.Equals(id));
            db_Meal.MenuMeals.Remove(mn);
            try
            {
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MenuItem", "MealControlCanteen", "Delete", "Del Item", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("MenuItem");
            }
            return RedirectToAction("MenuItem");
        }

        //Edit Menu item
        [HttpGet]
        public ActionResult EditItem(int id)
        {
            //get all file in folder image slider
            string searchFolder = @"/Content/adminlte/dist/img/meals/";
            DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
            FileInfo[] Images = di.GetFiles();
            List<string> filesFound = new List<string>();
            foreach (var item in Images)
            {
                filesFound.Add(searchFolder + item.Name.ToString());
            }
            ViewBag.lstSlider = filesFound;

            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenuItem").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            MenuMeal lstMenuItem = db_Meal.MenuMeals.SingleOrDefault(n => n.ID == id);
            if (lstMenuItem == null)
            {
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("MenuItem");
            }

            //pass data lstMenuItem to view
            ViewBag.lstMenuItem = lstMenuItem;

            //Get list/material
            var lstMaterial = db_Meal.Materials.ToList();
            var lstMaterial_DB = db_Meal.MenuMealDetails.Where(m => m.ID == id).ToList();
            //Add Records material in db 
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            foreach (var i in lstMaterial)
            {
                if (i.Status == 1)
                {
                    if (lstMaterial_DB.Where(k => (k.MaterialID == i.ID)).ToList().Count > 0)
                    {
                        obj.Add(new CheckBoxModel { Value = i.ID, Code = i.Code, Name = i.Name, IsChecked = true });
                    }
                    else
                    {
                        obj.Add(new CheckBoxModel { Value = i.ID, Code = i.Code, Name = i.Name, IsChecked = false });
                    }
                }
            }
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            return View(objBind);
        }
        //[HttpPost]
        //public ActionResult EditItem(CheckBoxList Obj, FormCollection fc)
        //{
        //    int key = mFunction.ToInt(fc["ID"]);
        //    //Edit data parent
        //    MenuMeal objMenu = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == key);
        //    objMenu.Name = fc["Name"];
        //    objMenu.Description = fc["Description"];
        //    objMenu.Image = fc["Image"];
        //    objMenu.PCIDUpd = Request.Url.Host;
        //    objMenu.UserUpd = User.Identity.GetUserName().ToString();
        //    objMenu.DateUpd = DateTime.Now;

        //    //edit data children
        //    //Delete
        //    List<MenuMealDetail> lst_Detail = db_Meal.MenuMealDetails.Where(m => m.ID.Equals(key)).ToList();
        //    foreach (var item in lst_Detail)
        //    {
        //        db_Meal.MenuMealDetails.Remove(item);
        //    }

        //    //insert
        //    foreach (var item in Obj.CheckBox)
        //    {
        //        if (item.IsChecked == true)
        //        {
        //            MenuMealDetail obj_Child = new MenuMealDetail
        //            {
        //                ID = key,
        //                MaterialID = mFunction.ToInt(item.Value),
        //                MaterialCode = mFunction.ToString(item.Code),
        //                MaterialName = mFunction.ToString(item.Name),
        //                TableName = "MenuItemsDetail",
        //                Status = 1
        //            };
        //            db_Meal.MenuMealDetails.Add(obj_Child);
        //        }
        //    }
        //    //Check exists item in children
        //    if (Obj.CheckBox.Where(m => m.IsChecked == true).ToList().Count == 0)
        //    {
        //        //get all file in folder image slider
        //        string searchFolder = @"~/Content/adminlte/dist/img/meals/";
        //        DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
        //        FileInfo[] Images = di.GetFiles();
        //        List<string> filesFound = new List<string>();
        //        foreach (var item in Images)
        //        {
        //            filesFound.Add(@"~/Content/adminlte/dist/img/meals/" + item.Name.ToString());
        //        }
        //        ViewBag.lstSlider = filesFound;

        //        // CheckCookie and ChangeLanguage always have in action (it pass data to View)
        //        CheckCookie();
        //        ChangeLanguage(mFunction.ToString(cookie.Value));

        //        //get list menu item in db
        //        MenuMeal lstMenuItem = db_Meal.MenuMeals.SingleOrDefault(n => n.ID == key);
        //        if (lstMenuItem == null)
        //        {
        //            ViewBag.Message = "► " + Language.label("NotExistsObject");
        //            return View("MenuItem");
        //        }

        //        //pass data lstMenuItem to view
        //        ViewBag.lstMenuItem = lstMenuItem;

        //        ViewBag.Message = "► " + Language.label("PleaseChooseItem");
        //        ViewBag.id = key.ToString();
        //        return View(Obj);
        //    }
        //    //execute data to Database
        //    try
        //    {
        //        db_Meal.SaveChanges();
        //        //Save log
        //        var objLog = db_Meal.sp_ActionLog("MenuItem", "MealControlCanteen", "Update", "Update Item", "ID: " + key.ToString(), User.Identity.GetUserName(), DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        //get all file in folder image slider
        //        string searchFolder = @"~/Content/adminlte/dist/img/meals/";
        //        DirectoryInfo di = new DirectoryInfo(Server.MapPath(searchFolder));
        //        FileInfo[] Images = di.GetFiles();
        //        List<string> filesFound = new List<string>();
        //        foreach (var item in Images)
        //        {
        //            filesFound.Add(@"~/Content/adminlte/dist/img/meals/" + item.Name.ToString());
        //        }
        //        ViewBag.lstSlider = filesFound;

        //        // CheckCookie and ChangeLanguage always have in action (it pass data to View)
        //        CheckCookie();
        //        ChangeLanguage(mFunction.ToString(cookie.Value));

        //        ex.ToString();
        //        ViewBag.id = key.ToString();
        //        ViewBag.Message = "► " + Language.label("ErrorSave");
        //        //get list menu item in db
        //        MenuMeal lstMenuItem = db_Meal.MenuMeals.SingleOrDefault(n => n.ID == key);
        //        if (lstMenuItem == null)
        //        {
        //            ViewBag.Message = "► " + Language.label("NotExistsObject");
        //            return View("MenuItem");
        //        }

        //        //pass data lstMenuItem to view
        //        ViewBag.lstMenuItem = lstMenuItem;
        //        return View(Obj);
        //    }
        //    return RedirectToAction("MenuItem");
        //}

        //Edit menu have upload image
        [HttpPost]
        public ActionResult EditItem(CheckBoxList Obj, FormCollection fc , Avatar avatar, HttpPostedFileBase file)
        {
            int key                 = mFunction.ToInt(fc["ID"]);
            string fileExtension    = string.Empty;
            string fileUrl          = string.Empty;
            string folder           = @"/Content/adminlte/dist/img/meals/";
            DirectoryInfo di        = new DirectoryInfo(Server.MapPath(folder));
            FileInfo[] Images       = di.GetFiles();
            List<string> filesFound = new List<string>();

            //Edit data parent
            MenuMeal objMenu = db_Meal.MenuMeals.FirstOrDefault(m => m.ID == key);
            objMenu.Name = fc["Name"];
            objMenu.Description = fc["Description"];
            
            objMenu.PCIDUpd = Request.Url.Host;
            objMenu.UserUpd = User.Identity.GetUserName().ToString();
            objMenu.DateUpd = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    //Nếu có upload file mới thì lưu file mới
                    if (file != null)
                    {
                        string path = Path.Combine(Server.MapPath(folder), Path.GetFileName(file.FileName));
                        fileUrl = Path.GetFileName(file.FileName.Trim().ToLower());
                        file.SaveAs(path);
                        objMenu.Image = fileUrl.IsNullOrEmpty() ? "/Content/adminlte/dist/img/no_image.jpg" : folder + fileUrl;
                    }
                    //Ngược lại thì lấy url cũ
                    else
                    {
                        if (avatar.input_Avatar != null)
                        {
                            objMenu.Image = avatar.input_Avatar.ToString();
                        }
                        else
                        {
                            objMenu.Image = "/Content/adminlte/dist/img/no_image.jpg";
                        }
                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "► " + Language.label("ErrorSave");
                }
            }


            //Edit data children
            //Delete
            List<MenuMealDetail> lst_Detail = db_Meal.MenuMealDetails.Where(m => m.ID.Equals(key)).ToList();
            foreach (var item in lst_Detail)
            {
                db_Meal.MenuMealDetails.Remove(item);
            }

            //insert
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    MenuMealDetail obj_Child = new MenuMealDetail
                    {
                        ID = key,
                        MaterialID = mFunction.ToInt(item.Value),
                        MaterialCode = mFunction.ToString(item.Code),
                        MaterialName = mFunction.ToString(item.Name),
                        TableName = "MenuItemsDetail",
                        Status = 1
                    };
                    db_Meal.MenuMealDetails.Add(obj_Child);
                }
            }
            //Check exists item in children
            if (Obj.CheckBox.Where(m => m.IsChecked == true).ToList().Count == 0)
            {
                //get all file in folder image slider
                foreach (var item in Images)
                {
                    filesFound.Add(folder + item.Name.ToString());
                }
                ViewBag.lstSlider = filesFound;

                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                CheckCookie();
                ChangeLanguage(mFunction.ToString(cookie.Value));

                //get list menu item in db
                MenuMeal lstMenuItem = db_Meal.MenuMeals.FirstOrDefault(n => n.ID == key);
                if (lstMenuItem == null)
                {
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("MenuItem");
                }

                //pass data lstMenuItem to view
                ViewBag.lstMenuItem = lstMenuItem;

                ViewBag.Message = "► " + Language.label("PleaseChooseItem");
                ViewBag.id = key.ToString();
                return View(Obj);
            }
            //execute data to Database
            try
            {
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MenuItem", "MealControlCanteen", "Update", "Update Item", "ID: " + key.ToString(), User.Identity.GetUserName(), DateTime.Now);
            }
            catch (Exception ex)
            {
                //get all file in folder image slider
                foreach (var item in Images)
                {
                    filesFound.Add(folder + item.Name.ToString());
                }
                ViewBag.lstSlider = filesFound;

                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                CheckCookie();
                ChangeLanguage(mFunction.ToString(cookie.Value));

                ex.ToString();
                ViewBag.id = key.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                //get list menu item in db
                MenuMeal lstMenuItem = db_Meal.MenuMeals.FirstOrDefault(n => n.ID == key);
                if (lstMenuItem == null)
                {
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("MenuItem");
                }

                //pass data lstMenuItem to view
                ViewBag.lstMenuItem = lstMenuItem;
                return View(Obj);
            }
            return RedirectToAction("MenuItem");
        }


        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region menu meal schedule
        // GET: MealMenuItem
        public ActionResult MenusChedule()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenusChedule").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //pass data to menu
            PassData(DateTime.Now);

            return View();
        }

        //pass data to menu
        private void PassData(DateTime today)
        {
            int currentDayOfWeek = (int)today.DayOfWeek;
            DateTime sunday = today.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);
            // If we started on Sunday, we should actually have gone *back*
            // 6 days instead of forward 1...
            if (currentDayOfWeek == 0)
            {
                monday = monday.AddDays(-7);
            }
            var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
            ViewBag.dates = dates;
            ViewBag.date_str = today.ToString("yyyy-MM-dd");

            //get data meal schedule
            List<sp_MealSchedule_GetByDay_Result> lst_mealSchedule = db_Meal.sp_MealSchedule_GetByDay(dates[0].ToString("yyyyMMdd"), dates[6].ToString("yyyyMMdd")).ToList();
            ViewBag.lst_mealSchedule = lst_mealSchedule;
        }

        public ActionResult MenusChedulePre(string dt)
        {
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //pass data to menu
            DateTime today = mFunction.ToDateTime(dt).AddDays(-7);
            PassData(today);

            return View("MenusChedule");
        }

        public ActionResult MenusCheduleNext(string dt)
        {
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //pass data to menu

            DateTime today = mFunction.ToDateTime(dt).AddDays(+7);
            PassData(today);

            return View("MenusChedule");
        }

        [HttpPost]
        public ActionResult MenusCheduleSearch(FormCollection fc)
        {
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //pass data to menu
            var dt = mFunction.ToString(fc["dateSearch"]);
            DateTime today = mFunction.ToDateTime(dt);
            PassData(today);

            return View("MenusChedule");
        }

        //Edit Menu item
        public ActionResult MenusCheduleEdit(string strdate)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenusChedule").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get data meal schedule and passing data to view
            DateTime date = mFunction.ToDateTime(strdate);
            List<sp_MealSchedule_GetByDay_Result> lst_mealSchedule = db_Meal.sp_MealSchedule_GetByDay(date.ToString("yyyyMMdd"), date.ToString("yyyyMMdd")).ToList();
            ViewBag.lst_mealSchedule = lst_mealSchedule;
            ViewBag.date_str = strdate;

            return View();
        }

        //Edit Menu item
        [HttpGet]
        public ActionResult MenusCheduleAdd(string strdate)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenusChedule").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get data meal schedule and passing data to view
            ViewBag.date_str = strdate;

            //passing list meal and shift to view
            List<ShiftWork> lstShift = db_Meal.ShiftWorks.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
            List<MenuMeal> lstMeal = db_Meal.MenuMeals.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
            ViewBag.lstShift = lstShift;
            ViewBag.lstMeal = lstMeal;
            return View();
        }

        //Edit Menu item
        [HttpPost]
        public ActionResult MenusCheduleAdd(FormCollection fc)
        {
            string date_str = fc["dtDate"].ToString();
            if (date_str.Length <= 0)
            {
                ViewBag.Message = Language.label("PleaseTryAgain");

                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));

                //get data meal schedule and passing data to view
                ViewBag.date_str = date_str;

                //passing list meal and shift to view
                List<ShiftWork> lstShift = db_Meal.ShiftWorks.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
                List<MenuMeal> lstMeal = db_Meal.MenuMeals.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
                ViewBag.lstShift = lstShift;
                ViewBag.lstMeal = lstMeal;
                return View();
            }

            DateTime date = mFunction.ToDateTime(fc["dtDate"]);
            string shift = mFunction.ToString(fc["ShiftID"]);
            string meal = mFunction.ToString(fc["MealID"]);

            ShiftWork objShift = db_Meal.ShiftWorks.SingleOrDefault(m => m.Name == shift);
            MenuMeal objMeal = db_Meal.MenuMeals.SingleOrDefault(m => m.Name == meal);

            //check data null
            if (objShift == null || objMeal == null)
            {
                ViewBag.Message = Language.label("MealInfoOrShiftInfoIncorrect");

                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));

                //get data meal schedule and passing data to view
                ViewBag.date_str = date_str;

                //passing list meal and shift to view
                List<ShiftWork> lstShift = db_Meal.ShiftWorks.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
                List<MenuMeal> lstMeal = db_Meal.MenuMeals.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
                ViewBag.lstShift = lstShift;
                ViewBag.lstMeal = lstMeal;
                return View();
            }
            else
            {
                string _day = date.ToString("yyyyMMdd");
                int _shiftID = objShift.ID;
                int _MealID = objMeal.ID;

                //check data is not exist in table meal schedule
                MealSchedule notNull = db_Meal.MealSchedules.SingleOrDefault(m => (m.Day == _day) && (m.ShiftWorkID == _shiftID) && (m.MenuMealID == _MealID));
                if (notNull != null)
                {
                    ViewBag.Message = Language.label("DuplicateData");
                }
                //Save data to meal schedule
                else
                {
                    MealSchedule objSave = new MealSchedule()
                    {
                        Day = _day,
                        ShiftWorkID = _shiftID,
                        ShiftWorkCode = objShift.Code,
                        MenuMealID = _MealID,
                        MenuMealCode = objMeal.Code,
                        TableName = "MealSchedule",
                        Status = 1,
                        UserAdd = User.Identity.GetUserName(),
                        DateAdd = DateTime.Now
                    };
                    //Save log
                    var objLog = db_Meal.sp_ActionLog("MenusChedule", "MealControlCanteen", "Insert", "Add New MenusChedule", "", User.Identity.GetUserName(), DateTime.Now);

                    db_Meal.MealSchedules.Add(objSave);
                    db_Meal.SaveChanges();
                    DataOperation.ExecuteNonQuery(GlobalVariable.DBMealControl, "exec sp_InsertAutoNoodles '" + _day + "','" + _shiftID.ToString() + "'");
                }
            }

            return RedirectToAction("MenusCheduleEdit", new { strdate = date_str });
        }

        //delete Menu item
        public ActionResult MenusCheduleDel(string day, int shift, int Meal)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MenusChedule").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            DateTime date = mFunction.ToDateTime(day);
            string _day = date.ToString("yyyyMMdd");

            //check data 
            MealSchedule notNull = db_Meal.MealSchedules.SingleOrDefault(m => (m.Day == _day) && (m.ShiftWorkID == shift) && (m.MenuMealID == Meal));
            if (notNull == null)
            {
                ViewBag.Message = Language.label("NotExistsObject");
            }
            //delete data
            else
            {
                db_Meal.MealSchedules.Remove(notNull);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MenusChedule", "MealControlCanteen", "Delete", "Delete MenusChedule", "", User.Identity.GetUserName(), DateTime.Now);
            }

            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get data meal schedule and passing data to view
            List<sp_MealSchedule_GetByDay_Result> lst_mealSchedule = db_Meal.sp_MealSchedule_GetByDay(date.ToString("yyyyMMdd"), date.ToString("yyyyMMdd")).ToList();
            ViewBag.lst_mealSchedule = lst_mealSchedule;
            ViewBag.date_str = day;

            //passing list meal and shift to view
            List<ShiftWork> lstShift = db_Meal.ShiftWorks.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
            List<MenuMeal> lstMeal = db_Meal.MenuMeals.Where(m => m.Status == 1).OrderBy(o => o.Code).ToList();
            ViewBag.lstShift = lstShift;
            ViewBag.lstMeal = lstMeal;

            return View("MenusCheduleEdit");
        }

        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Material type

        // GET: MaterialType
        public ActionResult MaterialType()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MaterialType").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass date to view
            SendDataMaterialType();
            return View();
        }

        //pass date to view
        private void SendDataMaterialType()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var lstMaterialType = db_Meal.MaterialTypes.Where(m => m.Status == 1).ToList();
            ViewBag.lstMaterialType = lstMaterialType;
        }

        // Delete MaterialType
        public ActionResult MaterialTypeDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MaterialType").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                MaterialType obj = db_Meal.MaterialTypes.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //pass date to view
                    SendDataMaterialType();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("MaterialType");
                }
                db_Meal.MaterialTypes.Remove(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MaterialType", "MealControlCanteen", "Delete", "Del MaterialType", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("MaterialType");
            }
            catch
            {
                //pass date to view
                SendDataMaterialType();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("MaterialType");
            }
        }

        //Edit MaterialType
        [HttpGet]
        public ActionResult MaterialTypeEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MaterialType").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //get list menu item in db
            MaterialType lstMaterialType = db_Meal.MaterialTypes.SingleOrDefault(n => n.ID == id);
            if (lstMaterialType == null)
            {
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("MaterialType");
            }
            //pass data lstMenuItem to view
            return View(lstMaterialType);
        }
        [HttpPost]
        public ActionResult MaterialTypeEdit(MaterialType obj)
        {
            try
            {
                if (mFunction.ToString(obj.Code) == "" || mFunction.ToString(obj.Name) == "")
                {
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    return View(obj);
                }
                MaterialType objMaterialType = db_Meal.MaterialTypes.SingleOrDefault(m => m.ID == obj.ID);
                objMaterialType.Code = obj.Code;
                objMaterialType.Name = obj.Name;
                objMaterialType.Description = obj.Description;
                objMaterialType.PCIDUpd = mFunction.ToString(User.Identity.GetUserName());
                objMaterialType.DateUpd = DateTime.Now;
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MaterialType", "MealControlCanteen", "Update", "Update MaterialType", "", User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("MaterialType");
            }
            catch
            {
                //
                CheckCookie();
                // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                ChangeLanguage(mFunction.ToString(cookie.Value));
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }

        //Edit MaterialType
        [HttpGet]
        public ActionResult MaterialTypeAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MaterialType").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));
            //Go to view
            return View();
        }

        [HttpPost]
        public ActionResult MaterialTypeAdd(FormCollection fc)
        {
            try
            {
                var a = mFunction.ToString(fc["Code"]);
                var b = mFunction.ToString(fc["Name"]);
                var c = mFunction.ToString(fc["Description"]);
                if (a == "" || b == "")
                {
                    //pass date to view
                    SendDataMaterialType();
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name");
                    return View("");
                }
                if (db_Meal.MaterialTypes.Where(m => m.Code == a).ToList().Count > 0)
                {
                    //pass date to view
                    SendDataMaterialType();
                    ViewBag.Message = "► " + Language.label("ExistsObject");
                    return View("");
                }
                MaterialType obj = new MaterialType
                {
                    Code = a,
                    Name = b,
                    Description = c,
                    TableName = "MaterialType",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db_Meal.MaterialTypes.Add(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MaterialType", "MealControlCanteen", "Insert", "Add New MaterialType", "", User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("MaterialType");
            }
            catch
            {
                //pass date to view
                SendDataMaterialType();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View();
            }
        }
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Material

        // GET: Material
        public ActionResult Material()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Material").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass date to view
            SendDataMaterial();
            return View();
        }

        //pass date to view
        private void SendDataMaterial()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var lstMaterial = db_Meal.Materials.Where(m => m.Status == 1).ToList();
            ViewBag.lstMaterial = lstMaterial;
        }

        // Delete Material
        public ActionResult MaterialDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Material").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                Material obj = db_Meal.Materials.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //pass date to view
                    SendDataMaterial();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("Material");
                }
                db_Meal.Materials.Remove(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("Material", "MealControlCanteen", "Delete", "Del Material", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("Material");
            }
            catch
            {
                //pass date to view
                SendDataMaterial();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("Material");
            }
        }

        //Edit Material
        [HttpGet]
        public ActionResult MaterialEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Material").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //get list menu item in db
            Material lstMaterial = db_Meal.Materials.SingleOrDefault(n => n.ID == id);
            if (lstMaterial == null)
            {
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("Material");
            }

            //pass data to view
            MaterialAddPassData();

            int idtype = mFunction.ToInt(lstMaterial.MaterialTypeID);
            MaterialType obj = db_Meal.MaterialTypes.SingleOrDefault(m => m.ID == idtype);
            ViewBag.strMaterialTypes = mFunction.ToString(obj.Name);

            return View(lstMaterial);
        }
        [HttpPost]
        public ActionResult MaterialEdit(Material obj, FormCollection fc)
        {
            try
            {
                if (mFunction.ToString(obj.Code) == "" || mFunction.ToString(obj.Name) == "")
                {
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name") + "," + Language.label("MaterialType");
                    //pass data to view
                    MaterialAddPassData();
                    return View(obj);
                }

                string strMaterialTypes = mFunction.ToString(fc["MaterialType"]);
                MaterialType obj_type = db_Meal.MaterialTypes.SingleOrDefault(m => m.Name == strMaterialTypes);
                if (obj_type == null)
                {
                    ViewBag.Message = "► " + Language.label("NotExistsObject") + ": " + Language.label("MaterialType");
                    //pass data to view
                    MaterialAddPassData();
                    return View(obj);
                }

                Material objMaterial = db_Meal.Materials.SingleOrDefault(m => m.ID == obj.ID);
                objMaterial.Code = obj.Code;
                objMaterial.Name = obj.Name;
                objMaterial.Description = obj.Description;
                objMaterial.MaterialTypeID = obj_type.ID;
                objMaterial.PCIDUpd = mFunction.ToString(User.Identity.GetUserName());
                objMaterial.DateUpd = DateTime.Now;
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("Material", "MealControlCanteen", "Update", "Update Material", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("Material");
            }
            catch
            {
                ViewBag.Message = "► " + Language.label("ErrorSave");
                //pass data to view
                MaterialAddPassData();
                return View(obj);
            }
        }

        //Edit Material
        [HttpGet]
        public ActionResult MaterialAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Material").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass data to view
            MaterialAddPassData();
            //Go to view
            return View();
        }

        //pass data to view
        public void MaterialAddPassData()
        {
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            //pass data lstMaterialType to view
            List<MaterialType> lstMaterialTypes = db_Meal.MaterialTypes.Where(n => n.Status == 1).ToList();
            ViewBag.lstMaterialTypes = lstMaterialTypes;
        }

        [HttpPost]
        public ActionResult MaterialAdd(FormCollection fc)
        {
            try
            {
                var a = mFunction.ToString(fc["Code"]);
                var b = mFunction.ToString(fc["Name"]);
                var c = mFunction.ToString(fc["Description"]);
                var d = mFunction.ToString(fc["MaterialType"]);

                //Check null data input
                if (a == "" || b == "" || d == "")
                {
                    //pass data to view
                    MaterialAddPassData();
                    ViewBag.Message = "► " + Language.label("WarningNullData") + " " + Language.label("Code") + "," + Language.label("Name") + "," + Language.label("MaterialType");
                    return View("");
                }

                //check materialtypes
                MaterialType obj_type = db_Meal.MaterialTypes.SingleOrDefault(m => m.Name == d);
                if (obj_type == null)
                {
                    ViewBag.Message = "► " + Language.label("NotExistsObject") + ": " + Language.label("MaterialType");
                    //pass data to view
                    MaterialAddPassData();
                    return View("");
                }

                //check code code is exists in table material
                if (db_Meal.Materials.Where(m => m.Code == a).ToList().Count > 0)
                {
                    //pass data to view
                    MaterialAddPassData();
                    ViewBag.Message = "► " + Language.label("ExistsObject");
                    return View("");
                }
                Material obj = new Material
                {
                    Code = a,
                    Name = b,
                    Description = c,
                    MaterialTypeID = obj_type.ID,
                    TableName = "Material",
                    Image = "/Content/adminlte/dist/img/RiceChicken.jpg",
                    Status = 1,
                    PCIDAdd = mFunction.ToString(User.Identity.GetUserName()),
                    DateAdd = DateTime.Now
                };
                db_Meal.Materials.Add(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("Material", "MealControlCanteen", "Insert", "Add New Material", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("Material");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //pass data to view
                MaterialAddPassData();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View();
            }
        }
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Meal Delivery
        //get datata MealDelivery
        public ActionResult MealDelivery()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealDelivery").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass data to view
            MealDeliveryPassData();
            return View();
        }

        //pass data to view
        public void MealDeliveryPassData()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        public JsonResult GetNotificationDelivery()
        {
            return Json(SendNotificationsDelivery.GetNotificationDelivery(), JsonRequestBehavior.AllowGet);
        }
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Order Meal for new employee, customer
        //get datata MealOrderException
        public ActionResult MealOrderException(FormCollection fc)
        {
            DateTime dt_search = mFunction.ToDateTime(fc["dateSearch"]);
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealOrderException").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass date to view
            SendDataMealOrderException(dt_search);
            return View();
        }

        //pass date to view
        private void SendDataMealOrderException(DateTime strDateSearch)
        {
            var lstTimeLimit = db_Meal.TimeLimitOrders.ToList();
            ViewBag.timeHC = lstTimeLimit.SingleOrDefault(m => m.ID == 1).TimeLimit;
            ViewBag.timeOT = lstTimeLimit.SingleOrDefault(m => m.ID == 2).TimeLimit;
            ViewBag.time1 = lstTimeLimit.SingleOrDefault(m => m.ID == 3).TimeLimit;
            ViewBag.time2 = lstTimeLimit.SingleOrDefault(m => m.ID == 4).TimeLimit;
            ViewBag.time3 = lstTimeLimit.SingleOrDefault(m => m.ID == 5).TimeLimit;
            ViewBag.strDateSearch = strDateSearch;
            var lstOrder = db_Meal.sp_MealOrderException_Search(strDateSearch.ToString("yyyyMMdd")).Where(m => m.IsTakeMeal == false).ToList();
            ViewBag.lstOrder = lstOrder;
            ViewBag.strDateSearch = strDateSearch;
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));
        }

        //Add data MealOrderException
        [HttpGet]
        public ActionResult MealOrderExceptionAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealOrderException").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //passing data to view
            PassingDataToMealOrderExceptionAdd();
            return View();
        }

        //passing data to view
        public void PassingDataToMealOrderExceptionAdd()
        {
            var lstShiftWork = db_Meal.ShiftWorks.ToList();
            ViewBag.lstShiftWork = lstShiftWork;
            string strTime = DateTime.Now.ToString("yyyyMMdd");
            var lstMenuMeal = db_Meal.sp_MealSchedule_GetByDay(strTime, strTime).ToList();
            ViewBag.lstMenuMeal = lstMenuMeal;
            var lstDepartment = db_Hr.MaBoPhans.ToList();
            ViewBag.lstDepartment = lstDepartment;
            var lstUser = db_Hr.sp_EmployeeGet().ToList();
            ViewBag.lstUser = lstUser;

            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));
        }

        //Add data MealOrderException
        [HttpPost]
        public ActionResult MealOrderExceptionAdd(FormCollection fc)
        {
            try
            {
                //take data from view
                int strShift = mFunction.ToInt(fc["Shift"]);
                int strMeal = mFunction.ToInt(fc["Meal"]);
                string strDepartment = mFunction.ToString(fc["Department"]);
                string strUserRequest = mFunction.ToString(fc["UserRequest"]);
                int idUserRequest;
                var objUser = db_Hr.sp_EmployeeGet().SingleOrDefault(m => m.TenNhanVien == strUserRequest);
                if (objUser == null)
                {
                    idUserRequest = 0;
                }
                else
                {
                    idUserRequest = mFunction.ToInt(objUser.MaSoNhanVien);
                }
                string strName = mFunction.ToString(fc["Name"]);
                //check null or error value
                if (strShift == 0 || strMeal == 0 || strDepartment == "0" || idUserRequest == 0 || strName == "")
                {
                    ViewBag.Message = "► " + Language.label("DataInputIsIncorrect");
                    //passing data to view
                    PassingDataToMealOrderExceptionAdd();
                    return View();
                }
                //prepare data for save
                MealOrderException lstOrder = new MealOrderException()
                {
                    Day             = DateTime.Now.ToString("yyyyMMdd"),
                    ShiftWorkID     = strShift,
                    MenuMealID      = strMeal,
                    DepartmentID    = strDepartment,
                    UserNameRequest = idUserRequest.ToString(),
                    FullName        = strName,
                    IsTakeMeal      = false,
                    DateAdd         = DateTime.Now,
                    UserAdd         = User.Identity.GetUserName()
                };
                //save data
                db_Meal.MealOrderExceptions.Add(lstOrder);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrderException", "MealControlCanteen", "Insert", "Add New MealOrderException", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("MealOrderException");
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                //passing data to view
                PassingDataToMealOrderExceptionAdd();
                return View();
            }
        }

        //get datata MealOrderExceptionEdit
        [HttpGet]
        public ActionResult MealOrderExceptionEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealOrderException").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            MealOrderException obj = db_Meal.MealOrderExceptions.SingleOrDefault(m => m.ID == id);
            if (obj == null)
            {
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("MealOrderException");
            }
            //passing data to view
            PassingDataToMealOrderExceptionAdd();
            string user = mFunction.ToString(obj.UserNameRequest);
            var objUser = db_Hr.sp_EmployeeGet().SingleOrDefault(m => m.MaSoNhanVien == user);
            var strUserRequest = mFunction.ToString(objUser.TenNhanVien);
            ViewBag.strUserRequest = strUserRequest;
            return View(obj);
        }

        //update data MealOrderExceptionEdit
        [HttpPost]
        public ActionResult MealOrderExceptionEdit(MealOrderException obj, FormCollection fc)
        {
            try
            {
                //take data from view
                int strShift = mFunction.ToInt(fc["Shift"]);
                int strMeal = mFunction.ToInt(fc["Meal"]);
                string strDepartment = mFunction.ToString(fc["Department"]);
                string strUserRequest = mFunction.ToString(fc["UserRequest"]);
                int idUserRequest;
                var objUser = db_Hr.sp_EmployeeGet().SingleOrDefault(m => m.TenNhanVien == strUserRequest);
                if (objUser == null)
                {
                    idUserRequest = 0;
                }
                else
                {
                    idUserRequest = mFunction.ToInt(objUser.MaSoNhanVien);
                }

                string strName = mFunction.ToString(fc["Name"]);
                //check null or error value
                if (strShift == 0 || strMeal == 0 || strDepartment == "0" || idUserRequest == 0 || strName == "")
                {
                    ViewBag.Message = "► " + Language.label("DataInputIsIncorrect");
                    //passing data to view
                    PassingDataToMealOrderExceptionAdd();
                    ViewBag.strUserRequest = strUserRequest;
                    return View(obj);
                }
                //prepare data for save
                MealOrderException obj_save = db_Meal.MealOrderExceptions.SingleOrDefault(m => m.ID == obj.ID);
                obj_save.ShiftWorkID = strShift;
                obj_save.MenuMealID = strMeal;
                obj_save.DepartmentID = strDepartment;
                obj_save.UserNameRequest = idUserRequest.ToString();
                obj_save.FullName = strName;
                obj_save.DateAdd = DateTime.Now;
                obj_save.UserAdd = User.Identity.GetUserName();
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrderException", "MealControlCanteen", "Update", "Update MealOrderException", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("MealOrderException");
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                //passing data to view
                PassingDataToMealOrderExceptionAdd();
                return View(obj);
            }
        }

        //delete data MealOrderException
        public ActionResult MealOrderExceptionDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealOrderException").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                MealOrderException obj = db_Meal.MealOrderExceptions.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //pass date to view
                    SendDataMealOrderException(DateTime.Now);
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("MealOrderException");
                }
                db_Meal.MealOrderExceptions.Remove(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrderException", "MealControlCanteen", "Delete", "Del MealOrderException", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("MealOrderException");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //pass date to view
                SendDataMealOrderException(DateTime.Now);
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("Material");
            }
        }

        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Meal Delivery Exception
        //get datata MealDelivery
        public ActionResult MealDeliveryException(FormCollection fc)
        {
            DateTime dt_search = mFunction.ToDateTime(fc["dateSearch"]);
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealDeliveryException").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass data to view
            PassDataMealDeliveryException(dt_search);
            return View();
        }

        //pass data to view
        public void PassDataMealDeliveryException(DateTime dt)
        {
            ViewBag.strDateSearch = dt;
            var lstOrder = db_Meal.sp_MealOrderException_Search(dt.ToString("yyyyMMdd")).ToList();
            ViewBag.lstOrder = lstOrder;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        //confirm is take meal
        public ActionResult MealDeliveryExceptionIsTakeMeal(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "MealDeliveryException").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                //prepare data for save
                MealOrderException obj_save = db_Meal.MealOrderExceptions.SingleOrDefault(m => m.ID == id);
                //check null
                if (obj_save == null)
                {
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    //passing data to view
                    PassDataMealDeliveryException(DateTime.Now);
                    return View("MealDeliveryException");
                }
                obj_save.IsTakeMeal = true;
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealDeliveryException", "MealControlCanteen", "Update", "Update MealDeliveryException", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("MealDeliveryException");
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                //passing data to view
                PassDataMealDeliveryException(DateTime.Now);
                return View("MealDeliveryException");
            }
        }

        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Instant Noodles
        //get datata InstantNoodles
        public ActionResult InstantNoodlesDelivery(FormCollection fc)
        {
            DateTime dt_search = mFunction.ToDateTime(fc["dateSearch"]);
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "InstantNoodlesDelivery").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            //pass data to view
            PassDataInstantNoodlesDelivery(dt_search);
            return View();
        }

        //pass data to view
        public void PassDataInstantNoodlesDelivery(DateTime dt)
        {
            ViewBag.strDateSearch = dt;
            var lstOrder = db_Meal.sp_DeliveryInstantNoodles_Search(dt.ToString("yyyyMMdd")).ToList();
            ViewBag.lstOrder = lstOrder;
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        //confirm is take Instant Noodles
        public ActionResult InstantNoodlesDeliveryIsTakeMeal(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "InstantNoodlesDelivery").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            try
            {
                //prepare data for save
                OrderMeal obj_save = db_Meal.OrderMeals.SingleOrDefault(m => m.ID == id);
                //check null
                if (obj_save == null)
                {
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    //passing data to view
                    PassDataInstantNoodlesDelivery(DateTime.Now);
                    return View("InstantNoodlesDelivery");
                }
                obj_save.IsTakeMeal = true;
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("InstantNoodlesDelivery", "MealControlCanteen", "Update", "Update InstantNoodlesDelivery", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("InstantNoodlesDelivery");
            }
            catch (Exception ex)
            {
                ex.ToString();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                //passing data to view
                PassDataInstantNoodlesDelivery(DateTime.Now);
                return View("InstantNoodlesDelivery");
            }
        }
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Order meal offline

        //get datata Offline Order Meal
        public ActionResult OfflineOrderMeal(FormCollection fc)
        {
            DateTime dt_search = mFunction.ToDateTime(fc["dateSearch"]);
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OfflineOrderMeal").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //pass data to view
            PassDataOfflineOrderMeal(dt_search);
            return View();
        }

        //pass data to view
        public void PassDataOfflineOrderMeal(DateTime dt)
        {
            //check time limit change
            List<TimeLimitOrder> lstLimit = db_Meal.TimeLimitOrders.ToList();
            TimeSpan timeHC = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 1).TimeLimit;
            TimeSpan timeOT = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 2).TimeLimit;
            TimeSpan time1 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 3).TimeLimit;
            TimeSpan time2 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 4).TimeLimit;
            TimeSpan time3 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 5).TimeLimit;
            ViewBag.timeHC = timeHC;
            ViewBag.timeOT = timeOT;
            ViewBag.time1 = time1;
            ViewBag.time2 = time2;
            ViewBag.time3 = time3;

            string strDate = dt.ToString("yyyyMMdd");
            var lstOrder = db_Meal.sp_OrderMealOffline_Get(strDate).ToList();
            ViewBag.lstOrder = lstOrder;

            ViewBag.strDateSearch = dt;

            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        //get datata InstantNoodles
        public ActionResult OfflineOrderMealAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OfflineOrderMeal").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            //pass data to view
            PassDataOfflineOrderMealAdd();

            string strDeptID = "";
            string strUser = User.Identity.GetUserName();
            var objDept = db_Hr.tblHoSoLyLichNhanViens.SingleOrDefault(m => m.MaSoNhanVien == strUser);
            if (objDept != null)
            {
                strDeptID = mFunction.ToString(objDept.BoPhan).Trim();
            }
            ViewBag.strDeptID = strDeptID;

            var lstUser = db_Hr.sp_EmployeeGet().Where(m => m.BoPhan == strDeptID).ToList();
            ViewBag.lstUser = lstUser;

            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            foreach (var i in lstUser)
            {
                obj.Add(new CheckBoxModel { Value = mFunction.ToInt(i.MaSoNhanVien), Code = i.MaSoNhanVien, Name = i.TenNhanVien, IsChecked = false });
            }
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };

            return View(objBind);
        }
        [HttpPost]
        public ActionResult OfflineOrderMealAdd(CheckBoxList Obj, FormCollection fc)
        {
            string strDept = mFunction.ToString(fc["txtDepartment"]);
            string strDate = mFunction.ToString(fc["DateAdd"]);
            DateTime dtDate = mFunction.ToDateTime(strDate);
            string strDay = dtDate.ToString("yyyyMMdd");
            int intShift = mFunction.ToInt(fc["Shift"]);
            string strShiftCode = "";
            var objShift = db_Meal.ShiftWorks.SingleOrDefault(m => m.ID == intShift);
            if (objShift != null)
            {
                strShiftCode = objShift.Code;
            }
            int intMeal = mFunction.ToInt(fc["Meal"]);
            string strMealCode = "";
            var objMeal = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == intMeal);
            if (objMeal != null)
            {
                strMealCode = objMeal.Code;
            }
            var objCheck = Obj.CheckBox.Where(m => m.IsChecked == true).ToList();
            try
            {
                //Check null value
                if (strDept == "" || strDate == "" || intShift == 0 || intMeal == 0 || objCheck.Count <= 0)
                {
                    //pass data to view
                    PassDataOfflineOrderMealAdd();

                    string strDeptID = strDept;
                    string strUser = User.Identity.GetUserName();
                    if (strDeptID != "")
                    {
                        var objDept = db_Hr.tblHoSoLyLichNhanViens.SingleOrDefault(m => m.MaSoNhanVien == strUser);
                        if (objDept != null)
                        {
                            strDeptID = mFunction.ToString(objDept.BoPhan).Trim();
                        }
                    }
                    ViewBag.strDeptID = strDeptID;

                    var lstUser = db_Hr.sp_EmployeeGet().Where(m => m.BoPhan == strDeptID).ToList();
                    ViewBag.lstUser = lstUser;

                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("Date") + ", " + Language.label("Shift") + ", " + Language.label("Meal") + ", " + Language.label("User") + ", " + Language.label("Department");
                    return View(Obj);
                }

                //Prepare data
                foreach (var item in objCheck)
                {
                    OrderMeal objOrder = new OrderMeal()
                    {
                        Day = dtDate.ToString("yyyyMMdd"),
                        ShiftWorkID = intShift,
                        ShiftWorkCode = strShiftCode,
                        MenuMealID = intMeal,
                        MenuMealCode = strMealCode,
                        UserName = item.Code,
                        UserAdd = User.Identity.GetUserName(),
                        DateAdd = dtDate
                    };
                    db_Meal.OrderMeals.Add(objOrder);
                }

                //Check data is not exists in table OrderMeal
                string str_Message = "";
                foreach (var item in objCheck)
                {
                    var obj = db_Meal.OrderMeals.SingleOrDefault(m => (m.Day == strDay) && (m.ShiftWorkID == intShift) && (m.UserName == item.Code));
                    if (obj != null)
                    {
                        str_Message = str_Message + item.Name + ": " + Language.label("ExistsObject") + ".\\n";
                    }
                }
                if (str_Message != "")
                {
                    //pass data to view
                    PassDataOfflineOrderMealAdd();

                    string strDeptID = strDept;
                    string strUser = User.Identity.GetUserName();
                    if (strDeptID != "")
                    {
                        var objDept = db_Hr.tblHoSoLyLichNhanViens.SingleOrDefault(m => m.MaSoNhanVien == strUser);
                        if (objDept != null)
                        {
                            strDeptID = mFunction.ToString(objDept.BoPhan).Trim();
                        }
                    }
                    ViewBag.strDeptID = strDeptID;

                    var lstUser = db_Hr.sp_EmployeeGet().Where(m => m.BoPhan == strDeptID).ToList();
                    ViewBag.lstUser = lstUser;

                    ViewBag.Message = str_Message;
                    return View(Obj);
                }

                //check time limit change
                List<TimeLimitOrder> lstLimit = db_Meal.TimeLimitOrders.ToList();
                TimeSpan timeHC = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 1).TimeLimit;
                TimeSpan timeOT = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 2).TimeLimit;
                TimeSpan time1 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 3).TimeLimit;
                TimeSpan time2 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 4).TimeLimit;
                TimeSpan time3 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 5).TimeLimit;
                Boolean check = false;
                if (intShift == 1 && timeHC <= DateTime.Now.TimeOfDay)
                    check = true;
                if (intShift == 2 && timeOT <= DateTime.Now.TimeOfDay)
                    check = true;
                if (intShift == 3 && time1 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (intShift == 4 && time2 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (intShift == 5 && time3 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (check == true)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("CanNotDeleteMeal");
                    return View("OfflineOrderMeal");
                }

                //Save data
                db_Meal.SaveChanges();

                //Save log
                var objLog = db_Meal.sp_ActionLog("OfflineOrderMeal", "MealControlCanteen", "Insert", "Insert OfflineOrderMeal", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("OfflineOrderMeal");
            }
            catch
            {
                //pass data to view
                PassDataOfflineOrderMealAdd();

                string strDeptID = strDept;
                string strUser = User.Identity.GetUserName();
                if (strDeptID != "")
                {
                    var objDept = db_Hr.tblHoSoLyLichNhanViens.SingleOrDefault(m => m.MaSoNhanVien == strUser);
                    if (objDept != null)
                    {
                        strDeptID = mFunction.ToString(objDept.BoPhan).Trim();
                    }
                }
                ViewBag.strDeptID = strDeptID;

                var lstUser = db_Hr.sp_EmployeeGet().Where(m => m.BoPhan == strDeptID).ToList();
                ViewBag.lstUser = lstUser;

                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(Obj);
            }
        }

        //pass data to view
        public void PassDataOfflineOrderMealAdd()
        {

            var lstDept = db_Hr.MaBoPhans.ToList();
            ViewBag.lstDept = lstDept;

            DateTime dtNow = DateTime.Now;
            TimeSpan mySpan = new TimeSpan(dtNow.Hour, dtNow.Minute, dtNow.Second);
            var lstShift = db_Meal.sp_ShiftWorkGet().Where(m => m.TimeLimit > mySpan).ToList();
            ViewBag.lstShift = lstShift;

            string strTime = DateTime.Now.ToString("yyyyMMdd");
            var lstMeal = db_Meal.sp_MealSchedule_GetByDay(strTime, strTime).ToList();
            ViewBag.lstMeal = lstMeal;

            CheckCookie();
            ChangeLanguage(cookie.Value);

        }

        //Delete data
        public ActionResult OfflineOrderMealDel(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OfflineOrderMeal").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                int key = mFunction.ToInt(id);
                //check exists obj
                OrderMeal obj = db_Meal.OrderMeals.SingleOrDefault(m => m.ID == key);
                if (obj == null)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("NotExistsObject");
                    return View("OfflineOrderMeal");
                }
                //check time limit change
                List<TimeLimitOrder> lstLimit = db_Meal.TimeLimitOrders.ToList();
                TimeSpan timeHC = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 1).TimeLimit;
                TimeSpan timeOT = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 2).TimeLimit;
                TimeSpan time1 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 3).TimeLimit;
                TimeSpan time2 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 4).TimeLimit;
                TimeSpan time3 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 5).TimeLimit;
                Boolean check = false;
                if (obj.ShiftWorkID == 1 && timeHC <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 2 && timeOT <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 3 && time1 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 4 && time2 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 5 && time3 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (check == true)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("CanNotDeleteMeal");
                    return View("OfflineOrderMeal");
                }
                //Delete meal
                db_Meal.OrderMeals.Remove(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("OfflineOrderMeal", "MealControlCanteen", "Delete", "Delete OfflineOrderMeal", "ID: " + id.ToString(), User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("OfflineOrderMeal");
            }
            catch
            {
                //pass data to view
                PassDataOfflineOrderMeal(DateTime.Now);
                ViewBag.Message = Language.label("ErrorSave");
                return View("OfflineOrderMeal");
            }
        }

        //Edit data
        public ActionResult OfflineOrderMealEdit(string id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "OfflineOrderMeal").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                int key = mFunction.ToInt(id);
                //check exists obj
                OrderMeal obj = db_Meal.OrderMeals.SingleOrDefault(m => m.ID == key);
                if (obj == null)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("NotExistsObject");
                    return View("OfflineOrderMeal");
                }

                //check time limit change
                List<TimeLimitOrder> lstLimit = db_Meal.TimeLimitOrders.ToList();
                TimeSpan timeHC = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 1).TimeLimit;
                TimeSpan timeOT = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 2).TimeLimit;
                TimeSpan time1 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 3).TimeLimit;
                TimeSpan time2 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 4).TimeLimit;
                TimeSpan time3 = lstLimit.SingleOrDefault(m => m.ShiftWorkID == 5).TimeLimit;
                Boolean check = false;
                if (obj.ShiftWorkID == 1 && timeHC <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 2 && timeOT <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 3 && time1 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 4 && time2 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (obj.ShiftWorkID == 5 && time3 <= DateTime.Now.TimeOfDay)
                    check = true;
                if (check == true)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("CanNotDeleteMeal");
                    return View("OfflineOrderMeal");
                }

                //
                string strDT = DateTime.Now.ToString("yyyyMMdd");
                var objPassData = db_Meal.sp_OrderMealOffline_Get(strDT).SingleOrDefault(m => m.ID == key);

                int _shiftID = objPassData.ShiftWorkID;
                string strTime = DateTime.Now.ToString("yyyyMMdd");
                var lstMeal = db_Meal.sp_MealSchedule_GetByDay(strTime, strTime).Where(m => m.ShiftWorkID == _shiftID).ToList();
                ViewBag.lstMeal = lstMeal;

                CheckCookie();
                ChangeLanguage(cookie.Value);

                return View(objPassData);
            }
            catch
            {
                //pass data to view
                PassDataOfflineOrderMeal(DateTime.Now);
                ViewBag.Message = Language.label("ErrorSave");
                return View("OfflineOrderMeal");
            }
        }
        [HttpPost]
        public ActionResult OfflineOrderMealEdit(sp_OrderMealOffline_Get_Result Obj, FormCollection fc)
        {
            int intMeal = mFunction.ToInt(fc["Meal"]);

            string strMealCode = "";
            var objMeal = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == intMeal);
            if (objMeal != null)
            {
                strMealCode = objMeal.Code;
            }

            try
            {
                //Check null value
                if (intMeal == 0)
                {

                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    //
                    string strDT = DateTime.Now.ToString("yyyyMMdd");
                    var objPassData = db_Meal.sp_OrderMealOffline_Get(strDT).SingleOrDefault(m => m.ID == Obj.ID);

                    int _shiftID = objPassData.ShiftWorkID;
                    string strTime = DateTime.Now.ToString("yyyyMMdd");
                    var lstMeal = db_Meal.sp_MealSchedule_GetByDay(strTime, strTime).Where(m => m.ShiftWorkID == _shiftID).ToList();
                    ViewBag.lstMeal = lstMeal;

                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("Meal");
                    return View(objPassData);
                }

                //Prepare data
                var objSave = db_Meal.OrderMeals.SingleOrDefault(m => m.ID == Obj.ID);
                if (objSave == null)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("NotExistsObject");
                    return View("OfflineOrderMeal");
                }
                objSave.MenuMealID = intMeal;
                objSave.MenuMealCode = strMealCode;
                objSave.UserUpd = User.Identity.GetUserName();
                objSave.DateUpd = DateTime.Now;

                int _shift = mFunction.ToInt(objSave.ShiftWorkID);
                //check time limit change
                List<TimeLimitOrder> lstLimit = db_Meal.TimeLimitOrders.ToList();
                TimeSpan time = lstLimit.SingleOrDefault(m => m.ShiftWorkID == _shift).TimeLimit;
                if (time <= DateTime.Now.TimeOfDay)
                {
                    //pass data to view
                    PassDataOfflineOrderMeal(DateTime.Now);
                    ViewBag.Message = Language.label("CanNotDeleteMeal");
                    return View("OfflineOrderMeal");
                }

                //Save data
                db_Meal.SaveChanges();

                //Save log
                var objLog = db_Meal.sp_ActionLog("OfflineOrderMeal", "MealControlCanteen", "Update", "Update OfflineOrderMeal", "", User.Identity.GetUserName(), DateTime.Now);

                return RedirectToAction("OfflineOrderMeal");
            }
            catch
            {
                //pass data to view
                PassDataOfflineOrderMeal(DateTime.Now);
                ViewBag.Message = Language.label("ErrorSave");
                return View("OfflineOrderMeal");
            }
        }


        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region cookie and language
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
        /// //////////////////////////////////////////////////////////////////
    }
}