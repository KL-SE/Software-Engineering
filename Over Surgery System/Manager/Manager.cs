using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OverSurgerySystem.Manager
{
    [Serializable]
    public class DuplicatedObjectError: Exception
    {
        public DuplicatedObjectError() {}
        public DuplicatedObjectError( string message ) : base( message ) { }
        public DuplicatedObjectError( string message , Exception inner ) : base( message , inner ) { }
    }

    public class Manager<T> where T: DatabaseObject, new()
    {
        Dictionary<int,T> all;
        public Manager()
        {
            all = new Dictionary<int,T>();
        }

        // Add the object
        public void Add( T obj )
        {
            // Verify that the object is valid before adding it to the dictionary.
            if( !obj.Valid )
                return;

            if( !all.ContainsKey( obj.Id ) )
            {
                all.Add( obj.Id , obj );
            }
            else if( all[obj.Id] != obj )
            {
                throw new DuplicatedObjectError();
            }
        }

        // Remove the object
        public void Remove( T obj )
        {
            all.Remove( obj.Id );
        }

        public virtual T Load( int id )
        {
            T obj = new T();
            obj.Load( id );
            if( obj.Loaded )
                all.Add( obj.Id , obj );

            return obj;
        }

        // Try to retrieve the object of the specified ID.
        // If such object does not exist in the dictionary, we'll create it and load it from the database.
        public T Get( int id )
        {
            T obj;
            if( all.TryGetValue( id , out obj ) )
                return obj;

            return Load( id );
        }

        // Merge the list of objects from the database.
        public List<T> Merge( string tableName , QueryComparator comparator )
        {
            DatabaseQuery query = new DatabaseQuery( tableName );
            query.Comparator    = comparator;
            query.Add( Database.Tables.Generic.ID );

            List<T> results         = new List<T>();
            Console.WriteLine( query.Select );
            MySqlCommand command    = new MySqlCommand( query.Select , Database.Connection );
            MySqlDataReader reader  = command.ExecuteReader();

            while( reader.Read() )
            {
                int id  = reader.GetInt32( 0 );
                T obj   = Get( id );
                results.Add( obj );
            }

            reader.Close();
            return results;
        }
    }

    public class ManagerHelper
    {
        // Filter a list based on a set condition.
        public static List<T> Filter<T>( IEnumerable<T> list , Func<T,bool> include ) where T: DatabaseObject
        {
            List<T> results = new List<T>();
            foreach( T obj in list )
                if( include( obj ) )
                    results.Add( obj );

            return results;
        }

        // Filter a list based on types.
        public static List<T> FilterType<T>( IEnumerable<DatabaseObject> list ) where T: DatabaseObject
        {
            List<T> results = new List<T>();
            foreach( DatabaseObject obj in list )
                if( obj is T )
                    results.Add( ( T )( obj ) );

            return results;
        }

        // Convenience comparators
        public static QueryComparator GetEqualComparator( string columnName , object value )
        {
            QueryComparator comparator  = new QueryComparator();
            comparator.Source           = new QueryElement( columnName );
            comparator.Operand          = new QueryElement( null , value );
            comparator.Equal            = true;
            return comparator;
        }

        public static QueryComparator GetAndComparator( QueryComparator source , QueryComparator operand )
        {
            QueryComparator comparator  = new QueryComparator();
            comparator.Source           = source;
            comparator.Operand          = operand;
            comparator.And              = true;
            return comparator;
        }

        public static QueryComparator GetLikeComparator( string columnName , object value )
        {
            QueryComparator comparator  = new QueryComparator();
            comparator.Source           = new QueryElement( columnName                                  );
            comparator.Operand          = new QueryElement( null , String.Format( "%{0}%" , value  )    );
            comparator.Like             = true;
            return comparator;
        }

        public static QueryComparator GetInComparator( string columnName , DatabaseQuery subquery )
        {
            QueryComparator comparator  = new QueryComparator();
            comparator.Source           = new QueryElement( columnName  );
            comparator.Operand          = new QueryElement( subquery    );
            return comparator;
        }

        public static DatabaseQuery GetInLikeQuery( string tableName , string columnName , object value )
        {
            DatabaseQuery subquery  = new DatabaseQuery( tableName );
            subquery.Comparator     = ManagerHelper.GetLikeComparator( columnName , value );
            subquery.Add( Database.Tables.Generic.ID );
            return subquery;
        }

        public static DatabaseQuery GetInEqualQuery( string tableName , string columnName , object value )
        {
            DatabaseQuery subquery  = new DatabaseQuery( tableName );
            subquery.Comparator     = ManagerHelper.GetEqualComparator( columnName , value );
            subquery.Add( Database.Tables.Generic.ID );
            return subquery;
        }

        public static QueryComparator GetInLikeComparator( string columnName , string secondaryTable , string secondaryColumn , object value )
        {
            return GetInComparator( columnName , GetInLikeQuery( secondaryTable , secondaryColumn , value ) );
        }

        public static QueryComparator GetInEqualComparator( string columnName , string secondaryTable , string secondaryColumn , object value )
        {
            return GetInComparator( columnName , GetInEqualQuery( secondaryTable , secondaryColumn , value ) );
        }
    }
}
