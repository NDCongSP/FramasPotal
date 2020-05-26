using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using FramasVietNam.Models.Meal;
using System.Data;
using FramasVietNam.Models.FingerScan;
using System.Web.Script.Serialization;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class MealControlController : Controller
    {


        /// //////////////////////////////////////////////////////////////////
        #region global variable
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        //Add db meal menu
        private MealControlEntities db_Meal = new MealControlEntities();
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Order meal
        // GET: MealControl
        [HttpGet]
        public ActionResult MealOrder()
        {
            //call function pass data to view
            PassData();
            //
            return View();
        }

        //Pass data meal menu to view
        public void PassData()
        {
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));

            DateTime today = DateTime.Now;

            //passing office hours to view
            DataTable dt_HC = DataOperation.getDataTable(GlobalVariable.DBHumanResource, "exec sp_IsOfficeHoursGet '" + User.Identity.GetUserName().Trim() + "'");
            if (dt_HC.Rows.Count > 0)
            {
                ViewBag.OfficeHours = 1;
                //pass data_HC to menu
                List<sp_MealSchedule_Parent_Result> lst_parent_HC = db_Meal.sp_MealSchedule_Parent(1, today).ToList();
                List<sp_MealSchedule_Child_Result> lst_Child_HC = db_Meal.sp_MealSchedule_Child(1, today).ToList();
                ViewBag.ds_MenuItem_HC = lst_parent_HC;
                ViewBag.ds_MenuItemsDetails_HC = lst_Child_HC;
                //pass data_HC_OT to menu
                List<sp_MealSchedule_Parent_Result> lst_parent_HC_OT = db_Meal.sp_MealSchedule_Parent(2, today).ToList();
                List<sp_MealSchedule_Child_Result> lst_Child_HC_OT = db_Meal.sp_MealSchedule_Child(2, today).ToList();
                ViewBag.ds_MenuItem_HC_OT = lst_parent_HC_OT;
                ViewBag.ds_MenuItemsDetails_HC_OT = lst_Child_HC_OT;
            }
            else
            {
                ViewBag.OfficeHours = 0;
                //pass data_Ca1 to menu
                List<sp_MealSchedule_Parent_Result> lst_parent_Ca1 = db_Meal.sp_MealSchedule_Parent(3, today).ToList();
                List<sp_MealSchedule_Child_Result> lst_Child_Ca1 = db_Meal.sp_MealSchedule_Child(3, today).ToList();
                ViewBag.ds_MenuItem_Ca1 = lst_parent_Ca1;
                ViewBag.ds_MenuItemsDetails_Ca1 = lst_Child_Ca1;
                //pass data_Ca2 to menu
                List<sp_MealSchedule_Parent_Result> lst_parent_Ca2 = db_Meal.sp_MealSchedule_Parent(4, today).ToList();
                List<sp_MealSchedule_Child_Result> lst_Child_Ca2 = db_Meal.sp_MealSchedule_Child(4, today).ToList();
                ViewBag.ds_MenuItem_Ca2 = lst_parent_Ca2;
                ViewBag.ds_MenuItemsDetails_Ca2 = lst_Child_Ca2;
                //pass data_Ca3 to menu
                List<sp_MealSchedule_Parent_Result> lst_parent_Ca3 = db_Meal.sp_MealSchedule_Parent(5, today).ToList();
                List<sp_MealSchedule_Child_Result> lst_Child_Ca3 = db_Meal.sp_MealSchedule_Child(5, today).ToList();
                ViewBag.ds_MenuItem_Ca3 = lst_parent_Ca3;
                ViewBag.ds_MenuItemsDetails_Ca3 = lst_Child_Ca3;
            }

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
        }

        //action Save data
        [HttpPost]
        public ActionResult MealOrder(FormCollection fc)
        {

            //variable save data to OrderMeal
            List<OrderMeal> lstSave = new List<OrderMeal>();

            //variable get data from view
            string str_CaHanhChinh = mFunction.ToString(fc["txt_CaHanhChinh"]);
            string str_CaHanhChinh_OT = mFunction.ToString(fc["txt_CaHanhChinh_OT"]);
            string str_Ca1 = mFunction.ToString(fc["txt_Ca1"]);
            string str_Ca2 = mFunction.ToString(fc["txt_Ca2"]);
            string str_Ca3 = mFunction.ToString(fc["txt_Ca3"]);
            int int_MealCaHanhChinh = mFunction.ToInt(fc["txt_MealCaHanhChinh"]);
            int int_MealCaHanhChinh_OT = mFunction.ToInt(fc["txt_MealCaHanhChinh_OT"]);
            int int_MealCa1 = mFunction.ToInt(fc["txt_MealCa1"]);
            int int_MealCa2 = mFunction.ToInt(fc["txt_MealCa2"]);
            int int_MealCa3 = mFunction.ToInt(fc["txt_MealCa3"]);

            if (str_CaHanhChinh == "1")
            {
                if (int_MealCaHanhChinh == 0 && str_CaHanhChinh_OT == "0")
                {
                    string mess = Language.label("PleaseChooseMealForOfficeHours");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
                if (str_CaHanhChinh_OT == "1" && int_MealCaHanhChinh_OT == 0)
                {
                    string mess = Language.label("PleaseChooseMealForOfficeHours") + " & " + Language.label("Overtime");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
            }
            else
            {
                if (str_Ca1 == "1")
                {
                    if (int_MealCa1 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 1";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                if (str_Ca2 == "1")
                {
                    if (int_MealCa2 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 2";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                if (str_Ca3 == "1")
                {
                    if (int_MealCa3 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 3";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                else if (str_Ca1 != "1" && str_Ca2 != "1" && str_Ca3 != "1")
                {
                    string mess = Language.label("PleaseChooseMeal") + " & " + Language.label("Meal");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
            }

            //check user choose office hours
            if (str_CaHanhChinh == "1")
            {

                if (int_MealCaHanhChinh != 0)
                {
                    //create data meal order for office hours
                    OrderMeal lst_OfficeHours = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 1,
                        ShiftWorkCode = "Shift_HC",
                        MenuMealID = int_MealCaHanhChinh,
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCaHanhChinh).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),
                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_OfficeHours);
                }

                //check user Overtime
                if (str_CaHanhChinh_OT == "1")
                {
                    //create data meal order for overtime
                    OrderMeal lst_Overtime = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 2,
                        ShiftWorkCode = "Shift_HC_OT",
                        MenuMealID = int_MealCaHanhChinh_OT,
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCaHanhChinh_OT).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Overtime);
                }
            }

            //Check user choose shift 1,2,3
            else if (str_CaHanhChinh == "0")
            {

                //Check shift 1
                if (str_Ca1 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 3,
                        ShiftWorkCode = "Shift_1",
                        MenuMealID = mFunction.ToInt(int_MealCa1),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa1).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }

                //Check shift 2
                if (str_Ca2 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 4,
                        ShiftWorkCode = "Shift_2",
                        MenuMealID = mFunction.ToInt(int_MealCa2),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa2).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }

                //Check shift 3
                if (str_Ca3 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 5,
                        ShiftWorkCode = "Shift_3",
                        MenuMealID = mFunction.ToInt(int_MealCa3),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa3).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }
            }

            //Check data is not exists in table OrderMeal
            string str_Message = "";
            foreach (var item in lstSave)
            {
                var obj = db_Meal.OrderMeals.SingleOrDefault(m => (m.Day == item.Day) && (m.ShiftWorkID == item.ShiftWorkID) && (m.UserName == item.UserName));
                if (obj != null)
                {
                    str_Message = str_Message + item.ShiftWorkCode + ": " + Language.label("OrderedMeal") + ".\\n";
                }
            }
            if (str_Message != "")
            {
                ViewBag.Message = str_Message;
                //call function pass data to view
                PassData();
                return View();
            }

            //send date to function save
            Boolean result = MealOrderSave(lstSave);
            if (result == false)
            {
                ViewBag.Message = Language.label("ErrorSave");
                //call function pass data to view
                PassData();
                return View();
            }

            //return view meal order success
            return View("MealOrderSuccess");
        }

        //function Save data
        private Boolean MealOrderSave(List<OrderMeal> lstOrderMeal)
        {
            try
            {
                foreach (var item in lstOrderMeal)
                {
                    db_Meal.OrderMeals.Add(item);
                }
                //Save data
                db_Meal.SaveChanges();

                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrder", "MealControl", "Insert", "Meal Order", "", User.Identity.GetUserName(), DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        // GET: MealOrderFinger
        [HttpGet]
        public ActionResult MealOrderFingerM1()
        {
            //call function pass data to view
            PassData();
            //
            return View();
        }
        
        public JsonResult GetNotificationExistsLoginM1()
        {
            List<tbl_LogFinger_Login_M1> obj = SendNotificationsLoginM1.GetNotificationLoginM1();
            Session["IDUserM1"] = "";
            if (obj.Count > 0)
            {
                Session["IDUserM1"] = obj[0].IndRegID;
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(obj);
            var result = Json(json, JsonRequestBehavior.AllowGet);
            return result;
        }

        //action Save data
        [HttpPost]
        public ActionResult MealOrderFingerM1(FormCollection fc)
        {

            //variable save data to OrderMeal
            List<OrderMeal> lstSave = new List<OrderMeal>();

            //variable get data from view
            string str_CaHanhChinh = mFunction.ToString(fc["txt_CaHanhChinh"]);
            string str_CaHanhChinh_OT = mFunction.ToString(fc["txt_CaHanhChinh_OT"]);
            string str_Ca1 = mFunction.ToString(fc["txt_Ca1"]);
            string str_Ca2 = mFunction.ToString(fc["txt_Ca2"]);
            string str_Ca3 = mFunction.ToString(fc["txt_Ca3"]);
            int int_MealCaHanhChinh = mFunction.ToInt(fc["txt_MealCaHanhChinh"]);
            int int_MealCaHanhChinh_OT = mFunction.ToInt(fc["txt_MealCaHanhChinh_OT"]);
            int int_MealCa1 = mFunction.ToInt(fc["txt_MealCa1"]);
            int int_MealCa2 = mFunction.ToInt(fc["txt_MealCa2"]);
            int int_MealCa3 = mFunction.ToInt(fc["txt_MealCa3"]);

            if (str_CaHanhChinh == "1")
            {
                if (int_MealCaHanhChinh == 0 && str_CaHanhChinh_OT == "0")
                {
                    string mess = Language.label("PleaseChooseMealForOfficeHours");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
                if (str_CaHanhChinh_OT == "1" && int_MealCaHanhChinh_OT == 0)
                {
                    string mess = Language.label("PleaseChooseMealForOfficeHours") + " & " + Language.label("Overtime");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
            }
            else
            {
                if (str_Ca1 == "1")
                {
                    if (int_MealCa1 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 1";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                if (str_Ca2 == "1")
                {
                    if (int_MealCa2 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 2";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                if (str_Ca3 == "1")
                {
                    if (int_MealCa3 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 3";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                else if (str_Ca1 != "1" && str_Ca2 != "1" && str_Ca3 != "1")
                {
                    string mess = Language.label("PleaseChooseMeal");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
            }

            //check user choose office hours
            if (str_CaHanhChinh == "1")
            {

                if (int_MealCaHanhChinh != 0)
                {
                    //create data meal order for office hours
                    OrderMeal lst_OfficeHours = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 1,
                        ShiftWorkCode = "Shift_HC",
                        MenuMealID = int_MealCaHanhChinh,
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCaHanhChinh).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),
                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_OfficeHours);
                }

                //check user Overtime
                if (str_CaHanhChinh_OT == "1")
                {
                    //create data meal order for overtime
                    OrderMeal lst_Overtime = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 2,
                        ShiftWorkCode = "Shift_HC_OT",
                        MenuMealID = int_MealCaHanhChinh_OT,
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCaHanhChinh_OT).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Overtime);
                }
            }

            //Check user choose shift 1,2,3
            else if (str_CaHanhChinh == "0")
            {

                //Check shift 1
                if (str_Ca1 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 3,
                        ShiftWorkCode = "Shift_1",
                        MenuMealID = mFunction.ToInt(int_MealCa1),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa1).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }

                //Check shift 2
                if (str_Ca2 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 4,
                        ShiftWorkCode = "Shift_2",
                        MenuMealID = mFunction.ToInt(int_MealCa2),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa2).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }

                //Check shift 3
                if (str_Ca3 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 5,
                        ShiftWorkCode = "Shift_3",
                        MenuMealID = mFunction.ToInt(int_MealCa3),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa3).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }
            }

            //Check data is not exists in table OrderMeal
            string str_Message = "";
            foreach (var item in lstSave)
            {
                var obj = db_Meal.OrderMeals.SingleOrDefault(m => (m.Day == item.Day) && (m.ShiftWorkID == item.ShiftWorkID) && (m.UserName == item.UserName));
                if (obj != null)
                {
                    str_Message = str_Message + item.ShiftWorkCode + ": " + Language.label("OrderedMeal") + ".\\n";
                }
            }
            if (str_Message != "")
            {
                ViewBag.Message = str_Message;
                //call function pass data to view
                PassData();
                return View();
            }

            //send date to function save
            Boolean result = MealOrderSaveM1(lstSave);
            if (result == false)
            {
                ViewBag.Message = Language.label("ErrorSave");
                //call function pass data to view
                PassData();
                return View();
            }
 
            //return view meal order success
            return RedirectToAction("LogOff_FingerM1_Get", "Account");
        }

        //function Save data
        private Boolean MealOrderSaveM1(List<OrderMeal> lstOrderMeal)
        {
            try
            {
                foreach (var item in lstOrderMeal)
                {
                    db_Meal.OrderMeals.Add(item);
                }
                //Save data
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrder", "MealControl", "Insert", "Meal Order M1", "", User.Identity.GetUserName(), DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        // GET: MealOrderFinger
        [HttpGet]
        public ActionResult MealOrderFingerM2()
        {
            //call function pass data to view
            PassData();
            //
            return View();
        }

        public JsonResult GetNotificationExistsLoginM2()
        {
            List<tbl_LogFinger_Login_M1> obj = SendNotificationsLoginM2.GetNotificationLoginM2();
            Session["IDUserM2"] = "";
            if (obj.Count > 0)
            {
                Session["IDUserM2"] = obj[0].IndRegID;
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(obj);
            var result = Json(json, JsonRequestBehavior.AllowGet);
            return result;
        }

        //action Save data
        [HttpPost]
        public ActionResult MealOrderFingerM2(FormCollection fc)
        {

            //variable save data to OrderMeal
            List<OrderMeal> lstSave = new List<OrderMeal>();

            //variable get data from view
            string str_CaHanhChinh = mFunction.ToString(fc["txt_CaHanhChinh"]);
            string str_CaHanhChinh_OT = mFunction.ToString(fc["txt_CaHanhChinh_OT"]);
            string str_Ca1 = mFunction.ToString(fc["txt_Ca1"]);
            string str_Ca2 = mFunction.ToString(fc["txt_Ca2"]);
            string str_Ca3 = mFunction.ToString(fc["txt_Ca3"]);
            int int_MealCaHanhChinh = mFunction.ToInt(fc["txt_MealCaHanhChinh"]);
            int int_MealCaHanhChinh_OT = mFunction.ToInt(fc["txt_MealCaHanhChinh_OT"]);
            int int_MealCa1 = mFunction.ToInt(fc["txt_MealCa1"]);
            int int_MealCa2 = mFunction.ToInt(fc["txt_MealCa2"]);
            int int_MealCa3 = mFunction.ToInt(fc["txt_MealCa3"]);

            if (str_CaHanhChinh == "1")
            {
                if (int_MealCaHanhChinh == 0 && str_CaHanhChinh_OT == "0")
                {
                    string mess = Language.label("PleaseChooseMealForOfficeHours");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
                if (str_CaHanhChinh_OT == "1" && int_MealCaHanhChinh_OT == 0)
                {
                    string mess = Language.label("PleaseChooseMealForOfficeHours") + " & " + Language.label("Overtime");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
            }
            else
            {
                if (str_Ca1 == "1")
                {
                    if (int_MealCa1 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 1";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                if (str_Ca2 == "1")
                {
                    if (int_MealCa2 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 2";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                if (str_Ca3 == "1")
                {
                    if (int_MealCa3 == 0)
                    {
                        string mess = Language.label("PleaseChooseMeal") + ": " + Language.label("Shift") + " 3";
                        ViewBag.Message = mess;
                        //call function pass data to view
                        PassData();
                        return View();
                    }
                }
                else if (str_Ca1 != "1" && str_Ca2 != "1" && str_Ca3 != "1")
                {
                    string mess = Language.label("PleaseChooseMeal");
                    ViewBag.Message = mess;
                    //call function pass data to view
                    PassData();
                    return View();
                }
            }

            //check user choose office hours
            if (str_CaHanhChinh == "1")
            {

                if (int_MealCaHanhChinh != 0)
                {
                    //create data meal order for office hours
                    OrderMeal lst_OfficeHours = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 1,
                        ShiftWorkCode = "Shift_HC",
                        MenuMealID = int_MealCaHanhChinh,
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCaHanhChinh).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),
                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_OfficeHours);
                }

                //check user Overtime
                if (str_CaHanhChinh_OT == "1")
                {
                    //create data meal order for overtime
                    OrderMeal lst_Overtime = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 2,
                        ShiftWorkCode = "Shift_HC_OT",
                        MenuMealID = int_MealCaHanhChinh_OT,
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCaHanhChinh_OT).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Overtime);
                }
            }

            //Check user choose shift 1,2,3
            else if (str_CaHanhChinh == "0")
            {

                //Check shift 1
                if (str_Ca1 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 3,
                        ShiftWorkCode = "Shift_1",
                        MenuMealID = mFunction.ToInt(int_MealCa1),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa1).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }

                //Check shift 2
                if (str_Ca2 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 4,
                        ShiftWorkCode = "Shift_2",
                        MenuMealID = mFunction.ToInt(int_MealCa2),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa2).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }

                //Check shift 3
                if (str_Ca3 == "1")
                {
                    OrderMeal lst_Shift1 = new OrderMeal
                    {
                        Day = DateTime.Now.ToString("yyyyMMdd"),
                        ShiftWorkID = 5,
                        ShiftWorkCode = "Shift_3",
                        MenuMealID = mFunction.ToInt(int_MealCa3),
                        MenuMealCode = db_Meal.MenuMeals.SingleOrDefault(m => m.ID == int_MealCa3).Code.ToString(),
                        UserName = User.Identity.GetUserName().Trim(),

                        UserAdd = User.Identity.GetUserName().Trim(),
                        DateAdd = DateTime.Now
                    };
                    lstSave.Add(lst_Shift1);
                }
            }

            //Check data is not exists in table OrderMeal
            string str_Message = "";
            foreach (var item in lstSave)
            {
                var obj = db_Meal.OrderMeals.SingleOrDefault(m => (m.Day == item.Day) && (m.ShiftWorkID == item.ShiftWorkID) && (m.UserName == item.UserName));
                if (obj != null)
                {
                    str_Message = str_Message + item.ShiftWorkCode + ": " + Language.label("OrderedMeal") + ".\\n";
                }
            }
            if (str_Message != "")
            {
                ViewBag.Message = str_Message;
                //call function pass data to view
                PassData();
                return View();
            }

            //send date to function save
            Boolean result = MealOrderSaveM2(lstSave);
            if (result == false)
            {
                ViewBag.Message = Language.label("ErrorSave");
                //call function pass data to view
                PassData();
                return View();
            }

            //return view meal order success
            return RedirectToAction("LogOff_FingerM2_Get", "Account");
        }

        //function Save data
        private Boolean MealOrderSaveM2(List<OrderMeal> lstOrderMeal)
        {
            try
            {
                foreach (var item in lstOrderMeal)
                {
                    db_Meal.OrderMeals.Add(item);
                }
                //Save data
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrder", "MealControl", "Insert", "Meal Orde M2", "", User.Identity.GetUserName(), DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        //View success
        [HttpGet]
        public ActionResult MealOrderSuccess()
        {
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));
            //
            return View();
        }

        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region Basket meal
        //meal basket
        [HttpGet]
        public ActionResult MyBasketMeal()
        {
            //Pass data to view basket
            PassDataToBasket();

            return View();
        }
        //Pass data to view basket
        private void PassDataToBasket()
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

            //pass data_HC to basket
            List<sp_OrderMeal_GetDay_Result> lst_basket = db_Meal.sp_OrderMeal_GetDay(DateTime.Now.ToString("yyyyMMdd"),User.Identity.GetUserName()).ToList();
            ViewBag.lst_basket = lst_basket;
            //
            CheckCookie();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            ChangeLanguage(mFunction.ToString(cookie.Value));
            //
        }
        //Delete meal in basket
        public ActionResult MyBasketMeal_Del(int id)
        {
            try
            {
                //check exists obj
                OrderMeal obj = db_Meal.OrderMeals.SingleOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //Pass data to view basket
                    PassDataToBasket();
                    ViewBag.Message = Language.label("NotExistsObject");
                    return View("MyBasketMeal");
                }
                //check time limit change
                List<TimeLimitOrder> lstLimit = db_Meal.TimeLimitOrders.ToList();
                TimeSpan timeHC = lstLimit.SingleOrDefault(m=>m.ShiftWorkID==1).TimeLimit;
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
                    //Pass data to view basket
                    PassDataToBasket();
                    ViewBag.Message = Language.label("CanNotDeleteMeal");
                    return View("MyBasketMeal");
                }
                //Delete meal
                db_Meal.OrderMeals.Remove(obj);
                db_Meal.SaveChanges();
                //Save log
                var objLog = db_Meal.sp_ActionLog("MealOrder", "MealControl", "Delete", "Del Meal Order", "", User.Identity.GetUserName(), DateTime.Now);
                return RedirectToAction("MyBasketMeal");
            }
            catch
            {
                //Pass data to view basket
                PassDataToBasket();
                ViewBag.Message = Language.label("ErrorSave");
                return View("MyBasketMeal");
            }
        }
        #endregion
        /// //////////////////////////////////////////////////////////////////


        /// //////////////////////////////////////////////////////////////////
        #region cookie
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
            object ds_Module = db.sp_Module_Get(mFunction.ToInt(textInput), User.Identity.GetUserName()).OrderBy(o => o.Code).ToList();
            object ds_Group = db.sp_Group_Get(mFunction.ToInt(textInput)).OrderBy(o => o.Code).ToList();
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