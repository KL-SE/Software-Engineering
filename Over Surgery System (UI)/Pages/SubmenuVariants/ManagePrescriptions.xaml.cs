using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.Prescriptions;
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
    /// Interaction logic for ManagePrescriptions.xaml
    /// </summary>
    public partial class ManagePrescriptions : Page
    {
        public ManagePrescriptions()
        {
            InitializeComponent();
            MainMenuButton.Click += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddPrescriptionButton.Click  += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToAddPrescriptionPage();
                EditPrescription.OnConfirm   = null;
                EditPrescription.OnCancel    = OnCancel;
            };

            FindPrescriptionButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindPrescriptionPage();
                FindPrescription.OnFind      = OnFindPatient;
                FindPrescription.OnFound     = null;
                FindPrescription.OnCancel    = OnCancel;
                FindPrescription.OnSelect    = ( Prescription prescription ) => App.GoToEditPrescriptionPage( prescription , EditPrescription.View );
            };
        }
        
        public static void OnCancel()
        {
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += ( object i , RoutedEventArgs a ) => App.GoToManagePrescriptions();
        }
        
        public static void OnFindPatient( Prescription prescription )
        {
            FindPrescription.FindFromPrototype( prescription );
            App.GoToFindPrescriptionResultPage();
        }
    }
}
