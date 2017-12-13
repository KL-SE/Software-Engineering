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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public static MainMenu instance;
        public static MainMenu Instance
        {
            private set
            {
                instance = value;
            }
            get
            {
                if( instance == null )
                    instance = new MainMenu();

                return instance;
            }
        }

        static MainMenu() { }
        private MainMenu()
        {
            InitializeComponent();
            Loaded                  += OnLoad;
            MenuButtons.Navigating  += App.PreventNavigation;
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
            App.SetTitle( "Main Menu" );
            App.GoToMainMenu();
        }
    }
}
