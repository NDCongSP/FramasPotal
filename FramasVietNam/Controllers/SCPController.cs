using FramasVietNam.Common;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.SCP;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class SCPController : Controller
    {

        ////////////////////////////////////////////////////////////
        //Variabal
        #region Variabal
        //important Variabal for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        private SCPEntities db_SCP = new SCPEntities();
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region UploadFileExcel
        // GET: UploadFileExcel
        public ActionResult UploadFileExcel()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UploadFileExcel").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            //
            return View();
        }
        // post: UploadFileExcel
        [HttpPost]
        public ActionResult UploadFileExcel(HttpPostedFileBase file)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UploadFileExcel").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                                     System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/Excel/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                          System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    }

                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                    excelConnection.Close();
                    excelConnection1.Close();
                }

                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/Excel/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    // DataSet ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }

                if ((ds.Tables[0].Rows.Count) > 0)
                {
                    string conn = ConfigurationManager.ConnectionStrings["dbSCI"].ConnectionString;
                    SqlConnection con = new SqlConnection(conn);
                    string query = "truncate table [dbo].[scp1]";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SqlConnection con1 = new SqlConnection(conn);
                        string query1 = "Insert into [dbo].[scp1]([Year] ,[Month] ,[Material Type],[Vendor Name],[Factory Group],[Factory Country],[Factory],[Material Shipped From Country],[Brand],[Order Quantity],[Shipped Quantity],[RFT],[SDP],[MDP],[OTIF],[PML],[PMT],[TLT],[Develop Quantity],[SDPs],[DML],[SCP],[SCP Rank]) Values(N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][0].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][1].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][2].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][3].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][4].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][5].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][6].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][7].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][8].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][9].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][10].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][11].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][12].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][13].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][14].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][15].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][16].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][17].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][18].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][19].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][20].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][21].ToString()) + "',N'" +
                        //  ds.Tables[0].Rows[i][22].ToString() + "','" + ds.Tables[0].Rows[i][23].ToString() + "','" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][22].ToString()) + "')";
                        con1.Open();
                        SqlCommand cmd1 = new SqlCommand(query1, con1);
                        cmd1.ExecuteNonQuery();
                        con1.Close();
                    }

                    SqlConnection con2 = new SqlConnection(conn);
                    string query2 = "exec dbo.exe_data";
                    con2.Open();
                    SqlCommand cmd2 = new SqlCommand(query2, con2);
                    cmd2.ExecuteNonQuery();
                    con2.Close();
                }

            }
            ViewBag.err = "Upload file complete!";
            CheckCookie();
            ChangeLanguage(cookie.Value);
            return View();
        }
        //HandleSpecialCharacter
        private String HandleSpecialCharacter(String s)
        {
            if (s.Contains("'"))
            {
                s = s.Replace("'", "''");
            }
            return s;
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region UploadFileExcelCategory
        // GET: UploadFileExcelCategory
        public ActionResult UploadFileExcelCategory()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UploadFileExcelCategory").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            //
            return View();
        }
        [HttpPost]
        public ActionResult UploadFileExcelCategory(HttpPostedFileBase file)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "UploadFileExcelCategory").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                                     System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/Excel/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\"";

                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\"";
                    }

                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                                
                    excelConnection.Close();
                    excelConnection1.Close();
                    GC.Collect();
                }

                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/Excel/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    // DataSet ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }

                if ((ds.Tables[0].Rows.Count) > 0)
                {
                    string conn = ConfigurationManager.ConnectionStrings["dbSCI"].ConnectionString;
                    SqlConnection con = new SqlConnection(conn);
                    string query = "truncate table [dbo].[Scp_caregory_Template]";
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        //SqlConnection con1 = new SqlConnection(conn);
                        //string query1 = "Insert into [dbo].[scp1]([Year] ,[Month] ,[Material Type],[Vendor Name],[Factory Group],[Factory Country],[Factory],[Material Shipped From Country],[Brand],[Order Quantity],[Shipped Quantity],[RFT],[SDP],[MDP],[OTIF],[PML],[PMT],[TLT],[Develop Quantity],[SDPs],[DML],[SCP],[SCP Rank]) Values(N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][0].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][1].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][2].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][3].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][4].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][5].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][6].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][7].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][8].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][9].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][10].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][11].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][12].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][13].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][14].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][15].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][16].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][17].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][18].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][19].ToString()) + "',N'" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][20].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][21].ToString()) + "',N'" +
                        ////  ds.Tables[0].Rows[i][22].ToString() + "','" + ds.Tables[0].Rows[i][23].ToString() + "','" +
                        //HandleSpecialCharacter(ds.Tables[0].Rows[i][22].ToString()) + "')";
                        //con1.Open();
                        //SqlCommand cmd1 = new SqlCommand(query1, con1);
                        //cmd1.ExecuteNonQuery();
                        //con1.Close();

                        SqlConnection con1 = new SqlConnection(conn);
                        string query1 = "Insert into [dbo].[Scp_caregory_Template]([Vendor],[Factory],[PO number],[Material Name],[Material shipped from country],[Order Quantity],[Order Date],[Factory requested at  Agreed Turnover Point],[Supplier 1st confirm date],[Agreed Turnover Point],[Arrival at turnover point],[Arrival at Factory],[Shipped Quantity],[Reject Quantity],[RFT reason code],[Delay Reason Code_],[RFT],[SDP],[MDP],[PML],[OTIF (RFT*SDP)],[PMT],[MTL UOM For Hua Feng_(Yard / Pair)],[Order confirmation Date (T2)],[order cfm date],[vendor1],[fty],[country],[rftcode],[rft qty],[fty reqst date],[1st cfm date],[to turnover point],[to fty],[fty vs turnover point],[SDP vs MDP],[shipped qty vs order qty],[TOOLING],[CATOGERY]) Values(N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][0].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][1].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][2].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][3].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][4].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][5].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][6].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][7].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][8].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][9].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][10].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][11].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][12].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][13].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][14].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][15].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][16].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][17].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][18].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][19].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][20].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][21].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][22].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][23].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][24].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][25].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][26].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][27].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][28].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][29].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][30].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][31].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][32].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][33].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][34].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][35].ToString()) + "',N'" +
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][36].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][37].ToString()) + "',N'" +
                        //ds.Tables[0].Rows[i][38].ToString() + "','" + ds.Tables[0].Rows[i][39].ToString() + "','" +                        
                        HandleSpecialCharacter(ds.Tables[0].Rows[i][38].ToString()) + "')";
                        con1.Open();
                        SqlCommand cmd1 = new SqlCommand(query1, con1);
                        cmd1.ExecuteNonQuery();
                        con1.Close();
                    }

                    SqlConnection con2 = new SqlConnection(conn);
                    string query2 = "exec dbo.exe_data_category";
                    con2.Open();
                    SqlCommand cmd2 = new SqlCommand(query2, con2);
                    cmd2.ExecuteNonQuery();
                    con2.Close();
                }

            }

            //if (lst_Check.Count <= 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //DataSet ds = new DataSet();
            //if (Request.Files["file"].ContentLength > 0)
            //{
            //    string fileExtension =
            //                        System.IO.Path.GetExtension(Request.Files["file"].FileName);

            //    if (fileExtension == ".xls" || fileExtension == ".xlsx")
            //    {
            //        string fileLocation = Server.MapPath("~/Content/Excel/") + Request.Files["file"].FileName;
            //        if (System.IO.File.Exists(fileLocation))
            //        {
            //            System.IO.File.Delete(fileLocation);
            //        }

            //        Request.Files["file"].SaveAs(fileLocation);
            //        string excelConnectionString = string.Empty;
            //        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
            //        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

            //        //connection String for xls file format.
            //        if (fileExtension == ".xls")
            //        {
            //            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
            //            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
            //        }
            //        //connection String for xlsx file format.
            //        else if (fileExtension == ".xlsx")
            //        {
            //            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
            //            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            //        }

            //        //Create Connection to Excel work book and add oledb namespace
            //        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
            //        excelConnection.Open();
            //        DataTable dt = new DataTable();

            //        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //        if (dt == null)
            //        {
            //            return null;
            //        }

            //        String[] excelSheets = new String[dt.Rows.Count];
            //        int t = 0;
            //        //excel data saves in temp file here.
            //        foreach (DataRow row in dt.Rows)
            //        {
            //            excelSheets[t] = row["TABLE_NAME"].ToString();
            //            t++;
            //        }
            //        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


            //        string query = string.Format("Select * from [{0}]", excelSheets[0]);
            //        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
            //        {
            //            dataAdapter.Fill(ds);
            //        }
            //        excelConnection.Close();
            //        excelConnection1.Close();
            //    }

            //    if (fileExtension.ToString().ToLower().Equals(".xml"))
            //    {
            //        string fileLocation = Server.MapPath("~/Content/Excel/") + Request.Files["FileUpload"].FileName;
            //        if (System.IO.File.Exists(fileLocation))
            //        {
            //            System.IO.File.Delete(fileLocation);
            //        }

            //        Request.Files["FileUpload"].SaveAs(fileLocation);
            //        XmlTextReader xmlreader = new XmlTextReader(fileLocation);
            //        // DataSet ds = new DataSet();
            //        ds.ReadXml(xmlreader);
            //        xmlreader.Close();
            //    }

            //    if ((ds.Tables[0].Rows.Count) > 0)
            //    {
            //        string conn = ConfigurationManager.ConnectionStrings["dbSCI"].ConnectionString;
            //        SqlConnection con = new SqlConnection(conn);
            //        string query = "truncate table [dbo].[Scp_caregory_Template]";
            //        con.Open();
            //        SqlCommand cmd = new SqlCommand(query, con);
            //        cmd.ExecuteNonQuery();
            //        con.Close();
            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            SqlConnection con1 = new SqlConnection(conn);
            //            string query1 = "Insert into [dbo].[Scp_caregory_Template]([Vendor],[Factory],[PO number],[Material Name],[Material shipped from country],[Order Quantity],[Order Date],[Factory requested at  Agreed Turnover Point],[Supplier 1st confirm date],[Agreed Turnover Point],[Arrival at turnover point],[Arrival at Factory],[Shipped Quantity],[Reject Quantity],[RFT reason code],[Delay Reason Code_],[RFT],[SDP],[MDP],[PML],[OTIF (RFT*SDP)],[PMT],[MTL UOM For Hua Feng_(Yard / Pair)],[Order confirmation Date (T2)],[order cfm date],[vendor1],[fty],[country],[rftcode],[rft qty],[fty reqst date],[1st cfm date],[to turnover point],[to fty],[fty vs turnover point],[SDP vs MDP],[shipped qty vs order qty],[TOOLING],[CATOGERY]) Values(N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][0].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][1].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][2].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][3].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][4].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][5].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][6].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][7].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][8].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][9].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][10].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][11].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][12].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][13].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][14].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][15].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][16].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][17].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][18].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][19].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][20].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][21].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][22].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][23].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][24].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][25].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][26].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][27].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][28].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][29].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][30].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][31].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][32].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][33].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][34].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][35].ToString()) + "',N'" +
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][36].ToString()) + "',N'" + HandleSpecialCharacter(ds.Tables[0].Rows[i][37].ToString()) + "',N'" +
            //            //ds.Tables[0].Rows[i][38].ToString() + "','" + ds.Tables[0].Rows[i][39].ToString() + "','" +                        
            //            HandleSpecialCharacter(ds.Tables[0].Rows[i][38].ToString()) + "')";
            //            con1.Open();
            //            SqlCommand cmd1 = new SqlCommand(query1, con1);
            //            cmd1.ExecuteNonQuery();
            //            con1.Close();
            //        }

            //        SqlConnection con2 = new SqlConnection(conn);
            //        string query2 = "exec dbo.exe_data_category";
            //        con2.Open();
            //        SqlCommand cmd2 = new SqlCommand(query2, con2);
            //        cmd2.ExecuteNonQuery();
            //        con2.Close();
            //    }

            //}
            ViewBag.err = "Upload file complete!";
            CheckCookie();
            ChangeLanguage(cookie.Value);
            return View();
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region ShowData

        public ActionResult ShowData(string dropdownlistYear, string dropdownlistmonth, string dropdownlistvender)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ShowData").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            if (dropdownlistYear != "")
            {
                return View(db_SCP.show_data(dropdownlistYear, dropdownlistmonth, dropdownlistvender));
            }
            else
            {
                ViewBag.err = "Must be choice data filter!";
                return View(db_SCP.show_data(dropdownlistYear, dropdownlistmonth, dropdownlistvender));
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region ShowDataCategory

        public ActionResult ShowDataCategory(string dropdownlistYear, string dropdownlistmonth, string dropdownlistvender)
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ShowDataCategory").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            if (dropdownlistYear != "")
            {
                return View(db_SCP.show_data_category(dropdownlistYear, dropdownlistmonth));
            }
            else
            {
                ViewBag.err = "Must be choice data filter!";
                return View(db_SCP.show_data_category(dropdownlistYear, dropdownlistmonth));
            }
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
    }
}