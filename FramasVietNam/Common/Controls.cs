using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace FramasVietNam.Common
{
    //List controls setup permission
    public static class Controls
    {
        //create button
        public static MvcHtmlString ButtonRoles(this HtmlHelper helper, string button, string action, string controller, string innerHtml, object htmlAttributes)
        {
            return ButtonRoles(helper, button, action, controller, innerHtml,
                          HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
        }

        public static MvcHtmlString ButtonRoles(this HtmlHelper helper, string button, string action, string controller, string innerHtml, IDictionary<string, object> htmlAttributes)
        {
            //if (HttpContext.Current.User.IsInRole(roles))
            if (CheckingPermission(button, action, controller) == true)
            {
                var builder = new TagBuilder("button")
                {
                    InnerHtml = innerHtml
                };
                builder.MergeAttributes(htmlAttributes);
                return MvcHtmlString.Create(builder.ToString());
            }
            return MvcHtmlString.Empty;
        }
        //end button

        //create action link
        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, object routeValues)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, string controllerName)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }
        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, string controllerName, object htmlAttributes)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return htmlHelper.ActionLinkRole(button, action, controller, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {

            //if (HttpContext.Current.User.IsInRole(roles))
            if (CheckingPermission(button, action, controller) == true)
            {
                return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
            }
            return MvcHtmlString.Empty;
        }

        //Checking permission Of user with show modals
        public static MvcHtmlString ActionLinkRole(this HtmlHelper htmlHelper, string button, string action, string controller, string linkText, object htmlAttributes)
        {

            //if (HttpContext.Current.User.IsInRole(roles))
            if (CheckingPermission(button, action, controller) == true)
            {
                //public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes);
                return htmlHelper.ActionLink(linkText, "","", htmlAttributes);
            }
            return MvcHtmlString.Empty;
        }
        //end actionlink

        //Checking permission Of user
        private static bool CheckingPermission(string strButton, string strAction, string strController)
        {
            string _strUser = mFunction.ToString(HttpContext.Current.User.Identity.Name);
            DataTable _dtResult = DataOperation.getDataTable(GlobalVariable.DBUserManager, "exec sp_CheckingRole '" + strButton + "', '" + _strUser + "', '" + strAction + "', '" + strController + "'");
            if (_dtResult.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

    }
}