using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TickDataImporter;

namespace TestProject1
{
    /// <summary>
    ///This is a test class for TickToBarConverterTest and is intended
    ///to contain all TickToBarConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TickToBarConverterTest
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

        /// <summary>
        ///A test for GetDateTimeList
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TickDataImporter.exe")]
        public void GetDateTimeListTest()
        {
            TickToBarConverter_Accessor target = new TickToBarConverter_Accessor();
            DateTime maxDate = new DateTime(2012, 10, 10, 18, 05, 00);
            DateTime minDate = new DateTime(2012, 10, 10, 15, 05, 00);
            IntradayFrequency freq = IntradayFrequency.OneHour;
            List<DateTime> actual;
            actual = target.GetDateTimeList(maxDate, minDate, freq);
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(new DateTime(2012, 10, 10, 16, 0, 0).ToString(), actual[0].ToString());
            Assert.AreEqual(new DateTime(2012, 10, 10, 17, 0, 0).ToString(), actual[1].ToString());
            Assert.AreEqual(new DateTime(2012, 10, 10, 18, 0, 0).ToString(), actual[2].ToString());

            maxDate = new DateTime(2012, 10, 10, 18, 16, 17);
            minDate = new DateTime(2012, 10, 10, 15, 18, 22);
            freq = IntradayFrequency.FifteenMinutes;
            actual = target.GetDateTimeList(maxDate, minDate, freq);
            Assert.AreEqual(13, actual.Count);
            Assert.AreEqual(new DateTime(2012, 10, 10, 15, 30, 0).ToString(), actual[0].ToString());
            Assert.AreEqual(new DateTime(2012, 10, 10, 15, 45, 0).ToString(), actual[1].ToString());
            Assert.AreEqual(new DateTime(2012, 10, 10, 16, 00, 0).ToString(), actual[2].ToString());
        }
    }
}