using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrimeBusters.WebApp.Models.DAL;

namespace CrimeBusters.WebApp
{
    public partial class CreateUserTemp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateUser_Click(object sender, EventArgs e)
        {
            try
            {
                Membership.CreateUser(UserNameTextBox.Text, PasswordTextBox.Text, EmailTextBox.Text);
                LoginDAO.CreateUserDetailsTemp(UserNameTextBox.Text, FirstNameTextBox.Text, LastNameTextBox.Text, PhoneNumberTextBox.Text);

                Roles.AddUserToRole(UserNameTextBox.Text, RoleRadioButtonList.SelectedValue);

                ResultLabel.Text = "Successfully created user.";
            }
            catch (Exception ex)
            {
                ResultLabel.Text = ex.Message;
            }
        }
    }
}