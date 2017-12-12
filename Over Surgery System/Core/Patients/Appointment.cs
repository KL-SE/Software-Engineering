using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class Appointment : DatabaseObject
    {
        public MedicalStaff MedicalStaff    { set; get; }
        public Patient Patient              { set; get; }
        public string Remark                { set; get; }
        public bool Cancelled               { set; get; }
        DateTime dateAppointed;

        public Appointment() : base()
        {
            DateAppointed = DateTime.Now;
        }

        public string StringId
        {
            get
            {
                return "AP" + Id.ToString( "D6" );
            }
        }

        public static int GetIdFromString( string StrId )
        {
            try
            {
                return Int32.Parse( StrId.Substring( 2 ) );
            }
            catch
            {
                return INVALID_ID;
            }
        }
        
        // Appointment's date & time.
        public DateTime DateAppointed
        {
            set
            {
                dateAppointed = value >= DateTime.Now ? value : DateTime.Now;
            }

            get
            {
                return dateAppointed;
            }
        }
        
        // Returns whether the appointment is active, i.e: it is not cancelled or passed it's date & time.
        public bool Active
        {
            get
            {
                return !Cancelled && DateTime.Now < DateAppointed;
            }
        }

        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.APPOINTMENTS );
            DoDelete( query );
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.APPOINTMENTS );
            query.Add( Database.Tables.Appointments.MedicalStaffId  );
            query.Add( Database.Tables.Appointments.PatientId       );
            query.Add( Database.Tables.Appointments.Remark          );
            query.Add( Database.Tables.Appointments.DateAppointed   );
            query.Add( Database.Tables.Appointments.Cancelled       );
            
            MySqlDataReader reader  = DoLoad( query );
            int staffId             = INVALID_ID;
            int patientId           = INVALID_ID;
            
            if( Loaded )
            {
                staffId         = reader.GetInt32( 0 );
                patientId       = reader.GetInt32( 1 );
                Remark          = reader.GetString( 2 );
                DateAppointed   = reader.GetDateTime( 3 );
                Cancelled       = reader.GetByte( 4 ) > 0 ? true : false;
                PatientsManager.Add( this );
            }

            reader.Close();
            MedicalStaff    = StaffsManager.GetMedicalStaff( staffId );
            Patient         = PatientsManager.GetPatient( patientId );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.APPOINTMENTS );
            query.Add( Database.Tables.Appointments.MedicalStaffId  , MedicalStaff      );
            query.Add( Database.Tables.Appointments.PatientId       , Patient           );
            query.Add( Database.Tables.Appointments.Remark          , Remark            );
            query.Add( Database.Tables.Appointments.DateAppointed   , DateAppointed     );
            query.Add( Database.Tables.Appointments.Cancelled       , Cancelled ? 1 : 0 );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}
