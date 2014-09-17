using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.Handlers;


namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Web service for the web application part.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Login : System.Web.Services.WebService
    {

        /// <summary>
        /// Validate User login and set authenthication cookie and returns error message if not policemann
        /// </summary>
        [WebMethod]
        public string ValidateUser(string userName, string password, bool rememberMe)
        {
            if (Membership.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, rememberMe);
                return Roles.IsUserInRole(userName, "Police") 
                    ? "Police" : "User";
            }
            MembershipUser user = Membership.GetUser(userName);
            return ShowMeaningfulErrorMessage(userName, user);
        }


        /// <summary>
        /// Log out User and reset the cookie
        /// </summary>
        [WebMethod]
        public string LogOutUser()
        {
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;
            
            FormsAuthentication.SignOut();            

            // clear authentication cookie
            if (request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                response.Cookies.Add(cookie1);
                return "User successfully logged out";
            }
            
            return "User logout error";
            
        }

        /// <summary>
        /// Return Error Message based on User Error status
        /// </summary>
        private static string ShowMeaningfulErrorMessage(string userName, MembershipUser user)
        {
            if (user == null)
            {
                return "There is no user in the database with the username " + userName;
            }
            else if (!user.IsApproved)
            {
                return "Your account has not yet been approved by the site's administrators. Please try again later.";
            }
            else if (user.IsLockedOut)
            {
                return "Your account has been locked out because of a maximum number of incorrect login attempts. " +
                       "You will NOT be able to login until you contact a site administrator and have your account unlocked.";
            }

            return "Your password is incorrect.";
        }
    }
}
