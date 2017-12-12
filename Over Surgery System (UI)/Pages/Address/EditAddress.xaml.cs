using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.Address
{
    /// <summary>
    /// Interaction logic for EditAddress.xaml
    /// </summary>
    public partial class EditAddress : EditorPage<PostalCode>
    {
        public static PostalCode        CurrentPostcode { set { CurrentItem = value; } get => CurrentItem;  }
        public static City              CurrentCity     { set; get;                                         }
        public static State             CurrentState    { set; get;                                         }
        public static Country           CurrentCountry  { set; get;                                         }

        public static List<PostalCode>  PostcodesList   = new List<PostalCode>();
        public static List<City>        CitiesList      = new List<City>();
        public static List<State>       StatesList      = new List<State>();
        public static List<Country>     CountriesList   = new List<Country>();

        public EditAddress()
        {
            InitializeComponent();
            Loaded += OnLoad;

            PostcodeBox.TextChanged     += (object o, TextChangedEventArgs e) => HideError();
            CityBox.TextChanged         += (object o, TextChangedEventArgs e) => HideError();
            StateBox.TextChanged        += (object o, TextChangedEventArgs e) => HideError();
            CountryBox.TextChanged      += (object o, TextChangedEventArgs e) => HideError();

            PostcodeBox.StoppedTyping   += (object o, EventArgs a) => UpdatePostcodes();
            CityBox.StoppedTyping       += (object o, EventArgs a) => UpdateCities();
            StateBox.StoppedTyping      += (object o, EventArgs a) => UpdateStates();
            CountryBox.StoppedTyping    += (object o, EventArgs a) => UpdateCountries();
            ConfirmButton.Click         += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click          += (object o, RoutedEventArgs a) => DoCancel();
            PostcodeClearButton.Click   += (object o, RoutedEventArgs a) => PostcodeBox.Text = "";
            CityClearButton.Click       += (object o, RoutedEventArgs a) => CityBox.Text = "";
            StateClearButton.Click      += (object o, RoutedEventArgs a) => StateBox.Text = "";
            CountryClearButton.Click    += (object o, RoutedEventArgs a) => CountryBox.Text = "";

            if( IsFind )
            {
                PostcodeBox.Visibility          = Visibility.Collapsed;
                CityBox.Visibility              = Visibility.Collapsed;
                StateBox.Visibility             = Visibility.Collapsed;
                CountryBox.Visibility           = Visibility.Collapsed;
                CountrySeparator.Visibility     = Visibility.Collapsed;
                StateSeparator.Visibility       = Visibility.Collapsed;
                CitySeparator.Visibility        = Visibility.Collapsed;
                PostcodeClearButton.Visibility  = Visibility.Collapsed;
                StateClearButton.Visibility     = Visibility.Collapsed;
                CityClearButton.Visibility      = Visibility.Collapsed;
                CountryClearButton.Visibility   = Visibility.Collapsed;

                ConfirmButtonImg.Source     = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text      = "Find";
            }
        }

        private void OnLoad( object o , EventArgs e )
        {
            if( CurrentPostcode         != null ) PostcodeBox.Text  = CurrentPostcode.Postcode;     else CurrentPostcode            = new PostalCode();
            if( CurrentPostcode.City    != null ) CityBox.Text      = CurrentPostcode.City.Name;    else CurrentPostcode.City       = new City();
            if( CurrentPostcode.State   != null ) StateBox.Text     = CurrentPostcode.State.Name;   else CurrentPostcode.State      = new State();
            if( CurrentPostcode.Country != null ) CountryBox.Text   = CurrentPostcode.Country.Name; else CurrentPostcode.Country    = new Country();
            UpdateCountries();
        }

        private void HideError()
        {
            ErrorBox.Visibility = Visibility.Collapsed;
        }

        private void ShowError( string error_message )
        {
            ErrorBox.Visibility = Visibility.Visible;
            ErrorBox.Text       = error_message;
        }

        private void UpdatePostcodes()
        {
            PostcodesList = AddressManager.GetPostcodesByCode( PostcodeBox.Text );
            PostcodesList = ManagerHelper.Filter( PostcodesList , e => e.City == CurrentCity );
            UpdateMenu( PostcodesList , e => e.Postcode , PostcodeResult , PostcodeResultHeader );
        }

        private void UpdateCities()
        {
            CitiesList = AddressManager.GetCitiesByName( CityBox.Text );
            CitiesList = ManagerHelper.Filter( CitiesList , e => e.State == CurrentState );
            UpdateMenu( CitiesList , e => e.Name , CityResult , CityResultHeader );
            UpdatePostcodes();
        }

        private void UpdateStates()
        {
            StatesList = AddressManager.GetStatesByName( StateBox.Text );
            StatesList = ManagerHelper.Filter( StatesList , e => e.Country == CurrentCountry );
            UpdateMenu( StatesList , e => e.Name , StateResult , StateResultHeader );
            UpdateCities();
        }

        private void UpdateCountries()
        {
            CountriesList = AddressManager.GetCountriesByName( CountryBox.Text );
            UpdateMenu( CountriesList , e => e.Name , CountryResult , CountryResultHeader );
            UpdateStates();
        }

        private void UpdateMenu<T>( List<T> results , Func<T,string> selector , Menu menu , MenuItem header ) where T: new()
        {
            if( results.Count == 0 )
            {
                menu.IsEnabled      = false;
                header.Header       = "- Not Found -";
                header.ItemsSource  = new List<string>();
                MenuItemCallback( new T() );
            }
            else
            {
                List<MenuItem> list = new List<MenuItem>();
                menu.IsEnabled      = results.Count > 1;
                
                foreach( T obj in results )
                {
                    MenuItem menuItem   = new MenuItem();
                    menuItem.Header     = selector( obj );
                    menuItem.Click     += ( object o , RoutedEventArgs a ) =>
                    {
                        header.Header = selector( obj );
                        MenuItemCallback( obj );
                    };

                    list.Add( menuItem );
                }
                
                header.ItemsSource  = list;
                header.Header       = selector( results[0] );
                MenuItemCallback( results[0] );
            }
        }

        private void MenuItemCallback( object obj )
        {
            if( obj is PostalCode   ) { CurrentPostcode   = ( PostalCode  )( obj );                     } else
            if( obj is City         ) { CurrentCity       = ( City        )( obj ); UpdatePostcodes();  } else
            if( obj is State        ) { CurrentState      = ( State       )( obj ); UpdateCities();     } else
            if( obj is Country      ) { CurrentCountry    = ( Country     )( obj ); UpdateStates();     }
        }

        private void ResolvePostcode()
        {
            ResolveCity();
            if( CurrentPostcode == null || !CurrentPostcode.Valid )
            {
                List<PostalCode> list = AddressManager.GetPostcodesByExactCode( PostcodeBox.Text );
                if( list.Count > 0 )
                {
                    bool found = false;
                    foreach( PostalCode postcode in list )
                    {
                        if( postcode.City == CurrentCity )
                        {
                            found           = true;
                            CurrentPostcode = postcode;
                            break;
                        }
                    }

                    if( !found )
                    {
                        CurrentPostcode.Postcode = list[0].Postcode;
                    }
                }
                else
                {
                    CurrentPostcode             = new PostalCode();
                    CurrentPostcode.Postcode    = PostcodeBox.Text;
                }
            }
        }

        private void ResolveCity()
        {
            ResolveState();
            if( CurrentCity == null || !CurrentCity.Valid )
            {
                List<City> list = AddressManager.GetCitiesByName( CityBox.Text );
                if( list.Count > 0 )
                {
                    bool found = false;
                    foreach( City city in list )
                    {
                        if( city.State == CurrentState )
                        {
                            found       = true;
                            CurrentCity = city;
                            break;
                        }
                    }

                    if( !found )
                    {
                        CurrentCity.Name = list[0].Name;
                    }
                }
                else
                {
                    CurrentCity         = new City();
                    CurrentCity.Name    = CityBox.Text;
                }
            }
        }

        private void ResolveState()
        {
            ResolveCountry();
            if( CurrentState == null || !CurrentState.Valid )
            {
                List<State> list = AddressManager.GetStatesByExactName( StateBox.Text );
                if( list.Count > 0 )
                {
                    bool found = false;
                    foreach( State state in list )
                    {
                        if( state.Country == CurrentCountry )
                        {
                            found           = true;
                            CurrentState    = state;
                            break;
                        }
                    }

                    if( !found )
                    {
                        CurrentState.Name = list[0].Name;
                    }
                }
                else
                {
                    CurrentState        = new State();
                    CurrentState.Name   = StateBox.Text;
                }
            }
        }

        private void ResolveCountry()
        {
            if( CurrentCountry == null || !CurrentCountry.Valid )
            {
                List<Country> list = AddressManager.GetCountriesByExactName( CountryBox.Text );
                if( list.Count > 0 )
                {
                    CurrentCountry = list[0];
                }
                else
                {
                    CurrentCountry      = new Country();
                    CurrentCountry.Name = CountryBox.Text;
                }
            }
        }

        private void DoConfirm()
        {
            // Figure out if we actually need this.
            // ResolvePostcode();
            if( CurrentCountry  == null || !CurrentCountry.Valid    )   CurrentCountry.Name         = CountryBox.Text;
            if( CurrentState    == null || !CurrentState.Valid      )   CurrentState.Name           = StateBox.Text;
            if( CurrentCity     == null || !CurrentCity.Valid       )   CurrentCity.Name            = CityBox.Text;
            if( CurrentPostcode == null || !CurrentPostcode.Valid   )   CurrentPostcode.Postcode    = PostcodeBox.Text;
            
            if( CurrentCountry.Name.Length      == 0 ) { ShowError( "Please select a country"   ); return; }
            if( CurrentState.Name.Length        == 0 ) { ShowError( "Please select a state."    ); return; }
            if( CurrentCity.Name.Length         == 0 ) { ShowError( "Please select a city."     ); return; }
            if( CurrentPostcode.Postcode.Length == 0 ) { ShowError( "Please select a postcode." ); return; }

            CurrentPostcode.City    = CurrentCity;
            CurrentPostcode.State   = CurrentState;
            CurrentPostcode.Country = CurrentCountry;
            CurrentPostcode.Save();

            OnConfirm?.Invoke( CurrentPostcode );
        }

        private void DoCancel()
        {
            OnCancel?.Invoke();
        }
    }
}
