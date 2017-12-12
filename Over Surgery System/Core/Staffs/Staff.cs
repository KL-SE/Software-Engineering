using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Staffs
{
    [Serializable]
    public class UnknownStaffTypeError: Exception
    {
        public UnknownStaffTypeError() {}
        public UnknownStaffTypeError( string message ) : base( message ) { }
        public UnknownStaffTypeError( string message , Exception inner ) : base( message , inner ) { }
    }

    public class Staff : DatabaseObject
    {
        public string Password          { set; get; }
        public DateTime DateJoined      { set; get; }
        public PersonalDetails Details  { set; get; }
        public bool Active              { set; get; }

        public string StringId
        {
            get
            {
                return "S" + Id.ToString( "D8" );
            }
        }

        public static int GetIdFromString( string StrId )
        {
            try
            {
                return Int32.Parse( StrId.Substring( 1 ) );
            }
            catch
            {
                return INVALID_ID;
            }
        }

        // Avoid other part of the code from instantiating the Staff class.
        public Staff()
        {
            Active  = true;
            Details = new PersonalDetails();
            if( !( this is MedicalStaff || this is Receptionist ) )
                throw new UnknownStaffTypeError();
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            DoDelete( query );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            query.Add( Database.Tables.Staffs.Password      );
            query.Add( Database.Tables.Staffs.DateJoined    );
            query.Add( Database.Tables.Staffs.DetailsId     );
            query.Add( Database.Tables.Staffs.Active        );
            
            MySqlDataReader reader  = DoLoad( query );
            int detailsId           = INVALID_ID;
            
            if( reader.HasRows )
            {
                Password    = reader.GetString( 0 );
                DateJoined  = reader.GetDateTime( 1 );
                detailsId   = reader.GetInt32( 2 );
                Active      = reader.GetByte( 3 ) > 0 ? true : false;
            }
            
            reader.Close();
            Details = DetailsManager.GetPersonalDetail( detailsId );
        }

        public override void Save()
        {
            char staffType  = '\0';
            if( this is MedicalStaff ) staffType = 'M'; else
            if( this is Receptionist ) staffType = 'R';

            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            query.Add( Database.Tables.Staffs.Type          , staffType         );
            query.Add( Database.Tables.Staffs.Password      , Password          );
            query.Add( Database.Tables.Staffs.DateJoined    , DateJoined        );
            query.Add( Database.Tables.Staffs.DetailsId     , Details           );
            query.Add( Database.Tables.Staffs.Active        , Active ? 1 : 0    );
            DoSave( query );
        }
    }
}
