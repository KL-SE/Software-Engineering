using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.UI.Pages;
using OverSurgerySystem.UI.Pages.Common;
using OverSurgerySystem.UI.Pages.Staffs;
using OverSurgerySystem.UI.Persistent;
using OverSurgerySystem.UI.Pages.Patients;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.TestResults;
using OverSurgerySystem.UI.Pages.Appointments;
using OverSurgerySystem.UI.Pages.Prescriptions;
using OverSurgerySystem.UI.Pages.MedicalStaffs;
using OverSurgerySystem.UI.Core;
using OverSurgerySystem.Manager;

namespace OverSurgerySystem.UI
{
    /// <summary>
    /// Interaction logic for xaml
    /// </summary>
    public partial class App : Application
    {
        public static Staff LoggedInStaff;
        public static MainWindow CurrentWindow
        {
            get
            {
                return Current.MainWindow as MainWindow;
            }
        }

        public static Frame MainPage
        {
            get
            {
                return CurrentWindow.MainPage;
            }
        }
        
        // This delegated function is used to ensure that the user cannot use Mouse 4, Backspace, or other shortcuts to go to the previous page and possibly skip security checks.
        // The work here is not done. Frames that needs to prevent back-navigation still have to subscribe to this delegated function.
        public static void PreventNavigation( object sender , NavigatingCancelEventArgs e )
        {
            if( e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back )
            {
                e.Cancel = true;
            }
        }
        
        public static void GoToPage( Page destination )                 { MainPage.Navigate( destination );                         }
        public static void GoToMenu( Page destination )                 { MainMenu.Instance.MenuButtons.Navigate( destination );    }
        public static void GoToManageAppointments()                     { GoToMenu( new ManageAppointments() );                     }
        public static void GoToStaffsAvailability()                     { GoToMenu( new EditAvailableDate() );                      }
        public static void GoToStaffsOnDuty()                           { GoToMenu( new EditWorkingDay() );                         }
        public static void GoToManageStaffs()                           { GoToMenu( new ManageStaffs() );                           }
        public static void GoToManagePatients()                         { GoToMenu( new ManagePatients() );                         }
        public static void GoToManagePrescriptions()                    { GoToMenu( new ManagePrescriptions() );                    }
        public static void GoToManageTestResults()                      { GoToMenu( new ManageTestResults() );                      }
        public static void GoToLoginPage()                              { GoToPage( new LoginPage() );                              }
        public static void GoToFindStaffResultPage()                    { GoToPage( new FindStaff() );                              }
        public static void GoToFindPatientResultPage()                  { GoToPage( new FindPatient() );                            }
        public static void GoToFindTestResultListPage()                 { GoToPage( new FindTestResult() );                         }
        public static void GoToFindAppointmentResultPage()              { GoToPage( new FindAppointment() );                        }
        public static void GoToFindPrescriptionResultPage()             { GoToPage( new FindPrescription() );                       }

        public static void GoToAddStaffPage()                               { GoToEditStaffPage( null           , EditStaff.Edit        ); }
        public static void GoToFindStaffPage()                              { GoToEditStaffPage( null           , EditStaff.Find        ); }
        public static void GoToFindStaffPage( Staff proto )                 { GoToEditStaffPage( proto          , EditStaff.Find        ); }
        public static void GoToAddPatientPage()                             { GoToEditPatientPage( null         , EditPatient.Edit      ); }
        public static void GoToFindPatientPage()                            { GoToEditPatientPage( null         , EditPatient.Find      ); }
        public static void GoToFindPatientPage( Patient proto )             { GoToEditPatientPage( proto        , EditPatient.Find      ); }
        public static void GoToAddTestResultPage()                          { GoToEditTestResultPage( null      , EditTestResult.Edit   ); }
        public static void GoToFindTestResultPage()                         { GoToEditTestResultPage( null      , EditTestResult.Find   ); }
        public static void GoToFindTestResultPage( TestResult proto )       { GoToEditTestResultPage( proto     , EditTestResult.Find   ); }
        public static void GoToAddAppointmentPage()                         { GoToEditAppointmentPage( null     , EditAppointment.Edit  ); }
        public static void GoToFindAppointmentPage()                        { GoToEditAppointmentPage( null     , EditAppointment.Find  ); }
        public static void GoToFindAppointmentPage( Appointment proto )     { GoToEditAppointmentPage( proto    , EditAppointment.Find  ); }
        public static void GoToAddPrescriptionPage()                        { GoToEditPrescriptionPage( null    , EditPrescription.Edit ); }
        public static void GoToFindPrescriptionPage()                       { GoToEditPrescriptionPage( null    , EditPrescription.Find ); }
        public static void GoToFindPrescriptionPage( Prescription proto )   { GoToEditPrescriptionPage( proto   , EditPrescription.Find ); }

        public static void GoToEditStaffPage( Staff staff , int mode )
        {
            EditStaff.Setup( staff != null ? staff : new Receptionist() , mode );
            if( EditStaff.IsFind )
            {
                EditStaff.OnConfirm = FindStaff.OnInitialFind;
                EditStaff.OnCancel  = FindStaff.OnInitialCancel;
            }
            GoToPage( new EditStaff() );
        }

