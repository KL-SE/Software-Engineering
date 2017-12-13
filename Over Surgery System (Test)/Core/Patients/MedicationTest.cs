using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverSurgerySystem.Core.Patients;
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
    public class MedicationTest
    {
        public static Medication medication;

        public static void SetupMedication()
        {
            medication      = new Medication();
            medication.Code = "MED001";
            medication.Name = "Test Medication";
        }

        public static void CleanupMedication()
        {
            medication.Delete();
        }
        
        [TestInitialize]
        public void _SetupMedication()
        {
            SetupMedication();
        }
        
        [TestCleanup]
        public void _CleanupMedication()
        {
            CleanupMedication();
        }

        [TestMethod]
        public void CompleteMedicationTest()
        {
            // Save the object and test it's ID
            medication.Save();
            Assert.IsTrue( medication.Valid );

            // Duplicate Setup
            Medication newMedication = PatientsManager.GetMedication( medication.Id );
            
            // Test it's content
            Assert.AreEqual( medication.Id      , newMedication.Id      );
            Assert.AreEqual( medication.Code    , newMedication.Code    );
            Assert.AreEqual( medication.Name    , newMedication.Name    );
        }
    }
}
