using System;
using MySql.Data.MySqlClient;

namespace OverSurgerySystem
{
    public abstract class DatabaseObject
    {
        // A placeholder integer that represents an invalid id.
        public const int INVALID_ID             = -1;
        public static DateTime INVALID_DATETIME = DateTime.MinValue;
        
        // Members for DatabaseObject
        int id;
        public bool Loaded          { private set; get; }
        public DateTime CreatedOn   { private set; get; }
        public DateTime LastSaved   { private set; get; }
        
        // A database object can only be used in conjunction with a subclass.
        protected DatabaseObject()
        {
            Id          = INVALID_ID;
            LastSaved   = INVALID_DATETIME;
            CreatedOn   = INVALID_DATETIME;
            Loaded      = false;
        }
        
        protected DatabaseObject( int id ) : this()
        {
            Id = id;
        }
        
        public override bool Equals( object obj )
        {
            if( obj is DatabaseObject )
            {
                DatabaseObject actualObj = ( DatabaseObject )( obj );
                return actualObj.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // ID as assigned by the database.
        public int Id
        {
            set
            {
                if( !Loaded )
                {
                    id = value;
                }
            }

            get
            {
                return id;
            }
        }
        
        // A get-only property returning whether the object  is valid (i.e: have a valid ID)
        public bool Valid
        {
            get
            {
                return Id != INVALID_ID;
            }
        }

        // A get-only property returning the default comparator for the object.
        QueryComparator Identifier
        {
            get
            {
                QueryComparator idComparator    = new QueryComparator();
                idComparator.Source             = new QueryElement( Database.Tables.Generic.ID );
                idComparator.Operand            = new QueryElement( null , Id );
                idComparator.Equal              = true;
                return idComparator;
            }
        }

        // Function that executes an insert/update statement.
        protected int DoSave( DatabaseQuery query )
        {
            bool exists = query.Exists;
            if( Valid && exists )
            {
                // Use the default comparator if there is none existing.
                if( query.Comparator == null )
                    query.Comparator = Identifier;  

                Loaded      = true;
                LastSaved   = DateTime.Now;
                query.Add( Database.Tables.Generic.LAST_UPDATED_COLUMN_NAME , LastSaved );

                MySqlCommand command = new MySqlCommand( query.Update , Database.Connection );
                return command.ExecuteNonQuery();
            }
            else
            {
                // Add a new row and get it's ID.
                CreatedOn = DateTime.Now;
                LastSaved = DateTime.Now;
                query.Add( Database.Tables.Generic.CREATED_ON_COLUMN_NAME   , CreatedOn );
                query.Add( Database.Tables.Generic.LAST_UPDATED_COLUMN_NAME , LastSaved );
                
                MySqlCommand command = new MySqlCommand( query.Insert , Database.Connection );
                command.ExecuteNonQuery();

                if( !Valid )
                {
                    id      = ( int )( command.LastInsertedId );
                    Loaded  = true;
                }

                return 1;
            }
        }

        // Function that executes a select statement.
        protected MySqlDataReader DoLoad( DatabaseQuery query )
        {
            if( !Valid )
                return null;
            
            if( query.Comparator == null )
                query.Comparator = Identifier;

            int columnCount = query.Count;
            query.Add( Database.Tables.Generic.CREATED_ON_COLUMN_NAME   );
            query.Add( Database.Tables.Generic.LAST_UPDATED_COLUMN_NAME );
            
            MySqlCommand command    = new MySqlCommand( query.Select , Database.Connection );
            MySqlDataReader reader  = command.ExecuteReader();

            if( reader.Read() )
            {
                Loaded                  = true;
                DateTime creationTime   = reader.GetDateTime( columnCount++ );
                DateTime lastUpdateTime = reader.GetDateTime( columnCount   );
                
                if( CreatedOn < creationTime )
                    CreatedOn = creationTime;

                if( LastSaved < lastUpdateTime )
                    LastSaved = lastUpdateTime;
            }
            else
            {
                Loaded = false;
            }

            return reader;
        }

        // Function that executes a select statement.
        protected int DoDelete( DatabaseQuery query )
        {
            if( !Valid )
                return 0;
            
            if( query.Comparator == null )
                query.Comparator = Identifier;
            
            Loaded = false;

            MySqlCommand command = new MySqlCommand( query.Delete , Database.Connection );
            return command.ExecuteNonQuery();
        }

        // Function that loads the database object from the database with the specified ID
        public void Load( int id )
        {
            Id = id;
            Load();
        }

        // Function that saves the object and generates an Id if it does not have one.
        public void Validate()
        {
            if( !Valid )
                Save();
        }
        
        // Functions that must be overriden by the base class.
        public abstract void Save();
        public abstract void Load();
        public abstract void Delete();
    }
}
