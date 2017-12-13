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
        public List<Identification> identifications;
        public List<ContactNumber> contactNumbers;

        // Default Constructor
        public PersonalDetails() : base()
        {
            identifications = new List<Identification>();
            contactNumbers  = new List<ContactNumber>();
        }
        
        // Person's full name, which is a combination of their first and last name
        public string FullName
        {
            get
            {
                return String.Format( "{0} {1}" , FirstName , LastName );
            }
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
        
        // Person's identification
        public IList<Identification> Identifications
        {
            get
            {
                return identifications.AsReadOnly();
            }
        }
        
        // Person's contact numbers
        public IList<ContactNumber> ContactNumbers
        {
            get
            {
                return contactNumbers.AsReadOnly();
            }
        }

        // Adders and removers for identifications
        public bool HaveIdentification( string iden )
        {
            foreach( Identification identification in Identifications )
            {
                if( identification.Value.Equals( iden ) )
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool HaveIdentification( Identification iden )
        {
            return HaveIdentification( iden.Value );
        }
        
        public bool AddIdentification( string iden )
        {
            if( !HaveIdentification( iden ) )
            {
                Identification newIden  = new Identification();
                newIden.Value           = iden;
                newIden.Owner           = this;

                Identifications.Add( newIden );
            }
            return false;
        }
        
        public bool AddIdentification( Identification iden )
        {
            if( !HaveIdentification( iden ) )
            {
                Identifications.Add( iden );
            }
            return false;
        }
        
        public void RemoveIdentification( string iden )
        {
            for( int i = 0 ; i < identifications.Count ; i++ )
            {
                if( identifications[i].Valid.Equals( iden ) )
                {
                    identifications[i].Delete();
                    identifications.RemoveAt( i );
                    return;
                }
            }
        }
        
        public void RemoveIdentification( Identification iden )
        {
            RemoveIdentification( iden.Value );
        }

        // Adders and removers for contact numbers
        public bool HaveContactNumber( string iden )
        {
            foreach( ContactNumber contactNumbers in ContactNumbers )
            {
                if( contactNumbers.Number.Equals( iden ) )
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool HaveContactNumber( ContactNumber number )
        {
            return HaveContactNumber( number.Number );
        }
        
        public bool AddContactNumber( string number )
        {
            if( !HaveContactNumber( number ) )
            {
                ContactNumber newNo = new ContactNumber();
                newNo.Number        = number;
                newNo.Owner         = this;

                ContactNumbers.Add( newNo );
            }
            return false;
        }
        
        public bool AddContactNumber( ContactNumber number )
        {
            if( !HaveContactNumber( number ) )
            {
                ContactNumbers.Add( number );
            }
            return false;
        }
        
        public void RemoveContactNumber( string number )
        {
            for( int i = 0 ; i < contactNumbers.Count ; i++ )
            {
                if( contactNumbers[i].Valid.Equals( number ) )
                {
                    contactNumbers[i].Delete();
                    contactNumbers.RemoveAt( i );
                    return;
                }
            }
        }
        
        public void RemoveContactNumber( ContactNumber number )
        {
            RemoveContactNumber( number.Number );
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

            MySqlDataReader reader  = DoLoad( query );
            int postcodeId          = INVALID_ID;
            
            if( Loaded )
            {
                FirstName       = reader.GetString( 0 );
                LastName        = reader.GetString( 1 );
                Address         = reader.GetString( 2 );
                postcodeId      = reader.GetInt32( 3 );
                DateOfBirth     = reader.GetDateTime( 4 );
                Sex             = reader.GetChar( 5 );
                DetailsManager.Add( this );
            }

            reader.Close();

            Postcode        = AddressManager.GetPostcode( postcodeId );
            identifications = DetailsManager.GetIdentificationsWithOwner( this );
            contactNumbers  = DetailsManager.GetContactNumbersWithOwner( this );
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