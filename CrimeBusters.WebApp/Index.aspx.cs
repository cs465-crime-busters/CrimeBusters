using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrimeBusters.WebApp.Models.Report;

namespace CrimeBusters.WebApp
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GetReportsByDate(object sender, EventArgs e)
        {
            List<Report> l = Report.GetActiveReports();
            SearchList.Items.Clear();
            ResultLabel.Text = "";
            ResultLabel.Visible = false;
            bool first = true;
            foreach (Report r in l)
            {
                ListItem i = new ListItem();
                DateTime d = Convert.ToDateTime(r.TimeStampString);
                 //&& (int)r.ReportTypeId == CrimeTypeList.SelectedValue
                if (d >= Convert.ToDateTime(fromDate.Text) && d <= Convert.ToDateTime(toDate.Text) && r.ReportType.Trim().Equals(CrimeTypeList.SelectedValue))
                {
                    i.Text = r.Location;
                    if (first == true)
                    {
                        i.Selected = true;
                        first = false;
                    }
                    SearchList.Items.Add(i);
                    ResultLabel.Text = " ";
                    
                }
            }
        }
    }
}