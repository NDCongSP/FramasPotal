using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace FramasVietNam.Common
{
    public static class ValidationUtility
    {
        /// <summary>
        /// Kiểm tra giá trị value có Null không?
        /// Nếu value là Null, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là Null, trả về true; ngược lại, trả về false.</returns>
        public static bool IsNull(this object value)
        {
            //Nếu value = null
            if (value == null)
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value có rỗng không?
        /// Nếu value là rỗng, trả về true; ngược lại trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là rỗng, trả về true; ngược lại trả về false.</returns>
        public static bool IsEmpty(this object value)
        {
            //Nếu value là null
            if (value == null)
                //Trả về false
                return false;

            //Nếu value = ""
            if (value.ToString() == "")
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value có chứa toàn khoảng trắng không?
        /// Nếu value chứa toàn khoảng trắng, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value chứa toàn khoảng trắng, trả về true; ngược lại, trả về false.</returns>
        public static bool IsWhileSpace(this object value)
        {
            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            Regex regex = new Regex(@"^\s+$");

            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value là Null hoặc rỗng không?
        /// Nếu value là Null hoặc rỗng, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là Null hoặc rỗng, trả về true; ngược lại, trả về false.</returns>
        public static bool IsNullOrEmpty(this object value)
        {
            //Nếu value là null, trả về true
            if (value == null)
                return true;

            //Nếu value là empty, trả về true
            if (value.ToString() == "")
                return true;

            //Ngược lại, trả về false
            return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value là Null hoặc rỗng hoặc toàn khoảng trắng không?
        /// Nếu value là Null hoặc hoặc toàn khoảng trắng rỗng, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là Null hoặc rỗng hoặc toàn khoảng trắng, trả về true; ngược lại, trả về false.</returns>
        public static bool IsNullOrEmptyOrWhileSpace(this object value)
        {
            //Nếu value là NULL hoặc rỗng
            if (IsNullOrEmpty(value))
            {
                return true;
            }

            //Nếu value là toàn khoảng trắng
            if (IsWhileSpace(value))
            {
                return true;
            }
            //Ngược lại
            return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value chỉ chứa toàn số không?
        /// Nếu value chỉ chứa toàn số, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value chỉ chứa toàn số, trả về true; ngược lại, trả về false.</returns>
        public static bool IsOnlyNumber(this object value)
        {
            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            Regex regex = new Regex(@"^[0-9]+$");

            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value chỉ chứa toàn chữ không?
        /// Nếu value chỉ chứa toàn chữ, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value chỉ chứa toàn chữ, trả về true; ngược lại, trả về false.</returns>
        public static bool IsOnlyLetter(this object value)
        {
            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            Regex regex = new Regex(@"^[a-zA-Z]+$");

            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value chỉ chứa toàn chữ hoặc số không?
        /// Nếu value chỉ chứa toàn chữ hoặc số hoặc cả hai, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value chỉ chứa toàn chữ hoặc số hoặc cả hai, trả về true; ngược lại, trả về false.</returns>
        public static bool IsOnlyLetterOrNumeric(this object value)
        {
            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            Regex regex = new Regex(@"^[a-zA-Z0-9]+$");

            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value là định dạng email hợp lệ không?
        /// Nếu value là định dạng email hợp lệ, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là định dạng email hợp lệ, trả về true; ngược lại, trả về false.</returns>
        public static bool IsEmailFormat(this object value)
        {
            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            Regex regex = new Regex(@"^[0-9a-zA-Z](\w+\.)*[0-9a-zA-Z]+@\w+([-.]\w+)*(\w*[.])[a-zA-Z]{2,6}$");
            //Regex regex = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9\._]+[a-zA-Z0-9]@[a-zA-Z0-9-]+(\.[a-zA-Z]+)+$");

            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value là định dạng IP hợp lệ không?
        /// Nếu value là định dạng IP hợp lệ, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là định dạng IP hợp lệ, trả về true; ngược lại, trả về false.</returns>
        public static bool IsIPFormat(this object value)
        {
            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            //Regex regex = new Regex(@"^[1-9][0-9]{0,2}\.[0-9]+\.[0-9]+");
            Regex regex = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");


            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value là định dạng ngày tháng năm hợp lệ không?
        /// Nếu value là định dạng ngày tháng năm hợp lệ, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là định dạng ngày tháng năm hợp lệ, trả về true; ngược lại, trả về false.</returns>
        public static bool IsDateFormat(this object value)
        {

            if (value == null)
                return false;

            //Khai báo lớp regex để so khớp theo mẫu
            Regex regex = new Regex(@"^((((0?[1-9]|[12]\d|30)(?<s>[-\/])(0?[13-9]|1[012])\k<s>(\d{3}[1-9]))|(31(?<s>[-\/])([13578]|1[02])\k<s>(\d{3}[1-9])))|((29)(?<s>[-\/])0?2\k<s>(\d\d[2468][048]|\d\d[13579][26]|[13579][26]00|[02468][048]00))|(([0-2]?[0-8])(?<s>[-\/])0?2\k<s>(\d{4})))$");

            //Nếu value khớp với mẫu
            if (regex.IsMatch(value.ToString()))
                //Trả về true
                return true;
            //Ngược lại
            else
                //Trả về false
                return false;
        }

        /// <summary>
        /// Kiểm tra giá trị value là định dạng giờ phút giây hợp lệ không?
        /// Nếu value là định dạng giờ phút giây hợp lệ, trả về true; ngược lại, trả về false.
        /// </summary>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>Nếu value là định dạng giờ phút giây hợp lệ, trả về true; ngược lại, trả về false.</returns>
        public static bool IsTimeFormat(this object value)
        {

            {

                if (value == null)
                    return false;

                //Khai báo lớp regex để so khớp theo mẫu
                Regex regex = new Regex(@"^(((0?[1-9]|1[012]):[0-5]?\d(:[0-5]?\d)?\s*([aApP][mM])?)|((1[3-9]|2[0-3]):[0-5]?\d(:[0-5]\d)?\s*([pP][mM])?)|(0:[0-5]?\d(:[0-5]?\d)?\s*([Aa][mM])?))$");

                //Nếu value khớp với mẫu
                if (regex.IsMatch(value.ToString()))
                    //Trả về true
                    return true;
                //Ngược lại
                else
                    //Trả về false
                    return false;
            }

        }
    }

    public static class ConvertUtility
    {
        /// <summary>
        /// Trả về ngày cuối cùng của tháng
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertLastDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        /// <summary>
        /// Trả về ngày đầu tiên của tháng
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertFirtsDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static double ConvertPercen(double digit)
        {
            return (digit / 100);
        }
    }

    public static class PageConfig
    {
        public static int pageSize = 15;
    }

}