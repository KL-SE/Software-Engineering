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
    public class WorkingDaysTest
    {
        public static WorkingDays workingDays;

        public static void SetupWorkingDays()
        {
            StaffTest.SetupStaff();
            workingDays             = new WorkingDays( StaffTest.medicalStaff );
            workingDays.Sunday      = true;
            workingDays.Monday      = false;
            workingDays.Tuesday     = false;
            workingDays.Wednesday   = false;
            workingDays.Thursday    = false;
            workingDays.Friday      = false;
            workingDays.Saturday    = true;
        }

        public static void CleanupWorkingDays()
        {
            workingDays.Delete();
            StaffTest.CleanupStaff();
        }
        
        [TestInitialize]
        public void _SetupWorkingDays()
        {
            SetupWorkingDays();
        }
        
        [TestCleanup]
        public void _CleanupWorkingDays()
        {
            CleanupWorkingDays();
        }

        [TestMethod]
        public void CompleteWorkingDaysTest()
        {
            // Save the object and test it's ID
            workingDays.Save();
            Assert.IsTrue(  workingDays.Valid    );
            Assert.IsTrue(  workingDays.Weekends );
            Assert.IsFalse( workingDays.Weekdays );

            Assert.IsTrue(  workingDays.WorkingOn( WorkingDays.Day.SUNDAY       ) );
            Assert.IsFalse( workingDays.WorkingOn( WorkingDays.Day.MONDAY       ) );
            Assert.IsFalse( workingDays.WorkingOn( WorkingDays.Day.TUESDAY      ) );
            Assert.IsFalse( workingDays.WorkingOn( WorkingDays.Day.WEDNESDAY    ) );
            Assert.IsFalse( workingDays.WorkingOn( WorkingDays.Day.THURSDAY     ) );
            Assert.IsFalse( workingDays.WorkingOn( WorkingDays.Day.FRIDAY       ) );
            Assert.IsTrue(  workingDays.WorkingOn( WorkingDays.Day.SATURDAY     ) );

            // Duplicate Setup
            WorkingDays newWorkingDays = StaffsManager.GetWorkingDays( workingDays.Id );
            
            // Test it's content
            Assert.AreEqual( workingDays.Id , newWorkingDays.Id );

            Assert.IsTrue(  newWorkingDays.Valid    );
            Assert.IsTrue(  newWorkingDays.Weekends );
            Assert.IsFalse( newWorkingDays.Weekdays );

            Assert.IsTrue(  newWorkingDays.WorkingOn( WorkingDays.Day.SUNDAY        ) );
            Assert.IsFalse( newWorkingDays.WorkingOn( WorkingDays.Day.MONDAY        ) );
            Assert.IsFalse( newWorkingDays.WorkingOn( WorkingDays.Day.TUESDAY       ) );
            Assert.IsFalse( newWorkingDays.WorkingOn( WorkingDays.Day.WEDNESDAY     ) );
            Assert.IsFalse( newWorkingDays.WorkingOn( WorkingDays.Day.THURSDAY      ) );
            Assert.IsFalse( newWorkingDays.WorkingOn( WorkingDays.Day.FRIDAY        ) );
            Assert.IsTrue(  newWorkingDays.WorkingOn( WorkingDays.Day.SATURDAY      ) );
        }
    }
}
