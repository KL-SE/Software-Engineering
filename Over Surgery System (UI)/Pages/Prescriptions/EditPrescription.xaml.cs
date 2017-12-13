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
            AddMedicationButton.Click      += StartFindMedication;
            EndButton.Click                += (object o, RoutedEventArgs a) => DoEndPrescription();
            ConfirmButton.Click            += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click             += (object o, RoutedEventArgs a) => DoCancel();
            ClearStartDateButton.Click     += (object o, RoutedEventArgs a) => StartDatePicker.SelectedDate = null;
            ClearEndDateButton.Click       += (object o, RoutedEventArgs a) => EndDatePicker.SelectedDate = null;
            ClearPatientButton.Click       += (object o, RoutedEventArgs a) => { PatientIdBox.Text      = ""; CurrentItem.Patient       = null; HideMessage(); };
            ClearPrescriberButton.Click    += (object o, RoutedEventArgs a) => { PrescriberIdBox.Text   = ""; CurrentItem.Prescriber    = null; HideMessage(); };
            ProtoMedStaff                   = new MedicalStaff();
            FindPatient.Reset();

            PrescriptionIdBox.TextChanged          += (object o, TextChangedEventArgs e) => HideMessage();
            NameBox.TextChanged                    += (object o, TextChangedEventArgs e) => HideMessage();
            RemarkBox.TextChanged                  += (object o, TextChangedEventArgs e) => HideMessage();
            DateCreatedBox.TextChanged             += (object o, TextChangedEventArgs e) => HideMessage();
            StartDatePicker.SelectedDateChanged    += (object o, SelectionChangedEventArgs e) => HideMessage();
            EndDatePicker.SelectedDateChanged      += (object o, SelectionChangedEventArgs e) => HideMessage();

            if( IsFind )
            {
                DateCreatedText.Visibility  = Visibility.Collapsed;
                DateCreatedBox.Visibility   = Visibility.Collapsed;
                ConfirmButtonImg.Source     = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text      = "Find";
            }

            if( IsView )
            {
                PrescriptionIdBox.IsEnabled     = false;
                PatientIdBox.IsEnabled          = false;
                PrescriberIdBox.IsEnabled       = false;
                NameBox.IsEnabled               = false;
                RemarkBox.IsEnabled             = false;
                DateCreatedBox.IsEnabled        = false;
                StartDatePicker.IsEnabled       = false;
                ClearStartDateButton.IsEnabled  = false;
                EndDatePicker.IsEnabled         = false;
                ClearEndDateButton.IsEnabled    = false;
                AddMedicationButton.IsEnabled   = false;
                ClearPrescriberButton.IsEnabled = false;
                ClearPatientButton.IsEnabled    = false;

                PatientIdButton.Content         = "View";
                PrescriberIdButton.Content      = "View";
            }

            if( IsRestricted )
            {
                PrescriptionIdBox.IsEnabled     = false;
                PatientIdBox.IsEnabled          = false;
                DateCreatedBox.IsEnabled        = false;
                PrescriberIdBox.IsEnabled       = false;
                PrescriberIdButton.Content      = "View";
                ClearPrescriberButton.IsEnabled = false;

                if( CurrentItem.Valid )
                {
                    PatientIdButton.Content         = "View";
                    ClearPatientButton.IsEnabled    = false;
                }
            }

            if( CurrentItem.Ended )
            {
                EndImg.Visibility   = Visibility.Collapsed;
                EndText.Text        = "Ended";
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
                DateCreatedBox.Text = CurrentItem.CreatedOn.ToShortDateString();
            }
            
            if( !IsFind )
            {
                StartDatePicker.SelectedDate    = CurrentItem.StartDate;
                EndDatePicker.SelectedDate      = CurrentItem.EndDate;
            }
            else
            {
                StartDateText.Text  = "Starting After:";
                EndDateText.Text    = "Ending Before:";
            }

            PopulateMedications();
            
            EndButton.IsEnabled                 = IsEdit && CurrentItem.Valid;
            StartDatePicker.IsEnabled           = !IsView && ( !CurrentItem.Started || !CurrentItem.Valid );
            EndDatePicker.IsEnabled             = !IsView && !CurrentItem.Ended;
            PrescriptionIdBox.IsEnabled         = IsFind;

            EndText.Foreground = EndButton.IsEnabled ? Brushes.Black : Brushes.Gray;
        }

        private void StartFindMedstaff( object o , RoutedEventArgs e )
        {
            if( IsView || IsRestricted )
            {
                EditStaff.OnCancel  = () => App.GoToPage( this );
                App.GoToEditStaffPage( CurrentItem.Prescriber , EditStaff.View | EditStaff.BackOnly );
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
                EditPatient.OnCancel    = () => App.GoToPage( this );
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

        private void StartFindMedication( object o , EventArgs e )
        {
            RecordFields();
            EditMedication.OnConfirm    = OnConfirmMedication;
            EditMedication.OnCancel     = () => App.GoToPage( this );

            EditMedication.Setup( null , EditMedication.Find );
            App.GoToPage( new EditMedication() );
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

        public void OnConfirmMedication( Medication medication )
        {
            App.GoToPage( this );
            if( CurrentItem.AddMedication( medication ) )
            {
                PopulateMedications();
            }
            else
            {
                ShowMessage( "This medication already exists in the prescription." );
            }
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
                deleteItem.IsEnabled    = !IsView;
                deleteItem.Content      = "Delete";
                deleteItem.Click       += ( object i , RoutedEventArgs a ) =>
                {
                    CurrentItem.RemoveMedication( med );
                    MedicationsList.Children.Remove( itemRow );
                };
                
                itemRow.Children.Add( itemContent   );
                itemRow.Children.Add( deleteItem    );

                MedicationsList.Children.Add( itemRow );
            }
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

        private bool RecordFields()
        {
            CurrentItem.Name    = NameBox.Text;
            CurrentItem.Remark  = RemarkBox.Text;
            StartAfter          = StartDatePicker.SelectedDate != null ? StartDatePicker.SelectedDate.Value : DatabaseObject.INVALID_DATETIME;
            EndBefore           = EndDatePicker.SelectedDate != null ? EndDatePicker.SelectedDate.Value : DatabaseObject.INVALID_DATETIME;
            
            if( IsEdit && StartAfter < DateTime.Now.Date )
            {
                ShowMessage( "Select a valid start date" );
                return false;
            }
            
            if( IsEdit && EndBefore < StartAfter )
            {
                ShowMessage( "Select a valid end date" );
                return false;
            }

            return true;
        }

        private void DoConfirm()
        {
            if( !IsView && !RecordFields() )
                return;

            if( IsEdit )
            {

                if( CurrentItem.Patient     == null     ) { ShowMessage( "Please select a patient."         ); return; }
                if( CurrentItem.Prescriber  == null     ) { ShowMessage( "Please select a prescriber."      ); return; }
                if( CurrentItem.Medications.Count == 0  ) { ShowMessage( "Enter at least one medication."   ); return; }
                
                try
                {
                    CurrentItem.Save();
                    LoadDetails();
                    ShowMessage( "Prescription saved." );
                }
                catch
                {
                    ShowMessage( "Failed to save data. Please check your connection." );
                }
            }
            else if( IsFind )
            {
                CurrentItem.Id = Prescription.GetIdFromString( PrescriptionIdBox.Text );
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

        private void DoEndPrescription()
        {
            try
            {
                // Refresh the current item so we don't save a faulty item.
                App.GoToPage( new EndPrescription() );
                EndPrescription.OnCancel    = () => App.GoToPage( this );
                EndPrescription.OnConfirm   = () =>
                {
                    CurrentItem.Load();
                    CurrentItem.Ended = true;
                    CurrentItem.Save();

                    Mode = View | BackOnly;
                    App.GoToPage( new EditPrescription() );
                };
            }
            catch
            {
                ShowMessage( "Failed to load data. Please check your connection." );
            }
        }
    }
}
