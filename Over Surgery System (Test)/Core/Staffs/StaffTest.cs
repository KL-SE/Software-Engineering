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
    public class StaffTest
    {
        public static MedicalStaff medicalStaff;
        public static Receptionist receptionist;

        public static void SetupStaff()
        {
            PersonalDetailsTest.SetupPersonalDetails();
            medicalStaff            = new MedicalStaff();
            medicalStaff.Details    = PersonalDetailsTest.details;
            medicalStaff.Password   = "TEST";
            medicalStaff.Active     = true;
            medicalStaff.Nurse      = true;
            medicalStaff.LicenseNo  = "TESTLICENSE101";
            medicalStaff.DateJoined = DateTime.Now.Date;
            
            receptionist            = new Receptionist();
            receptionist.Details    = PersonalDetailsTest.details;
            receptionist.Password   = "TEST";
            receptionist.Active     = false;
            receptionist.Admin      = true;
            receptionist.DateJoined = DateTime.Now.Date;
        }

        public static void CleanupStaff()
        {
            medicalStaff.Delete();
            receptionist.Delete();
            PersonalDetailsTest.CleanupPersonalDetails();
        }
        
        [TestInitialize]
        public void _SetupStaff()
        {
            SetupStaff();
        }
        
        [TestCleanup]
        public void _CleanupStaff()
        {
            CleanupStaff();
        }

        [TestMethod]
        public void CompleteStaffTest()
        {
            // Save the detail and test it's ID and password (since the password is irretrievable)
            medicalStaff.Save();
            receptionist.Save();

            Assert.IsTrue(      medicalStaff.Valid                         );
            Assert.IsNotNull(   medicalStaff.StringId                      );
            Assert.IsTrue(      medicalStaff.HavePassword()                );
            Assert.IsTrue(      medicalStaff.IsPasswordCorrect( "TEST" )   );

            Assert.IsTrue(      receptionist.Valid                         );
            Assert.IsNotNull(   receptionist.StringId                      );
            Assert.IsTrue(      receptionist.HavePassword()                );
            Assert.IsTrue(      receptionist.IsPasswordCorrect( "TEST" )   );

            // Duplicate Setup
            MedicalStaff newMedicalStaff = StaffsManager.GetMedicalStaff( medicalStaff.Id  );
            Receptionist newReceptionist = StaffsManager.GetReceptionist( receptionist.Id  );
            
            // Test it's content
            Assert.AreEqual( medicalStaff.Id            , newMedicalStaff.Id            );
            Assert.AreEqual( medicalStaff.Details       , newMedicalStaff.Details       );
            Assert.AreEqual( medicalStaff.Active        , newMedicalStaff.Active        );
            Assert.AreEqual( medicalStaff.Nurse         , newMedicalStaff.Nurse         );
            Assert.AreEqual( medicalStaff.LicenseNo     , newMedicalStaff.LicenseNo     );
            Assert.AreEqual( medicalStaff.DateJoined    , newMedicalStaff.DateJoined    );
            Assert.IsTrue( newMedicalStaff.HavePassword()                               );
            Assert.IsTrue( newMedicalStaff.IsPasswordCorrect( "TEST" )                  );
            
            Assert.AreEqual( receptionist.Id            , newReceptionist.Id            );
            Assert.AreEqual( receptionist.Details       , newReceptionist.Details       );
            Assert.AreEqual( receptionist.Active        , newReceptionist.Active        );
            Assert.AreEqual( receptionist.Admin         , newReceptionist.Admin         );
            Assert.AreEqual( receptionist.DateJoined    , newReceptionist.DateJoined    );
            Assert.IsTrue( newReceptionist.HavePassword()                               );
            Assert.IsTrue( newReceptionist.IsPasswordCorrect( "TEST" )                  );
        }
    }
}
