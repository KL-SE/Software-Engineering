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
using OverSurgerySystem.UI.Pages;

namespace OverSurgerySystem.UI.Persistent
{
    /// <summary>
    /// Interaction logic for MenuBanner.xaml
    /// </summary>
    public partial class MenuBanner : Page
    {
        public static MenuBanner instance;
        public static MenuBanner Instance
        {
            private set
            {
                instance = value;
            }
            get
            {
                if( instance == null )
                    instance = new MenuBanner();

                return instance;
            }
        }

        static MenuBanner() { }
        private MenuBanner()
        {
            InitializeComponent();
            Loaded += OnLoad;

            LogoutButton.MouseLeftButtonDown    += CheckLogin;
            ExitButton.MouseLeftButtonDown      += DoExit;
            MaximizeButton.MouseLeftButtonDown  += DoRestore;
            MinimizeButton.MouseLeftButtonDown  += DoMinimize;
        }

        public void UpdateLoginText()
        {
            if( App.LoggedInStaff != null )
            {
                Welcome.Text            = "Welcome, ";
                LogoutButton.Text       = "Logout";
                Username.Visibility     = Visibility.Visible;
                StaffId.Visibility      = Visibility.Visible;
                StaffIdDesc.Visibility  = Visibility.Visible;
                Username.Text           = String.Format( "{0} {1}" , App.LoggedInStaff.Details.FirstName , App.LoggedInStaff.Details.LastName );
                StaffId.Text            = App.LoggedInStaff.StringId;
            }
            else
            {
                Welcome.Text            = "Welcome!";
                LogoutButton.Text       = "Log In";
                Username.Visibility     = Visibility.Collapsed;
                StaffId.Visibility      = Visibility.Collapsed;
                StaffIdDesc.Visibility  = Visibility.Collapsed;
            }
        }

        public void OnLoad( object sender       , RoutedEventArgs e         )   { UpdateLoginText();    }
        private void CheckLogin( object sender  , MouseButtonEventArgs e    )   { App.DoLogout();       }
        private void DoExit( object sender      , MouseButtonEventArgs e    )   { App.Close();          }
        private void DoRestore( object sender   , MouseButtonEventArgs e    )   { App.Restore();        }
        private void DoMinimize( object sender  , MouseButtonEventArgs e    )   { App.Minimize();       }
    }
}
