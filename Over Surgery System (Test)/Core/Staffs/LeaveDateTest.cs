using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverSurgerySystem_Test
{
    [TestClass]
    public class LeaveDateTest
    {
        public static LeaveDate leaveDate;

        public static void SetupLeaveDate()
        {
            StaffTest.SetupStaff();
            leaveDate           = new LeaveDate();
            leaveDate.Owner     = StaffTest.medicalStaff;
            leaveDate.Remark    = "Test Remark";
            leaveDate.Date      = DateTime.Now.Date;
        }

        public static void CleanupLeaveDate()
        {
            leaveDate.Delete();
            StaffTest.CleanupStaff();
        }
        
        [TestInitialize]
        public void _SetupLeaveDate()
        {
            SetupLeaveDate();
        }
        
        [TestCleanup]
        public void _CleanupLeaveDate()
        {
            CleanupLeaveDate();
        }

        [TestMethod]
        public void CompleteLeaveDateTest()
        {
            // Save the object and test it's ID
            leaveDate.Save();
            Assert.IsTrue( leaveDate.Valid );

            // Duplicate Setup
            LeaveDate newLeaveDate = StaffsManager.GetLeaveDate( leaveDate.Id );
            
            // Test it's content
            Assert.AreEqual( leaveDate.Id       , newLeaveDate.Id       );
            Assert.AreEqual( leaveDate.Owner    , newLeaveDate.Owner    );
            Assert.AreEqual( leaveDate.Remark   , newLeaveDate.Remark   );
            Assert.AreEqual( leaveDate.Date     , newLeaveDate.Date     );
        }
    }
}
