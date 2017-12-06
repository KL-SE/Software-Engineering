using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class Prescription : DatabaseObject
    {
        public MedicalStaff Prescriber  { private set; get; }
        public Patient Patient          { private set; get; }
        public string Name              {         set; get; }
        public string Remark            {         set; get; }
        DateTime startDate;
        DateTime endDate;
        
        List<Medication> medications;

        public Prescription() : base()
        {
            StartDate   = DateTime.MaxValue;
            EndDate     = DateTime.MaxValue;
            Medications = new List<Medication>();
        }

        // Prescription's start date & time
        public DateTime StartDate
        {
            set
            {
                startDate = value.Date >= DateTime.Now.Date ? value.Date : DateTime.Now.Date;
            }

            get
            {
                return startDate;
            }
        }
        
        // Prescription's end date & time
        public DateTime EndDate
        {
            set
            {
                endDate = value.Date >= StartDate.Date ? value.Date : StartDate.Date;
            }

            get
            {
                return endDate;
            }
        }
        
        // Prescription's started status
        public bool Started
        {
            set
            {
                if( value && StartDate > DateTime.Now.Date )
                {
                    StartDate = DateTime.Now.Date;
                    if( endDate.Date < DateTime.Now.Date )
                        endDate = DateTime.Now.Date;
                }
            }

            get
            {
                return DateTime.Now.Date >= StartDate;
            }
        }
        
        // Prescription's ended status.
        public bool Ended
        {
            set
            {
                if( value )
                {
                    if( StartDate > DateTime.Now.Date )
                        StartDate = DateTime.Now.Date;
                
                    endDate = DateTime.Now.Date;
                }
            }

            get
            {
                return DateTime.Now.Date >= EndDate;
            }
        }
        
        // Prescription's medications
        public IList<Medication> Medications
        {
            private set
            {
                medications = Medications as List<Medication>;
            }

            get
            {
                return medications.AsReadOnly();
            }
        }
        
        // Factory Function
        public static Prescription Make( MedicalStaff staff , Patient patient )
        {
            Prescription prescription   = new Prescription();
            prescription.Prescriber     = staff;
            prescription.Patient        = patient;
            return prescription;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PRESCRIPTIONS );
            DoDelete( query );
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PRESCRIPTIONS );
            query.Add( Database.Tables.Prescriptions.PrescriberId   );
            query.Add( Database.Tables.Prescriptions.PatientId      );
            query.Add( Database.Tables.Prescriptions.Name           );
            query.Add( Database.Tables.Prescriptions.Remark         );
            query.Add( Database.Tables.Prescriptions.StartDate      );
            query.Add( Database.Tables.Prescriptions.EndDate        );
            
            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Prescriber      = StaffsManager.GetMedicalStaff( reader.GetInt32( 0 ) );
                Patient         = PatientsManager.GetPatient( reader.GetInt32( 1 ) );
                Name            = reader.GetString( 2 );
                Remark          = reader.GetString( 3 );
                StartDate       = reader.GetDateTime( 4 );
                EndDate         = reader.GetDateTime( 5 );
                PatientsManager.Add( this );
            }

            reader.Close();
            // TODO: Implement fetching prescription's medications.
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PRESCRIPTIONS );
            query.Add( Database.Tables.Prescriptions.PrescriberId   , Prescriber                        );
            query.Add( Database.Tables.Prescriptions.PatientId      , Patient                           );
            query.Add( Database.Tables.Prescriptions.Name           , Name                              );
            query.Add( Database.Tables.Prescriptions.Remark         , Remark                            );
            query.Add( Database.Tables.Prescriptions.StartDate      , QueryElement.DateOf( StartDate )  );
            query.Add( Database.Tables.Prescriptions.EndDate        , QueryElement.DateOf( EndDate   )  );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}
