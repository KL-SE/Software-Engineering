using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;
using System;

namespace OverSurgerySystem_Test
{
    [TestClass]
    public class PersonalDetailsTest
    {
        public static PersonalDetails details;

        public static void SetupPersonalDetails()
        {
            // Address Setup
            AddressTest.SetupAddresses();

            // Personal Details Setup
            details                 = new PersonalDetails();
            details.FirstName       = "Test First Name";
            details.LastName        = "Test Last Name";
            details.Sex             = 'F';
            details.DateOfBirth     = DateTime.Now;
            details.Address         = "Test Address";
            details.Postcode        = AddressTest.postcode;

            for( int i = 0 ; i < 5 ; i++ )
            {
                Identification identification   = new Identification();
                identification.Value            = "112233-10-111" + i;
                identification.Owner            = details;
                details.AddIdentification( identification );
            }

            for( int i = 0 ; i < 5 ; i++ )
            {
                ContactNumber contactNumber = new ContactNumber();
                contactNumber.Number        = "012-345678" + i;
                contactNumber.Owner         = details;
                details.AddContactNumber( contactNumber );
            }
        }

        public static void CleanupPersonalDetails()
        {
            foreach( Identification identification in details.Identifications )
                identification.Delete();

            foreach( ContactNumber contactNumber in details.ContactNumbers )
                contactNumber.Delete();
            
            details.Delete();
            AddressTest.CleanupAddresses();
        }
        
        [TestInitialize]
        public void _SetupPersonalDetails()
        {
            SetupPersonalDetails();
        }
        
        [TestCleanup]
        public void _CleanupPersonalDetails()
        {
            CleanupPersonalDetails();
        }

        [TestMethod]
        public void CompletePersonalDetailsTest()
        {
            // Save the detail and test it's ID
            details.Save();
            Assert.IsTrue( details.Valid );

            // Duplicate Setup
            PersonalDetails newDetails = DetailsManager.GetPersonalDetail( details.Id );
            
            Assert.AreEqual( details.Id                     , newDetails.Id                     );
            Assert.AreEqual( details.FirstName              , newDetails.FirstName              );
            Assert.AreEqual( details.LastName               , newDetails.LastName               );
            Assert.AreEqual( details.Sex                    , newDetails.Sex                    );
            Assert.AreEqual( details.DateOfBirth.ToString() , newDetails.DateOfBirth.ToString() );
            Assert.AreEqual( details.Address                , newDetails.Address                );
            Assert.AreEqual( details.Postcode.Postcode      , newDetails.Postcode.Postcode      );

            // Make sure that all the identifications are there...
            foreach( Identification original in details.Identifications )
            {
                bool found = false;
                foreach( Identification duplicate in newDetails.Identifications )
                {
                    if( original.Id == duplicate.Id )
                    {
                        found = true;
                        break;
                    }
                }

                Assert.IsTrue( found );
            }

            // Make sure that all contact numbers are there...
            foreach( ContactNumber original in details.ContactNumbers )
            {
                bool found = false;
                foreach( ContactNumber duplicate in newDetails.ContactNumbers )
                {
                    if( original.Id == duplicate.Id )
                    {
                        found = true;
                        break;
                    }
                }

                Assert.IsTrue( found );
            }
        }
    }
}
