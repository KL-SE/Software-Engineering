using OverSurgerySystem.Core.Base;
using System;
using System.Collections.Generic;

namespace OverSurgerySystem.Manager
{
    public class DetailsManager
    {
        static Manager<PersonalDetails> PersonalDetailsManager;
        static Manager<Identification>  IdentificationsManager;
        static Manager<ContactNumber>   ContactNumbersManager;
        
        private DetailsManager() { }
        static DetailsManager()
        {
            PersonalDetailsManager  = new Manager<PersonalDetails>();
            IdentificationsManager  = new Manager<Identification>();
            ContactNumbersManager   = new Manager<ContactNumber>();
        }
        
        public static void Add( PersonalDetails obj )   { PersonalDetailsManager.Add( obj );    }
        public static void Add( Identification obj  )   { IdentificationsManager.Add( obj );    }
        public static void Add( ContactNumber obj   )   { ContactNumbersManager.Add( obj );     }
        
        public static void Remove( PersonalDetails obj  )   { PersonalDetailsManager.Remove( obj );    }
        public static void Remove( Identification obj   )   { IdentificationsManager.Remove( obj );    }
        public static void Remove( ContactNumber obj    )   { ContactNumbersManager.Remove( obj );     }
        
        public static PersonalDetails GetPersonalDetail( int id )   { return PersonalDetailsManager.Get( id );  }
        public static Identification GetIdentification( int id  )   { return IdentificationsManager.Get( id );  }
        public static ContactNumber GetContactNumber( int id    )   { return ContactNumbersManager.Get( id );   }

        public static List<PersonalDetails> GetDetailsByFirstName( string name      )   { return PersonalDetailsManager.Merge( Database.Tables.PERSONAL_DETAILS , ManagerHelper.GetLikeComparator( Database.Tables.PersonalDetails.FirstName    , name      ) ); }
        public static List<PersonalDetails> GetDetailsByLastName( string name       )   { return PersonalDetailsManager.Merge( Database.Tables.PERSONAL_DETAILS , ManagerHelper.GetLikeComparator( Database.Tables.PersonalDetails.LastName     , name      ) ); }
        public static List<PersonalDetails> GetDetailsByAddress( string address     )   { return PersonalDetailsManager.Merge( Database.Tables.PERSONAL_DETAILS , ManagerHelper.GetLikeComparator( Database.Tables.PersonalDetails.Address      , address   ) ); }
        public static List<PersonalDetails> GetDetailsByDateOfBirth( DateTime dob   )   { return PersonalDetailsManager.Merge( Database.Tables.PERSONAL_DETAILS , ManagerHelper.GetEqualComparator( Database.Tables.PersonalDetails.DateOfBirth , dob       ) ); }
        public static List<PersonalDetails> GetDetailsByPostcode( int id            )   { return PersonalDetailsManager.Merge( Database.Tables.PERSONAL_DETAILS , ManagerHelper.GetEqualComparator( Database.Tables.PersonalDetails.PostcodeId  , id        ) ); }
        public static List<PersonalDetails> GetDetailsByPostcode( string code       )
        {
            return PersonalDetailsManager.Merge
            (
                Database.Tables.PERSONAL_DETAILS,
                ManagerHelper.GetInLikeComparator
                (
                    Database.Tables.PersonalDetails.PostcodeId,
                    Database.Tables.POSTAL_CODES,
                    Database.Tables.PostalCodes.Code,
                    code
                )
            );
        }
        
        public static List<Identification> GetIdentificationsWithOwner( int id          )   { return IdentificationsManager.Merge( Database.Tables.IDENTIFICATIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Identifications.DetailsId   , id        ) ); }
        public static List<Identification> GetIdentificationsWithValue( string value    )   { return IdentificationsManager.Merge( Database.Tables.IDENTIFICATIONS  , ManagerHelper.GetEqualComparator( Database.Tables.Identifications.Value       , value     ) ); }
        public static List<ContactNumber> GetContactNumbersWithOwner( int id            )   { return ContactNumbersManager.Merge( Database.Tables.CONTACT_NUMBERS   , ManagerHelper.GetEqualComparator( Database.Tables.ContactNumbers.DetailsId    , id        ) ); }
        public static List<ContactNumber> GetContactNumbersWithNumber( string number    )   { return ContactNumbersManager.Merge( Database.Tables.CONTACT_NUMBERS   , ManagerHelper.GetEqualComparator( Database.Tables.ContactNumbers.Number       , number    ) ); }
        
        public static List<PersonalDetails> GetDetailsByPostcode( PostalCode postcode           )   { return GetDetailsByPostcode( postcode.Id );       }
        public static List<Identification> GetIdentificationsWithOwner( PersonalDetails owner   )   { return GetIdentificationsWithOwner( owner.Id );   }
        public static List<ContactNumber> GetContactNumbersWithOwner( PersonalDetails owner     )   { return GetContactNumbersWithOwner( owner.Id );    }
    }
}
