using OverSurgerySystem.Manager;
using MySql.Data.MySqlClient;

namespace OverSurgerySystem.Core.Patients
{
    public class PrescriptionMedication : Medication
    {
        public Prescription Prescription    { private set; get; }
        public Medication Base              { private set; get; }
        
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

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Prescription    = PatientsManager.GetPrescription( reader.GetInt32( 0 ) );
                Base            = PatientsManager.GetMedication( reader.GetInt32( 1 ) );
                Code            = Base.Code;
                Name            = Base.Name;
                PatientsManager.Add( this );
            }

            reader.Close();
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
