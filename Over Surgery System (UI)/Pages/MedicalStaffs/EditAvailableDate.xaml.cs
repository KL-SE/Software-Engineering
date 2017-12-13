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

namespace OverSurgerySystem.UI.Pages.MedicalStaffs
{
    /// <summary>
    /// Interaction logic for EditAvailableDate.xaml
    /// </summary>
    public partial class EditAvailableDate : FinderPage<LeaveDate>
    {
        public static string LastRemark             { set; get; }
        public static DateTime SelectedDate         { set; get; }
        public static LeaveDate SelectedLeaveDate   { set; get; }
        public static MedicalStaff SearchStaff      { set; get; }
        public static MedicalStaff CurrentStaff     { set; get; }

        public EditAvailableDate()
        {
            InitializeComponent();
            Loaded                     += OnLoad;
            StaffIdButton.Click        += StartFindStaff;
            ShowStaffsButton.Click     += (object o, RoutedEventArgs a) => ShowStaffsInResult();
            FindButton.Click           += (object o, RoutedEventArgs a) => DoFind();
            AddButton.Click            += (object o, RoutedEventArgs a) => DoConfirm();
            BackButton.Click           += (object o, RoutedEventArgs a) => DoCancel();
            ClearStaffButton.Click     += (object o, RoutedEventArgs a) => { StaffIdBox.Text = null; CurrentStaff = null;                                           };
            ClearLeaveDateButton.Click += (object o, RoutedEventArgs a) => { LeaveDatePicker.SelectedDate = null; SelectedDate = DatabaseObject.INVALID_DATETIME;   };
            
            FindStaff.LastPrototype = new MedicalStaff();
            SearchStaff             = new MedicalStaff();
            CurrentStaff            = new MedicalStaff();
            SelectedDate            = DatabaseObject.INVALID_DATETIME;
            OnSelect                = OnSelectLeaveDate;
            
            StaffIdBox.TextChanged                 += (object o, TextChangedEventArgs e) => HideMessage();
            RemarkBox.TextChanged                  += (object o, TextChangedEventArgs e) => HideMessage();
            LeaveDatePicker.SelectedDateChanged    += (object o, SelectionChangedEventArgs e) => HideMessage();
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            LoadDetails();
            PerformSearch();
        }

        public override string[] GetData( LeaveDate leaveDate )
        {
            return new string[]
            {
                leaveDate.Owner.StringId,
                String.Format( "{0} {1}" , leaveDate.Owner.Details.FirstName , leaveDate.Owner.Details.LastName ),
                leaveDate.Remark,
                leaveDate.Date.ToShortDateString()
            };
        }

        public override void SetEventHandler()
        { 
            EditStaff.OnConfirm = OnConfirmStaff;
            EditStaff.OnCancel  = () => App.GoToPage( this );
        }

        private void LoadDetails()
        {
            AddButton.IsEnabled         = Permission.CanEditLeaveDates;
            AddButtonText.Foreground    = Permission.CanEditLeaveDates ? Brushes.Black : Brushes.Gray;

            if( SelectedLeaveDate == null )
            {
                if( !Permission.CanEditLeaveDates )
                {
                    StaffIdButton.Content           = "Edit";
                    ClearStaffButton.IsEnabled      = true;
                    LeaveDatePicker.IsEnabled       = true;
                    ClearLeaveDateButton.IsEnabled  = true;
                    RemarkBox.IsEnabled             = true;
                }

                StaffIdBox.Text = CurrentStaff != null && CurrentStaff.Valid ? CurrentStaff.StringId : "";
                RemarkBox.Text  = LastRemark;
                if( SelectedDate != DatabaseObject.INVALID_DATETIME )
                {
                    LeaveDatePicker.SelectedDate = SelectedDate;
                }
                
                AddButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/save.png" ) );
                AddButtonText.Text  = "Add";
                FindButtonText.Text = "Find";
            }
            else
            {
                StaffIdButton.Content           = "View";
                ClearStaffButton.IsEnabled      = false;
                LeaveDatePicker.IsEnabled       = false;
                ClearLeaveDateButton.IsEnabled  = false;
                RemarkBox.IsEnabled             = false;

                StaffIdBox.Text                 = SelectedLeaveDate.Owner.StringId;
                RemarkBox.Text                  = SelectedLeaveDate.Remark;
                LeaveDatePicker.SelectedDate    = SelectedLeaveDate.Date;
                
                AddButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/cross.png" ) );
                AddButtonText.Text  = "Delete";
                FindButtonText.Text = "Reset";
            }
        }

