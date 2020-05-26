using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FramasVietNam.Models.Menu;
using FramasVietNam.Models.BarcodeScanner;
using Microsoft.AspNet.Identity;
using FramasVietNam.Common;
using FramasVietNam.Models.Meal;
using FramasVietNam.Models.VNT86;

namespace FramasVietNam.Controllers
{
    [Authorize]
    public class BarcodeScannerController : Controller
    {
        ////////////////////////////////////////////////////////////
        //Variable
        #region Variabal
        //important Variable for using get and post data from database
        private MenuManagerEntities db = new MenuManagerEntities();
        private BarcodeScannerEntities db_BS = new BarcodeScannerEntities();

        //Connection VNT86
        private VNT86Entities db_VNT86 = new VNT86Entities();
        //Connection VNL86
        //important Variabal cookie in multi language
        private HttpCookie cookie;
        #endregion
        ////////////////////////////////////////////////////////////
                

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //VNT

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




        ////////////////////////////////////////////////////////////
        #region ImportWarehouse QR Barcode Inventory VNT
        // GET: ImportWarehouse
        public ActionResult ImportWarehouseVNT()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ImportWarehouseVNT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _strOCDN = mFunction.ToString(Session[""]).Trim();
            string _strDateSearch = mFunction.ToString(Session[""]).Trim();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNT_GetParent("", "").ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }
        [HttpPost]
        public ActionResult ImportWarehouseVNT(FormCollection fc)
        {
            string _strOCDN = mFunction.ToString(fc["txtOCDN"]).Trim();
            string _strDateSearch = mFunction.ToString(fc["dateSearch"]).Trim();
            string _dtDateSearch = mFunction.ToDateTime(fc["dateSearch"]).ToString("yyyyMMdd");
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something

            var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNT_GetParent(((_strDateSearch == "") ? "" : _dtDateSearch), _strOCDN).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }

        //show popup when click detail
        public ActionResult ImportWarehouseVNTShowPopup(string strOC)
        {
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNT_GetChild(strOC).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            //
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            string _strID = "";
            int _TotalQuantity = 0;
            foreach (var item in lst_BarcodeScanner)
            {
                if (item.BusinessStatusID == 1 && item.StatusScanID == 1 && item.WHStatusID != 1)
                {
                    _TotalQuantity = _TotalQuantity + mFunction.ToInt(item.QIB);
                    obj.Add(new CheckBoxModel { Value = mFunction.ToInt(item.ID), Code = item.ID.ToString(), Name = item.ID.ToString(), IsChecked = false });
                    if (_strID == "")
                    {
                        _strID = item.ID.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.ID.ToString();
                    }
                }
            }
            //
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            //
            ViewBag._strID = _strID.Trim();
            ViewBag._TotalQuantity = _TotalQuantity;
            //
            var lst_WarehouseGet = db_BS.vWarehouseVNTs.ToList();
            ViewBag.lst_WarehouseGet = lst_WarehouseGet;
            //
            var lst_KeyPost = db_BS.sp_KeyPostVNT_Get().ToList();
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("ImportWarehouseVNTPopup", objBind);
        }

        //Close popup and go to ImportWarehouse page
        [HttpPost]
        public ActionResult ImportWarehouseVNTClosePopup()
        {
            return RedirectToAction("ImportWarehouseVNT");
        }

