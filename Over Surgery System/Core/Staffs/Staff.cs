using System;
using MySql.Data.MySqlClient;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;
using System.Security.Cryptography;
using System.Text;

namespace OverSurgerySystem.Core.Staffs
{
    [Serializable]
    public class UnknownStaffTypeError: Exception
    {
        public UnknownStaffTypeError() {}
        public UnknownStaffTypeError( string message ) : base( message ) { }
        public UnknownStaffTypeError( string message , Exception inner ) : base( message , inner ) { }
    }

    public class Staff : DatabaseObject
    {
        protected string passwordSalt;
        protected string passwordHash;

        public DateTime dateJoined;
        public PersonalDetails Details  { set; get; }
        public bool Active              { set; get; }

        // String ID for human readability.
        public string StringId
        {
            get
            {
                return "S" + Id.ToString( "D8" );
            }
        }

        public static int GetIdFromString( string StrId )
        {
            try
            {
                if( StrId.ToUpper().StartsWith( "S" ) )
                    return Int32.Parse( StrId.Substring( 1 ) );

                return INVALID_ID;
            }
            catch
            {
                return INVALID_ID;
            }
        }

        // Avoid other part of the code from instantiating the Staff class.
        public Staff()
        {
            Active  = true;
            Details = new PersonalDetails();
            if( !( this is MedicalStaff || this is Receptionist ) )
                throw new UnknownStaffTypeError();
        }

        // Date Joined
        public DateTime DateJoined
        {
            set
            {
                dateJoined = value.Date;
            }
            get
            {
                return dateJoined;
            }
        }

        // Password Generation
        // We will be generating more or less a one-way encryption by merging the user's password with a randomly generated number.
        public string Password
        {
            set
            {
                passwordSalt    = GenerateSalt();
                passwordHash    = GetPasswordHash( value , passwordSalt );
            }
        }
        
        // Generate a random salt of a length matching the database column's maximum length
        public static string GenerateSalt()
        {
            byte[] buffer = new byte[Database.Tables.Staffs.PasswordSaltMaxLength];
            new RNGCryptoServiceProvider().GetBytes( buffer );
            return Convert.ToBase64String( buffer );
        }

        // Merge the salt and the password together and hash it with a suitable algorithm.
        public static string GetPasswordHash( string password , string salt )
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] bytesToHash      = Encoding.UTF8.GetBytes( password + salt );
            byte[] hashedPassword   = algorithm.ComputeHash( bytesToHash );

            // Of course everything is converted to string at the end of the day.
            return Convert.ToBase64String( hashedPassword );            
        }

        // Lastly, a function to compare an incoming string with the hashed password.
        // To do so, we combine the incoming string with the existing hash.
        public bool IsPasswordCorrect( string inPassword )
        {
            string incomingHash = GetPasswordHash( inPassword , passwordSalt );
            return passwordHash.Equals( incomingHash );
        }

        // And now a simple copy function because the password will be inacccessible outside of the class.
        public void CopyPassword( Staff other )
        {
            passwordHash    = other.passwordHash;
            passwordSalt    = other.passwordSalt;
        }

        // Also something to tell whether the staff have a password.
        public bool HavePassword()
        {
            return passwordSalt != null && passwordHash != null;
        }
        
        // Inherited Functions
        public override void Delete()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            DoDelete( query );
            Details.Delete();
            StaffsManager.Remove( this );
        }

        public override void Load()
        {
            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            query.Add( Database.Tables.Staffs.PasswordHash  );
            query.Add( Database.Tables.Staffs.PasswordSalt  );
            query.Add( Database.Tables.Staffs.DateJoined    );
            query.Add( Database.Tables.Staffs.DetailsId     );
            query.Add( Database.Tables.Staffs.Active        );
            
            MySqlDataReader reader  = DoLoad( query );
            int detailsId           = INVALID_ID;
            
            if( Loaded )
            {
                passwordHash    = reader.GetString( 0 );
                passwordSalt    = reader.GetString( 1 );
                dateJoined      = reader.GetDateTime( 2 );
                detailsId       = reader.GetInt32( 3 );
                Active          = reader.GetByte( 4 ) > 0 ? true : false;
                StaffsManager.Add( this );
            }
            
            reader.Close();
            Details = DetailsManager.GetPersonalDetail( detailsId );
        }

        public override void Save()
        {
            char staffType  = '\0';
            if( this is MedicalStaff ) staffType = 'M'; else
            if( this is Receptionist ) staffType = 'R';

            DatabaseQuery query = new DatabaseQuery( Database.Tables.STAFFS );
            query.Add( Database.Tables.Staffs.Type          , staffType         );
            query.Add( Database.Tables.Staffs.PasswordHash  , passwordHash      );
            query.Add( Database.Tables.Staffs.PasswordSalt  , passwordSalt      );
            query.Add( Database.Tables.Staffs.DateJoined    , DateJoined        );
            query.Add( Database.Tables.Staffs.DetailsId     , Details           );
            query.Add( Database.Tables.Staffs.Active        , Active ? 1 : 0    );
            DoSave( query );

            StaffsManager.Add( this );
        }
    }
}
