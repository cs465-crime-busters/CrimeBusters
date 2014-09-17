using System.Data;
using System.Data.SqlClient;

namespace CrimeBusters.WebApp.Models.DAL
{
    public class LoginDAO
    {
        /// <summary>
        /// Creates User Details given username, firstName, lastName and email
        /// for Android account creation
        /// </summary>
        public static void CreateUserDetails(string userName, string firstName, 
            string lastName, string email)
        {
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("CreateUserDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserName", userName);
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Email", email);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete User Details given username
        /// </summary>
        public static void DeleteUser(string userName)
        {
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("DeleteUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserName", userName);
                command.ExecuteNonQuery();
            }
        }
    }
}