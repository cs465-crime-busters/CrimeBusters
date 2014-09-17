using System;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using CrimeBusters.WebApp.Models.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoginModel = CrimeBusters.WebApp.Models.Login;
using CrimeBusters.WebApp.Models.Users;
using CrimeBusters.WebApp.Models.DAL;
using CrimeBusters.WebApp.Services;



namespace CrimeBusters.WebApp.Tests
{
    [TestClass]
    public class LoginTest
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            if (Membership.FindUsersByName("test.user2") != null)
            {
                Membership.DeleteUser("test.user2");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            Membership.DeleteUser("test.user2");
            LoginDAO.DeleteUser("test.user2");
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "|DataDirectory|\\LoginData.xml",
                    "CreateUser",
                    DataAccessMethod.Sequential),
        DeploymentItem("~/LoginData.xml"),
        TestMethod]
        public void TestCreateUser()
        {
            String userName = Convert.ToString(TestContext.DataRow["UserName"]);
            String password = Convert.ToString(TestContext.DataRow["Password"]);
            String firstName = Convert.ToString(TestContext.DataRow["FirstName"]);
            String lastName = Convert.ToString(TestContext.DataRow["LastName"]);
            String email = Convert.ToString(TestContext.DataRow["Email"]);
            String expectedResult = Convert.ToString(TestContext.DataRow["Result"]);

            IUser newUser = new User
            {
                UserName = userName,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };
            LoginModel.Login login = new LoginModel.Login(newUser);
            String actualResult = login.CreateUser(new TestContentLocator()).ToString();

            Assert.AreEqual(expectedResult, actualResult);
        }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                    "|DataDirectory|\\LoginData.xml",
                    "Credentials", 
                    DataAccessMethod.Sequential), 
        DeploymentItem("~/LoginData.xml"), 
        TestMethod]
        public void TestValidateUser()
        {
            String userName = Convert.ToString(TestContext.DataRow["UserName"]);
            String password = Convert.ToString(TestContext.DataRow["Password"]);
            String expectedResult = Convert.ToString(TestContext.DataRow["Result"]);

            IUser user = new User
            {
                UserName = userName,
                Password = password
            };
            LoginModel.Login login = new LoginModel.Login(user);
            String actualResult = login.ValidateUser();

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
