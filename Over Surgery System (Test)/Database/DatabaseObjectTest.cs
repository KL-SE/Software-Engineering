using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverSurgerySystem;
using OverSurgerySystem.Core.Base;
using System;

namespace OverSurgerySystem_Test
{
    public partial class DatabaseTest
    {
        [TestMethod]
        public void DatabaseObjectSanityTest()
        {
            // Any class derived from the DatabaseObject class can be used.
            DatabaseObject dbObject = new City();

            Assert.IsFalse( dbObject.Loaded );
            Assert.AreEqual( DatabaseObject.INVALID_ID          , dbObject.Id           );
            Assert.AreEqual( DatabaseObject.INVALID_DATETIME    , dbObject.LastSaved    );
            Assert.AreEqual( DatabaseObject.INVALID_DATETIME    , dbObject.CreatedOn    );
        }
    }
}
