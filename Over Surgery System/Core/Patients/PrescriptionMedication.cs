using OverSurgerySystem.Manager;
using MySql.Data.MySqlClient;

namespace OverSurgerySystem.Core.Patients
{
    public class PrescriptionMedication : Medication
    {
        public Prescription Prescription    { private set; get; }
        public Medication Base              { private set; get; }
        
        public PrescriptionMedication() : base() { }
        public PrescriptionMedication( Prescription prescription , Medication medication ) : base()
        {
            Prescription    = prescription;
            Base            = medication;
            Name            = medication.Name;
            Code            = medication.Code;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PRESCRIPTION_MEDICATIONS );
            DoDelete( query );
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PRESCRIPTION_MEDICATIONS );
            query.Add( Database.Tables.PrescriptionMedications.PrescriptionId   );
            query.Add( Database.Tables.PrescriptionMedications.MedicationId     );
            
            MySqlDataReader reader  = DoLoad( query );
            int prescriptionId      = INVALID_ID;
            int baseId              = INVALID_ID;
            
            if( Loaded )
            {
                prescriptionId  = reader.GetInt32( 0 );
                baseId          = reader.GetInt32( 1 );
                PatientsManager.Add( this );
            }

            reader.Close();
            Prescription    = PatientsManager.GetPrescription( prescriptionId );
            Base            = PatientsManager.GetMedication( baseId );
            Code            = Base.Code;
            Name            = Base.Name;
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PRESCRIPTION_MEDICATIONS );
            query.Add( Database.Tables.PrescriptionMedications.PrescriptionId   , Prescription  );
            query.Add( Database.Tables.PrescriptionMedications.MedicationId     , Base          );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}
