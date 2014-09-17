using System;
using System.Web;
using CrimeBusters.WebApp.Models.Documents;
using CrimeBusters.WebApp.Models.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrimeBusters.WebApp.Models.Report;
using CrimeBusters.WebApp.Models.Users;
using CrimeBusters.WebApp.Models.DAL;
using System.Collections.Generic;

namespace CrimeBusters.WebApp.Tests
{
    [TestClass]
    public class ReportTest
    {
        public TestContext TestContext { get; set; }
        private Report _testReport;

        [TestInitialize]
        public void Initialize()
        {
            _testReport = new Report(
               ReportTypeEnum.HighPriority,
               "Test message",
               "40.104669",
               "-88.242254",
               "University of Illinois Campus",
               DateTime.UtcNow,
               new User("test.user"));
            _testReport.CreateReport(null);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _testReport = null;
            ReportsDAO.DeleteReportTest();
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\ReportData.xml",
            "CreateReport",
            DataAccessMethod.Sequential),
        DeploymentItem("~/ReportData.xml"),
        TestMethod]
        public void TestCreateReport()
        {
            ReportTypeEnum reportTypeId = (ReportTypeEnum)Enum.Parse(
                typeof(ReportTypeEnum),
                TestContext.DataRow["ReportTypeId"].ToString());
            String message = TestContext.DataRow["Message"].ToString();
            String latitude = TestContext.DataRow["Latitude"].ToString();
            String longitude = TestContext.DataRow["Longitude"].ToString();
            String location = TestContext.DataRow["Location"].ToString();
            DateTime dateReported = Convert.ToDateTime(TestContext.DataRow["DateReported"]);
            User user = new User
            {
                UserName = TestContext.DataRow["UserName"].ToString()
            };
            String expectedResult = TestContext.DataRow["Result"].ToString();

            Report report = new Report(reportTypeId, message, latitude,
                longitude, location, dateReported, user);
            string actualResult = report.CreateReport(new TestContentLocator());
            Assert.IsTrue(actualResult.Contains(expectedResult), actualResult);
        }

        [TestMethod]
        public void TestCreateReportWithNoFile()
        {
            Report report = new Report(
                ReportTypeEnum.HighPriority, 
                "Test message", 
                "40.104669", 
                "-88.242254", 
                "University of Illinois Campus",
                DateTime.UtcNow,
                new User("test.user"));
            string result = report.CreateReport(null);
       
            Assert.IsTrue(result.Equals("success"), result);
        }

        [TestMethod]
        public void TestGetReports()
        {
            List<Report> reports = Report.GetReports();
            Assert.IsTrue(reports.Count > 0);
        }
    }
}
