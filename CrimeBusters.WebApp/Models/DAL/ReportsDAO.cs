using System.Collections.Generic;
using CrimeBusters.WebApp.Models.Report;
using System;
using System.Data;
using System.Data.SqlClient;

namespace CrimeBusters.WebApp.Models.DAL
{
    /// <summary>
    /// Contains all the data access logic for Reports module.
    /// </summary>
    public class ReportsDAO
    {
        /// <summary>
        /// Creates Report given the 
        /// ReportTypeEnum reportTypeId : Report Type
        /// String message : Report Message 
        /// String latitude : Latitude coordinate of crime reported
        /// String longitude : Longitude coordinate of crime reported
        /// String location :  Location of crime reported
        /// DateTime dateReported : Time crime reported
        /// String userName : Username of account that submitted reported
        /// List<String> resourceUrlList :  URL location list of media    
        /// </summary>
        public static void CreateReport(ReportTypeEnum reportTypeId, String message, 
            String latitude, String longitude, String location, DateTime dateReported,
            String userName, List<String> resourceUrlList) 
        { 
            using (SqlConnection connection = ConnectionManager.GetConnection()) 
            {
                SqlCommand command = new SqlCommand("CreateReport", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ReportTypeId", reportTypeId);
                command.Parameters.AddWithValue("@Message", message);
                command.Parameters.AddWithValue("@Latitude", latitude);
                command.Parameters.AddWithValue("@Longitude", longitude);
                command.Parameters.AddWithValue("@Location", location);
                command.Parameters.AddWithValue("@DateReported", dateReported);
                command.Parameters.AddWithValue("@UserName", userName);
                
                for (int i = 1; i <= resourceUrlList.Count; i++)
                {
                    command.Parameters.AddWithValue("@Media" + i, resourceUrlList[i - 1]);
                }
 
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets Reports by initiating the execution of stored procedure on database 
        /// returning a SqlDataReader
        /// </summary>
        public static SqlDataReader GetReports()
        {
            SqlConnection connection = ConnectionManager.GetConnection();
            SqlCommand command = new SqlCommand("GetReports", connection);
            command.CommandType = CommandType.StoredProcedure;

            return command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.CloseConnection);
        }

        public static SqlDataReader GetActiveReports()
        {
            SqlConnection connection = ConnectionManager.GetConnection();
            SqlCommand command = new SqlCommand("GetActiveReports", connection);
            command.CommandType = CommandType.StoredProcedure;

            return command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.CloseConnection);
        }

        public static String GetPushId(int reportId)
        {
            String pushId;
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("GetPushId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ReportId", reportId);

                pushId = (String) command.ExecuteScalar();
            }
            return pushId;
        }

        public static void UpdatePushId(int reportId, String newPushId)
        {
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("UpdatePushId", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ReportId", reportId);
                command.Parameters.AddWithValue("@NewPushId", newPushId);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete Reports by initiating the execution of stored procedure on database 
        /// </summary>
        public static void DeleteReport(int reportId)
        {
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("DeleteReport", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ReportId", reportId);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete Report Test by initiating the execution of stored procedure on database 
        /// </summary>
        public static void DeleteReportTest()
        {
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("DeleteReportTest", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.ExecuteNonQuery();
            }
        }
    }
}