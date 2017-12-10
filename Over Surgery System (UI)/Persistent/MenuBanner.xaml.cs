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

namespace OverSurgerySystem.UI.Persistent
{
    /// <summary>
    /// Interaction logic for MenuBanner.xaml
    /// </summary>
    public partial class MenuBanner : Page
    {
        public MenuBanner()
        {
            InitializeComponent();
            Loaded += OnLoad;

            LogoutButton.MouseLeftButtonDown += new MouseButtonEventHandler( DoLogout );
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
        }

        private void DoLogout( object sender , MouseButtonEventArgs e )
        {
            Welcome.Text            = "Welcome!";
            PageName.Text           = "Sample Page";
            Username.Visibility     = Visibility.Collapsed;
            StaffId.Visibility      = Visibility.Collapsed;
            StaffIdDesc.Visibility  = Visibility.Collapsed;
            LogoutButton.Text       = "Log In";
        }
    }
}
