using System;

namespace OverSurgerySystem
{
    // Element of a databse query.
    public class QueryElement : IDatabaseQuery
    {
        public string Column    { set; get; }
        public object Value     { set; get; }

        // Default constructor.
        public QueryElement() { }

        // Constructor for a fully-valued query element.
        public QueryElement( string columnName , object value )
        {
            Column  = columnName;
            Value   = value;
        }

        // Constructor for a table column element.
        public QueryElement( string columnName )
        {
            Column  = columnName;
            Value   = new QueryElement( columnName , null );
        }

        // Convert an object into an SQL query value:
        public string Stringify()
        {
            // Format the value appropraitely.
            if( Value is DateTime )
            {
                DateTime actualValue = ( DateTime )( Value );
                return String.Format( "'{0}'" , actualValue.ToString( "yyyy-MM-dd HH:mm:ss" ) );
            }
            else if( Value is QueryElement )
            {
                QueryElement actualValue = ( QueryElement )( Value );
                return actualValue.Column;
            }
            else if( Value is IDatabaseQuery )
            {
                IDatabaseQuery actualValue = ( IDatabaseQuery )( Value );
                return actualValue.Stringify();
            }
            else
            {
                string strValue = Value.ToString();
                return String.Format( "'{0}'" , strValue.Replace( "\'" , "\\'" ) );
            }
        }

        // Formats date.
        public static string DateOf( DateTime date )
        {
            return date.ToString( "yyyy-MM-dd" );
        }

        // Formats time.
        public static string TimeOf( DateTime time )
        {
            return time.ToString( "HH:mm:ss" );
        }

    }
}
