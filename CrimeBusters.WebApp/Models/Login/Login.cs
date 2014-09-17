using System.IO;
using CrimeBusters.WebApp.Models.DAL;
using CrimeBusters.WebApp.Models.Users;
using System;
using System.Web.Security;
using CrimeBusters.WebApp.Models.Util;

namespace CrimeBusters.WebApp.Models.Login
{
    /// <summary>
    /// Contains the business logic for the Login module
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Login User
        /// </summary>
        public IUser User { get; set; }

        /// <summary>
        /// Empty Contstructor
        /// </summary>
        public Login() { }

        /// <summary>
        /// Constructor for creating a Login given a user
        /// </summary>
        public Login(IUser user)
        {
            this.User = user;
        }

        /// <summary>
        /// Creates a user to the database with IsApproved = false.
        /// </summary>
        /// <param name="contentLocator">Whether TestContentLocator or WebContentLocator</param>
        /// <returns></returns>
        public MembershipCreateStatus CreateUser(IContentLocator contentLocator)
        {
            if (String.IsNullOrEmpty(this.User.Email))
            {
                return MembershipCreateStatus.InvalidEmail;
            }

            MembershipCreateStatus createStatus;

            MembershipUser newUser = Membership.CreateUser(
                this.User.UserName, 
                this.User.Password,
                this.User.Email, 
                "dummy question",
                "dummy answer", 
                false, 
                out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                try
                {
                    Roles.AddUserToRole(this.User.UserName, "User");
                    CreateUserDetails();
                    SendVerificationEmail(newUser.ProviderUserKey.ToString(), contentLocator);
                }
                catch (Exception)
                {                    
                    return MembershipCreateStatus.UserRejected;
                }
            }
            return createStatus;
        }

        /// <summary>
        /// Sends an email to the user with verification link.
        /// </summary>
        private void SendVerificationEmail(String userId, IContentLocator contentLocator)
        {
            String filePath = contentLocator.GetPath("~/Content/documents/VerifyAccount.html");

            string emailContent =
                File.ReadAllText(filePath)
                    .Replace("%FirstName%", this.User.FirstName)
                    .Replace("%UserId%", userId);

            Email email = new Email
            {
                FromEmail = "admin@illinoiscrimebusters.com",
                FromName = "Crime Buster Admin",
                ToEmail = this.User.Email,
                Subject = "[Crime Busters] Please Verify Your Account",
                Body = emailContent,
                IsHighImportance = true
            };
            email.SendEmail();
        }

        /// <summary>
        /// Creates user details after creating a user. 
        /// </summary>
        private void CreateUserDetails()
        {
            LoginDAO.CreateUserDetails(this.User.UserName, this.User.FirstName, 
                this.User.LastName, this.User.Email);
        }

        /// <summary>
        /// Validates a user credential.
        /// </summary>
        /// <returns>true if the user credentials are valid</returns>
        public String ValidateUser()
        {
            if (Membership.ValidateUser(this.User.UserName, this.User.Password))
            {
                return "success";
            }
            MembershipUser user = Membership.GetUser(this.User.UserName);
            return ShowMeaningfulErrorMessage(this.User.UserName, user);
        }

        /// <summary>
        /// Shows a meaningful error message if the validation fails.
        /// </summary>
        /// <param name="userName">username of the user</param>
        /// <param name="user">MembershipUser object</param>
        /// <returns>Reason why the validation failed.</returns>
        private static String ShowMeaningfulErrorMessage(string userName, MembershipUser user)
        {
            if (user == null)
            {
                return "There is no user in the database with the username " + userName;
            }
            else if (!user.IsApproved)
            {
                return "Your account has not yet been verified. Please verify your account by clicking the link that you receive from your Illinois email address upon user creation.";
            }
            else if (user.IsLockedOut)
            {
                return "Your account has been locked out because of a maximum number of incorrect login attempts. " +
                       "You will NOT be able to login until you contact a site administrator and have your account unlocked.";
            }

            return "Your password is incorrect.";
        }

        /// <summary>
        /// Deletes a user to the database.
        /// </summary>
        public void DeleteUser()
        {
            Membership.DeleteUser(this.User.UserName);
            LoginDAO.DeleteUser(this.User.UserName);
        }
    }
}