        public static void GoToEditPatientPage( Patient patient , int mode )
        {
            EditPatient.Setup( patient != null ? patient : new Patient() , mode );
            if( EditPatient.IsFind )
            {
                EditPatient.OnConfirm   = FindPatient.OnInitialFind;
                EditPatient.OnCancel    = FindPatient.OnInitialCancel;
            }
            GoToPage( new EditPatient() );
        }

        public static void GoToEditTestResultPage( TestResult test , int mode )
        {
            // For test, appointments and prescriptions, the staff field will be auto filled with the current staff ID.
            TestResult arg = test == null ? new TestResult() : test;
            EditTestResult.Setup( arg , mode );
            if( EditTestResult.IsFind )
            {
                EditTestResult.OnConfirm    = FindTestResult.OnInitialFind;
                EditTestResult.OnCancel     = FindTestResult.OnInitialCancel;
            }
            else if( !arg.Valid && !Permission.HaveAdminPrivilege && LoggedInStaff is MedicalStaff )
            {
                arg.MedicalLicenseNo = ( ( MedicalStaff ) LoggedInStaff ).LicenseNo;
            }
            GoToPage( new EditTestResult() );
        }

        public static void GoToEditAppointmentPage( Appointment appointment , int mode )
        {
            // Add one our to the DateAppointed, which by default would be DateTime.Now
            // This is just to make sure that there's a logical timeframe to the appointment when the user forgets to change the date or time.
            Appointment arg = appointment;
            if( appointment == null )
            {
                arg                 = new Appointment();
                arg.DateAppointed   = DateTime.Now + new TimeSpan( 1 , 0 , 0 );
            }

            EditAppointment.Setup( arg , mode );
            if( EditAppointment.IsFind )
            {
                EditAppointment.OnConfirm   = FindAppointment.OnInitialFind;
                EditAppointment.OnCancel    = FindAppointment.OnInitialCancel;
            }
            else if( !arg.Valid && !Permission.HaveAdminPrivilege && LoggedInStaff is MedicalStaff )
            {
                arg.MedicalStaff = ( MedicalStaff )( LoggedInStaff );
            }
            GoToPage( new EditAppointment() );
        }

        public static void GoToEditPrescriptionPage( Prescription prescription , int mode )
        {
            // Similarly, prescription should have some time spacing too.
            Prescription arg = prescription;
            if( prescription == null )
            {
                arg             = new Prescription();
                arg.StartDate   = DateTime.Now.Date;
                arg.EndDate     = DateTime.Now.Date.AddDays( 1 );
            }

            EditPrescription.Setup( arg , mode );
            if( EditPrescription.IsFind )
            {
                EditPrescription.OnConfirm   = FindPrescription.OnInitialFind;
                EditPrescription.OnCancel    = FindPrescription.OnInitialCancel;
            }
            else if ( !arg.Valid && !Permission.HaveAdminPrivilege && LoggedInStaff is MedicalStaff )
            {
                arg.Prescriber = ( MedicalStaff )( LoggedInStaff );
            }
            GoToPage( new EditPrescription() );
        }

        public static void GoToMainMenu()
        {
            GoToPage( MainMenu.Instance );
            if( LoggedInStaff != null )
            {
                MainMenu.Instance.MenuButtons.Navigate( new MainMenuButtons() );
            }
            else
            {
                DoLogout();
            }
        }
        
        public static void SetTitle( string title )
        {
            MenuBanner.Instance.PageName.Text   = title;
            CurrentWindow.Title                 = title;
        }

        public static void DoLogin( string str_id , string password )
        {
            Staff staff = StaffsManager.GetStaffWithIdAndPassword( str_id , password );
            DoLogin( staff );
        }

        public static void DoLogin( Staff staff )
        {
            LoggedInStaff = staff;
            GoToMainMenu();
            MenuBanner.Instance.UpdateLoginText();
        }

        public static bool HaveAdminAcount()
        {
            List<Receptionist> adminStaffs = StaffsManager.GetAdminReceptionists();
            return adminStaffs.Count > 0;
        }

        public static void DoLogout()
        {
            GoToPage( new LoginPage() );
            
            LoggedInStaff = null;
            MenuBanner.Instance.UpdateLoginText();
        }

        // There is no staff in the system. Allow the user to create a staff.
        public static void DoLastResortLogin()
        {
            Receptionist newAdmin = new Receptionist() { Admin = true };
            
            EditStaff.RestrictGP            = true;
            EditStaff.RestrictNurse         = true;
            EditStaff.RestrictReceptionist  = true;
            EditStaff.OnCancel              = null;
            EditStaff.OnConfirm             = (Staff staff) =>
            {
                DoLogin( staff );
                EditStaff.RestrictGP            = false;
                EditStaff.RestrictNurse         = false;
                EditStaff.RestrictReceptionist  = false;
            };

            GoToEditStaffPage( newAdmin , EditStaff.Edit | EditStaff.Restricted | EditStaff.NoBack );
        }

        public static void Close()      { CurrentWindow.Close();                            }
        public static void Restore()    { CurrentWindow.WindowState = WindowState.Normal;   }
        public static void Minimize()
        {
            CurrentWindow.WindowState = WindowState.Normal;
            CurrentWindow.WindowState = WindowState.Minimized;
        }
    }
}
