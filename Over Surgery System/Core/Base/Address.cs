using MySql.Data.MySqlClient;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Base
{
    public class PostalCode : DatabaseObject
    {
        public string Postcode  { set; get; }
        public City City        { set; get; }
        
        // Location's state
        public State State
        {
            set { City.State = value;   }
            get { return City.State;    }
        }
        
        // Location's country
        public Country Country
        {
            set { City.State.Country = value;   }
            get { return City.State.Country;    }
        }

        // Implicit string converter
        public override string ToString()
        {
            return Postcode;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.POSTAL_CODES );
            DoDelete( query );
            AddressManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.POSTAL_CODES );
            query.Add( Database.Tables.PostalCodes.Code     );
            query.Add( Database.Tables.PostalCodes.CityId   );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Postcode    = reader.GetString( 0 );
                City        = AddressManager.GetCity( reader.GetInt32( 1 ) );
                AddressManager.Add( this );
            }

            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.POSTAL_CODES );
            query.Add( Database.Tables.PostalCodes.Code   , Postcode    );
            query.Add( Database.Tables.PostalCodes.CityId , City        );
            DoSave( query );
            AddressManager.Add( this );
        }
    }
    
    public class City : DatabaseObject
    {
        public string Name  { set; get; }
        public State State  { set; get; }
        
        // Location's country
        public Country Country
        {
            set { State.Country = value;    }
            get { return State.Country;     }
        }

        // Implicit string converter
        public override string ToString()
        {
            return Name;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            foreach( PostalCode postcode in AddressManager.GetPostcodesByCity( this ) )
                postcode.Delete();

            DatabaseQuery query = new DatabaseQuery( Database.Tables.CITIES );
            DoDelete( query );
            AddressManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.CITIES );
            query.Add( Database.Tables.Cities.Name      );
            query.Add( Database.Tables.Cities.StateId   );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Name    = reader.GetString( 0 );
                State   = AddressManager.GetState( reader.GetInt32( 1 ) );
                AddressManager.Add( this );
            }

            reader.Close();
            AddressManager.Add( this );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.CITIES );
            query.Add( Database.Tables.Cities.Name      , Name  );
            query.Add( Database.Tables.Cities.StateId   , State );
            DoSave( query );
            AddressManager.Add( this );
        }
    }

    public class State : DatabaseObject
    {
        public string Name      { set; get; }
        public Country Country  { set; get; }

        // Implicit string converter
        public override string ToString()
        {
            return Name;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            foreach( City city in AddressManager.GetCitiesByState( this ) )
                city.Delete();

            DatabaseQuery query = new DatabaseQuery( Database.Tables.STATES );
            DoDelete( query );
            AddressManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STATES );
            query.Add( Database.Tables.States.Name      );
            query.Add( Database.Tables.States.CountryId );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Name    = reader.GetString( 0 );
                Country = AddressManager.GetCountry( reader.GetInt32( 1 ) );
                AddressManager.Add( this );
            }
            
            reader.Close();
            AddressManager.Add( this );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STATES );
            query.Add( Database.Tables.States.Name      , Name      );
            query.Add( Database.Tables.States.CountryId , Country   );
            DoSave( query );
            AddressManager.Add( this );
        }
    }
    
    public class Country : DatabaseObject
    {
        public string Name  { set; get; }

        // Implicit string converter
        public override string ToString()
        {
            return Name;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            foreach( State state in AddressManager.GetStatesByCountry( this ) )
                state.Delete();

            DatabaseQuery query = new DatabaseQuery( Database.Tables.COUNTRIES );
            DoDelete( query );
            AddressManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.COUNTRIES );
            query.Add( Database.Tables.Countries.Name );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                Name = reader.GetString( 0 );
                AddressManager.Add( this );
            }

            reader.Close();
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.COUNTRIES );
            query.Add( Database.Tables.Countries.Name , Name );
            DoSave( query );
            AddressManager.Add( this );
        }
    }
}
