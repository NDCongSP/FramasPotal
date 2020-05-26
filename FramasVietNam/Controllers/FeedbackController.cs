using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.Meal;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        private MealControlEntities db_meal = new MealControlEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        //private object ds_Functions;
        // GET: Feedback
        public ActionResult Feedback()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "Feedback").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Pass data to feedback
            PassDataToFeedback();
            return View();
        }

        //Pass data to feedback
        private void PassDataToFeedback()
        {
            CheckCookie();
            ChangeLanguage(cookie.Value);
            //pass data to view feedback
            List<Feedback> lst_Feedback = db_meal.Feedbacks.Where(m => m.Status == 1).OrderByDescending(o=>o.DateAdd).ToList();
            List<Feedback_Reply> lst_Feedback_Reply = db_meal.Feedback_Reply.Where(m => m.Status == 1).ToList();
            ViewBag.lst_Feedback = lst_Feedback;
            ViewBag.lst_Feedback_Reply = lst_Feedback_Reply;
        }

        // function post comment to cannten
        [HttpPost]
        public ActionResult FeedbackPost(FormCollection fc)
        {
            //get data comment from view
            string str_Cmt = mFunction.ToString(fc["txt_Cmt"]).Trim();
            //check data null
            if (str_Cmt == "")
            {
                ViewBag.Message = Language.label("PleaseInputData");
                //Pass data to feedback
                PassDataToFeedback();
                return View("Feedback");
            }
            //save data
            Feedback obj = new Feedback()
            {
                UserName=User.Identity.GetUserName(),
                TextContain = str_Cmt,
                Status = 1,
                UserAdd = User.Identity.GetUserName(),
                DateAdd = DateTime.Now
            };
            db_meal.Feedbacks.Add(obj);
            db_meal.SaveChanges();
            //Save log
            var objLog = db_meal.sp_ActionLog("Feedback", "Feedback", "Insert", "Add Feedback", str_Cmt, User.Identity.GetUserName(), DateTime.Now);
            return RedirectToAction("Feedback");
        }

        // function post reply comment to cannten
        [HttpPost]
        public ActionResult FeedbackReply(string id, string text)
        {
            //get data comment from view
            //check data null
            if (text.Trim() == "")
            {
                ViewBag.Message = Language.label("PleaseInputData");
                //Pass data to feedback
                PassDataToFeedback();
                return View("Feedback");
            }
            //save data
            Feedback_Reply obj = new Feedback_Reply()
            {
                ConversationsID = mFunction.ToInt(id),
                UserName = User.Identity.GetUserName(),
                TextContain = text,
                Status = 1,
                UserAdd = User.Identity.GetUserName(),
                DateAdd = DateTime.Now
            };
            db_meal.Feedback_Reply.Add(obj);
            db_meal.SaveChanges();
            //Save log
            var objLog = db_meal.sp_ActionLog("Feedback", "Feedback", "Insert", "Reply Feedback", text, User.Identity.GetUserName(), DateTime.Now);
            //Pass data to feedback
            PassDataToFeedback();
            return View("Feedback");
        }

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
    }
}