using System.IO;
using CrimeBusters.WebApp.Models.Documents;
using CrimeBusters.WebApp.Models.Report;
using CrimeBusters.WebApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
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
                HttpPostedFile photo1 = request.Files["photo1"];
                HttpPostedFile photo2 = request.Files["photo2"];
                HttpPostedFile photo3 = request.Files["photo3"];
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
                    Message = request.Form["desc"]
                };
                AddMedia(report, photo1, photo2, photo3, video, audio);

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
        private static void AddMedia(Report report, HttpPostedFile photo1, HttpPostedFile photo2, HttpPostedFile photo3,
            HttpPostedFile video, HttpPostedFile audio)
        {
            if (photo1 != null)
            {
                FileInfo fileInfo = new FileInfo(photo1.FileName);
                report.AddMedia(new Photo
                {
                    Url = "~/Content/uploads/" + DateTime.Now.Ticks + "1_" + fileInfo.Name,
                    File = photo1
                });
            }
            else
            {
                report.AddMedia(null);
            }

            if (photo2 != null)
            {
                FileInfo fileInfo = new FileInfo(photo2.FileName);
                report.AddMedia(new Photo
                {
                    Url = "~/Content/uploads/" + DateTime.Now.Ticks + "2_" + fileInfo.Name,
                    File = photo2
                });
            }
            else
            {
                report.AddMedia(null);
            }

            if (photo3 != null)
            {
                FileInfo fileInfo = new FileInfo(photo3.FileName);
                report.AddMedia(new Photo
                {
                    Url = "~/Content/uploads/" + DateTime.Now.Ticks + "3_" + fileInfo.Name,
                    File = photo3
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