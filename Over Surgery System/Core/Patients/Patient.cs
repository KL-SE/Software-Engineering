using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;
using System;

namespace OverSurgerySystem.Core.Patients
{
    public class Patient : DatabaseObject
    {
        public PersonalDetails Details { set; get; }

        public string StringId
        {
            get
            {
                return "P" + Id.ToString( "D8" );
            }
        }

        public static int GetIdFromString( string StrId )
        {
            try
            {
                if( StrId.ToUpper().StartsWith( "P" ) )
                    return Int32.Parse( StrId.Substring( 2 ) );

                return INVALID_ID;
            }
            catch
            {
                return INVALID_ID;
            }
        }

        public List<Prescription> GetPrescriptions()    { return PatientsManager.GetPrescriptionsByPatient( this ); }
        public List<Appointment> GetAppointments()      { return PatientsManager.GetAppointmentsByPatient( this );  }
        public List<TestResult> GetTests()              { return PatientsManager.GetTestResultsByPatient( this );   }

        public Patient() : base()
        {
            Details = new PersonalDetails();
        }

        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PATIENTS );
            DoDelete( query );
            Details.Delete();
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PATIENTS );
            query.Add( Database.Tables.Patients.DetailsId );
            
            MySqlDataReader reader  = DoLoad( query );
            int detailsId           = INVALID_ID;
            
            if( Loaded )
            {
                detailsId   = reader.GetInt32( 0 );
                PatientsManager.Add( this );
            }
            
            reader.Close();
            Details = DetailsManager.GetPersonalDetail( detailsId );
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