        private void ShowStaffsInResult()
        {
            try
            {
                RecordFields();
                List<Staff> results = StaffsManager.GetAllStaffs();

                results = ManagerHelper.Filter( results , e =>
                {
                    return e is MedicalStaff && !( ( MedicalStaff ) e ).IsOnLeave( SelectedDate );
                });

                FindStaff.AcquireList( results  );
                FindStaff.NoResetButton = true;
                FindStaff.OnSelect      = OnViewStaff;
                FindStaff.OnCancel      = () => { App.GoToPage( this ); FindStaff.NoResetButton = false; };

                results.Sort( (c,o) => c.Details.FullName.ToUpper().CompareTo( o.Details.FullName.ToUpper() ) );

                App.GoToFindStaffResultPage();
            }
            catch
            {
                ShowMessage( "Failed to load data. Please check your connection." );
            }

        }

        private void OnViewStaff( Staff staff )
        {
            App.GoToEditStaffPage( staff , EditStaff.View | EditStaff.BackOnly );
        }

        private void StartFindStaff( object o , RoutedEventArgs e )
        {
            RecordFields();
            FindStaff.OnSelect              = OnSelectStaff;
            FindStaff.OnFind                = OnFindStaff;
            FindStaff.OnFound               = OnConfirmStaff;
            FindStaff.OnCancel              = () => App.GoToPage( this );
            EditStaff.RestrictActive        = true;
            EditStaff.RestrictAdmin         = true;
            EditStaff.RestrictReceptionist  = true;
            App.GoToEditStaffPage( FindStaff.LastPrototype , EditStaff.Find | EditStaff.Restricted );
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

        public void OnConfirmStaff( Staff staff )
        {
            CurrentStaff = ( MedicalStaff )( staff );
            App.GoToPage( this );
        }

        public void OnSelectLeaveDate( LeaveDate leaveDate )
        {
            SelectedLeaveDate = leaveDate;
            LoadDetails();
        }

        private void RecordFields()
        {
            if( SelectedLeaveDate == null )
            {
                LastRemark  = RemarkBox.Text;
                if( LeaveDatePicker.SelectedDate != null )
                {
                    SelectedDate = LeaveDatePicker.SelectedDate.Value;
                }
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

        private void DoConfirm()
        {
            // Add Mode
            if( SelectedLeaveDate == null )
            {   
                if( CurrentStaff == null )
                {
                    ShowMessage( "Please select a staff." );
                    return;
                }

                if( LeaveDatePicker.SelectedDate == null )
                {
                    ShowMessage( "Please select a date." );
                    return;
                }

                if( LeaveDatePicker.SelectedDate.Value < DateTime.Now.Date )
                {
                    ShowMessage( "Please select a future date." );
                    return;
                }

                if( CurrentStaff.IsOnLeave( LeaveDatePicker.SelectedDate.Value ) )
                {
                    ShowMessage( "The staff is already on leave at this date." );
                    return;
                }
                
                try
                {
                    CurrentStaff.AddLeaveDate( LeaveDatePicker.SelectedDate.Value , RemarkBox.Text );
                    CurrentStaff.Save();
                }
                catch
                {
                    ShowMessage( "Failed to save data. Please check your connection." );
                }

                LeaveDatePicker.SelectedDate    = null;
                RemarkBox.Text                  = "";

                ShowMessage( "Leave date added." );
                LoadDetails();
                PerformSearch();
            }
            // Delete Mode
            else
            {
                try
                {
                    SelectedLeaveDate.Owner.RemoveLeaveDate( SelectedLeaveDate.Date );
                    SelectedLeaveDate = null;
                    LoadDetails();
                    PerformSearch();
                }
                catch
                {
                    ShowMessage( "Failed to delete data. Please check your connection." );
                }

            }
        }

        private void PerformSearch()
        {
            try
            {
                List<MedicalStaff> availableStaffs = StaffsManager.GetAllMedicalStaffs();

                SearchResult.Clear();
                foreach( MedicalStaff staff in availableStaffs )
                    SearchResult.AddRange( staff.LeaveDates );

                if( SearchStaff != null && SearchStaff.Valid    ) SearchResult = ManagerHelper.Filter( SearchResult , e => e.Owner.Id == SearchStaff.Id                             );
                if( LeaveDatePicker.SelectedDate != null        ) SearchResult = ManagerHelper.Filter( SearchResult , e => e.Date == SelectedDate                                   );
                if( RemarkBox.Text.Length > 0                   ) SearchResult = ManagerHelper.Filter( SearchResult , e => e.Remark.ToUpper().Contains( RemarkBox.Text.ToUpper() )  );
            
                SearchResult = ManagerHelper.Filter( SearchResult , e => e.Date >= DateTime.Now.Date );
                SearchResult.Sort( (c,o) => c.Date.CompareTo( o.date ) );

                Populate( ResultsList );
            }
            catch
            {
                ShowMessage( "Failed to load data. Please check your connection." );
            }
        }

        private void DoFind()
        {
            // Find Mode
            if( SelectedLeaveDate == null )
            {
                SearchStaff = CurrentStaff;
                RecordFields();
                PerformSearch();
            }
            // Reset Mode
            else
            {
                SelectedLeaveDate = null;
                LoadDetails();
            }
        }

        private void DoCancel()
        {
            App.GoToMainMenu();
            OnCancel?.Invoke();
        }
    }
}
