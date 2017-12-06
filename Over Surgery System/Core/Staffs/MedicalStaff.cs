using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Staffs
{
    public class MedicalStaff : Staff
    {
        public string LicenseNo         {         set; get; }
        public WorkingDays WorkingDays  { private set; get; }
        List<LeaveDate> leaveDates;
        
        public List<Prescription> GetPrescriptions()    { return PatientsManager.GetPrescriptionsByStaff( this );   }
        public List<Appointment> GetAppointments()      { return PatientsManager.GetAppointmentsByStaff( this );    }
        public List<TestResult> GetTests()              { return PatientsManager.GetTestResultsByStaff( this );     }

        // Custom comparator since MedicalStaff's identifying column is their staff_id, not the regular id.
        QueryComparator Identifier
        {
            get
            {
                QueryComparator idComparator    = new QueryComparator();
                idComparator.Source             = new QueryElement( Database.Tables.Receptionists.StaffId );
                idComparator.Operand            = new QueryElement( null , Id );
                idComparator.Equal              = true;
                return idComparator;
            }
        }
        
        // Medical Staff's Working Days
        public IList<LeaveDate> LeaveDates
        {
            private set { leaveDates = value as List<LeaveDate>;    }
            get         { return leaveDates;                        }
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.MEDICAL_STAFFS );
            query.Comparator    = Identifier;
            DoDelete( query );
            base.Delete();
        }

        public override void Load()
        {
            base.Load();
            DatabaseQuery query = new DatabaseQuery( Database.Tables.MEDICAL_STAFFS );
            query.Comparator    = Identifier;
            query.Add( Database.Tables.MedicalStaffs.LicenseNo );
            
            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                // GetWorkingDaysByStaff should return only one result, because each staff has maximum of one WorkingDays entry.
                LicenseNo   = reader.GetString( 0 );
                WorkingDays = StaffsManager.GetWorkingDaysByStaff( this )[0];
                LeaveDates  = StaffsManager.GetLeaveDatesByStaff( this );
            }
            
            reader.Close();
        }

        public override void Save()
        {
            base.Save();
            DatabaseQuery query = new DatabaseQuery( Database.Tables.MEDICAL_STAFFS );
            query.Add( Database.Tables.MedicalStaffs.StaffId    , Id        );
            query.Add( Database.Tables.MedicalStaffs.LicenseNo  , LicenseNo );
            DoSave( query );
        }
    }
}
