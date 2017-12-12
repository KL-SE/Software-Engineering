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

namespace OverSurgerySystem.UI.Pages.Prescriptions
{
    /// <summary>
    /// Interaction logic for EditPrescription.xaml
    /// </summary>
    public partial class EditPrescription : EditorPage<Prescription>
    {
        public static DateTime EndBefore    = DatabaseObject.INVALID_DATETIME;
        public static DateTime StartAfter   = DatabaseObject.INVALID_DATETIME;

        private MedicalStaff ProtoMedStaff;

        public EditPrescription()
        {
            InitializeComponent();
            Loaded                         += OnLoad;
            PatientIdButton.Click          += StartFindPatient;
            PrescriberIdButton.Click       += StartFindMedstaff;
            ConfirmButton.Click            += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click             += (object o, RoutedEventArgs a) => DoCancel();
            ClearStartDateButton.Click     += (object o, RoutedEventArgs a) => StartDatePicker.SelectedDate = null;
            ClearEndDateButton.Click       += (object o, RoutedEventArgs a) => EndDatePicker.SelectedDate = null;
            ProtoMedStaff                   = new MedicalStaff();
            FindPatient.Reset();

            if( IsFind )
            {
                DateCreatedText.Visibility      = Visibility.Collapsed;
                CreationDatePicker.Visibility   = Visibility.Collapsed;
                ConfirmButtonImg.Source         = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text          = "Find";
            }
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            PrescriptionIdBox.Text = !IsEdit ? "" : "- New Prescription -";
            LoadDetails();
        }

        private void LoadDetails()
        {
            if( CurrentItem.Valid               )   PrescriptionIdBox.Text  = CurrentItem.StringId;
            if( CurrentItem.Patient != null     )   PatientIdBox.Text       = String.Format( "{0} - {1}" , CurrentItem.Patient.StringId     , CurrentItem.Patient.Details.FullName      );
            if( CurrentItem.Prescriber != null  )   PrescriberIdBox.Text    = String.Format( "{0} - {1}" , CurrentItem.Prescriber.StringId  , CurrentItem.Prescriber.Details.FullName   );
            if( CurrentItem.Name != null        )   NameBox.Text            = CurrentItem.Name;
            if( CurrentItem.Remark != null      )   RemarkBox.Text          = CurrentItem.Remark;

            if( CurrentItem.CreatedOn != DatabaseObject.INVALID_DATETIME )
            {
                CreationDatePicker.SelectedDate = CurrentItem.CreatedOn;
                CreationDatePicker.Foreground   = Brushes.Black;
            }
            else if( !IsFind )
            {
                CreationDatePicker.Foreground = Brushes.White;
            }
            
            StartDatePicker.SelectedDate  = CurrentItem.StartDate;
            EndDatePicker.SelectedDate    = CurrentItem.EndDate;
            
            EndButton.IsEnabled                 = IsEdit && CurrentItem.Valid;
            PrescriptionIdBox.IsEnabled         = IsFind;
            StartDatePicker.IsEnabled           = IsFind;
            EndDatePicker.IsEnabled             = IsFind;
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
            CurrentItem.Prescriber = ( MedicalStaff )( staff );
            App.GoToPage( this );
        }

        private bool RecordFields()
        {
            CurrentItem.Name    = NameBox.Text;
            CurrentItem.Remark  = RemarkBox.Text;
            EndBefore           = StartDatePicker.SelectedDate != null ? StartDatePicker.SelectedDate.Value : DatabaseObject.INVALID_DATETIME;
            StartAfter          = EndDatePicker.SelectedDate != null ? EndDatePicker.SelectedDate.Value : DatabaseObject.INVALID_DATETIME;
            
            if( IsEdit && StartAfter < DateTime.Now )
            {
                ShowMessage( "Select a valid start date" );
                return false;
            }
            
            return true;
        }

        public void PopulateMedications()
        {
            MedicationsList.Children.RemoveRange( 0 , MedicationsList.Children.Count );
            foreach( Medication med in CurrentItem.Medications )
            {
                StackPanel itemRow  = new StackPanel();
                TextBox itemContent = new TextBox();
                Button deleteItem   = new Button();
                
                itemRow.Orientation     = Orientation.Horizontal;
                itemContent.Text        = med.Name;
                itemContent.Width       = 700;
                itemContent.IsEnabled   = false;
                deleteItem.Content      = "Delete";
                deleteItem.Click       += ( object i , RoutedEventArgs a ) =>
                {
                    CurrentItem.Medications.Remove( med );
                    MedicationsList.Children.Remove( itemRow );
                };
                
                itemRow.Children.Add( itemContent   );
                itemRow.Children.Add( deleteItem    );

                MedicationsList.Children.Add( itemRow );
            }
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
                if( CurrentItem.Patient     == null ) { ShowMessage( "Please select a patient."     ); return; }
                if( CurrentItem.Prescriber  == null ) { ShowMessage( "Please select a prescriber."  ); return; }

                CurrentItem.Save();
                ShowMessage( "Prescription saved." );
                LoadDetails();
            }
            else
            {
                CurrentItem.Id = Prescription.GetIdFromString( PrescriptionIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
        }

        private void DoCancel()
        {
            if( CurrentItem.Valid ) CurrentItem.Load();
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += (object sender, RoutedEventArgs e) => App.GoToManagePrescriptions();
            OnCancel?.Invoke();
        }
    }
}
