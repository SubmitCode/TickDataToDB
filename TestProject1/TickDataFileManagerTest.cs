using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TickDataImporter;

namespace TestProject1
{
    /// <summary>
    ///This is a test class for TickDataFileManagerTest and is intended
    ///to contain all TickDataFileManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TickDataFileManagerTest
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
        ///A test for DeleteAndCreateFolderStructure
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TickDataImporter.exe")]
        public void DeleteAndCreateFolderStructureTest()
        {
            TickDataFileManager_Accessor target = new TickDataFileManager_Accessor();
            string pathTempFolder = Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "Temp");
            Assert.IsFalse(System.IO.Directory.Exists(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "Temp")));
            target.DeleteAndCreateFolderStructure(pathTempFolder);
            Assert.IsTrue(System.IO.Directory.Exists(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "Temp")));
            System.IO.Directory.Delete(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "Temp"));
        }

        /// <summary>
        ///A test for ExtractGz
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TickDataImporter.exe")]
        public void ExtractGzTest()
        {
            TickDataFileManager_Accessor target = new TickDataFileManager_Accessor();
            string filePath = Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "BPH79_1978_04_04.asc.gz");
            string toFolder = CurrentFolder.GetCurrentTestFileFolder();
            target.ExtractGz(filePath, toFolder);
            Assert.IsTrue(System.IO.File.Exists(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "BPH79_1978_04_04.asc")));
            System.IO.File.Delete(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "BPH79_1978_04_04.asc"));
        }

        /// <summary>
        ///A test for ExtractZipIntoRawfiles
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TickDataImporter.exe")]
        public void ExtractZipIntoRawfilesTest()
        {
            TickDataFileManager_Accessor target = new TickDataFileManager_Accessor(); // TODO: Initialize to an appropriate value
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractRawfiles"));
            string pathFile = System.IO.Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "1987_01_CC.zip");
            string pathTempFolder = System.IO.Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractRawfiles");
            target.ExtractZipIntoRawfiles(pathFile, pathTempFolder);
            Assert.IsTrue(Directory.GetFiles(System.IO.Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractRawfiles"), "*.asc").Length == 83);
            System.IO.Directory.Delete(System.IO.Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractRawfiles"), true);
        }

        /// <summary>
        ///A test for ExtractZip
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TickDataImporter.exe")]
        public void ExtractZipTest()
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractZipTest"));
            TickDataFileManager_Accessor target = new TickDataFileManager_Accessor();
            string file = Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "1987_01_CC.zip");
            string extractToFolder = Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractZipTest");
            target.ExtractZip(file, extractToFolder);
            Assert.IsTrue(System.IO.Directory.GetFiles(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractZipTest"), "*.gz").Length == 84);
            System.IO.Directory.Delete(Path.Combine(CurrentFolder.GetCurrentTestFileFolder(), "ExtractZipTest"), true);
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