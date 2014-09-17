using System;
using System.Web;
using System.Web.Script.Serialization;
using CrimeBusters.WebApp.Models.Users;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Updates the user profile.
    /// </summary>
    public class UpdateProfile : IHttpHandler
    {
        /// <summary>
        /// Process Request that updates user info given the HttpContext, and returns confirmation message upon successful completion
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            String jsonString;
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                User user = new User
                {
                    FirstName = request.Form["firstName"],
                    LastName = request.Form["lastName"],
                    Gender = request.Form["gender"],
                    PhoneNumber = request.Form["phoneNumber"],
                    Address = request.Form["address"],
                    ZipCode = request.Form["zipCode"],
                    UserName = request.Form["userName"]
                };
                user.UpdateProfile();

                jsonString = serializer.Serialize(new { result = "success" });
            }
            catch (Exception ex)
            {
                jsonString = serializer.Serialize(new { result = ex.Message });
            }
           
            response.Write(jsonString);
            response.ContentType = "application/json";
        }

        /// <summary>
        /// UpdateProfile Resusable Property for class, always returns false
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