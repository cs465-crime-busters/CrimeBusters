using LoginModel = CrimeBusters.WebApp.Models.Login;
using CrimeBusters.WebApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using CrimeBusters.WebApp.Models.Util;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Creates a user and sends an email for authentication.
    /// </summary>
    public class CreateUser : IHttpHandler
    {
        /// <summary>
        /// Process Request to create user based on HTTP context
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            String jsonString = String.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            LoginModel.Login login = new LoginModel.Login(new User { 
                UserName = request.Form["userName"],
                Password = request.Form["password"],
                FirstName = request.Form["firstName"],
                LastName = request.Form["lastName"],
                Email = request.Form["email"]
            });

            MembershipCreateStatus createStatus = login.CreateUser(new WebContentLocator());
            jsonString = serializer.Serialize(new { result = createStatus.ToString() });

            response.Write(jsonString);
            response.ContentType = "application/json";
        }

        /// <summary>
        /// CreateUser Resusable Property for class, always returns false
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