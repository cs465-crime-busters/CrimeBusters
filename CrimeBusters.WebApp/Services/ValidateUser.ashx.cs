using LoginModel = CrimeBusters.WebApp.Models.Login;
using CrimeBusters.WebApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Summary description for ValidateUser
    /// </summary>
    public class ValidateUser : IHttpHandler
    {
        /// <summary>
        /// Summary description for ValidateUser
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            LoginModel.Login login = new LoginModel.Login(new User
            {
                UserName = request.QueryString["userName"],
                Password = request.QueryString["password"]
            });

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            String jsonString = serializer.Serialize(new { result = login.ValidateUser() });
            response.Write(jsonString);
            response.ContentType = "application/json";
        }

        /// <summary>
        /// ValidateUser Resusable Property for class, always returns false
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