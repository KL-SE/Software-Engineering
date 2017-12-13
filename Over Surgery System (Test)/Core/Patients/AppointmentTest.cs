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
    public class AppointmentTest
    {
        public static Appointment appointment;

        public static void SetupAppointment()
        {
            StaffTest.SetupStaff();
            PatientTest.SetupPatient();
            appointment                 = new Appointment();
            appointment.DateAppointed   = DateTime.Now.AddHours( 1 );
            appointment.Patient         = PatientTest.patient;
            appointment.MedicalStaff    = StaffTest.medicalStaff;
            appointment.Remark          = "Test Remark";
            appointment.Cancelled       = false;
        }

        public static void CleanupAppointment()
        {
            appointment.Delete();
            PatientTest.CleanupPatient();
            StaffTest.CleanupStaff();
        }
        
        [TestInitialize]
        public void _SetupAppointment()
        {
            SetupAppointment();
        }
        
        [TestCleanup]
        public void _CleanupAppointment()
        {
            CleanupAppointment();
        }

        [TestMethod]
        public void CompleteAppointmentTest()
        {
            // Save the object and test it's ID
            appointment.Save();
            Assert.IsTrue( appointment.Valid       );
            Assert.IsNotNull( appointment.StringId );
            
            Assert.IsFalse( appointment.Ended );

            // Duplicate Setup
            Appointment newAppointment     = PatientsManager.GetAppointment( appointment.Id );
            
            // Test it's content
            Assert.AreEqual( appointment.Id             , newAppointment.Id             );
            Assert.AreEqual( appointment.DateAppointed  , newAppointment.DateAppointed  );
            Assert.AreEqual( appointment.Patient        , newAppointment.Patient        );
            Assert.AreEqual( appointment.MedicalStaff   , newAppointment.MedicalStaff   );
            Assert.AreEqual( appointment.Remark         , newAppointment.Remark         );
            Assert.AreEqual( appointment.Cancelled      , newAppointment.Cancelled      );
        }
    }
}
