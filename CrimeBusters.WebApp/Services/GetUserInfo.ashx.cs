using CrimeBusters.WebApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Gets the user information and return a User object with properties filled up.
    /// </summary>
    public class GetUserInfo : IHttpHandler
    {
        /// <summary>
        /// Process Request given HTTP context to get user info and return a response with their properties
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            String userName = context.Request.QueryString["userName"];
            User user = User.GetUser(userName);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonString = serializer.Serialize(user);
            context.Response.Write(jsonString);
            context.Response.ContentType = "application/json";
        }

        /// <summary>
        /// GetUserInfo Resusable Property for class, always returns false
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