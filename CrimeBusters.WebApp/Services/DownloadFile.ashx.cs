using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Summary description for DownloadFile
    /// </summary>
    public class DownloadFile : IHttpHandler
    {
        /// <summary>
        /// Process Request to Download file given the HTTP context
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            string filePath = context.Request.QueryString["file"];

            string fileName = context.Server.MapPath(filePath);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);

            context.Response.ContentType = "audio/3gpp";
            context.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
            context.Response.TransmitFile(fileInfo.FullName);
            context.Response.End();
        }

        /// <summary>
        /// DownloadFile Resusable Property for class, always returns false
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