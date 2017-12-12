using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Base
{
    public class Identification : DatabaseObject
    {
        public string Value          { set; get; }
        public PersonalDetails Owner { set; get; }

        // Basic ToString
        public override String ToString()
        {
            return Value;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.IDENTIFICATIONS );
            DoDelete( query );
            DetailsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.IDENTIFICATIONS );
            query.Add( Database.Tables.Identifications.Value        );
            query.Add( Database.Tables.Identifications.DetailsId    );

            MySqlDataReader reader  = DoLoad( query );
            int ownerId             = INVALID_ID;
            
            if( Loaded )
            {
                Value   = reader.GetString( 0 );
                ownerId = reader.GetInt32( 1 );
                DetailsManager.Add( this );
            }

            reader.Close();
            Owner = DetailsManager.GetPersonalDetail( ownerId );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.IDENTIFICATIONS );
            query.Add( Database.Tables.Identifications.Value        , Value );
            query.Add( Database.Tables.Identifications.DetailsId    , Owner );
            DoSave( query );
            DetailsManager.Add( this );
        }
    }
}
