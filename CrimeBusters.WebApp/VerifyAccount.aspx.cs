using System;
using System.Web.Security;

namespace CrimeBusters.WebApp
{
    public partial class VerifyAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String userIdString = Request.QueryString["u"];

            try
            {
                MembershipUser user = Membership.GetUser(Guid.Parse(userIdString));
                user.IsApproved = true;
                Membership.UpdateUser(user);

                ResultLabel.Text = "Successfully verified user account. You can now log in to the Crime Buster app.";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = "Error verifying user account. Error: " + ex.Message;
            }
        }
    }
}