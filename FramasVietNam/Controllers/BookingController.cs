using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.UserManager;
using FramasVietNam.Models.HumanResource;
using FramasVietNam.Models.VNT86;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text;
using System.Data;
using FramasVietNam.Models;
using Newtonsoft.Json;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        ////////////////////////////////////////////////////////////

        #region Variabal

        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();

        private UserManagerEntities db_user = new UserManagerEntities();
        private HumanResourceEntities db_Hr = new HumanResourceEntities();
        private VNT86Entities db_VNT86 = new VNT86Entities();

        //important Variabal cookie in multi language
        private HttpCookie cookie;

        #endregion Variabal

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        #region Visio Booking

        // GET: Booking
        public ActionResult Booking()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Booking").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Passing data to view
            PassingDataToBooking();
            //
            return View();
        }

        //Passing data to view
        private void PassingDataToBooking()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            ViewBag.userID = User.Identity.GetUserName();
            var lstBooking = db.sp_BookingGet().ToList();
            ViewBag.lstBooking = lstBooking;
        }

        //Passing data to view
        private void PassingDataToBookingAdd()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            ViewBag.DateNow = DateTime.Now.AddDays(1);
        }

        // Add: Booking
        [HttpGet]
        public ActionResult BookingAdd()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Booking").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Passing data to view
            PassingDataToBookingAdd();
            //
            return View();
        }

        [HttpPost]
        public ActionResult BookingAdd(FormCollection fc)
        {
            try
            {
                //check email User
                string strUser = User.Identity.GetUserName();
                AspNetUser objUser = db_user.AspNetUsers.FirstOrDefault(m => m.UserName == strUser);
                string email = mFunction.ToString(objUser.Email);
                bool ckeck = email.Contains("nothing");
                if (email.Length <= 0 || ckeck == true)
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("PleaseUpdateYourEmail");
                    return View();
                }

                //get data from view
                string strStartDate = fc["StartDate"];
                string strEndDate = fc["EndDate"];
                string strSubject = mFunction.ToString(fc["Subject"]);
                string strContent = mFunction.ToString(fc["Content"]);

                //check data is not null
                if (strStartDate.Length <= 0 || strEndDate.Length <= 0 || strSubject.Length <= 0 || strContent.Length <= 0)
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("FromDate") + ", " + Language.label("ToDate") + ", " + Language.label("Subject") + ", " + Language.label("Content");
                    return View();
                }

                DateTime oStartDate = DateTime.Parse(strStartDate);
                DateTime oEndDate = mFunction.ToDateTime(strEndDate);

                string dateTomorrow = DateTime.Now.AddDays(1).ToString("MMM/dd/yyyy");
                //Check start date can not less than now
                if (oStartDate < mFunction.ToDateTime(dateTomorrow))
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("StartTimeMustBeGreaterThanTheCurrentDay");
                    return View();
                }

                //Check start date can not greater than or equal end date
                if (oStartDate >= oEndDate)
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("StartDateCanNotGreaterThanOrEqualEndDate");
                    return View();
                }

                //Check start date can not greater than or equal end date
                if (oStartDate.ToString("yyyyMMdd") != oEndDate.ToString("yyyyMMdd"))
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("YouAreOnlyAllowedToBookingDuringTheDay");
                    return View();
                }

                //Check start date and end date not exists in table booking
                List<Booking> objNotExists = db.Bookings.Where(m => ((m.StartDate <= oStartDate) && (m.EndDate >= oStartDate)) || ((m.StartDate <= oEndDate) && (m.EndDate >= oEndDate))).ToList();
                if (objNotExists.Count > 0)
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("DuplicateBookingTime");
                    return View();
                }

                //prepare data for data data to entity
                Booking obj = new Booking()
                {
                    UserName = User.Identity.GetUserName(),
                    StartDate = oStartDate,
                    EndDate = oEndDate,
                    Subject = strSubject,
                    Content = strContent,
                    TableName = "Booking",
                    Status = 1,
                    PCIDAdd = User.Identity.GetUserName(),
                    DateAdd = DateTime.Now
                };

                //Save log
                var objLog = db.sp_ActionLog("Booking", "Booking", "Insert", "Add Booking Data", strContent, User.Identity.GetUserName(), DateTime.Now);
                //Save data
                db.Bookings.Add(obj);
                db.SaveChanges();

                //send email
                tblDanhSachNhanVien objEmp = db_Hr.tblDanhSachNhanViens.FirstOrDefault(m => m.MaSoNhanVien == strUser);
                string name = objEmp.HoNhanVien + " " + objEmp.TenNhanVien;
                string strcontent = "User: " + strUser.ToString() + "-" + name + " was just add data.\n <br />";
                strcontent = strcontent + "Data content\n <br />[\n <br />";
                strcontent = strcontent + "Email: " + email + "\n <br />";
                strcontent = strcontent + "UserName: " + obj.UserName + "\n <br />";
                strcontent = strcontent + "Subject: " + obj.Subject + "\n <br />";
                strcontent = strcontent + "Content: " + obj.Content + "\n <br />";
                strcontent = strcontent + "StartDate: " + obj.StartDate + "\n <br />";
                strcontent = strcontent + "EndDate: " + obj.EndDate + "\n <br />";
                strcontent = strcontent + "]\n <br />";
                strcontent = strcontent + "Thanks for reading this email.";
                //BookingSendEmail("Add Booking Data", strcontent);
                BookingSendEmailMeeting("Add Booking Data", strcontent, obj.StartDate, obj.EndDate);

                return RedirectToAction("Booking");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Passing data to view
                PassingDataToBookingAdd();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View();
            }
        }

        //Passing data to view
        private void PassingDataToBookingEdit()
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
        }

        // Add: Booking
        [HttpGet]
        public ActionResult BookingEdit(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Booking").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Passing data to view
            PassingDataToBookingEdit();
            Booking obj = db.Bookings.FirstOrDefault(m => m.ID == id);
            if (obj == null)
            {
                //Passing data to view
                PassingDataToBooking();
                ViewBag.Message = "► " + Language.label("NotExistsObject");
                return View("Booking");
            }
            //
            return View(obj);
        }

        [HttpPost]
        public ActionResult BookingEdit(Booking obj, FormCollection fc)
        {
            try
            {
                //check email User
                string strUser = User.Identity.GetUserName();
                AspNetUser objUser = db_user.AspNetUsers.FirstOrDefault(m => m.UserName == strUser);
                string email = mFunction.ToString(objUser.Email);
                if (email.Length <= 0 || email == "nothing@framas.com.vn")
                {
                    //Passing data to view
                    PassingDataToBooking();
                    ViewBag.Message = "► " + Language.label("PleaseUpdateYourEmail");
                    return View("Booking");
                }

                //get data from view
                string strStartDate = fc["StartDate"];
                string strEndDate = fc["EndDate"];
                string strSubject = mFunction.ToString(fc["Subject"]);
                string strContent = mFunction.ToString(fc["Content"]);

                //check data is not null
                if (strStartDate.Length <= 0 || strEndDate.Length <= 0 || strSubject.Length <= 0 || strContent.Length <= 0)
                {
                    //Passing data to view
                    PassingDataToBookingEdit();
                    ViewBag.Message = "► " + Language.label("PleaseInputData") + ": " + Language.label("FromDate") + ", " + Language.label("ToDate") + ", " + Language.label("Subject") + ", " + Language.label("Content");
                    return View(obj);
                }

                DateTime oStartDate = DateTime.Parse(strStartDate);
                DateTime oEndDate = mFunction.ToDateTime(strEndDate);

                string dateTomorrow = DateTime.Now.AddDays(1).ToString("MMM/dd/yyyy");
                //Check start date can not less than now
                if (oStartDate < mFunction.ToDateTime(dateTomorrow))
                {
                    //Passing data to view
                    PassingDataToBookingEdit();
                    ViewBag.Message = "► " + Language.label("StartTimeMustBeGreaterThanTheCurrentDay");
                    return View(obj);
                }

                //Check start date can not greater than or equal end date
                if (oStartDate >= oEndDate)
                {
                    //Passing data to view
                    PassingDataToBookingAdd();
                    ViewBag.Message = "► " + Language.label("StartDateCanNotGreaterThanOrEqualEndDate");
                    return View(obj);
                }

                //Check start date can not greater than or equal end date
                if (oStartDate.ToString("yyyyMMdd") != oEndDate.ToString("yyyyMMdd"))
                {
                    //Passing data to view
                    PassingDataToBookingEdit();
                    ViewBag.Message = "► " + Language.label("YouAreOnlyAllowedToBookingDuringTheDay");
                    return View(obj);
                }

                List<Booking> lstBooking = db.Bookings.Where(m => m.ID != obj.ID).ToList();

                //Check start date and end date not exists in table booking
                List<Booking> objNotExists = lstBooking.Where(m => ((m.StartDate <= oStartDate) && (m.EndDate >= oStartDate)) || ((m.StartDate <= oEndDate) && (m.EndDate >= oEndDate))).ToList();
                if (objNotExists.Count > 0)
                {
                    //Passing data to view
                    PassingDataToBookingEdit();
                    ViewBag.Message = "► " + Language.label("DuplicateBookingTime");
                    return View(obj);
                }

                Booking objBooking = db.Bookings.FirstOrDefault(m => m.ID == obj.ID);
                objBooking.Subject = strSubject;
                objBooking.Content = strContent;
                objBooking.EndDate = oEndDate;
                objBooking.StartDate = oStartDate;
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Booking", "Booking", "Update", "Update Booking Data", strContent, User.Identity.GetUserName(), DateTime.Now);

                //send email
                tblDanhSachNhanVien objEmp = db_Hr.tblDanhSachNhanViens.FirstOrDefault(m => m.MaSoNhanVien == strUser);
                string name = objEmp.HoNhanVien + " " + objEmp.TenNhanVien;
                string strcontent = "User: " + strUser.ToString() + "-" + name + " was just update data.\n <br />";
                strcontent = strcontent + "Data content\n <br />[\n <br />";
                strcontent = strcontent + "Email: " + email + "\n <br />";
                strcontent = strcontent + "UserName: " + obj.UserName + "\n <br />";
                strcontent = strcontent + "Subject: " + obj.Subject + "\n <br />";
                strcontent = strcontent + "Content: " + obj.Content + "\n <br />";
                strcontent = strcontent + "StartDate: " + obj.StartDate + "\n <br />";
                strcontent = strcontent + "EndDate: " + obj.EndDate + "\n <br />";
                strcontent = strcontent + "]\n <br />";
                strcontent = strcontent + "Thanks for reading this email.";
                BookingSendEmail("Update Booking Data", strcontent);

                return RedirectToAction("Booking");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Passing data to view
                PassingDataToBookingEdit();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View(obj);
            }
        }

        // Delete
        public ActionResult BookingDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Booking").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //check email User
                string strUser = User.Identity.GetUserName();
                AspNetUser objUser = db_user.AspNetUsers.FirstOrDefault(m => m.UserName == strUser);
                string email = mFunction.ToString(objUser.Email);
                if (email.Length <= 0 || email == "nothing@framas.com.vn")
                {
                    //Passing data to view
                    PassingDataToBooking();
                    ViewBag.Message = "► " + Language.label("PleaseUpdateYourEmail");
                    return View("Booking");
                }

                Booking obj = db.Bookings.FirstOrDefault(m => m.ID == id);
                if (obj == null)
                {
                    //Passing data to view
                    PassingDataToBooking();
                    ViewBag.Message = "► " + Language.label("NotExistsObject");
                    return View("Booking");
                }
                //Delete data
                db.Bookings.Remove(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("Booking", "Booking", "Delete", "Delete Booking Data", "", User.Identity.GetUserName(), DateTime.Now);

                //send email
                tblDanhSachNhanVien objEmp = db_Hr.tblDanhSachNhanViens.FirstOrDefault(m => m.MaSoNhanVien == strUser);
                string name = objEmp.HoNhanVien + " " + objEmp.TenNhanVien;
                string strcontent = "User: " + strUser.ToString() + "-" + name + " was just detete data.\n <br />";
                strcontent = strcontent + "Data content\n <br />[\n <br />";
                strcontent = strcontent + "Email: " + email + "\n <br />";
                strcontent = strcontent + "UserName: " + obj.UserName + "\n <br />";
                strcontent = strcontent + "Subject: " + obj.Subject + "\n <br />";
                strcontent = strcontent + "Content: " + obj.Content + "\n <br />";
                strcontent = strcontent + "StartDate: " + obj.StartDate + "\n <br />";
                strcontent = strcontent + "EndDate: " + obj.EndDate + "\n <br />";
                strcontent = strcontent + "]\n <br />";
                strcontent = strcontent + "Thanks for reading this email.";
                BookingSendEmail("Delete Booking Data", strcontent);

                //Return view home
                return RedirectToAction("Booking");
            }
            catch
            {
                //Passing data to view
                PassingDataToBooking();
                ViewBag.Message = "► " + Language.label("ErrorSave");
                return View("Booking");
            }
        }

        //Send email to IT when data change
        private void BookingSendEmail(string strSubject, string strContent)
        {
            List<MailSupport> lstMail = db.MailSupports.Where(m => (m.Status == 1) && (m.Name == "Booking")).ToList();
            foreach (var item in lstMail)
            {
                SmtpClient mailClient;
                mailClient = new SmtpClient
                {
                    Credentials = new NetworkCredential("adv@framas.com.vn", "123@123"),
                    Host = "192.168.99.99",
                    EnableSsl = true,
                    Port = 25
                };
                MailMessage msgMail = null/* TODO Change to default(_) if this is not a reference type */;
                msgMail = new MailMessage
                {
                    From = new MailAddress("adv@framas.com.vn", "FVN Visio Booking")
                };
                msgMail.To.Add(new MailAddress(item.Mail));
                //msgMail.CC.Add(new MailAddress(user.Email));
                msgMail.Subject = "FVN Visio Booking - " + strSubject;
                msgMail.Body = strContent;
                msgMail.IsBodyHtml = true;
                ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                mailClient.Send(msgMail);
                msgMail.Dispose();
            }
        }

        private void BookingSendEmailMeeting(string strSubject, string strContent, DateTime startTime, DateTime endTime)
        {
            List<MailSupport> lstMail = db.MailSupports.Where(m => (m.Status == 1) && (m.Name == "Booking")).ToList();
            foreach (var item in lstMail)
            {
                ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;

                SmtpClient sc;
                sc = new SmtpClient
                {
                    Credentials = new NetworkCredential("adv@framas.com.vn", "123@123"),
                    Host = "192.168.99.99",
                    EnableSsl = true,
                    Port = 25
                };

                MailMessage msg = new MailMessage
                {
                    From = new MailAddress("adv@framas.com.vn", "Admin")
                };
                msg.To.Add(new MailAddress(item.Mail, "IT Department"));
                msg.Subject = strSubject;
                msg.Body = strContent;

                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Ahmed Abu Dagga Blog");
                str.AppendLine("VERSION:2.0");
                str.AppendLine("METHOD:REQUEST");
                str.AppendLine("BEGIN:VEVENT");
                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmss}", startTime));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmss}", DateTime.Now));
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmss}", endTime));
                str.AppendLine("LOCATION: Framas Viet Nam");
                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
                str.AppendLine(string.Format("DESCRIPTION:{0}", msg.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", msg.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");
                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
                ct.Parameters.Add("method", "REQUEST");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), ct);
                msg.AlternateViews.Add(avCal);

                sc.Send(msg);

                msg.Dispose();
            }
        }

        #endregion Visio Booking

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        #region Saturday off planning

        // GET: SaturdayOff
        public ActionResult SaturdayOff(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "SaturdayOff").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Passing data to view
            ViewBag.Dept = mFunction.ToString(fc["Department"]).ToString();
            DateTime dtSearch = mFunction.ToDateTime(fc["dtDateSearch"]);
            PassingDataToSaturdayOff(ViewBag.Dept, dtSearch);
            //
            return View();
        }

        //Passing data to view
        public void PassingDataToSaturdayOff(string deptID, DateTime dtSearch)
        {
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var strUserID = User.Identity.GetUserName();
            if (strUserID != null)
            {
                ViewBag.strUserID = strUserID;
                var objDept = db_Hr.sp_Employee_Get().FirstOrDefault(m => m.MaSoNhanVien == strUserID);
                if (objDept != null)
                {
                    ViewBag.strDeptName = mFunction.ToString(objDept.BoPhan).Trim();
                    ViewBag.strDeptID = mFunction.ToString(objDept.MaBoPhan).Trim();
                }
            }
            ViewBag.dtDateSearch = dtSearch;
            var lstDepartment = db_Hr.MaBoPhans.OrderBy(m => m.TenBoPhanA).ToList();
            ViewBag.lstDepartment = lstDepartment;
            var lstSaturdayOffPlanning = db.sp_SaturdayOffPlanning_Get(dtSearch, deptID).ToList();
            ViewBag.lstSaturdayOffPlanning = lstSaturdayOffPlanning;
        }

        // Add Saturday Off
        [HttpPost]
        public ActionResult SaturdayOffAdd(FormCollection fc)
        {
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "SaturdayOff").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Passing data to view
            ViewBag.Dept = mFunction.ToString(fc["Department"]).ToString();
            DateTime dtSearch = mFunction.ToDateTime(fc["dtDateSearch"]);
            PassingDataToSaturdayOff(ViewBag.Dept, dtSearch);

            try
            {
                //Get data from view
                string strDepartment = fc["Department"];
                string strUser = User.Identity.GetUserName();
                string strDateOff = mFunction.ToString(fc["DateOff"]);
                DateTime dtDateOff = mFunction.ToDateTime(strDateOff);
                string strContent = fc["Content"];

                //check data is not null
                if (strDepartment.Length <= 0 || strDateOff.Length <= 0)
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("PleaseInputData") + ": " + Language.label("Date");
                    return View("SaturdayOff");
                }

                //Check month great than current month
                if (dtDateOff.Month <= DateTime.Now.Month)
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("DateTimeMustBeGreaterThanTheCurrentMonth");
                    return View("SaturdayOff");
                }

                //check date is saturday
                string strDayName = NgayTrongTuan(dtDateOff.Year, dtDateOff.Month, dtDateOff.Day);
                if (strDayName != "7")
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("CanYouChooseOnlySaturday");
                    return View("SaturdayOff");
                }

                //Check count employees  in department(Kiểm tra số lượng nhân viên của phòng ban)
                var countUser = (from a in db_Hr.tblDanhSachNhanViens
                                 join b in db_Hr.tblHoSoLyLichNhanViens
                                 on a.MaSoNhanVien equals b.MaSoNhanVien
                                 where (a.Active == true && b.BoPhan == strDepartment)
                                 select (a.MaSoNhanVien)
                                 ).ToList();

                //Check count employees off saturday in department at the weekend.(Kiểm tra số lượng nhân viên đã xin nghỉ của phòng ban đó')
                var checkUserOff = db.SaturdayOffPlannings.Where(x => (x.DateOff.Day == dtDateOff.Day) && (x.DateOff.Month == dtDateOff.Month) && (x.DateOff.Year == dtDateOff.Year)).Select(x => x.UserName).ToList();
                double checkPercen = ((checkUserOff.Count + 1) * 100) / countUser.Count;// Cộng thêm 1 vì tính luôn data hiện tại chuẩn bị lưu vào nếu hợp lệ
                if (checkPercen >= 50)
                {
                    TempData["message"] = "► " + Language.label("SaturdayOffPOver");
                    return View("SaturdayOff");
                }

                //Check only one booking in month
                var objCheck = db.SaturdayOffPlannings.Where(x => (x.UserName == strUser) && (x.DateOff.Month == dtDateOff.Month) && (x.DateOff.Year == dtDateOff.Year)).ToList();
                if (objCheck.Count > 0)
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("ExistsObject");
                    return View("SaturdayOff");
                }

                //prepare data for data data to entity
                SaturdayOffPlanning obj = new SaturdayOffPlanning()
                {
                    UserName = strUser,
                    Content = strContent,
                    DepartmentID = strDepartment,
                    TableName = "SaturdayOffPlanning",
                    DateOff = dtDateOff,
                    PCIDAdd = User.Identity.GetUserName(),
                    DateAdd = DateTime.Now
                };

                //Save log
                var objLog = db.sp_ActionLog("SaturdayOff", "Booking", "Insert", "Add Saturday Off Data", strContent, User.Identity.GetUserName(), DateTime.Now);
                //Save data
                db.SaturdayOffPlannings.Add(obj);
                db.SaveChanges();
                TempData["message"] = "► " + Language.label("CommandsCompletedSuccessfully");
                return View("SaturdayOff");
            }
            catch (Exception ex)
            {
                ex.ToString();
                //Passing data to view
                ViewBag.Message = "► " + Language.label("ErrorSave");

                return View("SaturdayOff");
            }
        }

        // Delete Saturday Off
        public ActionResult SaturdayOffDel(int id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "SaturdayOff").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //
            SaturdayOffPlanning obj = db.SaturdayOffPlannings.FirstOrDefault(m => m.ID == id);
            if (obj == null)
            {
                TempData["message"] = "► " + Language.label("NotExistsObject");
                return RedirectToAction("SaturdayOff");
            }
            //Delete data
            try
            {
                db.SaturdayOffPlannings.Remove(obj);
                db.SaveChanges();
                //Save log
                var objLog = db.sp_ActionLog("SaturdayOff", "Booking", "Delete", "Del Saturday Off Data", "", User.Identity.GetUserName(), DateTime.Now);
                TempData["message"] = "► " + Language.label("CommandsCompletedSuccessfully");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "► " + Language.label("ErrorSave" + ex);
                throw;
            }
            return RedirectToAction("SaturdayOff");
        }

        //Get Edit SaturdayOff
        [HttpGet]
        public ActionResult SaturdayOffEdit(int? Id)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "SaturdayOff").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var data = db.SaturdayOffPlannings.FirstOrDefault(n => n.ID == Id);
            if (data == null)
            {
                return RedirectToAction("SaturdayOff");
            }
            //Passing data
            SendDataSaturdayOffEdit();
            if (data != null)
            {
                var objDept = db_Hr.sp_Employee_Get().FirstOrDefault(m => m.MaSoNhanVien == data.UserName);
                if (objDept != null)
                {
                    ViewBag.strDeptName = mFunction.ToString(objDept.BoPhan).Trim();
                    ViewBag.strDeptID = mFunction.ToString(objDept.MaBoPhan).Trim();
                }
                return PartialView("_SaturdayOffEdit", data);
            }
            return PartialView();
        }

        public void SendDataSaturdayOffEdit()
        {
        }

        //HttpPost SaturdayOffEdit
        [HttpPost]
        public ActionResult SaturdayOffEdit(FormCollection fc)
        {
            //Get data from view
            int Id = mFunction.ToInt(fc["ID"]);
            string strDeptID = fc["strDeptID"];
            string strUser = User.Identity.GetUserName();
            string strDateOff = mFunction.ToString(fc["DateOff"]);
            DateTime dtDateOff = mFunction.ToDateTime(strDateOff);
            string strContent = fc["Content"];
            try
            {
                //check data is not null
                if (strDateOff == string.Empty)
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("PleaseInputData") + ": " + Language.label("Date");
                    return View("SaturdayOff");
                }

                //Check month great than current month
                if (dtDateOff.Month <= DateTime.Now.Month)
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("DateTimeMustBeGreaterThanTheCurrentMonth");
                    return View("SaturdayOff");
                }

                //check date is saturday
                string strDayName = NgayTrongTuan(dtDateOff.Year, dtDateOff.Month, dtDateOff.Day);
                if (strDayName != "7")
                {
                    //Passing data to view
                    TempData["message"] = "► " + Language.label("CanYouChooseOnlySaturday");
                    return View("SaturdayOff");
                }

                //Check only one booking in month
                var data = db.SaturdayOffPlannings.FirstOrDefault(x => x.ID == Id);
                if (data != null)
                {
                    data.Content = strContent;
                    data.DateOff = dtDateOff;
                    data.PCIDAdd = User.Identity.GetUserName();
                    data.DateAdd = DateTime.Now;
                    //Save data
                    db.SaveChanges();
                    //Save log
                    var objLog = db.sp_ActionLog("SaturdayOff", "Booking", "Insert", "Add Saturday Off Data", strContent, User.Identity.GetUserName(), DateTime.Now);
                    TempData["message"] = "► " + Language.label("CommandsCompletedSuccessfully");
                }
                return RedirectToAction("SaturdayOff", "Booking");
            }
            catch
            {
                //Passing data to view
                SendDataSaturdayOffEdit();
                TempData["message"] = "► " + Language.label("ErrorSave");
                return View("SaturdayOff");
            }
        }

        public ActionResult SaturdayOffPrint(DateTime dtSearch, string dept)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "SaturdayOff").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var lstSaturdayOffPlanning = db.sp_SaturdayOffPlanning_Get(dtSearch, dept).ToList();
            ViewBag.lstSaturdayOffPlanning = lstSaturdayOffPlanning;

            //// CheckCookie and ChangeLanguage always have in action (it pass data to View)
            //CheckCookie();
            //ChangeLanguage(cookie.Value);
            //rpt_Test rpt = new rpt_Test();
            //DataTable dt = DataOperation.getDataTable(GlobalVariable.DBMenuManager, "exec sp_SaturdayOffPlanning_Get '" + dtSearch.ToString("dd/MMM/yyyy") + "','" + dept + "'");
            //rpt.DataSource = dt;
            //rpt.DataMember = "dt";
            return RedirectToAction("Index", "Report", new { strQr = "exec sp_SaturdayOffPlanning_Get '" + dtSearch.ToString("dd/MMM/yyyy") + "','" + dept + "'" });

            //return RedirectToAction("Index", "Report");
        }

        private bool LaNamNhuan(int Nam)
        {
            if (Nam % 4 != 0) return false;
            if (Nam % 100 != 0) return true;
            if (Nam % 400 != 0) return false;
            return true;
        }

        private int SoNgayTrongNam(int Nam)
        {
            if (LaNamNhuan(Nam)) return 366;
            return (365);
        }

        private int SoNgayTruocNam(int Nam)
        {
            int TongSoNgayTruoc = 0;
            for (int i = 1; i < Nam; i += 1)
                TongSoNgayTruoc += SoNgayTrongNam(i);
            return TongSoNgayTruoc;
        }

        private int SoNgayTrongThang(int Nam, int Thang)
        {
            switch (Thang)
            {
                case 4:
                case 6:
                case 9:
                case 11: return 30;
                case 2:
                    {
                        if (LaNamNhuan(Nam)) return 29;
                        return 28;
                    }
                default: return 31;
            }
        }

        private int SoNgayTruocThang(int Nam, int Thang)
        {
            var SoNgay = 0;
            for (int i = 1; i < Thang; i += 1)
                SoNgay += SoNgayTrongThang(Nam, i);
            return SoNgay;
        }

        private int TongSoNgay(int Nam, int Thang, int Ngay)
        {
            return SoNgayTruocNam(Nam) + SoNgayTruocThang(Nam, Thang) + Ngay;
        }

        private string NgayTrongTuan(int Nam, int Thang, int Ngay)
        {
            switch (TongSoNgay(Nam, Thang, Ngay) % 7)
            {
                case 0: return "8";
                case 1: return "2";
                case 2: return "3";
                case 3: return "4";
                case 4: return "5";
                case 5: return "6";
                default: return "7";
            }
        }

        #endregion Saturday off planning

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////



        #region Finance DA Show By Char
        // GET: FinanceCharDA
        public ActionResult FinanceCharDA(FormCollection fc)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "FinanceCharDA").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);

            //Passing data to view
            var lstDA = db_VNT86.sp_FinanceDAGet().ToList();
            ViewBag.lstDA = lstDA;
            //
            return View();
        }


        //Get Edit Roles
        [HttpGet]
        public ActionResult FinanceChartDetail(string productCode)
        {
            //Passing data to view
            var data = db_VNT86.sp_FinanceDAByProduct(productCode).ToList();

            List<DataPoint> dataPointsLineAmo = new List<DataPoint>();
            List<DataPoint> dataPointsLineSum = new List<DataPoint>();
            List<DataPoint> dataPointsColumnsQtyByMonth = new List<DataPoint>();

            double sumByMonth = 0;
            foreach (var item in data)
            {
                dataPointsLineAmo.Add(new DataPoint(item.C032, mFunction.ToDouble(item.AmoQty)));
                sumByMonth = sumByMonth + mFunction.ToDouble(item.Qty);
                dataPointsLineSum.Add(new DataPoint(item.C032, mFunction.ToDouble(sumByMonth)));
                dataPointsColumnsQtyByMonth.Add(new DataPoint(item.C032, mFunction.ToDouble(item.Qty)));
            }

            ViewBag.dataPointsLineAmo = JsonConvert.SerializeObject(dataPointsLineAmo);
            ViewBag.dataPointsLineSum = JsonConvert.SerializeObject(dataPointsLineSum);
            ViewBag.dataPointsColumnsQtyByMonth = JsonConvert.SerializeObject(dataPointsColumnsQtyByMonth);

            return PartialView("/Views/Booking/_FinanceCharDA.cshtml");
        }
        #endregion


        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////

        #region salary

        // GET: SaturdayOff
        public ActionResult salary(FormCollection fc)
        {

            CheckCookie();
            ChangeLanguage(cookie.Value);
            return View();
        }
        public ActionResult SalaryPrint(FormCollection fc)
        {
            return RedirectToAction("Index", "Report", new { strQr = "exec sp_SaturdayOffPlanning_Get '',''" });
        }

        #endregion salary

        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////



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
    }
}