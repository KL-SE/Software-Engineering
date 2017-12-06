using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem_Test
{
    [TestClass]
    public class AddressTest
    {
        public static PostalCode postcode;
        public static City city;
        public static State state;
        public static Country country;

        public static void SetupAddresses()
        {
            // Create a full test address...
            postcode    = new PostalCode();
            city        = new City();
            state       = new State();
            country     = new Country();
            
            country.Name        = "Test Country";
            state.Name          = "Test State";
            city.Name           = "Test City";
            postcode.Postcode   = "00000";

            postcode.City       = city;
            postcode.State      = state;
            postcode.Country    = country;
        }

        public static void CleanupAddresses()
        {
            // Delete the newly added data...
            postcode.Delete();
            city.Delete();
            state.Delete();
            country.Delete();
        }
        
        [TestInitialize]
        public void _SetupAddresses()
        {
            SetupAddresses();
        }

        [TestCleanup]
        public void _CleanupAddresses()
        {
            CleanupAddresses();
        }

        [TestMethod]
        public void CompleteAddressTest()
        {
            // Save the address into the database...
            postcode.Save();
            city.Save();
            state.Save();
            country.Save();
            
            // After saving, their ID must not be the default ID
            Assert.IsTrue( postcode.Valid   );
            Assert.IsTrue( city.Valid       );
            Assert.IsTrue( state.Valid      );
            Assert.IsTrue( country.Valid    );

            // Create a duplicate...
            // When we load the postcode data, the relevant city, state and country data should be automatically loaded.
            PostalCode newPostcode  = AddressManager.GetPostcode( postcode.Id );
            City newCity            = AddressManager.GetCity( city.Id );
            State newState          = AddressManager.GetState( state.Id );
            Country newCountry      = AddressManager.GetCountry( country.Id );

            // Check if the data is valid
            Assert.IsNotNull( newCity       );
            Assert.IsNotNull( newState      );
            Assert.IsNotNull( newCountry    );

            // After loading, the duplicates' data should match the originals'
            Assert.AreEqual( postcode.Id    , newPostcode.Id    );
            Assert.AreEqual( city.Id        , newCity.Id        );
            Assert.AreEqual( state.Id       , newState.Id       );
            Assert.AreEqual( country.Id     , newCountry.Id     );

            Assert.AreEqual( postcode.Postcode  , newPostcode.Postcode  );
            Assert.AreEqual( city.Name          , newCity.Name          );
            Assert.AreEqual( state.Name         , newState.Name         );
            Assert.AreEqual( country.Name       , newCountry.Name       );
            
            Assert.AreEqual( postcode.City.Id       , newPostcode.City.Id       );
            Assert.AreEqual( postcode.State.Id      , newPostcode.State.Id      );
            Assert.AreEqual( postcode.Country.Id    , newPostcode.Country.Id    );
        }
    }
}
