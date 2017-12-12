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
                EditPatient.OnCancel   = OnCancel;
            };

            FindPatientButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindPatientPage();
                FindPatient.OnFind      = OnFindPatient;
                FindPatient.OnFound     = null;
                FindPatient.OnCancel    = OnCancel;
                FindPatient.OnSelect    = ( Patient patient ) => App.GoToEditPatientPage( patient , EditPatient.View );
            };
        }
        
        public static void OnCancel()
        {
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += ( object i , RoutedEventArgs a ) => App.GoToManagePatients();
        }
        
        public static void OnFindPatient( Patient patient )
        {
            FindPatient.FindFromPrototype( patient );
            App.GoToFindPatientResultPage();
        }
    }
}
