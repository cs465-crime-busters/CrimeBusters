using System.Web.Script.Serialization;
using CrimeBusters.WebApp.Models.DAL;
using CrimeBusters.WebApp.Models.Documents;
using CrimeBusters.WebApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CrimeBusters.WebApp.Models.Util;

namespace CrimeBusters.WebApp.Models.Report
{
    /// <summary>
    /// Contains the business logic for the Report module.
    /// </summary>
    public class Report
    {
        private String _reportType;
        private List<IDocument> _media = new List<IDocument>();
        private List<String> _urlList = new List<string>();

        private const String COLLAPSE_KEY_ACK = "ack";
        private const String GCM_SERVICE = "https://android.googleapis.com/gcm/send";
        private const String GOOGLE_API_KEY = "AIzaSyBB-ZhNGNBWYRF1lK4NvnKehIHn48_ZXg4";

        /// <summary>
        /// Reported Id property
        /// </summary>
        public int ReportId { get; set; }

        /// <summary>
        /// ReportTypeId property
        /// </summary>
        public ReportTypeEnum ReportTypeId { get; set; }

        /// <summary>
        /// ReportType property validates and returns report type
        /// </summary>
        public String ReportType 
        {
            get 
            {
                if (String.IsNullOrEmpty(_reportType))
                {
                    return Enum.GetName(typeof(ReportTypeEnum), this.ReportTypeId);
                }
                return _reportType;
            }
            set
            {
                _reportType = value;
            }
        }

        /// <summary>
        /// MarkerImage property
        /// </summary>
        public String MarkerImage { get; set; }

        /// <summary>
        /// Message property
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Latitude property
        /// </summary>
        public String Latitude { get; set; }

        /// <summary>
        /// Longitude property
        /// </summary>
        public String Longitude { get; set; }

        /// <summary>
        /// Location property
        /// </summary>
        public String Location { get; set; }

        /// <summary>
        /// Date Reported property
        /// </summary>
        public DateTime DateReported { get; set; }

