using EffiprozSchemaInquiry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BigWoo.Apps.SqlCodeGen.Classes;
using System.Data.Common;
using System.Data.EffiProz;
using System;

namespace UnitTesting
{


    /// <summary>
    /// This is a test class for EffiprozTableInquiryTest and is intended
    /// to contain all EffiprozTableInquiryTest Unit Tests
    /// </summary>
    [TestClass()]
    public class EffiprozTableInquiryTest
    {
        private const string CONN_STRING = "Connection Type=File ; Initial Catalog=|DataDirectory|/Efz/TestDB; User=sa; Password=;";
        private DbConnection _connection = null;

        #region test attributes
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
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
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [TestInitialize()]
        public void EffiprozTableInquiryTestInitialize()
        {
            _connection = new EfzConnection(CONN_STRING);
            _connection.ConnectionString = CONN_STRING;
            _connection.Open();

            DbCommand command = _connection.CreateCommand();
            command.CommandText = "DROP TABLE ExistingTable IF EXISTS; CREATE TABLE ExistingTable(ID INT PRIMARY KEY, Name VARCHAR(100));";
            command.ExecuteNonQuery();

            command = _connection.CreateCommand();
            command.CommandText = "DROP TABLE AnotherExistingTable IF EXISTS; CREATE TABLE AnotherExistingTable(ID INT PRIMARY KEY, Name VARCHAR(100), ThirdColumn VARCHAR(10));";
            command.ExecuteNonQuery();

        }

        /// <summary>
        /// 
        /// </summary>
        [TestCleanup()]
        public void EffiprozTableInquiryTestCleanup()
        {
            _connection.Close();
        }

        /// <summary>
        /// A test for BuildTableInfo
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Effiproz.dll")]
        [DeploymentItem("EffiprozSchemaInquiry.dll")]
        public void BuildTableInfoTest()
        {
            EffiprozTableInquiry_Accessor target = new EffiprozTableInquiry_Accessor();

            TableDefinition table = new TableDefinition("ExistingTable");

            target.ConnectionStr = CONN_STRING;
            target.GetConnection();

            try
            {
                target.BuildTableInfo(table);

                Assert.IsTrue(2 == table.Columns.Count);
                Assert.AreEqual("ID", table.Columns[0].Name);
                Assert.IsTrue(true == table.Columns[0].IsPrimaryKey);
                Assert.AreEqual("Name", table.Columns[1].Name);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Exception type {0}", ex.GetType().Name));
            }
            finally
            {
                target.CloseConnection();
            }

        }

        /// <summary>
        /// A test for CheckForTable
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Effiproz.dll")]
        [DeploymentItem("EffiprozSchemaInquiry.dll")]
        public void CheckForTableTest()
        {
            EffiprozTableInquiry_Accessor target = new EffiprozTableInquiry_Accessor();
            TableDefinition table = new TableDefinition("ExistingTable");
            target.ConnectionStr = CONN_STRING;
            target.GetConnection();

            try
            {
                target.CheckForTable(table);
            }
            finally
            {
                target.CloseConnection();
            }

        }

        /// <summary>
        /// A test for CheckForTable
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Effiproz.dll")]
        [DeploymentItem("EffiprozSchemaInquiry.dll")]
        [ExpectedException(typeof(MissingTableException))]
        public void CheckForNonExistingTableTest()
        {
            EffiprozTableInquiry_Accessor target = new EffiprozTableInquiry_Accessor();
            TableDefinition table = new TableDefinition("DoesNotExistTable");
            target.ConnectionStr = CONN_STRING;
            target.GetConnection();
            try
            {
                target.CheckForTable(table);
            }
            finally
            {
                target.CloseConnection();
            }

        }

        /// <summary>
        /// A test for ProcessTables
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Effiproz.dll")]
        [DeploymentItem("EffiprozSchemaInquiry.dll")]
        public void ProcessTablesTest()
        {
            EffiprozTableInquiry target = new EffiprozTableInquiry();
            TableDefinition existingTable = new TableDefinition("ExistingTable");
            TableDefinition anotherExistingTable = new TableDefinition("AnotherExistingTable");
            TableDefinitionCollection tables = new TableDefinitionCollection();
            tables.Add(existingTable);
            tables.Add(anotherExistingTable);
            target.ConnectionStr = CONN_STRING;
            target.ProcessTables(tables);

            Assert.IsTrue(2 == existingTable.Columns.Count);
            Assert.AreEqual("ID", existingTable.Columns[0].Name);
            Assert.IsTrue(true == existingTable.Columns[0].IsPrimaryKey);
            Assert.AreEqual("NAME", existingTable.Columns[1].Name);

            Assert.IsTrue(3 == anotherExistingTable.Columns.Count);
            Assert.AreEqual("ID", anotherExistingTable.Columns[0].Name);
            Assert.IsTrue(true == anotherExistingTable.Columns[0].IsPrimaryKey);
            Assert.AreEqual("NAME", anotherExistingTable.Columns[1].Name);
            Assert.AreEqual("THIRDCOLUMN", anotherExistingTable.Columns[2].Name);

        }
    }
}
