using CrimeBusters.WebApp.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Contains all the web service calls on the police home page.
    /// </summary>
    [WebService(Namespace = "http://illinoiscrimebusters.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)] 
    [System.Web.Script.Services.ScriptService]
    public class Index : System.Web.Services.WebService
    {
        /// <summary>
        /// Gets active reports from the database. 
        /// </summary>
        /// <returns>List of Report object.</returns>
        [WebMethod]
        public List<Report> GetReports() 
        {
            return Report.GetReports();
        }
    }
}
