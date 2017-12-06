using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.Core.Base
{
    public class PersonalDetails : DatabaseObject
    {
        public string FirstName     { set; get; }
        public string LastName      { set; get; }
        public string Address       { set; get; }
        public PostalCode Postcode  { set; get; }
        public DateTime DateOfBirth { set; get; }

        char sex;
        public List<Identification> Identifications { set; get; }
        public List<ContactNumber> ContactNumbers   { set; get; }

        // Default Constructor
        public PersonalDetails()
        {
            Identifications = new List<Identification>();
            ContactNumbers  = new List<ContactNumber>();
        }
        
        // Person's full address, which is basically a combination of their individual address components
        public string FullAddress
        {
            get
            {
                return String.Format( "{0}, {1}, {2}, {3}, {4}" , Address , Postcode , Postcode.City , Postcode.State , Postcode.Country );
            }
        }
        
        // Person's sex
        public char Sex
        {
            set
            {
                char upperValue = value.ToString().ToUpper()[0];
                if( upperValue == 'M' || upperValue == 'F' )
                {
                    sex = upperValue;
                }
            }

            get
            {
                return sex;
            }
        }
        
        // Inherited Functions
        public override void Delete()
        {
            foreach( Identification identification in Identifications )
                identification.Delete();

            foreach( ContactNumber contactNumber in ContactNumbers )
                contactNumber.Delete();

            DatabaseQuery query = new DatabaseQuery( Database.Tables.PERSONAL_DETAILS );
            DoDelete( query );
            DetailsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PERSONAL_DETAILS );
            query.Add( Database.Tables.PersonalDetails.FirstName    );
            query.Add( Database.Tables.PersonalDetails.LastName     );
            query.Add( Database.Tables.PersonalDetails.Address      );
            query.Add( Database.Tables.PersonalDetails.PostcodeId   );
            query.Add( Database.Tables.PersonalDetails.DateOfBirth  );
            query.Add( Database.Tables.PersonalDetails.Sex          );

            MySqlDataReader reader = DoLoad( query );
            
            if( Loaded )
            {
                FirstName       = reader.GetString( 0 );
                LastName        = reader.GetString( 1 );
                Address         = reader.GetString( 2 );
                Postcode        = AddressManager.GetPostcode( reader.GetInt32( 3 ) );
                DateOfBirth     = reader.GetDateTime( 4 );
                Sex             = reader.GetChar( 5 );
                DetailsManager.Add( this );
            }

            reader.Close();

            Identifications = DetailsManager.GetIdentificationsWithOwner( this );
            ContactNumbers  = DetailsManager.GetContactNumbersWithOwner( this );
        }

        public override void Save()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.PERSONAL_DETAILS );
            query.Add( Database.Tables.PersonalDetails.FirstName    , FirstName     );
            query.Add( Database.Tables.PersonalDetails.LastName     , LastName      );
            query.Add( Database.Tables.PersonalDetails.Address      , Address       );
            query.Add( Database.Tables.PersonalDetails.PostcodeId   , Postcode      );
            query.Add( Database.Tables.PersonalDetails.DateOfBirth  , DateOfBirth   );
            query.Add( Database.Tables.PersonalDetails.Sex          , Sex           );
            DoSave( query );
            DetailsManager.Add( this );

            foreach( Identification identification in Identifications )
                identification.Validate();

            foreach( ContactNumber contactNumber in ContactNumbers )
                contactNumber.Validate();
        }
    }
}