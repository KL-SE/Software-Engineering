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
using OverSurgerySystem.UI.Core;

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
                EditAppointment.OnConfirm   = null;
                EditAppointment.OnCancel    = OnCancel;
                if( !Permission.CanAppointOtherStaffs )
                {
                    App.GoToEditAppointmentPage( null , EditAppointment.Edit );
                }
                else
                {
                    App.GoToEditAppointmentPage( null , EditAppointment.Edit | EditAppointment.Restricted );
                }
            };

            FindAppointmentButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindAppointmentPage();
                FindAppointment.OnFind      = OnFindAppointment;
                FindAppointment.OnFound     = null;
                FindAppointment.OnCancel    = OnCancel;
                FindAppointment.OnSelect    = HandleSelectAppointment;
            };
        }
        
        public static void HandleSelectAppointment( Appointment appointment )
        {
            if( Permission.CanAppointOtherStaffs || ( App.LoggedInStaff is MedicalStaff && appointment.MedicalStaff.Id == App.LoggedInStaff.Id ) )
            {
                App.GoToEditAppointmentPage( appointment , EditAppointment.Edit | EditAppointment.Restricted );
            }
            else
            {
                App.GoToEditAppointmentPage( appointment , EditAppointment.View | EditAppointment.BackOnly );
            }
        }
        
        public static void OnCancel()
        {
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += NavigateToMenu;
        }

        public static void NavigateToMenu( object i , RoutedEventArgs a )
        { 
            App.GoToManageAppointments();
            MainMenu.Instance.Loaded -= NavigateToMenu;
        }
        
        public static void OnFindAppointment( Appointment appointment )
        {
            FindAppointment.FindFromPrototype( appointment );
            App.GoToFindAppointmentResultPage();
        }
    }
}
