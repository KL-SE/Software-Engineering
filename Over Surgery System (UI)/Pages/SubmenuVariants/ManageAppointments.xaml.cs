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
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.Appointments;

namespace OverSurgerySystem.UI.Pages
{
    /// <summary>
    /// Interaction logic for ManageAppointments.xaml
    /// </summary>
    public partial class ManageAppointments : Page
    {
        public ManageAppointments()
        {
            InitializeComponent();
            MainMenuButton.Click        += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddAppointmentButton.Click  += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToAddAppointmentPage();
                EditAppointment.OnConfirm   = null;
                EditAppointment.OnCancel    = OnCancel;
            };

            FindAppointmentButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindAppointmentPage();
                FindAppointment.OnFind      = OnFindPatient;
                FindAppointment.OnFound     = null;
                FindAppointment.OnCancel    = OnCancel;
                FindAppointment.OnSelect    = ( Appointment appointment ) => App.GoToEditAppointmentPage( appointment , EditAppointment.View );
            };
        }
        
        public static void OnCancel()
        {
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += ( object i , RoutedEventArgs a ) => App.GoToManageAppointments();
        }
        
        public static void OnFindPatient( Appointment appointment )
        {
            FindAppointment.FindFromPrototype( appointment );
            App.GoToFindAppointmentResultPage();
        }
    }
}
