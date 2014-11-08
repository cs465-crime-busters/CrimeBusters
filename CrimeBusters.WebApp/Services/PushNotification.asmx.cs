using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CrimeBusters.WebApp.Models.Report;

namespace CrimeBusters.WebApp.Services
{
    /// <summary>
    /// Contains methods for Push Notifications
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class PushNotification : System.Web.Services.WebService
    {

        [WebMethod]
        public string AcknowledgeReport(int reportId)
        {
            Report report = new Report(reportId);
            return report.AcknowledgeReport();
        }
    }
}
