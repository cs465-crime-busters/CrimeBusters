using System.IO;
using CrimeBusters.WebApp.Models.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CrimeBusters.WebApp.Models.Users
{
    /// <summary>
    /// Contains the business logic for the Users (Android Users)
    /// </summary>
    public class User : IUser
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Gender { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String Address { get; set; }
        public String ZipCode { get; set; }
        public User() { }
        public User(string userName)
        {
            this.UserName = userName;
        }

        /// <summary>
        /// Gets the user information from the database.
        /// </summary>
        /// <param name="userName">UserName of the user.</param>
        /// <returns>User object with properties filled up.</returns>
        public static User GetUser(string userName) {

            User user = null;
            SqlDataReader reader = UsersDAO.GetUserInformation(userName);

            try
            {
                while (reader.Read())
                {
                    user = new User
                    {
                        UserName = reader["UserName"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNumber = reader["PhoneNumber"].ToString(),
                        Address = reader["Address"].ToString(),
                        ZipCode = reader["ZipCode"].ToString()
                    };
                }
            }
            catch (Exception)
            {

            }
            finally 
            {
                reader.Close();
            }
            return user;
        }

        /// <summary>
        /// Updates the user information with the new information 
        /// that will be provided.
        /// </summary>
        public void UpdateProfile()
        {
            if (this.Gender.ToUpper() != "M" 
                && this.Gender.ToUpper() != "F")
            {
                throw  new InvalidDataException("Invalid gender");
            }

            UsersDAO.UpdateUserInformation(this.FirstName, 
                this.LastName, this.Gender, this.PhoneNumber, 
                this.Address, this.ZipCode, this.UserName);
        }
    }
}