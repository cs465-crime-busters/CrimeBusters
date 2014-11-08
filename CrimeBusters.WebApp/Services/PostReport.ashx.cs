using System.IO;
using CrimeBusters.WebApp.Models.Documents;
using CrimeBusters.WebApp.Models.Report;
using CrimeBusters.WebApp.Models.Users;
using System;
using System.Web;
using System.Web.Script.Serialization;
using CrimeBusters.WebApp.Models.Util;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Saves the user report from the Android to our database.
    /// </summary>
    public class PostReport : IHttpHandler
    {
        /// <summary>
        /// Process Request saves the user report including photos,video, and audio given the HttpContext
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            String jsonString = String.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                HttpPostedFile photo = request.Files["photo"];
                HttpPostedFile video = request.Files["video"];
                HttpPostedFile audio = request.Files["audio"];

                Report report = new Report
                {
                    ReportTypeId = (ReportTypeEnum) Int16.Parse(request.Form["reportTypeId"]),
                    Latitude = request.Form["lat"],
                    Longitude = request.Form["lng"],
                    Location = request.Form["location"],
                    DateReported = Convert.ToDateTime(request.Form["timeStamp"]),
                    User = new User(request.Form["userName"]),
                    Message = request.Form["desc"],
                    PushId = request.Form["pushId"],
                    ContactMethodPref = request.Form["contactMethodPref"]
                };
                AddMedia(report, photo, video, audio);

                jsonString = serializer.Serialize(
                    new { result = report.CreateReport(new WebContentLocator()) });
            }
            catch (Exception ex)
            {
                jsonString = serializer.Serialize(new { result = ex.Message });
            }
            finally
            {
                response.Write(jsonString);
                response.ContentType = "application/json";
            }
        }

        /// <summary>
        /// saves the media to Disk
        /// </summary>
        private static void AddMedia(Report report, HttpPostedFile photo, HttpPostedFile video, HttpPostedFile audio)
        {
            if (photo != null)
            {
                FileInfo fileInfo = new FileInfo(photo.FileName);
                report.AddMedia(new Photo
                {
                    Url = "~/Content/uploads/" + DateTime.Now.Ticks + "1_" + fileInfo.Name,
                    File = photo
                });
            }
            else
            {
                report.AddMedia(null);
            }

            if (video != null)
            {
                FileInfo fileInfo = new FileInfo(video.FileName);
                report.AddMedia(new Video
                {
                    Url = "~/Content/uploads/" + DateTime.Now.Ticks + "4_" + fileInfo.Name,
                    File = video
                });
            }
            else
            {
                report.AddMedia(null);
            }

            if (audio != null)
            {
                FileInfo fileInfo = new FileInfo(audio.FileName);
                report.AddMedia(new Audio
                {
                    Url = "~/Content/uploads/" + DateTime.Now.Ticks + "5_" + fileInfo.Name,
                    File = audio
                });
            }
            else
            {
                report.AddMedia(null);
            }
        }

        /// <summary>
        /// PostReport Resusable Property for class, always returns false
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}