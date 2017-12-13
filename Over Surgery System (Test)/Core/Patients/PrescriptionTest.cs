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
    public class PrescriptionTest
    {
        public static Prescription prescription;
        public static PrescriptionMedication prescriptionMed;

        public static void SetupPrescription()
        {
            StaffTest.SetupStaff();
            PatientTest.SetupPatient();
            MedicationTest.SetupMedication();
            prescription            = new Prescription();
            prescription.StartDate  = DateTime.Now.Date;
            prescription.EndDate    = DateTime.Now.Date.AddDays( 5 );
            prescription.Patient    = PatientTest.patient;
            prescription.Prescriber = StaffTest.medicalStaff;
            prescription.Name       = "Test Name";
            prescription.Remark     = "Test Remark";
            prescriptionMed         = new PrescriptionMedication( prescription , MedicationTest.medication );

            prescription.AddMedication( prescriptionMed );
        }

        public static void CleanupPrescription()
        {
            prescriptionMed.Delete();
            prescription.Delete();
            MedicationTest.CleanupMedication();
            PatientTest.CleanupPatient();
            StaffTest.CleanupStaff();
        }
        
        [TestInitialize]
        public void _SetupPrescription()
        {
            SetupPrescription();
        }
        
        [TestCleanup]
        public void _CleanupPrescription()
        {
            CleanupPrescription();
        }

        [TestMethod]
        public void CompletePrescriptionTest()
        {
            // Save the object and test it's ID
            prescription.Save();
            prescriptionMed.Save();
            Assert.IsTrue( prescription.Valid       );
            Assert.IsTrue( prescriptionMed.Valid    );
            Assert.IsNotNull( prescription.StringId );
            
            Assert.IsTrue( prescription.Started  );
            Assert.IsFalse( prescription.Ended   );
            
            Assert.AreEqual( prescriptionMed.Prescription   , prescription                      );
            Assert.AreEqual( prescriptionMed.Code           , MedicationTest.medication.Code    );
            Assert.AreEqual( prescriptionMed.Name           , MedicationTest.medication.Name    );

            // Duplicate Setup
            Prescription            newPrescription     = PatientsManager.GetPrescription( prescription.Id );
            PrescriptionMedication  newPrescriptionMed  = PatientsManager.GetPrescriptionMedication( prescriptionMed.Id );
            
            // Test it's content
            Assert.AreEqual( prescription.Id                , newPrescription.Id                );
            Assert.AreEqual( prescription.StartDate         , newPrescription.StartDate         );
            Assert.AreEqual( prescription.EndDate           , newPrescription.EndDate           );
            Assert.AreEqual( prescription.Patient           , newPrescription.Patient           );
            Assert.AreEqual( prescription.Prescriber        , newPrescription.Prescriber        );
            Assert.AreEqual( prescription.Remark            , newPrescription.Remark            );

            Assert.AreEqual( prescriptionMed.Id             , newPrescriptionMed.Id             );
            Assert.AreEqual( prescriptionMed.Code           , newPrescriptionMed.Code           );
            Assert.AreEqual( prescriptionMed.Name           , newPrescriptionMed.Name           );
            Assert.AreEqual( prescriptionMed.Prescription   , newPrescriptionMed.Prescription   );
        }
    }
}
