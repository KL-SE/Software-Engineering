using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem_Test
{
    [TestClass]
    public class PatientTest
    {
        public static Patient patient;

        public static void SetupPatient()
        {
            PersonalDetailsTest.SetupPersonalDetails();
            patient         = new Patient();
            patient.Details = PersonalDetailsTest.details;
        }

        public static void CleanupPatient()
        {
            patient.Delete();
            PersonalDetailsTest.CleanupPersonalDetails();
        }
        
        [TestInitialize]
        public void _SetupPatient()
        {
            SetupPatient();
        }
        
        [TestCleanup]
        public void _CleanupPatient()
        {
            CleanupPatient();
        }

        [TestMethod]
        public void CompletePatientTest()
        {
            // Save the detail and test it's ID
            patient.Save();
            Assert.IsTrue(      patient.Valid       );
            Assert.IsNotNull(   patient.StringId    );

            // Duplicate Setup
            Patient newPatient = PatientsManager.GetPatient( patient.Id );
            
            // Patient class is really simple at the moment, so we just need to match these up.
            Assert.AreEqual( patient.Id         , newPatient.Id         );
            Assert.AreEqual( patient.Details    , newPatient.Details    );
        }
    }
}
