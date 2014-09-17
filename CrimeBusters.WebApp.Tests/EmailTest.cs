using System;
using CrimeBusters.WebApp.Models.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrimeBusters.WebApp.Tests
{
    [TestClass]
    public class EmailTest
    {
        public TestContext TestContext { get; set; }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\EmailData.xml",
            "SendEmail",
            DataAccessMethod.Sequential),
        DeploymentItem("~/EmailData.xml"),
        TestMethod]
        public void TestSendEmail()
        {
            String fromEmail = TestContext.DataRow["FromEmail"].ToString();
            String fromName = TestContext.DataRow["FromName"].ToString();
            String toEmail = TestContext.DataRow["ToEmail"].ToString();
            String subject = TestContext.DataRow["Subject"].ToString();
            String body = TestContext.DataRow["Body"].ToString();
            Boolean isHighImportance = Convert.ToBoolean(TestContext.DataRow["IsHighImportance"]);
            String expectedResult = TestContext.DataRow["Result"].ToString();

            Email email = new Email
            {
                FromEmail = fromEmail,
                FromName = fromName,
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                IsHighImportance = isHighImportance
            };
            string actualResult = email.SendEmail();
            Assert.IsTrue(actualResult.Contains(expectedResult), actualResult);
        }
    }
}
