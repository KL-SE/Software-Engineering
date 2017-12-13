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
    public class TestResultTest
    {
        public static OverSurgerySystem.Core.Patients.TestResult testResult;

        public static void SetupTestResult()
        {
            StaffTest.SetupStaff();
            PatientTest.SetupPatient();
            testResult                  = new OverSurgerySystem.Core.Patients.TestResult();
            testResult.Patient          = PatientTest.patient;
            testResult.MedicalLicenseNo = StaffTest.medicalStaff.LicenseNo;
            testResult.Name             = "Test Name";
            testResult.Description      = "Test Description";
            testResult.Result           = "Test Result";
            testResult.Remark           = "Test Remark";
        }

        public static void CleanupTestResult()
        {
            testResult.Delete();
            PatientTest.CleanupPatient();
            StaffTest.CleanupStaff();
        }
        
        [TestInitialize]
        public void _SetupTestResult()
        {
            SetupTestResult();
        }
        
        [TestCleanup]
        public void _CleanupTestResult()
        {
            CleanupTestResult();
        }

        [TestMethod]
        public void CompleteTestResultTest()
        {
            // Save the object and test it's ID
            testResult.Save();
            Assert.IsTrue( testResult.Valid       );
            Assert.IsNotNull( testResult.StringId );

            // Duplicate Setup
            OverSurgerySystem.Core.Patients.TestResult newTestResult = PatientsManager.GetTestResult( testResult.Id );
            
            // Test it's content
            Assert.AreEqual( testResult.Id                  , newTestResult.Id                  );
            Assert.AreEqual( testResult.Patient             , newTestResult.Patient             );
            Assert.AreEqual( testResult.MedicalLicenseNo    , newTestResult.MedicalLicenseNo    );
            Assert.AreEqual( testResult.Name                , newTestResult.Name                );
            Assert.AreEqual( testResult.Description         , newTestResult.Description         );
            Assert.AreEqual( testResult.Result              , newTestResult.Result              );
            Assert.AreEqual( testResult.Remark              , newTestResult.Remark              );
        }
    }
}
