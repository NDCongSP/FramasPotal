using FramasVietNam.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace FramasVietNam.Models.FingerScan
{
    public class SendNotificationsLoginM2
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
        /// 
        public static List<tbl_LogFinger_Login_M1> GetNotificationLoginM2()
        {
            try
            {

                var messages = new List<tbl_LogFinger_Login_M1>();

                using (var connection = new SqlConnection(connString))
                {
                    connection.Open();
                    //// Sanjay : Alwasys use "dbo" prefix of database to trigger change event
                    using (command = new SqlCommand(@"SELECT ID,MachineNumber,IndRegID,VerifyMode,InOutMode,DateTimeRecord FROM [dbo].[tbl_LogFinger_Login_M2]", connection))
                    {
                        command.Notification = null;

                        if (dependency == null)
                        {
                            dependency = new SqlDependency(command);
                            dependency.OnChange += new OnChangeEventHandler(Dependency_OnChangeLoginM2);
                        }

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            messages.Add(item: new tbl_LogFinger_Login_M1
                            {
                                ID = (long)reader["ID"],
                                MachineNumber = reader["MachineNumber"] != DBNull.Value ? (int)reader["MachineNumber"] : 0,
                                IndRegID = reader["IndRegID"] != DBNull.Value ? (int)reader["IndRegID"] : 0,
                                VerifyMode = reader["VerifyMode"] != DBNull.Value ? (int)reader["VerifyMode"] : 0,
                                InOutMode = reader["InOutMode"] != DBNull.Value ? (int)reader["InOutMode"] : 0,
                                DateTimeRecord = reader["DateTimeRecord"] != DBNull.Value ? (DateTime)reader["DateTimeRecord"] : DateTime.Now
                            });
                        }
                    }
                }
                //var jsonSerialiser = new JavaScriptSerializer();
                //var json = jsonSerialiser.Serialize(messages);
                return messages;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        private static void Dependency_OnChangeLoginM2(object sender, SqlNotificationEventArgs e)
        {
            if (dependency != null)
            {
                dependency.OnChange -= Dependency_OnChangeLoginM2;
                dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.Send();
            }
        }
    }
}