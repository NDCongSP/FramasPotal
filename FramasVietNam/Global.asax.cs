using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FramasVietNam
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Default;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //// Start Sql dependency to track db changes
            //SqlDependency.Start(@"data source=FVN-PC-IT-07\SQLEXPRESS;initial catalog=FingerScan;user id=sa;password=1234;");
            SqlDependency.Start(@"data source=APP01;initial catalog=FingerScan;user id=sa;password=Fdw24$110 ;");
        }
    }
}
