using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using CrimeBusters.WebApp.Models.Users;
using CrimeBusters.WebApp.Models.Util;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Summary description for DeleteUser
    /// </summary>
    public class DeleteUser : IHttpHandler
    {
        /// <summary>
        /// Process request on deletion of user based on HTTP context, returns message upon success
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            String jsonString = String.Empty;
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Models.Login.Login login = new Models.Login.Login(new User
            {
                UserName = request.Form["userName"]
            });

            try
            {
                login.DeleteUser();
                jsonString = serializer.Serialize(new { result = "success" });
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
        /// DeleteUser Resusable Property for class, always returns false
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