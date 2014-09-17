using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace CrimeBusters.WebApp.Models.Util
{
    public class WebContentLocator : IContentLocator
    {
        /// <summary>
        /// Return the path on the server given the relative path for the content
        /// </summary>
        public string GetPath(string relativePath)
        {
            return HttpContext.Current.Server.MapPath(relativePath);
        }
    }

    public class TestContentLocator : IContentLocator
    {
        string _contentRoot;

        /// <summary>
        /// Constructor for Test Content Locator, initialize the content's root
        /// </summary>
        public TestContentLocator()
        {
            _contentRoot = ConfigurationManager.AppSettings["ContentRoot"];
        }

        /// <summary>
        /// Return the path formatted given the relative path
        /// </summary>
        public string GetPath(string relativePath)
        {
            return Path.Combine(_contentRoot, relativePath.Replace("~/", String.Empty));
        }
    }

    public interface IContentLocator
    {
        string GetPath(string relativePath);
    }
}