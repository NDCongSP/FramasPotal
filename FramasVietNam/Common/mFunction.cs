using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace FramasVietNam.Common
{
    public static class mFunction
    {

        public static string ToString(object Value)
        {
            if (Value == null) return "";
            return Value.ToString();
        }

        public static int ToInt(object Value)
        {
            if (Value == null) return 0;
            int result;
            if (int.TryParse(Value.ToString(), out result))
                return result;
            else
                return 0;
        }
        public static bool ToBool(object Value)
        {
            if (Value == null) return false;
            bool result = false;
            try
            {
                result = Convert.ToBoolean(Value);
                return result;
            }
            catch
            {
                return false;
            }
        }
        public static float ToFloat(object Value)
        {
            if (Value == null) return 0;
            float result;
            if (float.TryParse(Value.ToString(), out result))
                return result;
            else
                return 0;
        }

        public static double ToDouble(object Value)
        {
            if (Value == null) return 0;
            double result;
            if (double.TryParse(Value.ToString(), out result))
                return result;
            else
                return 0;
        }

        public static decimal ToDecimal(object Value)
        {
            if (Value == null) return 0;
            decimal result;
            if (decimal.TryParse(Value.ToString(), out result))
                return result;
            else
                return 0;
        }

        public static DateTime ToDateTime(object Value)
        {
            if (Value == null) return DateTime.Now;
            DateTime result;
            if (DateTime.TryParse(Value.ToString(), out result))
                return result;
            else
                return DateTime.Now;
        }

        public static DataTable ToListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static List<T> ToDataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static string ToMMddYY(DateTime dteNgay)
        {
            string sDay;
            string sMonth;
            string sYear;
            char cSplit = System.Convert.ToChar("/");

            sDay = dteNgay.Day.ToString();
            sMonth = dteNgay.Month.ToString();
            sYear = dteNgay.Year.ToString();

            return sMonth + cSplit + sDay + cSplit + System.Convert.ToString(dteNgay.ToString().Split(cSplit).GetValue(2));  // trả về ngày theo định dạng MM/dd/YY
        }

        public static string ToddMMYY(DateTime dteNgay)
        {
            string sDay;
            string sMonth;
            string sYear;
            char cSplit = System.Convert.ToChar("/");

            sDay = dteNgay.Day.ToString();
            sMonth = dteNgay.Month.ToString();
            sYear = dteNgay.Year.ToString();

            return sDay + cSplit + sMonth + cSplit + System.Convert.ToString(dteNgay.ToString().Split(cSplit).GetValue(2));  // trả về ngày theo định dạng dd/MM/YY
        }

        // lấy ngày cuối tháng của một ngày bất kỳ
        public static DateTime ToNgayCuoiThang(DateTime dteTime)
        {
            int intNgayCuoiThang;

            switch (dteTime.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    {
                        intNgayCuoiThang = 31;
                        break;
                    }

                case 2:
                    {
                        if (dteTime.Year % 4 == 0)
                            intNgayCuoiThang = 29;
                        else
                            intNgayCuoiThang = 28;
                        break;
                    }

                default:
                    {
                        intNgayCuoiThang = 30;
                        break;
                    }
            }

            return DateTime.Parse(intNgayCuoiThang.ToString() + "/" + dteTime.Month + "/" + dteTime.Year + " 23:0:0");
        }


        public static int IsInRole(string usename, string action)
        {
            string[] name = { "UserName", "Action" };
            string[] value = { usename, action };
            DataSet dsResult = DataOperation.GetDataSet(GlobalVariable.DBUserManager, "spCheckRoles", name, value);

            return 1;
        }


    }
}