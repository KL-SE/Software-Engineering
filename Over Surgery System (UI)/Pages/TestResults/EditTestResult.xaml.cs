﻿using System;
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

namespace OverSurgerySystem.UI.Pages.TestResults
{
    /// <summary>
    /// Interaction logic for EditTestResult.xaml
    /// </summary>
    public partial class EditTestResult : EditorPage<TestResult>
    {
        public static DateTime DateBefore = DatabaseObject.INVALID_DATETIME;

        private MedicalStaff LinkedMedStaff;

        public EditTestResult()
        {
            InitializeComponent();
            Loaded                         += OnLoad;
            PatientIdButton.Click          += StartFindPatient;
            LicenseNoButton.Click          += StartFindMedstaff;
            LicenseNoBox.StoppedTyping     += ResolveMedicalStaff;
            ConfirmButton.Click            += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click             += (object o, RoutedEventArgs a) => DoCancel();
            PrintButton.Click              += (object o, RoutedEventArgs e) => DoPrint();
            LinkedMedStaff                  = new MedicalStaff();
            FindPatient.Reset();

            if( IsFind )
            {
                ConfirmButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text  = "Find";
            }
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            TestIdBox.Text = !IsEdit || IsFind ? "" : "- New TestResult -";
            LoadDetails();
        }

        private void ResolveMedicalStaff( object o , EventArgs e )
        {
            List<MedicalStaff> medStaffs = StaffsManager.GetMedicalStaffWithLicenseNo( LicenseNoBox.Text );
            if( medStaffs.Count == 1 )
            {
                LinkedMedStaff      = medStaffs[0];
                MedStaffBox.Text    = String.Format( "{0} - {1}" , LinkedMedStaff.StringId , LinkedMedStaff.Details.FullName );
            }
            else if( medStaffs.Count > 1 )
            {
                // This should NEVER be true.
                MedStaffBox.Text    = "- Multiple Found -";
            }
            else
            {
                MedStaffBox.Text = "- External Examiner -";
            }
        }

        private void LoadDetails()
        {
            if( CurrentItem.Valid )
                TestIdBox.Text = CurrentItem.StringId;

            if( CurrentItem.Patient != null )
                PatientIdBox.Text   = String.Format( "{0} - {1}" , CurrentItem.Patient.StringId , CurrentItem.Patient.Details.FullName );

            if( CurrentItem.CreatedOn != DatabaseObject.INVALID_DATETIME )
            {
                TestDatePicker.SelectedDate = CurrentItem.CreatedOn;
                TestDatePicker.Foreground   = Brushes.Black;
            }
            else if( !IsFind )
            {
                TestDatePicker.Foreground = Brushes.White;
            }

            LicenseNoBox.Text   = CurrentItem.MedicalLicenseNo;
            TestNameBox.Text    = CurrentItem.Name;
            TestDescBox.Text    = CurrentItem.Description;
            TestResultBox.Text  = CurrentItem.Result;
            TestRemarkBox.Text  = CurrentItem.Remark;
            
            PrintButton.IsEnabled       = IsView && CurrentItem.Valid;
            TestIdBox.IsEnabled         = IsFind;
            TestDatePicker.IsEnabled    = IsFind;
            DateText.Text               = IsFind ? "Date Before:" : "Date Created:";
            
            PrintText.Foreground    = PrintButton.IsEnabled ? Brushes.Black : Brushes.Gray;
        }

        private void StartFindMedstaff( object o , RoutedEventArgs e )
        {
            RecordFields();
            ResolveMedicalStaff( null , null );
            FindStaff.OnSelect  = OnSelectStaff;
            FindStaff.OnFind    = OnFindStaff;
            FindStaff.OnFound   = OnConfirmStaff;
            FindStaff.OnCancel  = () => App.GoToPage( this );
            App.GoToEditStaffPage( LinkedMedStaff , EditStaff.Find | EditStaff.Restricted );
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
                EditPatient.OnCancel   = () => App.GoToFindPatientResultPage();
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
            }
            else
            {
                OnSelectStaff( list[0] );
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
            LinkedMedStaff                  = ( MedicalStaff )( staff );
            CurrentItem.MedicalLicenseNo    = LinkedMedStaff.LicenseNo;
            App.GoToPage( this );
        }

        private void RecordFields()
        {
            CurrentItem.MedicalLicenseNo    = LicenseNoBox.Text;
            CurrentItem.Name                = TestNameBox.Text;
            CurrentItem.Description         = TestDescBox.Text;
            CurrentItem.Result              = TestResultBox.Text;
            CurrentItem.Remark              = TestRemarkBox.Text;
            DateBefore                      = TestDatePicker.SelectedDate != null ? TestDatePicker.SelectedDate.Value : DatabaseObject.INVALID_DATETIME;
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
            RecordFields();

            if( IsEdit )
            {
                if( CurrentItem.Patient == null              ) { ShowMessage( "Please select a patient."                        ); return; }
                if( CurrentItem.MedicalLicenseNo.Length == 0 ) { ShowMessage( "Enter the examiner's medical licence number."    ); return; }
                if( CurrentItem.Name.Length             == 0 ) { ShowMessage( "Enter the test name."                            ); return; }
                if( CurrentItem.Description.Length      == 0 ) { ShowMessage( "Enter the test description."                     ); return; }
                if( CurrentItem.Result.Length           == 0 ) { ShowMessage( "Enter the test result."                          ); return; }

                CurrentItem.Save();
                ShowMessage( "Test saved." );
                LoadDetails();
            }
            else
            {
                CurrentItem.Id = TestResult.GetIdFromString( TestIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
        }

        private void DoCancel()
        {
            if( CurrentItem.Valid ) CurrentItem.Load();
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += (object sender, RoutedEventArgs e) => App.GoToManageTestResults();
            OnCancel?.Invoke();
        }

        private void DoPrint()
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintVisual( this , String.Format( "{0} - {1} - {2}" , CurrentItem.StringId , CurrentItem.Name , CurrentItem.CreatedOn.ToLongDateString() ) );
        }
    }
}