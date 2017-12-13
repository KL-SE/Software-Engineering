using OverSurgerySystem.UI.Core;
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
using OverSurgerySystem.UI.Pages.MedicalStaffs;

namespace OverSurgerySystem.UI.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuButtons.xaml
    /// </summary>
    public partial class MainMenuButtons : Page
    {
        public MainMenuButtons()
        {
            InitializeComponent();
            ManageAppointmentsButton.Click  += ( object sender , RoutedEventArgs e ) => App.GoToManageAppointments();
            StaffsAvailabilityButton.Click  += ( object sender , RoutedEventArgs e ) => { EditAvailableDate.SelectedLeaveDate   = null; App.GoToStaffsAvailability();   };
            StaffsOnDutyButton.Click        += ( object sender , RoutedEventArgs e ) => { EditWorkingDay.InEditMode             = false; App.GoToStaffsOnDuty();        };
            ManageStaffsButton.Click        += ( object sender , RoutedEventArgs e ) => App.GoToManageStaffs();
            ManagePatientsButton.Click      += ( object sender , RoutedEventArgs e ) => App.GoToManagePatients();
            ManagePrescriptionsButton.Click += ( object sender , RoutedEventArgs e ) => App.GoToManagePrescriptions();
            ManageTestResultsButton.Click   += ( object sender , RoutedEventArgs e ) => App.GoToManageTestResults();

            if( !Permission.CanPrescribePatients )
            {
                ManagePrescriptionsImg.Source   = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/find_prescription.png" ) );
                ManagePrescriptionsText.Text    = "Find Prescription";
            }
            
            if( !Permission.CanAddTestResults )
            {
                ManageTestResultsImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/find_prescription.png" ) );
                ManageTestResultsText.Text  = "Find Test Result";
            }
        }
    }
}
