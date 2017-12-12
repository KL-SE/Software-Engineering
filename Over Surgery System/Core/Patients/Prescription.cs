using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class Prescription : DatabaseObject
    {
        public MedicalStaff Prescriber  { set; get; }
        public Patient Patient          { set; get; }
        public string Name              { set; get; }
        public string Remark            { set; get; }
        DateTime startDate;
        DateTime endDate;
        
        List<PrescriptionMedication> medications;

        public string StringId
        {
            get
            {
                return "PRE" + Id.ToString( "D6" );
            }
        }

        public static int GetIdFromString( string StrId )
        {
            try
            {
                return Int32.Parse( StrId.Substring( 3 ) );
            }
            catch
            {
                return INVALID_ID;
            }
        }

        public Prescription() : base()
        {
            StartDate   = DateTime.MaxValue;
            EndDate     = DateTime.MaxValue;
            medications = new List<PrescriptionMedication>();
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
            get
            {
                List<Medication> results = new List<Medication>();
                foreach( PrescriptionMedication med in medications )
                    results.Add( med.Base );

                return results.AsReadOnly();
            }
        }

        public bool HaveMedication( Medication medication )
        {
            foreach( PrescriptionMedication pMed in medications )
            {
                if( medication.Id == pMed.Base.Id )
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddMedication( Medication medication )
        {
            if( medication.Valid )
            {
                if( !HaveMedication( medication ) )
                {
                    medications.Add( new PrescriptionMedication( this , medication ) );
                    return true;
                }
            }
            return false;
        }

        public void RemoveMedication( Medication medication )
        {
            for( int index = 0 ; index < medications.Count ; index++ )
            {
                if( medication.Id == medications[index].Base.Id )
                {
                    medications.RemoveAt( index );
                    return;
                }
            }
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
            
            MySqlDataReader reader  = DoLoad( query );
            int prescriberId        = INVALID_ID;
            int patientId           = INVALID_ID;
            
            if( Loaded )
            {
                prescriberId    = reader.GetInt32( 0 );
                patientId       = reader.GetInt32( 1 );
                Name            = reader.GetString( 2 );
                Remark          = reader.GetString( 3 );
                StartDate       = reader.GetDateTime( 4 );
                EndDate         = reader.GetDateTime( 5 );
                PatientsManager.Add( this );
            }

            reader.Close();
            Prescriber  = StaffsManager.GetMedicalStaff( prescriberId );
            Patient     = PatientsManager.GetPatient( patientId );
            medications = PatientsManager.GetMedicationsForPrescription( this );
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
