using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrimeBusters.WebApp.Models.Users
{
    /// <summary>
    /// Interface for the User module to the outside module. 
    /// </summary>
    public interface IUser
    {
        String UserName { get; set; }
        String Password { get; set; }
        String FirstName { get; set; }
        String LastName { get; set; }
        String Gender { get; set; }
        String Email { get; set; }
        String PhoneNumber { get; set; }
        String Address { get; set; }
        String ZipCode { get; set; }
    }
}