        //Post data to warehouse
        [HttpPost]
        public ActionResult ImportWarehouseVNT_Post(CheckBoxList Obj, FormCollection fc)
        {

            string str = Request.Params["command"];
            if (str == "Close")
            {
                return RedirectToAction("ImportWarehouseVNT");
            }

            //Take id barcode checked on view
            string _strID = "";
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    if (_strID == "")
                    {
                        _strID = item.Value.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.Value.ToString();
                    }
                }
            }

            //Check value
            string _strType = mFunction.ToString(fc["CboPostType"]);
            string _strWarehouseFrom = null;
            string _strWarehouseTo = mFunction.ToString(fc["CboWarehouseTo"]);
            string _strKeyFrom = null;
            string _strKeyTo = mFunction.ToString(fc["CboKeyTo"]);

            string _User = User.Identity.GetUserName();
            DateTime _DTInport = DateTime.Now;
            if (_strID.Length > 0)
            {
                //check not exists barcode in logfile
                var lstcheck = db_BS.sp_CheckBarcodeScanner(_strID, "ImportWarehouseVNT").ToList();
                if (lstcheck[0].Value == false)
                {
                    //
                    var obj = db_BS.sp_ImportInventoryVNT_Ins(_strID, _strType, _strWarehouseFrom, _strKeyFrom, _strWarehouseTo, _strKeyTo, _DTInport, _User);
                    //Save log
                    var objLog = db_BS.sp_ActionLog("ImportWarehouseVNT", "BarcodeScanner", "Insert", "Import data to warehouse", _strID, _User, _DTInport);
                }
                else
                {
                    string _strOCDN = mFunction.ToString(Session[""]).Trim();
                    string _strDateSearch = mFunction.ToString(Session[""]).Trim();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    // to do something
                    var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNT_GetParent("", "").ToList();
                    ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
                    ViewBag._strOCDN = _strOCDN;
                    ViewBag._strDateSearch = _strDateSearch;
                    ViewBag.Message = "Please try a again ( update data before post )";
                    return View("ImportWarehouseVNT");
                }
            }
            //
            return RedirectToAction("ImportWarehouseVNT");
        }

        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region TransferWarehouseVNT QR Barcode Inventory VNT
        // GET: TransferWarehouseVNT
        public ActionResult TransferWarehouseVNT()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TransferWarehouseVNT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _strOCDN = mFunction.ToString(Session[""]).Trim();
            string _strDateSearch = mFunction.ToString(Session[""]).Trim();

            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNT_GetParent("", "").ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }
        [HttpPost]
        public ActionResult TransferWarehouseVNT(FormCollection fc)
        {
            string _strOCDN = mFunction.ToString(fc["txtOCDN"]).Trim();
            string _strDateSearch = mFunction.ToString(fc["dateSearch"]).Trim();
            string _dtDateSearch = mFunction.ToDateTime(fc["dateSearch"]).ToString("yyyyMMdd");
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something

            var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNT_GetParent(((_strDateSearch == "") ? "" : _dtDateSearch), _strOCDN).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;
            //
            return View();
        }

        //show popup when click detail
        public ActionResult TransferWarehouseVNTShowPopup(string strOC)
        {
            // to do something
            var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNT_GetChild(strOC).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            //
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            //
            string _strID = "";
            int _TotalQuantity = 0;
            foreach (var item in lst_BarcodeScanner)
            {
                if (item.BusinessStatusID == 2 && item.StatusScanID == 1 && item.WHStatusID != 2)
                {
                    _TotalQuantity = _TotalQuantity + mFunction.ToInt(item.QIB);
                    obj.Add(new CheckBoxModel { Value = mFunction.ToInt(item.ID), Code = item.ID.ToString(), Name = item.ID.ToString(), IsChecked = false });
                    if (_strID == "")
                    {
                        _strID = item.ID.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.ID.ToString();
                    }
                }
            }
            //
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            //
            ViewBag._strID = _strID.Trim();
            ViewBag._TotalQuantity = _TotalQuantity;
            //
            var lst_WarehouseGet = db_BS.vWarehouseVNTs.ToList();
            ViewBag.lst_WarehouseGet = lst_WarehouseGet;
            //
            var lst_KeyPost = db_BS.sp_KeyPostVNT_Get().ToList();
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("TransferWarehouseVNTPopup", objBind);
        }

        //Close popup and go to TransferWarehouse page
        [HttpPost]
        public ActionResult TransferWarehouseVNTClosePopup()
        {
            return RedirectToAction("TransferWarehouseVNT");
        }

        //Post data to warehouse
        [HttpPost]
        public ActionResult TransferWarehouseVNT_Post(CheckBoxList Obj, FormCollection fc)
        {
            string str = Request.Params["command"];
            if (str == "Close")
            {
                return RedirectToAction("TransferWarehouseVNT");
            }

            //Take id barcode checked on view
            string _strID = "";
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    if (_strID == "")
                    {
                        _strID = item.Value.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.Value.ToString();
                    }
                }
            }

            //Check value
            string _strType = mFunction.ToString(fc["CboPostType"]);
            string _strWarehouseFrom = mFunction.ToString(fc["CboWarehouseFrom"]);
            string _strWarehouseTo = mFunction.ToString(fc["CboWarehouseTo"]);
            string _strKeyFrom = mFunction.ToString(fc["CboKeyFrom"]);
            string _strKeyTo = mFunction.ToString(fc["CboKeyTo"]);
            string _User = User.Identity.GetUserName();
            DateTime _DTInport = DateTime.Now;
            if (_strID.Length > 0)
            {
                //check not exists barcode in logfile
                var lstcheck = db_BS.sp_CheckBarcodeScanner(_strID, "TransferWarehouseVNT").ToList();
                if (lstcheck[0].Value == false)
                {
                    //
                    var obj = db_BS.sp_TransferInventoryVNT_Ins(_strID, _strType, _strWarehouseFrom, _strKeyFrom, _strWarehouseTo, _strKeyTo, _DTInport, _User);
                    //Save log
                    var objLog = db_BS.sp_ActionLog("TransferWarehouseVNT", "BarcodeScanner", "Insert", "Transfer data to warehouse", _strID, _User, _DTInport);
                }
                else
                {
                    string _strOCDN = mFunction.ToString(Session[""]).Trim();
                    string _strDateSearch = mFunction.ToString(Session[""]).Trim();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    // to do something
                    var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNT_GetParent("", "").ToList();
                    ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
                    ViewBag._strOCDN = _strOCDN;
                    ViewBag._strDateSearch = _strDateSearch;
                    ViewBag.Message = "Please try a again ( update data before post )";
                    return View("TransferWarehouseVNT");
                }
            }
            //
            return RedirectToAction("TransferWarehouseVNT");
        }

        #endregion
        ////////////////////////////////////////////////////////////



        ////////////////////////////////////////////////////////////
        #region ExportWarehouseVNT QR Barcode Inventory VNT
        // GET: ExportWarehouseVNT
        public ActionResult ExportWarehouseVNT()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ExportWarehouseVNT").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _strOCDN = mFunction.ToString(Session[""]).Trim();
            string _strDateSearch = mFunction.ToString(Session[""]).Trim();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNT_GetParent("", "").ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }
        [HttpPost]
        public ActionResult ExportWarehouseVNT(FormCollection fc)
        {
            string _strOCDN = mFunction.ToString(fc["txtOCDN"]).Trim();
            string _strDateSearch = mFunction.ToString(fc["dateSearch"]).Trim();
            string _dtDateSearch = mFunction.ToDateTime(fc["dateSearch"]).ToString("yyyyMMdd");
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something

            var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNT_GetParent(((_strDateSearch == "") ? "" : _dtDateSearch), _strOCDN).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;
            //
            return View();
        }

        //show popup when click detail
        public ActionResult ExportWarehouseVNTShowPopup(string strOC)
        {
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNT_GetChild(strOC).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            //
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            //
            string _strID = "";
            int _TotalQuantity = 0;
            foreach (var item in lst_BarcodeScanner)
            {
                if (item.BusinessStatusID == 3 && item.StatusScanID == 2 && item.WHStatusID != 3)
                {
                    _TotalQuantity = _TotalQuantity + mFunction.ToInt(item.QIB);
                    obj.Add(new CheckBoxModel { Value = mFunction.ToInt(item.ID), Code = item.ID.ToString(), Name = item.ID.ToString(), IsChecked = false });
                    if (_strID == "")
                    {
                        _strID = item.ID.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.ID.ToString();
                    }
                }
            }
            //
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            //
            ViewBag._strID = _strID.Trim();
            ViewBag._TotalQuantity = _TotalQuantity;
            //
            var lst_WarehouseGet = db_BS.vWarehouseVNTs.ToList();
            ViewBag.lst_WarehouseGet = lst_WarehouseGet;
            //
            var lst_KeyPost = db_BS.sp_KeyPostVNT_Get().ToList();
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("ExportWarehouseVNTPopup", objBind);
        }

        //Close popup and go to ExportWarehouse page
        [HttpPost]
        public ActionResult ExportWarehouseVNTClosePopup()
        {
            return RedirectToAction("ExportWarehouseVNT");
        }

        //Post data to warehouse
        [HttpPost]
        public ActionResult ExportWarehouseVNT_Post(CheckBoxList Obj, FormCollection fc)
        {

            string str = Request.Params["command"];
            if (str == "Close")
            {
                return RedirectToAction("ExportWarehouseVNT");
            }

            //Take id barcode checked on view
            string _strID = "";
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    if (_strID == "")
                    {
                        _strID = item.Value.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.Value.ToString();
                    }
                }
            }

            //Check value
            string _strType = mFunction.ToString(fc["CboPostType"]);
            string _strWarehouseFrom = mFunction.ToString(fc["CboWarehouseFrom"]);
            string _strWarehouseTo = null;
            string _strKeyFrom = mFunction.ToString(fc["CboKeyFrom"]);
            string _strKeyTo = null;
            string _User = User.Identity.GetUserName();
            DateTime _DTInport = DateTime.Now;
            if (_strID.Length > 0)
            {
                //check not exists barcode in logfile
                var lstcheck = db_BS.sp_CheckBarcodeScanner(_strID, "ExportWarehouseVNT").ToList();
                if (lstcheck[0].Value == false)
                {
                    //
                    var obj = db_BS.sp_ExportInventoryVNT_Ins(_strID, _strType, _strWarehouseFrom, _strKeyFrom, _strWarehouseTo, _strKeyTo, _DTInport, _User);
                    //Save log
                    var objLog = db_BS.sp_ActionLog("ExportWarehouseVNT", "BarcodeScanner", "Insert", "Export data to warehouse", _strID, _User, _DTInport);
                }
                else
                {
                    string _strOCDN = mFunction.ToString(Session[""]).Trim();
                    string _strDateSearch = mFunction.ToString(Session[""]).Trim();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    // to do something
                    var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNT_GetParent("", "").ToList();
                    ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
                    ViewBag._strOCDN = _strOCDN;
                    ViewBag._strDateSearch = _strDateSearch;
                    ViewBag.Message = "Please try a again ( update data before post )";
                    return View("ExportWarehouseVNT");
                }
            }
            //
            return RedirectToAction("ExportWarehouseVNT");
        }

        #endregion
        ////////////////////////////////////////////////////////////





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //VNL

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region ImportWarehouse QR Barcode Inventory VNL
        // GET: ImportWarehouse
        public ActionResult ImportWarehouseVNL()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ImportWarehouseVNL").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _strOCDN = mFunction.ToString(Session[""]).Trim();
            string _strDateSearch = mFunction.ToString(Session[""]).Trim();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNL_GetParent("", "").ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }
        [HttpPost]
        public ActionResult ImportWarehouseVNL(FormCollection fc)
        {
            string _strOCDN = mFunction.ToString(fc["txtOCDN"]).Trim();
            string _strDateSearch = mFunction.ToString(fc["dateSearch"]).Trim();
            string _dtDateSearch = mFunction.ToDateTime(fc["dateSearch"]).ToString("yyyyMMdd");
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something

            var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNL_GetParent(((_strDateSearch == "") ? "" : _dtDateSearch), _strOCDN).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }

        //show popup when click detail
        public ActionResult ImportWarehouseVNLShowPopup(string strOC)
        {
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNL_GetChild(strOC).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            //
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            string _strID = "";
            int _TotalQuantity = 0;
            foreach (var item in lst_BarcodeScanner)
            {
                if (item.BusinessStatusID == 1 && item.StatusScanID == 1 && item.WHStatusID != 1)
                {
                    _TotalQuantity = _TotalQuantity + mFunction.ToInt(item.QIB);
                    obj.Add(new CheckBoxModel { Value = mFunction.ToInt(item.ID), Code = item.ID.ToString(), Name = item.ID.ToString(), IsChecked = false });
                    if (_strID == "")
                    {
                        _strID = item.ID.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.ID.ToString();
                    }
                }
            }
            //
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            //
            ViewBag._strID = _strID.Trim();
            ViewBag._TotalQuantity = _TotalQuantity;
            //
            var lst_WarehouseGet = db_BS.vWarehouseVNLs.ToList();
            ViewBag.lst_WarehouseGet = lst_WarehouseGet;
            //
            var lst_KeyPost = db_BS.sp_KeyPostVNL_Get().ToList();
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("ImportWarehouseVNLPopup", objBind);
        }

        //Close popup and go to ImportWarehouse page
        [HttpPost]
        public ActionResult ImportWarehouseVNLClosePopup()
        {
            return RedirectToAction("ImportWarehouseVNL");
        }

        //Post data to warehouse
        [HttpPost]
        public ActionResult ImportWarehouseVNL_Post(CheckBoxList Obj, FormCollection fc)
        {

            string str = Request.Params["command"];
            if (str == "Close")
            {
                return RedirectToAction("ImportWarehouseVNL");
            }

            //Take id barcode checked on view
            string _strID = "";
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    if (_strID == "")
                    {
                        _strID = item.Value.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.Value.ToString();
                    }
                }
            }

            //Check value
            string _strType = mFunction.ToString(fc["CboPostType"]);
            string _strWarehouseFrom = null;
            string _strWarehouseTo = mFunction.ToString(fc["CboWarehouseTo"]);
            string _strKeyFrom = null;
            string _strKeyTo = mFunction.ToString(fc["CboKeyTo"]);


            string _User = User.Identity.GetUserName();
            DateTime _DTInport = DateTime.Now;
            if (_strID.Length > 0)
            {
                //check not exists barcode in logfile
                var lstcheck = db_BS.sp_CheckBarcodeScanner(_strID, "ImportWarehouseVNL").ToList();
                if (lstcheck[0].Value == false)
                {
                    //
                    var obj = db_BS.sp_ImportInventoryVNL_Ins(_strID, _strType, _strWarehouseFrom, _strKeyFrom, _strWarehouseTo, _strKeyTo, _DTInport, _User);
                    //Save log
                    var objLog = db_BS.sp_ActionLog("ImportWarehouseVNL", "BarcodeScanner", "Insert", "Import data to warehouse", _strID, _User, _DTInport);
                }
                else
                {
                    string _strOCDN = mFunction.ToString(Session[""]).Trim();
                    string _strDateSearch = mFunction.ToString(Session[""]).Trim();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    // to do something
                    var lst_BarcodeScanner = db_BS.sp_ImportInventoryVNL_GetParent("", "").ToList();
                    ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
                    ViewBag._strOCDN = _strOCDN;
                    ViewBag._strDateSearch = _strDateSearch;
                    ViewBag.Message = "Please try a again ( update data before post )";
                    return View("ImportWarehouseVNL");
                }
            }
            //
            return RedirectToAction("ImportWarehouseVNL");
        }

        #endregion
        ////////////////////////////////////////////////////////////




        ////////////////////////////////////////////////////////////
        #region TransferWarehouseVNL QR Barcode Inventory VNL
        // GET: TransferWarehouseVNL
        public ActionResult TransferWarehouseVNL()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "TransferWarehouseVNL").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _strOCDN = mFunction.ToString(Session[""]).Trim();
            string _strDateSearch = mFunction.ToString(Session[""]).Trim();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNL_GetParent("", "").ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }
        [HttpPost]
        public ActionResult TransferWarehouseVNL(FormCollection fc)
        {
            string _strOCDN = mFunction.ToString(fc["txtOCDN"]).Trim();
            string _strDateSearch = mFunction.ToString(fc["dateSearch"]).Trim();
            string _dtDateSearch = mFunction.ToDateTime(fc["dateSearch"]).ToString("yyyyMMdd");
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something

            var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNL_GetParent(((_strDateSearch == "") ? "" : _dtDateSearch), _strOCDN).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;
            //
            return View();
        }

        //show popup when click detail
        public ActionResult TransferWarehouseVNLShowPopup(string strOC)
        {
            // to do something
            var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNL_GetChild(strOC).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            //
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            //
            string _strID = "";
            int _TotalQuantity = 0;
            foreach (var item in lst_BarcodeScanner)
            {
                if (item.BusinessStatusID == 2 && item.StatusScanID == 1 && item.WHStatusID != 2)
                {
                    _TotalQuantity = _TotalQuantity + mFunction.ToInt(item.QIB);
                    obj.Add(new CheckBoxModel { Value = mFunction.ToInt(item.ID), Code = item.ID.ToString(), Name = item.ID.ToString(), IsChecked = false });
                    if (_strID == "")
                    {
                        _strID = item.ID.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.ID.ToString();
                    }
                }
            }
            //
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            //
            ViewBag._strID = _strID.Trim();
            ViewBag._TotalQuantity = _TotalQuantity;
            //
            var lst_WarehouseGet = db_BS.vWarehouseVNLs.ToList();
            ViewBag.lst_WarehouseGet = lst_WarehouseGet;
            //
            var lst_KeyPost = db_BS.sp_KeyPostVNL_Get().ToList();
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("TransferWarehouseVNLPopup", objBind);
        }

        //Close popup and go to TransferWarehouse page
        [HttpPost]
        public ActionResult TransferWarehouseVNLClosePopup()
        {
            return RedirectToAction("TransferWarehouseVNL");
        }

        //Post data to warehouse
        [HttpPost]
        public ActionResult TransferWarehouseVNL_Post(CheckBoxList Obj, FormCollection fc)
        {
            string str = Request.Params["command"];
            if (str == "Close")
            {
                return RedirectToAction("TransferWarehouseVNL");
            }

            //Take id barcode checked on view
            string _strID = "";
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    if (_strID == "")
                    {
                        _strID = item.Value.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.Value.ToString();
                    }
                }
            }

            //Check value
            string _strType = mFunction.ToString(fc["CboPostType"]);
            string _strWarehouseFrom = mFunction.ToString(fc["CboWarehouseFrom"]);
            string _strWarehouseTo = mFunction.ToString(fc["CboWarehouseTo"]);
            string _strKeyFrom = mFunction.ToString(fc["CboKeyFrom"]);
            string _strKeyTo = mFunction.ToString(fc["CboKeyTo"]);
            string _User = User.Identity.GetUserName();
            DateTime _DTInport = DateTime.Now;
            if (_strID.Length > 0)
            {
                //check not exists barcode in logfile
                var lstcheck = db_BS.sp_CheckBarcodeScanner(_strID, "TransferWarehouseVNL").ToList();
                if (lstcheck[0].Value == false)
                {
                    //
                    var obj = db_BS.sp_TransferInventoryVNL_Ins(_strID, _strType, _strWarehouseFrom, _strKeyFrom, _strWarehouseTo, _strKeyTo, _DTInport, _User);
                    //Save log
                    var objLog = db_BS.sp_ActionLog("TransferWarehouseVNL", "BarcodeScanner", "Insert", "Transfer data to warehouse", _strID, _User, _DTInport);
                }
                else
                {
                    string _strOCDN = mFunction.ToString(Session[""]).Trim();
                    string _strDateSearch = mFunction.ToString(Session[""]).Trim();
                    // CheckCookie and ChangeLanguage always have in action (it pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    // to do something
                    var lst_BarcodeScanner = db_BS.sp_TransferInventoryVNL_GetParent("", "").ToList();
                    ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
                    ViewBag._strOCDN = _strOCDN;
                    ViewBag._strDateSearch = _strDateSearch;
                    ViewBag.Message = "Please try a again ( update data before post )";
                    return View("TransferWarehouseVNL");
                }
            }
            //
            return RedirectToAction("TransferWarehouseVNL");
        }

        #endregion
        ////////////////////////////////////////////////////////////




        ////////////////////////////////////////////////////////////
        #region ExportWarehouseVNL QR Barcode Inventory VNL
        // GET: ExportWarehouseVNL
        public ActionResult ExportWarehouseVNL()
        {
            //Check permission user
            var lst_Check = db.sp_CheckPermissionUser(User.Identity.GetUserName(), "ExportWarehouseVNL").ToList();
            if (lst_Check.Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            string _strOCDN = mFunction.ToString(Session[""]).Trim();
            string _strDateSearch = mFunction.ToString(Session[""]).Trim();
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNL_GetParent("", "").ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;

            //
            return View();
        }
        [HttpPost]
        public ActionResult ExportWarehouseVNL(FormCollection fc)
        {
            string _strOCDN = mFunction.ToString(fc["txtOCDN"]).Trim();
            string _strDateSearch = mFunction.ToString(fc["dateSearch"]).Trim();
            string _dtDateSearch = mFunction.ToDateTime(fc["dateSearch"]).ToString("yyyyMMdd");
            // CheckCookie and ChangeLanguage always have in action (it pass data to View)
            CheckCookie();
            ChangeLanguage(cookie.Value);
            // to do something

            var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNL_GetParent(((_strDateSearch == "") ? "" : _dtDateSearch), _strOCDN).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            ViewBag._strOCDN = _strOCDN;
            ViewBag._strDateSearch = _strDateSearch;
            //
            return View();
        }

        //show popup when click detail
        public ActionResult ExportWarehouseVNLShowPopup(string strOC)
        {
            // to do something
            var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNL_GetChild(strOC).ToList();
            ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
            //
            //Add Records into generic list
            List<CheckBoxModel> obj = new List<CheckBoxModel>();
            //
            string _strID = "";
            int _TotalQuantity = 0;
            foreach (var item in lst_BarcodeScanner)
            {
                if (item.BusinessStatusID == 3 && item.StatusScanID == 2 && item.WHStatusID != 3)
                {
                    _TotalQuantity = _TotalQuantity + mFunction.ToInt(item.QIB);
                    obj.Add(new CheckBoxModel { Value = mFunction.ToInt(item.ID), Code = item.ID.ToString(), Name = item.ID.ToString(), IsChecked = false });
                    if (_strID == "")
                    {
                        _strID = item.ID.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.ID.ToString();
                    }
                }
            }
            //
            CheckBoxList objBind = new CheckBoxList
            {
                CheckBox = obj
            };
            //
            ViewBag._strID = _strID.Trim();
            ViewBag._TotalQuantity = _TotalQuantity;
            //
            var lst_WarehouseGet = db_BS.vWarehouseVNLs.ToList();
            ViewBag.lst_WarehouseGet = lst_WarehouseGet;
            //
            var lst_KeyPost = db_BS.sp_KeyPostVNL_Get().ToList();
            ViewBag.lst_KeyPost = lst_KeyPost;
            //
            return PartialView("ExportWarehouseVNLPopup", objBind);
        }

        //Close popup and go to ExportWarehouse page
        [HttpPost]
        public ActionResult ExportWarehouseVNLClosePopup()
        {
            return RedirectToAction("ExportWarehouseVNL");
        }

        //Post data to warehouse
        [HttpPost]
        public ActionResult ExportWarehouseVNL_Post(CheckBoxList Obj, FormCollection fc)
        {

            string str = Request.Params["command"];
            if (str == "Close")
            {
                return RedirectToAction("ExportWarehouseVNL");
            }

            //Take id barcode checked on view
            string _strID = "";
            foreach (var item in Obj.CheckBox)
            {
                if (item.IsChecked == true)
                {
                    if (_strID == "")
                    {
                        _strID = item.Value.ToString();
                    }
                    else
                    {
                        _strID = _strID + "|" + item.Value.ToString();
                    }
                }
            }

            //Check value
            string _strType = mFunction.ToString(fc["CboPostType"]);
            string _strWarehouseFrom = mFunction.ToString(fc["CboWarehouseFrom"]);
            string _strWarehouseTo = null;
            string _strKeyFrom = mFunction.ToString(fc["CboKeyFrom"]);
            string _strKeyTo = null;
            string _User = User.Identity.GetUserName();
            DateTime _DTInport = DateTime.Now;
            if (_strID.Length > 0)
            {
                //check not exists barcode in logfile
                var lstcheck = db_BS.sp_CheckBarcodeScanner(_strID, "ExportWarehouseVNL").ToList();
                if (lstcheck[0].Value == false)
                {
                    var obj = db_BS.sp_ExportInventoryVNL_Ins(_strID, _strType, _strWarehouseFrom, _strKeyFrom, _strWarehouseTo, _strKeyTo, _DTInport, _User);
                    //Save log
                    var objLog = db_BS.sp_ActionLog("ExportWarehouseVNL", "BarcodeScanner", "Insert", "Export data to warehouse", _strID, _User, _DTInport);
                    //
                }
                else
                {
                    string _strOCDN = mFunction.ToString(Session[""]).Trim();
                    string _strDateSearch = mFunction.ToString(Session[""]).Trim();
                    // CheckCookie and ChangeLanguage always have in action (it will pass data to View)
                    CheckCookie();
                    ChangeLanguage(cookie.Value);
                    // to do something
                    var lst_BarcodeScanner = db_BS.sp_ExportInventoryVNL_GetParent("", "").ToList();
                    ViewBag.lst_BarcodeScanner = lst_BarcodeScanner;
                    ViewBag._strOCDN = _strOCDN;
                    ViewBag._strDateSearch = _strDateSearch;
                    ViewBag.Message = "Please try a again ( update data before post )";
                    return View("ExportWarehouseVNL");
                }
            }
            //
            return RedirectToAction("ExportWarehouseVNL");
        }

        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        #region Show data warehouse
        // GET: ShowDataWH
        public ActionResult ShowDataWH()
         {
            CheckCookie();
            ChangeLanguage(cookie.Value);
            var dataVNT86 = db_VNT86.STOCKIN_WS_NEW.Select(x => new STOCKIN_WS_NEW_VNT_ViewModels
            {
                MESOKEY = x.MESOKEY,
                C400 = x.C400,
                C401 = x.C401,
                C402 = x.C402,
                C403 = x.C403,
                C404 = x.C404,
                C405 = x.C405,
                C406 = x.C406,
                C409 = x.C409,
                C412 = x.C412,
                C417 =x.C417
            }).OrderByDescending(x=> x.C404).Take(100).ToList();
            if (dataVNT86 != null)
            {
                return View(dataVNT86);
            }
            return View();
        }
        #endregion
        ////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////
        //Cookie always exists in controller
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