using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Patients
{
    public class Medication : DatabaseObject
    {
        public string Code { set; get; }
        public string Name { set; get; }
                
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.MEDICATIONS );
            DoDelete( query );
            PatientsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.MEDICATIONS );
            query.Add( Database.Tables.Medications.Code );
            query.Add( Database.Tables.Medications.Name );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Code = reader.GetString( 0 );
                Name = reader.GetString( 1 );
                PatientsManager.Add( this );
            }

            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.MEDICATIONS );
            query.Add( Database.Tables.Medications.Code , Code );
            query.Add( Database.Tables.Medications.Name , Name );
            DoSave( query );
            PatientsManager.Add( this );
        }
    }
}