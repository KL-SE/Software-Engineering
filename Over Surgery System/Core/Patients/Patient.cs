using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class Patient : DatabaseObject
    {
        public PersonalDetails Details { private set; get; }

        public List<Prescription> GetPrescriptions()    { return PatientsManager.GetPrescriptionsByPatient( this ); }
        public List<Appointment> GetAppointments()      { return PatientsManager.GetAppointmentsByPatient( this );  }
        public List<TestResult> GetTests()              { return PatientsManager.GetTestResultsByPatient( this );   }

        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PATIENTS );
            DoDelete( query );
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PATIENTS );
            query.Add( Database.Tables.Patients.DetailsId );
            
            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                int detailsId   = reader.GetInt32( 0 );
                Details         = DetailsManager.GetPersonalDetail( detailsId );
                PatientsManager.Add( this );
            }
            
            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PATIENTS );
            query.Add( Database.Tables.Patients.DetailsId , Details );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}
