using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Base
{
    public class ContactNumber : DatabaseObject
    {
        public string Number            { set; get; }
        public PersonalDetails Owner    { set; get; }
        
        // Basic ToString
        public override String ToString()
        {
            return Number;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.CONTACT_NUMBERS );
            DoDelete( query );
            DetailsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.CONTACT_NUMBERS );
            query.Add( Database.Tables.ContactNumbers.Number    );
            query.Add( Database.Tables.ContactNumbers.DetailsId );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Number  = reader.GetString( 0 );
                Owner   = DetailsManager.GetPersonalDetail( reader.GetInt32( 1 ) );
                DetailsManager.Add( this );
            }

            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.CONTACT_NUMBERS );
            query.Add( Database.Tables.ContactNumbers.Number    , Number    );
            query.Add( Database.Tables.ContactNumbers.DetailsId , Owner     );
            DoSave( query );
            DetailsManager.Add( this );
        }
    }
}
