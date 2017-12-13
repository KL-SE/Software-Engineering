using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.UI.Core;
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
            Loaded                      += OnLoad;
            MainMenuButton.Click        += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddPrescriptionButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToEditAppointmentPage( null , EditPrescription.Edit | EditPrescription.Restricted );
                EditPrescription.OnConfirm   = null;
                EditPrescription.OnCancel    = OnCancel;
            };

            FindPrescriptionButton.Click += HandleFindPrescription;
        }

        public void OnLoad( object sender , EventArgs e )
        {
            if( !Permission.CanPrescribePatients )
            {
                // The user doesn't have any other option than to find prescription.
                // Thus, redirect them to the find prescriptions page.
                HandleFindPrescription( null , null );
            }
        }

        public static void HandleFindPrescription( object sender , RoutedEventArgs e )
        {
            App.GoToFindPrescriptionPage();
            FindPrescription.OnFind   = OnFindPrescription;
            FindPrescription.OnFound  = null;
            FindPrescription.OnCancel = OnCancel;
            FindPrescription.OnSelect = HandlePrescriptionSelect;
        }
        
        public static void HandlePrescriptionSelect( Prescription prescription )
        {
            if( Permission.CanPrescribePatients || ( App.LoggedInStaff is MedicalStaff && prescription.Prescriber.Id == App.LoggedInStaff.Id ) )
            {
                App.GoToEditPrescriptionPage( prescription , EditPrescription.Edit | EditPrescription.Restricted );
            }
            else
            {
                App.GoToEditPrescriptionPage( prescription , EditPrescription.View | EditPrescription.BackOnly );
            }
        }
        
        public static void OnCancel()
        {
            App.GoToMainMenu();
            if( Permission.CanPrescribePatients )
            {
                MainMenu.Instance.Loaded += NavigateToMenu;
            }
        }

        public static void NavigateToMenu( object i , RoutedEventArgs a )
        { 
            App.GoToManagePrescriptions();
            MainMenu.Instance.Loaded -= NavigateToMenu;
        }
        
        public static void OnFindPrescription( Prescription prescription )
        {
            FindPrescription.FindFromPrototype( prescription );
            App.GoToFindPrescriptionResultPage();
        }
    }
}
