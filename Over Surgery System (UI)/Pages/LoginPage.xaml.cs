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
using OverSurgerySystem.UI.Persistent;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.UI.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            Loaded                  += OnLoad;
            LoginButton.Click       += DoLogin;
            IdField.TextChanged     += (object o, TextChangedEventArgs e    ) => HideMessage();
            PasswordField.TextInput += (object o, TextCompositionEventArgs e) => HideMessage();

            App.SetTitle( "Log In" );
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
            try
            {
                if( !App.HaveAdminAcount() )
                {
                    App.DoLastResortLogin();
                }
            }
            catch
            {
                ShowMessage( "Unable to connect to database server." );
            }
        }
        
        public void HideMessage()
        {
            MsgBox.Visibility = Visibility.Collapsed;
        }

        public void ShowMessage( string error_message )
        {
            MsgBox.Visibility = Visibility.Visible;
            MsgBox.Text       = error_message;
        }

        private void DoLogin( object sender , RoutedEventArgs e )
        {
            try
            {
                if( !App.HaveAdminAcount() )
                {
                    App.DoLastResortLogin();
                }

                App.DoLogin( IdField.Text , PasswordField.Password );
            }
            catch( StaffNotFoundError )
            {
                ShowMessage( "Invalid username and password combination." );
            }
            catch
            {
                ShowMessage( "Unable to connect to database server." );
            }
        }
    }
}
