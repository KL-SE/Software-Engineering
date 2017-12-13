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

namespace OverSurgerySystem.UI.Pages.MainMenuVariants
{
    /// <summary>
    /// Interaction logic for MenuButtonsReceptionist.xaml
    /// </summary>
    public partial class MenuButtonsReceptionist : Page
    {
        public MenuButtonsReceptionist()
        {
            InitializeComponent();
            ManageAppointmentsButton.Click  += ( object sender , RoutedEventArgs e ) => App.GoToManageAppointments();
            StaffsAvailabilityButton.Click  += ( object sender , RoutedEventArgs e ) => App.GoToStaffsAvailability();
            StaffsOnDutyButton.Click        += ( object sender , RoutedEventArgs e ) => App.GoToStaffsOnDuty();
            ManagePatientsButton.Click      += ( object sender , RoutedEventArgs e ) => App.GoToManagePatients();
            ManagePrescriptionsButton.Click += ( object sender , RoutedEventArgs e ) => App.GoToManagePrescriptions();
            ManageTestResultsButton.Click   += ( object sender , RoutedEventArgs e ) => App.GoToManageTestResults();
        }
    }
}
