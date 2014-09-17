using System;
using System.Data;
using System.Data.SqlClient;

namespace CrimeBusters.WebApp.Models.DAL
{
    /// <summary>
    ///  Contains all the data access logic for the Users module.
    /// </summary>
    public class UsersDAO
    {
        /// <summary>
        /// Get User Information given the userName
        /// </summary>
        public static SqlDataReader GetUserInformation(String userName) {
            SqlConnection connection = ConnectionManager.GetConnection();
            SqlCommand command = new SqlCommand("GetUserInformation", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@UserName", userName);

            return command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Updated User Information
        /// String firstName : First Name
        /// String lastName : Last Name
        /// String gender : Gender
        /// String phoneNumber : Phone Number
        /// String address  : Address
        /// String zipCode :  Zip code
        /// String userName : User Name
        /// </summary>
        public static void UpdateUserInformation(String firstName, String lastName, String gender, 
            String phoneNumber, String address, String zipCode, String userName)
        {
            using (SqlConnection connection = ConnectionManager.GetConnection())
            {
                SqlCommand command = new SqlCommand("UpdateUserInformation", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Gender", gender);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@Address", address);
                command.Parameters.AddWithValue("@ZipCode", zipCode);
                command.Parameters.AddWithValue("@UserName", userName);

                command.ExecuteNonQuery();
            }
        }
    }
}