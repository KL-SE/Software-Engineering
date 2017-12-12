using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OverSurgerySystem
{
    // Database query builder thingy
    public class DatabaseQuery : IDatabaseQuery
    {  
        private string columns;
        private string values;
        private string columnValues;
        private List<QueryElement> elements;

        // Table name & comparator for a query...
        public string Table                 { set; get; }
        public QueryComparator Comparator   { set; get; }

        // Constructor
        public DatabaseQuery( string tableName )
        {
            Table       = tableName;
            elements    = new List<QueryElement>();
        }

        // Returns the query's columns string.
        string Columns
        {
            get
            {
                if( columns == null )
                    columns = String.Join( "," , elements.Select( e => e.Column ) );
                return columns;
            }
        }

        // Returns the query's values string.
        string Values
        {
            get
            {
                if( values == null )
                    values = String.Join( "," , elements.Select( e => e.Stringify() ) );
                return values;
            }
        }

        // Returns the query's columns-values pair string.
        string ColumnValues
        {
            get
            {
                if( columnValues == null )
                    columnValues = String.Join( "," , elements.Select( e => String.Format( "{0}={1}" , e.Column , e.Stringify() ) ) );
                return columnValues;
            }
        }

        // Returns the number of query elements
        public int Count
        {
            get
            {
                return elements.Count;
            }
        }
        
        // Add a fully valued database column as a query element.
        public void Add( string columnName , object value )
        {
            QueryElement element    = new QueryElement( columnName , value );
            columns                 = null;
            values                  = null;
            columnValues            = null;
            elements.Add( element );
        }
        
        // Add a database column as a query element.
        public void Add( string columnName )
        {
            QueryElement element    = new QueryElement( columnName );
            columns                 = null;
            values                  = null;
            columnValues            = null;
            elements.Add( element );
        }

        // Make a query to see if the row exists...
        public bool Exists
        {
            get
            {
                string comparatorStatement  = Comparator != null ? " WHERE " + Comparator : "";
                string sqlStatement         = String.Format( "SELECT 1 FROM {0}{1}" , Table , comparatorStatement );

                MySqlCommand command        = new MySqlCommand( sqlStatement , Database.Connection );
                MySqlDataReader reader      = command.ExecuteReader();
                bool exists                 = reader.HasRows;

                reader.Close();
                return exists;
            }
        }

        // Make a SELECT query...
        public string Select
        {
            get
            {
                string comparatorStatement  = Comparator != null ? " WHERE " + Comparator : "";
                string sqlStatement         = String.Format( "SELECT {0} FROM {1}{2}" , Columns , Table , comparatorStatement );
                return sqlStatement;
            }
        }

        // Make a INSERT query...
        public string Insert
        {
            get
            {
                return String.Format( "INSERT INTO {0}({1}) VALUES({2})" , Table , Columns , Values );
            }
        }

        // Make an UPDATE query...
        public string Update
        {
            get
            {
                string comparatorStatement  = Comparator != null ? " WHERE " + Comparator : "";
                string sqlStatement         = String.Format( "UPDATE {0} SET {1}{2}" , Table , ColumnValues , comparatorStatement );
                return sqlStatement;
            }
        }

        // Make a DELETE query...
        public string Delete
        {
            get
            {
                string comparatorStatement  = Comparator != null ? " WHERE " + Comparator : "";
                string sqlStatement         = String.Format( "DELETE FROM {0}{1}" , Table , comparatorStatement );
                return sqlStatement;
            }
        }

        // Convert to string
        public string Stringify()
        {
            return String.Format( "({0})" , Select );
        }
    }
}
