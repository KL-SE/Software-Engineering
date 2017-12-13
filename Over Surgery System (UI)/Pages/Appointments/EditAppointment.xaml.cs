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
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Manager;
using OverSurgerySystem.UI.Pages.Common;
using OverSurgerySystem.UI.Pages.Address;
using OverSurgerySystem.UI.Pages.Patients;
using OverSurgerySystem.UI.Pages.Staffs;
using OverSurgerySystem.Core.Staffs;
using System.IO;
using OverSurgerySystem.UI.Pages.Core;
using OverSurgerySystem.UI.Core;

namespace OverSurgerySystem.UI.Pages.Appointments
{
    /// <summary>
    /// Interaction logic for EditAppointment.xaml
    /// </summary>
    public partial class EditAppointment : EditorPage<Appointment>
    {
        public static DateTime DateAfter    = DatabaseObject.INVALID_DATETIME;
        public static DateTime SelectedDate = DatabaseObject.INVALID_DATETIME;

        private MedicalStaff ProtoMedStaff;

        public EditAppointment()
        {
            InitializeComponent();
            Loaded                             += OnLoad;
            PatientIdButton.Click              += StartFindPatient;
            StaffIdButton.Click                += StartFindMedstaff;
            ConfirmButton.Click                += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click                 += (object o, RoutedEventArgs a) => DoCancel();
            EndButton.Click                    += (object o, RoutedEventArgs a) => DoCancelAppointment();
            ClearCreationDateButton.Click      += (object o, RoutedEventArgs a) => CreationDatePicker.SelectedDate = null;
            ClearAppointmentDateButton.Click   += (object o, RoutedEventArgs a) => AppointmentDatePicker.SelectedDate = null;
            SetIncludeCancelled.Click          += (object o, RoutedEventArgs a) => { IncludeCancelledHeader.Header = "Yes"; CurrentItem.Cancelled = true;  HideMessage();   };
            SetExcludeCancelled.Click          += (object o, RoutedEventArgs a) => { IncludeCancelledHeader.Header = "No";  CurrentItem.Cancelled = false; HideMessage();   };
            ClearPatientButton.Click           += (object o, RoutedEventArgs a) => { PatientIdBox.Text  = ""; CurrentItem.Patient       = null; HideMessage();              };
            ClearStaffButton.Click             += (object o, RoutedEventArgs a) => { StaffIdBox.Text    = ""; CurrentItem.MedicalStaff  = null; HideMessage();              };
            ProtoMedStaff                       = new MedicalStaff();
            FindPatient.Reset();
            
            AppointmentIdBox.TextChanged               += (object o, TextChangedEventArgs e) => HideMessage();
            RemarkBox.TextChanged                      += (object o, TextChangedEventArgs e) => HideMessage();
            TimeHourBox.TextChanged                    += (object o, TextChangedEventArgs e) => HideMessage();
            TimeMinBox.TextChanged                     += (object o, TextChangedEventArgs e) => HideMessage();
            CreationDatePicker.SelectedDateChanged     += (object o, SelectionChangedEventArgs e) => HideMessage();
            AppointmentDatePicker.SelectedDateChanged  += (object o, SelectionChangedEventArgs e) => HideMessage();

            if( IsFind )
            {
                IncludeCancelled.Visibility     = Visibility.Visible;
                IncludeCancelledText.Visibility = Visibility.Visible;
                AppointmentTimeText.Visibility  = Visibility.Collapsed;
                AppointmentTimePanel.Visibility = Visibility.Collapsed;

                ConfirmButtonImg.Source         = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text          = "Find";
            }

            if( IsView )
            {
                AppointmentIdBox.IsEnabled              = false;
                PatientIdBox.IsEnabled                  = false;
                StaffIdBox.IsEnabled                    = false;
                RemarkBox.IsEnabled                     = false;
                CreationDatePicker.IsEnabled            = false;
                ClearCreationDateButton.IsEnabled       = false;
                AppointmentDatePicker.IsEnabled         = false;
                ClearAppointmentDateButton.IsEnabled    = false;
                TimeHourBox.IsEnabled                   = false;
                TimeMinBox.IsEnabled                    = false;
                TimeMPicker.IsEnabled                   = false;
                ClearStaffButton.IsEnabled              = false;
                ClearPatientButton.IsEnabled            = false;

                PatientIdButton.Content     = "View";
                StaffIdButton.Content       = "View";
            }

            if( CurrentItem.Cancelled )
            {
                EndImg.Visibility   = Visibility.Collapsed;
                EndText.Text        = "Cancelled";
            }
            else if( CurrentItem.Ended )
            {
                EndImg.Visibility   = Visibility.Collapsed;
                EndText.Text        = "Completed";
            }

            if( IsRestricted )
            {
                PatientIdBox.IsEnabled              = false;
                StaffIdBox.IsEnabled                = false;
                CreationDatePicker.IsEnabled        = false;
                ClearCreationDateButton.IsEnabled   = false;

                if( !Permission.CanAppointOtherStaffs )
                {
                    StaffIdButton.Content       = "View";
                    ClearStaffButton.IsEnabled  = false;
                }
                
                if( CurrentItem.Valid )
                {
                    PatientIdButton.Content         = "View";
                    ClearPatientButton.IsEnabled    = false;
                }
            }

            if( IsBackOnly )
            {
                ConfirmButton.Visibility    = Visibility.Collapsed;
                CancelButtonImg.Source      = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/main_menu.png" ) );
                CancelButtonText.Text       = "Back";
            }

            if( IsNoBack )
            {
                CancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            AppointmentIdBox.Text = !IsEdit ? "" : "- New Appointment -";
            LoadDetails();
        }

        private void LoadDetails()
        {
            if( CurrentItem.Valid                   )   AppointmentIdBox.Text   = CurrentItem.StringId;
            if( CurrentItem.Patient != null         )   PatientIdBox.Text       = String.Format( "{0} - {1}" , CurrentItem.Patient.StringId      , CurrentItem.Patient.Details.FullName       );
            if( CurrentItem.MedicalStaff != null    )   StaffIdBox.Text         = String.Format( "{0} - {1}" , CurrentItem.MedicalStaff.StringId , CurrentItem.MedicalStaff.Details.FullName  );
            if( CurrentItem.Remark != null          )   RemarkBox.Text          = CurrentItem.Remark;
            
            // Don't let the date from setting to the current date in find mode.
            // It's not a proper solution. The date would get reset each time the user search again instead of persisting.
            if( !IsFind )
            {
                // Dates are non-nullable, so they can be set without checking.
                AppointmentDatePicker.SelectedDate = CurrentItem.DateAppointed;

                int hour    = CurrentItem.DateAppointed.Hour;
                int minutes = CurrentItem.DateAppointed.Minute;

                if( CurrentItem.DateAppointed.Hour >= 12 )
                {
                    TimeHourBox.Text            = ( hour > 12 ? hour - 12 :hour ).ToString();
                    TimeMinBox.Text             = minutes.ToString();
                    TimeMPickerHeader.Header    = "P.M.";
                }
                else if( CurrentItem.DateAppointed.Hour == 0 )
                {
                    TimeHourBox.Text            = "12";
                    TimeMinBox.Text             = minutes.ToString();
                    TimeMPickerHeader.Header    = "A.M.";
                }
                else
                {
                    TimeHourBox.Text            = hour.ToString();
                    TimeMinBox.Text             = minutes.ToString();
                    TimeMPickerHeader.Header    = "A.M.";
                }
            }

            if( CurrentItem.CreatedOn != DatabaseObject.INVALID_DATETIME )
            {
                CreationDatePicker.SelectedDate = CurrentItem.CreatedOn;
                CreationDatePicker.Foreground   = Brushes.Black;
            }
            else if( !IsFind )
            {
                CreationDatePicker.Foreground = Brushes.White;
            }
            
            EndButton.IsEnabled                 = IsEdit && CurrentItem.Valid && ( CurrentItem.MedicalStaff.Id == App.LoggedInStaff.Id || Permission.CanAppointOtherStaffs );
            ClearCreationDateButton.IsEnabled   = IsFind;
            AppointmentIdBox.IsEnabled          = IsFind;
            CreationDatePicker.IsEnabled        = IsFind;
            DateCreatedText.Text                = IsFind ? "Date After:" : "Date Created:";

            EndText.Foreground  = EndButton.IsEnabled ? Brushes.Black : Brushes.Gray;
        }

        private void StartFindMedstaff( object o , RoutedEventArgs e )
        {
            if( IsView || ( IsRestricted && ( CurrentItem.Valid || !Permission.CanAppointOtherStaffs ) ) )
            {
                EditStaff.OnCancel  = () => App.GoToPage( this );
                App.GoToEditStaffPage( CurrentItem.MedicalStaff , EditStaff.View | EditStaff.BackOnly );
            }
            else
            {
                RecordFields();
                FindStaff.OnSelect              = OnSelectStaff;
                FindStaff.OnFind                = OnFindStaff;
                FindStaff.OnFound               = OnConfirmStaff;
                FindStaff.OnCancel              = () => App.GoToPage( this );
                EditStaff.RestrictActive        = true;
                EditStaff.RestrictAdmin         = true;
                EditStaff.RestrictReceptionist  = true;
                App.GoToEditStaffPage( ProtoMedStaff , EditStaff.Find | EditStaff.Restricted );
            }
        }

        private void StartFindPatient( object o , EventArgs e )
        {
            if( IsView || ( IsRestricted && CurrentItem.Valid ) )
            {
                EditPatient.OnCancel = () => App.GoToPage( this );
                App.GoToEditPatientPage( CurrentItem.Patient , EditPatient.View | EditPatient.BackOnly );
            }
            else
            {
                RecordFields();
                FindPatient.OnSelect    = OnSelectPatient;
                FindPatient.OnFind      = OnFindPatient;
                FindPatient.OnFound     = OnConfirmPatient;
                FindPatient.OnCancel    = () => App.GoToPage( this );
                App.GoToFindPatientPage();
            }
        }

        public void OnFindPatient( Patient patient )
        {
            List<Patient> list = FindPatient.FindFromPrototype( patient );
            if( list.Count != 1 )
            {
                App.GoToFindPatientResultPage();
                EditPatient.OnConfirm   = OnConfirmPatient;
                EditPatient.OnCancel    = () => App.GoToFindPatientResultPage();;
            }
            else
            {
                OnSelectPatient( list[0] );
                EditPatient.OnConfirm = OnConfirmPatient;
            }
        }

        public void OnFindStaff( Staff staff )
        {
            List<Staff> list = FindStaff.FindFromPrototype( staff );
            if( list.Count != 1 )
            {
                App.GoToFindStaffResultPage();
                EditStaff.OnConfirm = OnConfirmStaff;
                EditStaff.OnCancel  = () => App.GoToFindStaffResultPage();
            }
            else
            {
                OnSelectStaff( list[0] );
                EditStaff.OnConfirm = OnConfirmStaff;
            }
        }

        public void OnSelectStaff( Staff staff )
        {
            App.GoToEditStaffPage( staff , EditStaff.View );
        }

        public void OnSelectPatient( Patient patient )
        {
            App.GoToEditPatientPage( patient , EditPatient.View );
        }

        public void OnConfirmPatient( Patient patient )
        {
            CurrentItem.Patient = patient;
            App.GoToPage( this );
        }

        public void OnConfirmStaff( Staff staff )
        {
            CurrentItem.MedicalStaff = ( MedicalStaff )( staff );
            App.GoToPage( this );
        }

        private bool RecordFields()
        {
            CurrentItem.Remark          = RemarkBox.Text;
            DateAfter                   = CreationDatePicker.SelectedDate != null ? CreationDatePicker.SelectedDate.Value : DatabaseObject.INVALID_DATETIME;
            DateTime appointmentDate    = DateTime.MinValue;

            if( AppointmentDatePicker.SelectedDate != null )
            {
                appointmentDate = AppointmentDatePicker.SelectedDate.Value;
                try
                {
                    int hour    = Int32.Parse( TimeHourBox.Text );
                    int minutes = Int32.Parse( TimeMinBox.Text  );
                    TimeSpan ts;

                    if( TimeMPickerHeader.Header.Equals( "P.M." ) )
                    {
                        ts = new TimeSpan( hour < 12 ? hour + 12 : hour , minutes , 0 );
                    }
                    else
                    {
                        ts = new TimeSpan( hour == 0 ? 12 : hour , minutes , 0 );
                    }

                    appointmentDate = appointmentDate.Date + ts;
                }
                catch
                {
                    ShowMessage( "Please enter a valid time." );
                    return false;
                }

                if( IsEdit && appointmentDate < DateTime.Now )
                {
                    ShowMessage( "Appointment cannot be earlier than the current date & time." );
                    return false;
                }
            }
            else
            {
                SelectedDate = DatabaseObject.INVALID_DATETIME;
                if( !IsFind )
                {
                    ShowMessage( "Please select a date." );
                    return false;
                }
            }

            SelectedDate = CurrentItem.DateAppointed = appointmentDate;
            return true;
        }

        public void HideMessage()
        {
            MsgBox.Visibility = Visibility.Collapsed;
        }

        public void ShowMessage( string error_message )
        {
            MsgBox.Visibility = Visibility.Visible;
            MsgBox.Text       = error_message;
        }

        private void DoConfirm()
        {
            if( !IsView && !RecordFields() )
                return;

            if( IsEdit )
            {
                try
                {
                    if( CurrentItem.Patient      == null ) { ShowMessage( "Please select a patient." ); return; }
                    if( CurrentItem.MedicalStaff == null ) { ShowMessage( "Please select a staff."   ); return; }
                    else 
                    {
                        // We will not restrict the ability for staffs to make appointments on dates which they are not on duty.
                        // The only restriction we will place is when the staff is on leave.
                        if( CurrentItem.MedicalStaff.IsOnLeave( CurrentItem.DateAppointed ) )
                        {
                            ShowMessage( "The staff is on leave at the set date. Choose another date." );
                            return;
                        }
                    }

                    CurrentItem.Save();
                    LoadDetails();
                    ShowMessage( "Appointment saved." );
                }
                catch
                {
                    ShowMessage( "Failed to load data. Please check your connection." );
                }
            }
            else if( IsFind )
            {
                CurrentItem.Id = Appointment.GetIdFromString( AppointmentIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
        }

        private void DoCancel()
        {
            try
            {
                if( CurrentItem.Valid )
                {
                    CurrentItem.Load();
                }
            }
            catch
            {
                ShowMessage( "Failed to load data. Please check your connection." );
            }
            OnCancel?.Invoke();
        }

        private void DoCancelAppointment()
        {
            try
            {
                // Refresh the current item so we don't save a faulty item.
                App.GoToPage( new EndAppointment() );
                EndAppointment.OnCancel     = () => App.GoToPage( this );
                EndAppointment.OnConfirm    = () =>
                {
                    CurrentItem.Load();
                    CurrentItem.Cancelled = true;
                    CurrentItem.Save();

                    Mode = View | BackOnly;
                    App.GoToPage( new EditAppointment() );
                };
            }
            catch
            {
                ShowMessage( "Failed to load data. Please check your connection." );
            }
        }
    }
}
