using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class TestResult : DatabaseObject
    {
        public Patient Patient          { private set; get; }
        public string MedicalLicenseNo  { private set; get; }
        public string Name              { private set; get; }
        public string Description       { private set; get; }
        public string Result            { private set; get; }
        public string Remark            { private set; get; }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.TEST_RESULTS );
            DoDelete( query );
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.TEST_RESULTS );
            query.Add( Database.Tables.TestResults.PatientId        );
            query.Add( Database.Tables.TestResults.MedicalLicenseNo );
            query.Add( Database.Tables.TestResults.Name             );
            query.Add( Database.Tables.TestResults.Description      );
            query.Add( Database.Tables.TestResults.Result           );
            query.Add( Database.Tables.TestResults.Remark           );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Patient             = PatientsManager.GetPatient( reader.GetInt32( 0 ) );
                MedicalLicenseNo    = reader.GetString( 1 );
                Name                = reader.GetString( 2 );
                Description         = reader.GetString( 3 );
                Result              = reader.GetString( 4 );
                Remark              = reader.GetString( 5 );
                PatientsManager.Add( this );
            }

            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.TEST_RESULTS );
            query.Add( Database.Tables.TestResults.PatientId        , Patient           );
            query.Add( Database.Tables.TestResults.MedicalLicenseNo , MedicalLicenseNo  );
            query.Add( Database.Tables.TestResults.Name             , Name              );
            query.Add( Database.Tables.TestResults.Description      , Description       );
            query.Add( Database.Tables.TestResults.Result           , Result            );
            query.Add( Database.Tables.TestResults.Remark           , Remark            );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}
