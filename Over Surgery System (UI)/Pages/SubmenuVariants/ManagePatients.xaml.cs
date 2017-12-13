using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.Patients;
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

namespace OverSurgerySystem.UI.Pages
{
    /// <summary>
    /// Interaction logic for ManagePatients.xaml
    /// </summary>
    public partial class ManagePatients : Page
    {
        public ManagePatients()
        {
            InitializeComponent();
            MainMenuButton.Click    += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddPatientButton.Click  += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToAddPatientPage();
                EditPatient.OnConfirm   = null;
                EditPatient.OnCancel    = OnCancel;
                App.SetTitle( "Manage Patients | Add" );
            };

            FindPatientButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindPatientPage();
                FindPatient.OnFind      = OnFindPatient;
                FindPatient.OnFound     = null;
                FindPatient.OnCancel    = OnCancel;
                FindPatient.OnSelect    = ( Patient patient ) =>
                {
                    App.GoToEditPatientPage( patient , EditPatient.Edit );
                    App.SetTitle( "Manage Patients | Edit" );
                };

                App.SetTitle( "Manage Patients | Find" );
            };

            App.SetTitle( "Manage Patients" );
        }
        
        public static void OnCancel()
        {
            EditPatient.Reset();
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += NavigateToMenu;
        }

        public static void NavigateToMenu( object i , RoutedEventArgs a )
        { 
            App.GoToManagePatients();
            MainMenu.Instance.Loaded -= NavigateToMenu;
        }
        
        public static void OnFindPatient( Patient patient )
        {
            FindPatient.FindFromPrototype( patient );
            App.GoToFindPatientResultPage();
        }
    }
}
