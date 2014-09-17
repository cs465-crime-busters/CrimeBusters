using System;
using System.IO;
using System.Web;
using CrimeBusters.WebApp.Models.Util;

namespace CrimeBusters.WebApp.Models.Documents
{
    public class Audio : IDocument
    {
        /// <summary>
        /// Property for the Audio File
        /// </summary>
        public HttpPostedFile File { get; set; }
        
        /// <summary>
        /// Property for the URL of the File
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Check if the file extension is valid
        /// </summary>
        public bool IsValidFile
        {
            get
            {
                string extension = Path.GetExtension(this.File.FileName);
                if (!String.IsNullOrEmpty(extension))
                {
                    switch (extension.ToLower())
                    {
                        case ".mp3":
                        case ".wav":
                        case ".3gp":
                            return true;
                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Saves the Audio to the file system
        /// </summary>
        public void Save(IContentLocator contentLocator)
        {
            if (!this.IsValidFile)
            {
                throw new Exception("Invalid file type. Can only accept mp3 and wav extensions.");
            }

            String filePath = contentLocator.GetPath(this.Url);
            this.File.SaveAs(filePath);
        }
    }
}