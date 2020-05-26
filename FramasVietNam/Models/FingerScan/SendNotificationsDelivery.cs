using FramasVietNam.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using FramasVietNam.Common;

namespace FramasVietNam.Models.FingerScan
{
    public static class SendNotificationsDelivery
    {
        //static readonly string connString = @"data source=FVN-PC-IT-07\SQLEXPRESS;initial catalog=FingerScan;user id=sa;password=1234;";
        static readonly string connString = @"data source=APP01;initial catalog=FingerScan;user id=sa;password=Fdw24$110 ;";

        internal static SqlCommand command = null;
        internal static SqlCommand command_Rs = null;
        internal static SqlDependency dependency = null;

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns></returns>
        public static string GetNotificationDelivery()
        {
            try
            {

                //var messages = new List<tbl_LogFinger_Delivery>();
                var lstDelivery = new List<DeliveryShow>();

                using (var connection = new SqlConnection(connString))
                {
                    connection.Open();
                    //// Sanjay : Alwasys use "dbo" prefix of database to trigger change event
                    using (command = new SqlCommand(@"SELECT ID, IndRegID FROM [dbo].[tbl_LogFinger_Delivery]", connection))
                    {
                        command.Notification = null;

                        if (dependency == null)
                        {
                            dependency = new SqlDependency(command);
                            dependency.OnChange += new OnChangeEventHandler(Dependency_OnChangeDelivery);
                        }

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = command.ExecuteReader();
                        //while (reader.Read())
                        //{
                        //    messages.Add(item: new tbl_LogFinger
                        //    {
                        //        ID = (long)reader["ID"],
                        //        MachineNumber = reader["MachineNumber"] != DBNull.Value ? (int)reader["MachineNumber"] : 0,
                        //        IndRegID = reader["IndRegID"] != DBNull.Value ? (int)reader["IndRegID"] : 0,
                        //        VerifyMode = reader["VerifyMode"] != DBNull.Value ? (int)reader["VerifyMode"] : 0,
                        //        InOutMode = reader["InOutMode"] != DBNull.Value ? (int)reader["InOutMode"] : 0,
                        //        DateTimeRecord = reader["DateTimeRecord"] != DBNull.Value ? (DateTime)reader["DateTimeRecord"] : DateTime.Now
                        //    });
                        //}
                        using (var conn = new SqlConnection(connString))
                        {
                            conn.Open();
                            using (command_Rs = new SqlCommand(@"EXEC sp_DeliveryMealShow", conn))
                            {
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                var result = command_Rs.ExecuteReader();
                                while (result.Read())
                                {
                                    lstDelivery.Add(item: new DeliveryShow
                                    {
                                        ID = (long)result["ID"],
                                        MachineNumber = result["MachineNumber"] != DBNull.Value ? (int)result["MachineNumber"] : 0,
                                        IndRegID = result["IndRegID"] != DBNull.Value ? (int)result["IndRegID"] : 0,
                                        TenNhanVien = result["TenNhanVien"] != DBNull.Value ? (string)result["TenNhanVien"] : "",
                                        MenuMealID = result["MenuMealID"] != DBNull.Value ? (int)result["MenuMealID"] : 0,
                                        MenuMealName = result["MenuMealName"] != DBNull.Value ? (string)result["MenuMealName"] : "",
                                        ShiftWorkID = result["ShiftWorkID"] != DBNull.Value ? (int)result["ShiftWorkID"] : 0,
                                        ShiftWorkName = result["ShiftWorkName"] != DBNull.Value ? (string)result["ShiftWorkName"] : "",
                                        IsTakeMeal = result["IsTakeMeal"] != DBNull.Value ? (Boolean)result["IsTakeMeal"] : false,
                                        isCheck = result["isCheck"] != DBNull.Value ? (Boolean)result["isCheck"] : false,
                                        MsgCheck = result["MsgCheck"] != DBNull.Value ? (string)result["MsgCheck"] : "",
                                        DateTimeFinger = result["DateTimeFinger"] != DBNull.Value ? (string)result["DateTimeFinger"] : ""
                                    });
                                }
                            }
                        }

                    }
                }
                var jsonSerialiser = new JavaScriptSerializer();
                //var json = jsonSerialiser.Serialize(messages);
                var json = jsonSerialiser.Serialize(lstDelivery);
                return json;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        private static void Dependency_OnChangeDelivery(object sender, SqlNotificationEventArgs e)
        {
            if (dependency != null)
            {
                dependency.OnChange -= Dependency_OnChangeDelivery;
                dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.Send();
            }
        }
    }
}