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
            Loaded += OnLoad;

            LoginButton.Click += DoLogin;
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
            App.SetTitle( "Log In" );
        }

        private void DoLogin( object sender , RoutedEventArgs e )
        {
            App.DoLogin( IdField.Text , null );
        }
    }
}
