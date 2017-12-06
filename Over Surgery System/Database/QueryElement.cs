using System;

namespace OverSurgerySystem
{
    // Element of a databse query.
    public class QueryElement
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

        // Constructor for a sub-query
        public QueryElement( DatabaseQuery subquery )
        {
            Column  = null;
            Value   = subquery;
        }

        // Convert an object into an SQL query value:
        public virtual string Stringify()
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
            else if( Value is DatabaseObject )
            {
                DatabaseObject actualValue = ( DatabaseObject )( Value );
                actualValue.Validate();
                
                return String.Format( "'{0}'" , actualValue.Id );
            }
            else if( Value is DatabaseQuery )
            {
                DatabaseQuery actualValue = ( DatabaseQuery )( Value );
                return String.Format( "({0})" , actualValue.Select );
            }
            else
            {
                return String.Format( "'{0}'" , Value );
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
