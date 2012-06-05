using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TickDataImporter;

namespace TestProject1
{
    /// <summary>
    ///This is a test class for TickDateTest and is intended
    ///to contain all TickDateTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TickDateTest
    {
        private TestContext testContextInstance;

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

        /// <summary>
        ///A test for TickDate Constructor
        ///</summary>
        [TestMethod()]
        public void TickDateConstructorTest()
        {
            string tickentry = "01/08/1987,09:30:00,1861.0,0,P";
            string instrumentID = "CCH87";
            TickDate target = new TickDate(tickentry, instrumentID);
            Assert.AreEqual(new DateTime(1987, 01, 08, 9, 30, 0).ToString(), target.GetDateTime().ToString());
            Assert.AreEqual("CCH87", target.GetSymbol());
            Assert.AreEqual(1861.0, target.GetPrice());
            Assert.AreEqual(0, target.GetVolume());
            target = new TickDate("11/07/2011,04:00:00.433,2750,1,E,,,2750", "CCH12");
            Assert.AreEqual(new DateTime(2011, 11, 07, 4, 0, 0, 433).ToString(), target.GetDateTime().ToString(), "different Date Time");
            Assert.AreEqual("CCH12", target.GetSymbol(), "Differenet Symbol");
            Assert.AreEqual(2750.0, target.GetPrice(), "Different Price");
            Assert.AreEqual(1, target.GetVolume(), "Different Volume");
        }

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes
    }
}