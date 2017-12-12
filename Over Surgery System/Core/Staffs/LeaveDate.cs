using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Staffs
{
    public class LeaveDate : DatabaseObject
    {
        public DateTime Date        { private set; get; }
        public MedicalStaff Owner   { private set; get; }

        // Constructor & Factory Function
        public LeaveDate()
        {
            Date = INVALID_DATETIME;
        }

        public static LeaveDate Make( MedicalStaff staff , DateTime date )
        {
            LeaveDate leaveDate = new LeaveDate();
            leaveDate.Owner     = staff;
            leaveDate.Date      = date.Date;
            return leaveDate;
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
            query.Add( Database.Tables.LeaveDates.Date      );
            query.Add( Database.Tables.LeaveDates.StaffId   );
            
            MySqlDataReader reader  = DoLoad( query );
            int ownerId             = INVALID_ID;
            
            if( Loaded )
            {
                Date    = reader.GetDateTime( 0 );
                ownerId = reader.GetInt32( 1 );
                StaffsManager.Add( this );
            }

            reader.Close();
            Owner = StaffsManager.GetMedicalStaff( ownerId );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.LEAVE_DATES );
            query.Add( Database.Tables.LeaveDates.Date      , QueryElement.DateOf( Date )   );
            query.Add( Database.Tables.LeaveDates.StaffId   , Owner                         );
            DoSave( query );
            StaffsManager.Add( this );
        }
    }
}
