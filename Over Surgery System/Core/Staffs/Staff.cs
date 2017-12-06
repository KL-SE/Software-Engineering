using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Base;

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
        public string Password          {         set; get; }
        public DateTime DateJoined      {         set; get; }
        public PersonalDetails Details  { private set; get; }
        public bool Active              {         set; get; }

        // Avoid other part of the code from instantiating the Staff class.
        public Staff()
        {
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
            
            MySqlDataReader reader = DoLoad( query );
            
            Details     = new PersonalDetails();
            Password    = reader.GetString( 0 );
            Details.Id  = reader.GetInt32( 1 );
            DateJoined  = reader.GetDateTime( 0 );
            Active      = reader.GetByte( 0 ) > 0 ? true : false;
            
            reader.Close();
            Details.Load();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            query.Add( Database.Tables.Staffs.Password      , Password          );
            query.Add( Database.Tables.Staffs.DateJoined    , DateJoined        );
            query.Add( Database.Tables.Staffs.DetailsId     , Details           );
            query.Add( Database.Tables.Staffs.Active        , Active ? 1 : 0    );
            DoSave( query );
        }
    }
}