        /// <summary>
        /// TimeStampString property returns converted time 
        /// </summary>
        public string TimeStampString
        {
            get
            {
                return Convert.ToString(this.DateReported,
                            System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public String ContactMethodPref { get; set; }

        public String PushId { get; set; }

        /// <summary>
        /// User property user property for report
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// Media Property returns all media for report
        /// </summary>
        public List<IDocument> Media 
        {
            get { return _media; }
        }

        /// <summary>
        /// UrlList Property returns urlList for report
        /// </summary>
        public List<String> UrlList
        {
            get { return _urlList; }
        }

        /// <summary>
        /// We do not differentiate a certain media when we dump the values in the database. 
        /// Since we only need the MediaUrl when we retrieve the values in the database, 
        /// </summary>
        public List<String> MediaUrl { get; set; }

        /// <summary>
        /// Report Constructor
        /// </summary>
        public Report() { }

        /// <summary>
        /// Report Constructor given the report Id
        /// </summary>
        public Report(int reportId) 
        {
            this.ReportId = reportId;
        }

        /// <summary>
        /// Report Constructor given type id, message, latitude, longitude, dateReported, and user
        /// </summary>
        public Report(ReportTypeEnum reportTypeId, String message, 
            String latitude, String longitude, String location,
            DateTime dateReported, IUser user) 
        {
            this.ReportTypeId = reportTypeId;
            this.Message = message;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Location = location;
            this.DateReported = dateReported;
            this.User = user;
        }

        /// <summary>
        /// Creates a report that will be saved to the database.
        /// </summary>
        /// <returns>success for successful insert, else will return the error message.</returns>
        public string CreateReport(IContentLocator contentLocator) 
        {
            foreach (var document in Media.Where(document => document != null))
            {
                AddUrlList(document.Url);
                document.Save(contentLocator);
            }

            try
            {
                ReportsDAO.CreateReport(ReportTypeId, Message, 
                    Latitude, Longitude, Location, DateReported, 
                    User.UserName, UrlList, PushId, ContactMethodPref);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets a list of reports from the database.
        /// </summary>
        /// <returns>The list of Report object.</returns>
        public static List<Report> GetReports()
        {
            SqlDataReader reader = ReportsDAO.GetReports();
            List<Report> reports = new List<Report>();

            try
            {
                int oReportId = reader.GetOrdinal("ReportId");
                int oReportType = reader.GetOrdinal("ReportType");
                int oMarkerImage = reader.GetOrdinal("MarkerImage");
                int oMessage = reader.GetOrdinal("Message");
                int oLatitude = reader.GetOrdinal("Latitude");
                int oLongitude = reader.GetOrdinal("Longitude");
                int oLocation = reader.GetOrdinal("Location");
                int oTimeStamp = reader.GetOrdinal("TimeStamp");
                int oUserName = reader.GetOrdinal("UserName");
                int oFirstName = reader.GetOrdinal("FirstName");
                int oLastName = reader.GetOrdinal("LastName");
                int oGender = reader.GetOrdinal("Gender");
                int oEmail = reader.GetOrdinal("Email");
                int oPhoneNumber = reader.GetOrdinal("PhoneNumber");
                int oAddress = reader.GetOrdinal("Address");
                int oZipCode = reader.GetOrdinal("ZipCode");

                while (reader.Read())
                {
                    Report report = new Report
                    {
                        ReportId = Convert.ToInt32(reader[oReportId]),
                        ReportType = reader[oReportType].ToString(),
                        MarkerImage = reader[oMarkerImage].ToString(),
                        Message = reader[oMessage].ToString(),
                        Latitude = reader[oLatitude].ToString(),
                        Longitude = reader[oLongitude].ToString(),
                        Location = reader[oLocation].ToString(),
                        DateReported = Convert.ToDateTime(reader[oTimeStamp]),
                        User = new User 
                        {
                            UserName = reader[oUserName].ToString(),
                            FirstName = reader[oFirstName].ToString(),
                            LastName = reader[oLastName].ToString(),
                            Gender = reader[oGender].ToString(),
                            Email = reader[oEmail].ToString(),
                            PhoneNumber = reader[oPhoneNumber].ToString(),
                            Address = reader[oAddress].ToString(),
                            ZipCode = reader[oZipCode].ToString()
                        }
                    };

                    for (int i = 1; i <= 5; i++)
                    {
                        String mediaUrl = reader["Media" + i].ToString();
                        if (!String.IsNullOrEmpty(mediaUrl))
                        {
                            report.AddUrlList(mediaUrl);
                        }
                    }
                    reports.Add(report);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                reader.Close();
            }
            return reports;
        }

        /// <summary>
        /// Gets a list of active reports from the database.
        /// </summary>
        /// <returns>The list of Report objects.</returns>
        public static List<Report> GetActiveReports()
        {
            SqlDataReader reader = ReportsDAO.GetReports();
            List<Report> reports = new List<Report>();

            try
            {
                int oReportId = Convert.ToInt32(reader.GetOrdinal("ReportId"));
                int oReportType = reader.GetOrdinal("Type");
                int oReportTypeId = reader.GetOrdinal("ReportTypeId");
                int oMessage = reader.GetOrdinal("Message");
                int oLatitude = reader.GetOrdinal("Latitude");
                int oLongitude = reader.GetOrdinal("Longitude");
                int oLocation = reader.GetOrdinal("Location");
                int oTimeStamp = reader.GetOrdinal("TimeStamp");
                int oContactMethodPref = reader.GetOrdinal("ContactMethodPref");
                int oUserName = reader.GetOrdinal("UserName");
                int oFirstName = reader.GetOrdinal("FirstName");
                int oLastName = reader.GetOrdinal("LastName");
                int oEmail = reader.GetOrdinal("Email");
                int oPhoneNumber = reader.GetOrdinal("PhoneNumber");

                while (reader.Read())
                {
                    Report report = new Report
                    {
                        ReportId = Convert.ToInt32(reader[oReportId]),
                        ReportType = reader[oReportType].ToString(),
                        ReportTypeId = (ReportTypeEnum) Convert.ToInt32(reader[oReportTypeId]),
                        Message = reader[oMessage].ToString(),
                        Latitude = reader[oLatitude].ToString(),
                        Longitude = reader[oLongitude].ToString(),
                        Location = reader[oLocation].ToString(),
                        DateReported = Convert.ToDateTime(reader[oTimeStamp]),
                        ContactMethodPref = reader[oContactMethodPref].ToString(),
                        User = new User
                        {
                            UserName = reader[oUserName].ToString(),
                            FirstName = reader[oFirstName].ToString(),
                            LastName = reader[oLastName].ToString(),
                            Email = reader[oEmail].ToString(),
                            PhoneNumber = reader[oPhoneNumber].ToString()
                        }
                    };

                    reports.Add(report);
                }
            }
            catch (Exception ex)
            {
                String error = ex.Message;
            }
            finally
            {
                reader.Close();
            }
            return reports;
        }

        /// <summary>
        /// Gets a list of active reports from the database.
        /// </summary>
        /// <returns>The list of Report objects.</returns>
        public static List<Report> GetReportsByType(ReportTypeEnum reportType, int startRowIndex, int maximumRows)
        {
            SqlDataReader reader = ReportsDAO.GetReports(reportType, startRowIndex, maximumRows);
            List<Report> reports = new List<Report>();

            try
            {
                int oReportId = Convert.ToInt32(reader.GetOrdinal("ReportId"));
                int oReportType = reader.GetOrdinal("Type");
                int oReportTypeId = reader.GetOrdinal("ReportTypeId");
                int oMessage = reader.GetOrdinal("Message");
                int oLatitude = reader.GetOrdinal("Latitude");
                int oLongitude = reader.GetOrdinal("Longitude");
                int oLocation = reader.GetOrdinal("Location");
                int oTimeStamp = reader.GetOrdinal("TimeStamp");
                int oContactMethodPref = reader.GetOrdinal("ContactMethodPref");
                int oUserName = reader.GetOrdinal("UserName");
                int oFirstName = reader.GetOrdinal("FirstName");
                int oLastName = reader.GetOrdinal("LastName");
                int oEmail = reader.GetOrdinal("Email");
                int oPhoneNumber = reader.GetOrdinal("PhoneNumber");

                while (reader.Read())
                {
                    Report report = new Report
                    {
                        ReportId = Convert.ToInt32(reader[oReportId]),
                        ReportType = reader[oReportType].ToString(),
                        ReportTypeId = (ReportTypeEnum)Convert.ToInt32(reader[oReportTypeId]),
                        Message = reader[oMessage].ToString(),
                        Latitude = reader[oLatitude].ToString(),
                        Longitude = reader[oLongitude].ToString(),
                        Location = reader[oLocation].ToString(),
                        DateReported = Convert.ToDateTime(reader[oTimeStamp]),
                        ContactMethodPref = reader[oContactMethodPref].ToString(),
                        User = new User
                        {
                            UserName = reader[oUserName].ToString(),
                            FirstName = reader[oFirstName].ToString(),
                            LastName = reader[oLastName].ToString(),
                            Email = reader[oEmail].ToString(),
                            PhoneNumber = reader[oPhoneNumber].ToString()
                        }
                    };

                    reports.Add(report);
                }
            }
            catch (Exception ex)
            {
                String error = ex.Message;
            }
            finally
            {
                reader.Close();
            }
            return reports;
        }

        public static String UpdateIsActive(int reportId, bool isActive)
        {
            try
            {
                ReportsDAO.UpdateIsActive(reportId, isActive);

                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Deletes a report from the database.
        /// </summary>
        /// <returns>true if the deletion is successful.</returns>
        public bool DeleteReport()
        {
            try
            {
                ReportsDAO.DeleteReport(this.ReportId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a media to the list of media associated to a report.
        /// </summary>
        /// <param name="document">Document that implements the IDocument interface.</param>
        public void AddMedia(IDocument document)
        {
            _media.Add(document);
        }

        /// <summary>
        /// Adds a URL to the list of URLs in the report.
        /// </summary>
        /// <param name="url">url to be added to the list.</param>
        public void AddUrlList(String url)
        {
            _urlList.Add(url);
        }

        public String AcknowledgeReport()
        {
            try
            {
                String pushId = ReportsDAO.GetPushId(ReportId);

                if (String.IsNullOrEmpty(pushId))
                {
                    return "Cannot acknowledge report. No push ID received from device.";
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                String payloadJsonString = serializer.Serialize(new
                {
                    registration_ids = new[] { pushId },
                    data = new { notificationType = "ack" },
                    collapse_key = COLLAPSE_KEY_ACK,
                    delay_while_idle = true
                });

                var client = new RestClient(
                        GCM_SERVICE,
                        HttpVerb.POST
                    )
                {
                    ContentType = "application/json",
                    PostData = payloadJsonString
                };
                client.AddHeader("Authorization", String.Format("key = {0}", GOOGLE_API_KEY));
                AndroidResponse androidResponse = serializer.Deserialize<AndroidResponse>(client.MakeRequest());

                if (NoErrorAndNoNewPushId(androidResponse))
                {
                    return "success";
                }

                ProcessResponseFurther(androidResponse);

                return "error";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static bool NoErrorAndNoNewPushId(AndroidResponse androidResponse)
        {
            return Convert.ToInt32(androidResponse.failure) == 0 &&
                   Convert.ToInt32(androidResponse.canonical_ids) == 0;
        }

        private void ProcessResponseFurther(AndroidResponse androidResponse)
        {
            AndroidResponseResult[] responseResultArray =
                androidResponse.results;

            foreach (var responseResult in responseResultArray)
            {
                if (!String.IsNullOrEmpty(responseResult.message_id))
                {
                    if (!String.IsNullOrEmpty(responseResult.registration_id))
                    {
                        ReportsDAO.UpdatePushId(ReportId, responseResult.registration_id);
                    }
                    else
                    {
                        // Handle the error further
                    }
                }
            }
        }
    }
}