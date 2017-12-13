using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Staffs
{
    public class LeaveDate : DatabaseObject
    {
        public DateTime date;
        public MedicalStaff Owner   { set; get; }
        public string Remark        { set; get; }

        // Constructor & Factory Function
        public LeaveDate()
        {
            Date = INVALID_DATETIME;
        }

        // Date property
        public DateTime Date
        {
            set
            {
                date = value.Date;
            }
            get
            {
                return date;
            }
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.LEAVE_DATES );
            DoDelete( query );
            StaffsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.LEAVE_DATES );
            query.Add( Database.Tables.LeaveDates.StaffId   );
            query.Add( Database.Tables.LeaveDates.Date      );
            query.Add( Database.Tables.LeaveDates.Remark    );
            
            MySqlDataReader reader  = DoLoad( query );
            int ownerId             = INVALID_ID;
            
            if( Loaded )
            {
                ownerId = reader.GetInt32( 0 );
                Date    = reader.GetDateTime( 1 );
                Remark  = reader.GetString( 2 );
                StaffsManager.Add( this );
            }

            reader.Close();
            Owner = StaffsManager.GetMedicalStaff( ownerId );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.LEAVE_DATES );
            query.Add( Database.Tables.LeaveDates.StaffId   , Owner                         );
            query.Add( Database.Tables.LeaveDates.Remark    , Remark                        );
            query.Add( Database.Tables.LeaveDates.Date      , QueryElement.DateOf( Date )   );
            DoSave( query );
            StaffsManager.Add( this );
        }
    }
}
