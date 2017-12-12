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

namespace OverSurgerySystem.UI.Pages.Appointments
{
    /// <summary>
    /// Interaction logic for EditAppointment.xaml
    /// </summary>
    public partial class EditAppointment : EditorPage<Appointment>
    {
        public static DateTime DateAfter    = DatabaseObject.INVALID_DATETIME;
        public static DateTime DateSelected = DatabaseObject.INVALID_DATETIME;

        private MedicalStaff ProtoMedStaff;

        public EditAppointment()
        {
            InitializeComponent();
            Loaded                             += OnLoad;
            PatientIdButton.Click              += StartFindPatient;
            StaffIdButton.Click                += StartFindMedstaff;
            ConfirmButton.Click                += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click                 += (object o, RoutedEventArgs a) => DoCancel();
            ClearCreationDateButton.Click      += (object o, RoutedEventArgs a) => CreationDatePicker.SelectedDate = null;
            ClearAppointmentDateButton.Click   += (object o, RoutedEventArgs a) => AppointmentDatePicker.SelectedDate = null;
            SetIncludeCancelled.Click          += (object o, RoutedEventArgs a) => { IncludeCancelledHeader.Header = "Yes"; CurrentItem.Cancelled = true;   };
            SetExcludeCancelled.Click          += (object o, RoutedEventArgs a) => { IncludeCancelledHeader.Header = "No";  CurrentItem.Cancelled = false;  };
            ProtoMedStaff                       = new MedicalStaff();
            FindPatient.Reset();

            if( IsFind )
            {
                AppointmentTimeText.Visibility  = Visibility.Collapsed;
                AppointmentTimePanel.Visibility = Visibility.Collapsed;
                ConfirmButtonImg.Source         = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text          = "Find";
            }
            else
            {
                IncludeCancelled.Visibility     = Visibility.Collapsed;
                IncludeCancelledText.Visibility = Visibility.Collapsed;
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
            
            EndButton.IsEnabled                 = IsEdit && CurrentItem.Valid;
            ClearCreationDateButton.IsEnabled   = IsFind;
            AppointmentIdBox.IsEnabled          = IsFind;
            CreationDatePicker.IsEnabled        = IsFind;
            DateCreatedText.Text                = IsFind ? "Date After:" : "Date Created:";

            EndText.Foreground  = EndButton.IsEnabled ? Brushes.Black : Brushes.Gray;
        }

        private void StartFindMedstaff( object o , RoutedEventArgs e )
        {
            RecordFields();
            FindStaff.OnSelect  = OnSelectStaff;
            FindStaff.OnFind    = OnFindStaff;
            FindStaff.OnFound   = OnConfirmStaff;
            FindStaff.OnCancel  = () => App.GoToPage( this );
            App.GoToEditStaffPage( ProtoMedStaff , EditStaff.Find | EditStaff.Restricted );
        }

        private void StartFindPatient( object o , EventArgs e )
        {
            RecordFields();
            FindPatient.OnSelect    = OnSelectPatient;
            FindPatient.OnFind      = OnFindPatient;
            FindPatient.OnFound     = OnConfirmPatient;
            FindPatient.OnCancel    = () => App.GoToPage( this );
            App.GoToFindPatientPage();
        }

        public void OnFindPatient( Patient patient )
        {
            List<Patient> list = FindPatient.FindFromPrototype( patient );
            if( list.Count != 1 )
            {
                App.GoToFindPatientResultPage();
                EditPatient.OnConfirm   = OnConfirmPatient;
                EditPatient.OnCancel   = () => App.GoToFindPatientResultPage();;
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
                DateSelected = DatabaseObject.INVALID_DATETIME;
                if( !IsFind )
                {
                    ShowMessage( "Please select a date." );
                    return false;
                }
            }

            DateSelected = CurrentItem.DateAppointed = appointmentDate;
            return true;
        }

        public void HideError()
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
            if( !RecordFields() )
                return;

            if( IsEdit && !IsFind )
            {
                if( CurrentItem.Patient      == null ) { ShowMessage( "Please select a patient." ); return; }
                if( CurrentItem.MedicalStaff == null ) { ShowMessage( "Please select a staff."   ); return; }

                CurrentItem.Save();
                ShowMessage( "Appointment saved." );
                LoadDetails();
            }
            else
            {
                CurrentItem.Id = Appointment.GetIdFromString( AppointmentIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
        }

        private void DoCancel()
        {
            if( CurrentItem.Valid ) CurrentItem.Load();
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += (object sender, RoutedEventArgs e) => App.GoToManageAppointments();
            OnCancel?.Invoke();
        }
    }
}
