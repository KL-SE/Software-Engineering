using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;
using OverSurgerySystem.UI.Pages.Address;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.Common
{
    /// <summary>
    /// Interaction logic for AddStaff.xaml
    /// </summary>
    public partial class EditDetails : EditorPage<PersonalDetails>
    {
        public EditDetails()
        {
            InitializeComponent();
            PostcodeBox.StoppedTyping              += (object o, EventArgs e) => ResolvePostcode();
            PostcodeBox.TextChanged                += (object o, TextChangedEventArgs e) => HideError();
            FirstNameBox.TextChanged               += (object o, TextChangedEventArgs e) => HideError();
            LastNameBox.TextChanged                += (object o, TextChangedEventArgs e) => HideError();
            AddressBox.TextChanged                 += (object o, TextChangedEventArgs e) => HideError();
            IdentificationBox.TextChanged          += (object o, TextChangedEventArgs e) => HideError();
            ContactNoBox.TextChanged               += (object o, TextChangedEventArgs e) => HideError();
            DateOfBirthPicker.SelectedDateChanged  += (object o, SelectionChangedEventArgs e) => HideError();
            PostcodeButton.Click                   += (object o, RoutedEventArgs e) => RecordAndNavigateToEditAddress();
            ConfirmButton.Click                    += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click                     += (object o, RoutedEventArgs a) => DoCancel();
            ClearDobButton.Click                   += ClearDateOfBirth;
            IdentificationButton.Click             += AddIdentification;
            ContactNoButton.Click                  += AddContactNumbers;
            Loaded                                 += OnLoad;
            EditAddress.OnConfirm                   = OnPostcodeEdit;
            EditAddress.OnCancel                    = OnPostcodeCancel;

            if( IsFind )
            {
                PostcodeButton.Content          = "Find";
                UserSexText.Visibility          = Visibility.Collapsed;
                UserSexPicker.Visibility        = Visibility.Collapsed;
                IdentificationButton.Visibility = Visibility.Collapsed;
                ContactNoButton.Visibility      = Visibility.Collapsed;
                IdentificationBox.Width         = 900;
                ContactNoBox.Width              = 900;

                ConfirmButtonImg.Source         = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text          = "Find";
            }
        }

        public void OnLoad( object o , RoutedEventArgs e )
        {
            if( CurrentItem != null )
            {
                FirstNameBox.Text   = CurrentItem.FirstName;
                LastNameBox.Text    = CurrentItem.LastName;
                AddressBox.Text     = CurrentItem.Address;
                if( CurrentItem.DateOfBirth != DatabaseObject.INVALID_DATETIME )
                {
                    DateOfBirthPicker.SelectedDate = CurrentItem.DateOfBirth;
                }

                if( CurrentItem.Sex == 'M' ) UserSexHeader.Header = "Male";   else
                if( CurrentItem.Sex == 'F' ) UserSexHeader.Header = "Female"; else
                    CurrentItem.Sex = '\0';

                if( CurrentItem.Postcode != null )
                {
                    PostcodeBox.FreezeTimer = true;
                    PostcodeBox.Text        = CurrentItem.Postcode.Postcode;
                    CityBox.Text            = CurrentItem.Postcode.City.Name;
                    StateBox.Text           = CurrentItem.Postcode.State.Name;
                    CountryBox.Text         = CurrentItem.Postcode.Country.Name;
                    PostcodeBox.FreezeTimer = false;
                }

                PopulateIdentifications();
                PopulateContactNumbers();
            }
            
            List<MenuItem> list = new List<MenuItem>();
            MenuItem maleItem   = new MenuItem();
            MenuItem femaleItem = new MenuItem();
            list.Add( maleItem      );
            list.Add( femaleItem    );

            maleItem.Header     = "Male";
            femaleItem.Header   = "Female";
            maleItem.Click     += ( object i , RoutedEventArgs a ) => { UserSexHeader.Header = maleItem.Header;     CurrentItem.Sex = 'M'; HideError(); };
            femaleItem.Click   += ( object i , RoutedEventArgs a ) => { UserSexHeader.Header = femaleItem.Header;   CurrentItem.Sex = 'F'; HideError(); };

            UserSexHeader.ItemsSource = list;
        }

        public void PopulateIdentifications()
        {
            IdentificationsList.Children.RemoveRange( 0 , IdentificationsList.Children.Count );
            if( IsFind )
            {
                if( CurrentItem.Identifications.Count > 0 )
                {
                    IdentificationBox.Text = CurrentItem.Identifications[0].Value;
                }
            }
            else
            {
                foreach( Identification iden in CurrentItem.Identifications )
                {
                    StackPanel itemRow  = new StackPanel();
                    TextBox itemContent = new TextBox();
                    Button deleteItem   = new Button();
                
                    itemRow.Orientation     = Orientation.Horizontal;
                    itemContent.Text        = iden.Value;
                    itemContent.Width       = 700;
                    itemContent.IsEnabled   = false;
                    deleteItem.Content      = "Delete";
                    deleteItem.Click       += ( object i , RoutedEventArgs a ) =>
                    {
                        CurrentItem.Identifications.Remove( iden );
                        IdentificationsList.Children.Remove( itemRow );
                    };
                
                    itemRow.Children.Add( itemContent   );
                    itemRow.Children.Add( deleteItem    );

                    IdentificationsList.Children.Add( itemRow );
                }
            }
        }

        public void PopulateContactNumbers()
        {
            ContactNoList.Children.RemoveRange( 0 , ContactNoList.Children.Count );
            if( IsFind )
            {
                if( CurrentItem.ContactNumbers.Count > 0 )
                {
                    ContactNoBox.Text = CurrentItem.ContactNumbers[0].Number;
                }
            }
            else
            {
                foreach( ContactNumber contactNo in CurrentItem.ContactNumbers )
                {
                    StackPanel itemRow  = new StackPanel();
                    TextBox itemContent = new TextBox();
                    Button deleteItem   = new Button();
                
                    itemRow.Orientation     = Orientation.Horizontal;
                    itemContent.Text        = contactNo.Number;
                    itemContent.Width       = 700;
                    itemContent.IsEnabled   = false;
                    deleteItem.Content      = "Delete";
                    deleteItem.Click       += ( object i , RoutedEventArgs a ) =>
                    {
                        CurrentItem.ContactNumbers.Remove( contactNo );
                        ContactNoList.Children.Remove( itemRow );
                    };
                
                    itemRow.Children.Add( itemContent   );
                    itemRow.Children.Add( deleteItem    );

                    ContactNoList.Children.Add( itemRow );
                }
            }
        }

        public void ClearDateOfBirth( object o , RoutedEventArgs e )
        {
            DateOfBirthPicker.SelectedDate  = null;
            CurrentItem.DateOfBirth       = DatabaseObject.INVALID_DATETIME;
        }

        public void AddIdentification( object o , RoutedEventArgs e )
        {
            if( IdentificationBox.Text.Length > 0 )
            {
                foreach( Identification iden in CurrentItem.Identifications )
                {
                    if( iden.Value.Equals( IdentificationBox.Text ) )
                    {
                        ShowMessage( "The identification already exists." );
                        return;
                    }
                }

                Identification newIden  = new Identification();
                newIden.Value           = IdentificationBox.Text;
                newIden.Owner           = CurrentItem;
                CurrentItem.Identifications.Add( newIden );
                PopulateIdentifications();

                // Clear the text box.
                IdentificationBox.Text = "";
            }
        }

        public void AddContactNumbers( object o , RoutedEventArgs e )
        {
            if( ContactNoBox.Text.Length > 0 )
            {
                foreach( ContactNumber no in CurrentItem.ContactNumbers )
                {
                    if( no.Number.Equals( ContactNoBox.Text ) )
                    {
                        ShowMessage( "The contact number already exists." );
                        return;
                    }
                }
                
                ContactNumber newNo = new ContactNumber();
                newNo.Number        = ContactNoBox.Text;
                newNo.Owner         = CurrentItem;
                CurrentItem.ContactNumbers.Add( newNo );
                PopulateContactNumbers();

                // Clear the text box.
                ContactNoBox.Text = "";
            }
        }

        public void HideError()
        {
            MsgBox.Visibility = Visibility.Collapsed;
        }

        public void ShowMessage( string error_message )
        {
            MsgBox.Visibility = Visibility.Visible;
            MsgBox.Text       = error_message;
        }

        private void ResolvePostcode()
        {
            List<PostalCode> results    = AddressManager.GetPostcodesByExactCode( PostcodeBox.Text );
            CurrentItem.Postcode        = results.Count == 1 ? results[0] : null;
            if( results.Count == 1 )
            {
                CityBox.Text    = CurrentItem.Postcode.City.Name;
                StateBox.Text   = CurrentItem.Postcode.State.Name;
                CountryBox.Text = CurrentItem.Postcode.Country.Name;
            }
            else if( results.Count > 1 )
            {
                CityBox.Text    = "- Multiple Found -";
                StateBox.Text   = "- Multiple Found -";
                CountryBox.Text = "- Multiple Found -";
            }
            else
            {
                CityBox.Text    = "- Not Found -";
                StateBox.Text   = "- Not Found -";
                CountryBox.Text = "- Not Found -";
            }
        }

        public void OnPostcodeEdit( PostalCode postcode )
        {
            OnNavigate?.Invoke();
            CurrentItem.Postcode = postcode;
        }

        public void OnPostcodeCancel()
        {
            OnReturn?.Invoke();
        }

        private void RecordFields()
        {
            CurrentItem.FirstName   = FirstNameBox.Text;
            CurrentItem.LastName    = LastNameBox.Text;
            CurrentItem.Address     = AddressBox.Text;

            if( DateOfBirthPicker.SelectedDate != null )
            {
                CurrentItem.DateOfBirth = DateOfBirthPicker.SelectedDate.Value;
            }
            else
            {
                CurrentItem.DateOfBirth = DatabaseObject.INVALID_DATETIME;
            }

            AddIdentification( null , null );
            AddContactNumbers( null , null );
            
            if( CurrentItem.Postcode != null )
            {
                EditAddress.CurrentPostcode = CurrentItem.Postcode;
                EditAddress.CurrentCity     = CurrentItem.Postcode.City;
                EditAddress.CurrentState    = CurrentItem.Postcode.State;
                EditAddress.CurrentCountry  = CurrentItem.Postcode.Country;
            }
        }

        private void RecordAndNavigateToEditAddress()
        {
            RecordFields();
            EditAddress.Setup( new PostalCode() , Mode );
            App.GoToPage( new EditAddress() );
        }
        
        private void DoConfirm()
        {
            RecordFields();

            if( IsEdit )
            {
                if( CurrentItem.FirstName.Length == 0                             ) { ShowMessage( "Please enter the first name."                     ); return; }
                if( CurrentItem.LastName.Length == 0                              ) { ShowMessage( "Please enter the last name."                      ); return; }
                if( CurrentItem.Address.Length == 0                               ) { ShowMessage( "Please enter the address."                        ); return; }
                if( CurrentItem.DateOfBirth == DatabaseObject.INVALID_DATETIME    ) { ShowMessage( "Please enter the date of birth."                  ); return; }
                if( CurrentItem.DateOfBirth > DateTime.Now                        ) { ShowMessage( "Please enter a valid date of birth."              ); return; }
                if( CurrentItem.Sex == '\0'                                       ) { ShowMessage( "Please choose the sex."                           ); return; }
                if( CurrentItem.Identifications.Count == 0                        ) { ShowMessage( "Please add at least one identification number."   ); return; }
                if( CurrentItem.ContactNumbers.Count == 0                         ) { ShowMessage( "Please add at least one contact number."          ); return; }
            }

            // Postcode is an exception where whether we're in find more or edit mode, it must be valid.
            if( CurrentItem.Postcode == null && PostcodeBox.Text.Length > 0 )
            {
                ShowMessage( "Please enter a valid postcode." );
                return;
            }

            OnConfirm?.Invoke( CurrentItem );
        }

        private void DoCancel()
        {
            OnCancel?.Invoke();
        }
    }
}
