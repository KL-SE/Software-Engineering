using MySql.Data.MySqlClient;

namespace OverSurgerySystem.Core.Staffs
{

    public class Receptionist : Staff
    {
        // Administrative Permission
        public bool Admin { set; get; }

        // Custom comparator since Receptionist's identifying column is their staff_id, not the regular id.
        QueryComparator Identifier
        {
            get
            {
                QueryComparator idComparator    = new QueryComparator();
                idComparator.Source             = new QueryElement( Database.Tables.Receptionists.StaffId );
                idComparator.Operand            = this;
                idComparator.Equal              = true;
                return idComparator;
            }
        }

        // A helper function to get whether a staff is an admin.
        public static bool IsAdmin( Staff staff )
        {
            return staff is Receptionist && ( ( Receptionist ) staff ).Admin;
        }

        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.RECEPTIONISTS );
            query.Comparator    = Identifier;
            DoDelete( query );
            base.Delete();
        }

        public override void Load()
        {
            base.Load();
            DatabaseQuery query = new DatabaseQuery( Database.Tables.RECEPTIONISTS );
            query.Comparator    = Identifier;
            query.Add( Database.Tables.Receptionists.Admin );
            
            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
                Admin = reader.GetByte( 0 ) > 0 ? true : false;
            
            reader.Close();
        }

        public override void Save()
        {
            base.Save();
            DatabaseQuery query = new DatabaseQuery( Database.Tables.RECEPTIONISTS );
            query.Comparator    = Identifier;
            query.Add( Database.Tables.Receptionists.StaffId    , Id            );
            query.Add( Database.Tables.Receptionists.Admin      , Admin ? 1 : 0 );
            DoSave( query );
        }
    }
}
