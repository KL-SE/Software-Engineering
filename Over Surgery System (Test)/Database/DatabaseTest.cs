using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Data;
using OverSurgerySystem;

namespace OverSurgerySystem_Test
{
    [TestClass]
    public partial class DatabaseTest
    {
        [TestMethod]
        public void DatabaseConnectionTest()
        {
            MySqlConnection connection = Database.Connection;
            Assert.IsNotNull( connection );
            Assert.IsTrue( connection.State == ConnectionState.Open );
        }
    }
}
