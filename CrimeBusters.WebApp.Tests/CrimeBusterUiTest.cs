using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace CrimeBusters.WebApp.Tests
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CrimeBusterUiTest
    {
        public CrimeBusterUiTest()
        {
        }

        [TestInitialize]
        public void LoginUiTest()
        {
            this.UIMap.AssertUserNameExists();
            this.UIMap.AssertPasswordExists();
            this.UIMap.AssertRememberMeExists();
            this.UIMap.AssertLoginButtonExists();
            this.UIMap.LoginAsPolice();
        }

        [TestMethod]
        public void MainUiTest()
        {
            this.UIMap.AssertPoliceUserNameShown();
            this.UIMap.AssertMapExists();
            this.UIMap.AssertDropDownMenu();
            this.UIMap.AssertSignOutExists();
        }

        [TestMethod]
        public void MenuUiTest()
        {
            this.UIMap.FilterReports();
            this.UIMap.AssertAllAlerts();
            this.UIMap.AssertHighAlerts();
            this.UIMap.AssertLowAlerts();
        }

        [TestCleanup]
        public void LogoutUiTest()
        {
            this.UIMap.LogOut();
        }


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
