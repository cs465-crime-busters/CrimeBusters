using System;

namespace CrimeBusters.WebApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Index.aspx");
            }
        }
    }
}