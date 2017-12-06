using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class Appointment : DatabaseObject
    {
        public MedicalStaff MedicalStaff    { private set; get; }
        public Patient Patient              { private set; get; }
        public bool Cancelled               {         set; get; }
        DateTime dateAppointed;

        public Appointment() : base()
        {
            DateAppointed = DateTime.MaxValue;
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
        
        // Factory Function
        public static Appointment Make( MedicalStaff staff , Patient patient )
        {
            Appointment appointment     = new Appointment();
            appointment.MedicalStaff    = staff;
            appointment.Patient         = patient;
            return appointment;
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
            query.Add( Database.Tables.Appointments.Cancelled       );
            query.Add( Database.Tables.Appointments.DateAppointed   );
            
            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                MedicalStaff    = StaffsManager.GetMedicalStaff( reader.GetInt32( 0 ) );
                Patient         = PatientsManager.GetPatient( reader.GetInt32( 1 ) );
                Cancelled       = reader.GetByte( 2 ) > 0 ? true : false;
                DateAppointed   = reader.GetDateTime( 3 );
                PatientsManager.Add( this );
            }

            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.APPOINTMENTS );
            query.Add( Database.Tables.Appointments.MedicalStaffId  , MedicalStaff      );
            query.Add( Database.Tables.Appointments.PatientId       , Patient           );
            query.Add( Database.Tables.Appointments.Cancelled       , Cancelled ? 1 : 0 );
            query.Add( Database.Tables.Appointments.DateAppointed   , DateAppointed     );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}
