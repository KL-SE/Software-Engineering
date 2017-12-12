using OverSurgerySystem.Core.Base;
using System.Collections.Generic;

namespace OverSurgerySystem.Manager
{
    public class AddressManager
    {
        static Manager<PostalCode>  PostcodeManager;
        static Manager<City>        CityManager;
        static Manager<State>       StateManager;
        static Manager<Country>     CountryManager;
        
        private AddressManager() { }
        static AddressManager()
        {
            PostcodeManager = new Manager<PostalCode>();
            CityManager     = new Manager<City>();
            StateManager    = new Manager<State>();
            CountryManager  = new Manager<Country>();
        }
        
        public static void Add( PostalCode obj  )   { PostcodeManager.Add( obj );   }
        public static void Add( City obj        )   { CityManager.Add( obj );       }
        public static void Add( State obj       )   { StateManager.Add( obj );      }
        public static void Add( Country obj     )   { CountryManager.Add( obj );    }
        
        public static void Remove( PostalCode obj   )   { PostcodeManager.Remove( obj );    }
        public static void Remove( City obj         )   { CityManager.Remove( obj );        }
        public static void Remove( State obj        )   { StateManager.Remove( obj );       }
        public static void Remove( Country obj      )   { CountryManager.Remove( obj );     }
        
        public static PostalCode GetPostcode( int id    )   { return PostcodeManager.Get( id );     }
        public static City GetCity( int id              )   { return CityManager.Get( id );         }
        public static State GetState( int id            )   { return StateManager.Get( id );        }
        public static Country GetCountry( int id        )   { return CountryManager.Get( id );      }
        
        public static List<PostalCode>  GetPostcodesByCode( string code     )   { return PostcodeManager.Merge( Database.Tables.POSTAL_CODES    , ManagerHelper.GetLikeComparator( Database.Tables.PostalCodes.Code     , code      ) ); }
        public static List<PostalCode>  GetPostcodesByCity( int cityId      )   { return PostcodeManager.Merge( Database.Tables.POSTAL_CODES    , ManagerHelper.GetEqualComparator( Database.Tables.PostalCodes.CityId  , cityId    ) ); }
        public static List<City>        GetCitiesByName( string name        )   { return CityManager.Merge( Database.Tables.CITIES              , ManagerHelper.GetLikeComparator( Database.Tables.Cities.Name          , name      ) ); }
        public static List<City>        GetCitiesByState( int stateId       )   { return CityManager.Merge( Database.Tables.CITIES              , ManagerHelper.GetEqualComparator( Database.Tables.Cities.StateId      , stateId   ) ); }
        public static List<State>       GetStatesByName( string name        )   { return StateManager.Merge( Database.Tables.STATES             , ManagerHelper.GetLikeComparator( Database.Tables.States.Name          , name      ) ); }
        public static List<State>       GetStatesByCountry( int countryId   )   { return StateManager.Merge( Database.Tables.STATES             , ManagerHelper.GetLikeComparator( Database.Tables.States.CountryId     , countryId ) ); }
        public static List<Country>     GetCountriesByName( string name     )   { return CountryManager.Merge( Database.Tables.COUNTRIES        , ManagerHelper.GetLikeComparator( Database.Tables.Countries.Name       , name      ) ); }
        
        public static List<PostalCode>  GetPostcodesByExactCode( string code    )   { return PostcodeManager.Merge( Database.Tables.POSTAL_CODES    , ManagerHelper.GetEqualComparator( Database.Tables.PostalCodes.Code    , code      ) ); }
        public static List<City>        GetCitiesByExactName( string name       )   { return CityManager.Merge( Database.Tables.CITIES              , ManagerHelper.GetEqualComparator( Database.Tables.Cities.Name         , name      ) ); }
        public static List<State>       GetStatesByExactName( string name       )   { return StateManager.Merge( Database.Tables.STATES             , ManagerHelper.GetEqualComparator( Database.Tables.States.Name         , name      ) ); }
        public static List<Country>     GetCountriesByExactName( string name    )   { return CountryManager.Merge( Database.Tables.COUNTRIES        , ManagerHelper.GetEqualComparator( Database.Tables.Countries.Name      , name      ) ); }
        
        public static List<PostalCode>  GetPostcodesByCity( City city       )   { return GetPostcodesByCity( city.Id );     }
        public static List<City>        GetCitiesByState( State state       )   { return GetCitiesByState( state.Id );      }
        public static List<State>       GetStatesByCountry( Country country )   { return GetStatesByCountry( country.Id );  }
        
        public static List<PostalCode>  GetPostcodesByCity( string name )   { return PostcodeManager.Merge( Database.Tables.POSTAL_CODES    , ManagerHelper.GetInLikeComparator( Database.Tables.PostalCodes.CityId , Database.Tables.CITIES    , Database.Tables.Cities.Name       , name ) ); }
        public static List<City>        GetCitiesByState( string name   )   { return CityManager.Merge( Database.Tables.CITIES              , ManagerHelper.GetInLikeComparator( Database.Tables.Cities.StateId     , Database.Tables.STATES    , Database.Tables.States.Name       , name ) ); }
        public static List<State>       GetStatesByCountry( string name )   { return StateManager.Merge( Database.Tables.STATES             , ManagerHelper.GetInLikeComparator( Database.Tables.States.CountryId   , Database.Tables.COUNTRIES , Database.Tables.Countries.Name    , name ) ); }
    }
}
