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
    /// Interaction logic for EditWorkingDay.xaml
    /// </summary>
    public partial class EditWorkingDay : FinderPage<WorkingDays>
    {
        public static bool InEditMode                   { set; get; }
        public static int SelectedDay                   { set; get; }
        public static DateTime SelectedDate             { set; get; }
        public static WorkingDays SelectedWorkingDays   { set; get; }
        public static MedicalStaff SearchStaff          { set; get; }

        public EditWorkingDay()
        {
            InitializeComponent();
            Loaded                     += OnLoad;
            StaffIdButton.Click        += StartFindStaff;
            ModeButton.Click           += (object o, RoutedEventArgs a) => DoChangeMode();
            ConfirmButton.Click        += (object o, RoutedEventArgs a) => DoConfiirm();
            BackButton.Click           += (object o, RoutedEventArgs a) => DoCancel();
            ClearStaffButton.Click     += (object o, RoutedEventArgs a) => { StaffIdBox.Text = null; SearchStaff = null;                                            };
            ClearDutyDateButton.Click  += (object o, RoutedEventArgs a) => { DutyDatePicker.SelectedDate = null; SelectedDate = DatabaseObject.INVALID_DATETIME;    };

            SetAsSunday.Click          += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.SUNDAY    );
            SetAsMonday.Click          += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.MONDAY    );
            SetAsTuesday.Click         += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.TUESDAY   );
            SetAsWednesday.Click       += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.WEDNESDAY );
            SetAsThursday.Click        += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.THURSDAY  );
            SetAsFriday.Click          += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.FRIDAY    );
            SetAsSaturday.Click        += (object o, RoutedEventArgs a) => SetWorkingDay( WorkingDays.Day.SATURDAY  );
            SetAsWorking.Click         += (object o, RoutedEventArgs a) => SetWorking( true     );
            SetAsNotWorking.Click      += (object o, RoutedEventArgs a) => SetWorking( false    );
            
            FindStaff.LastPrototype     = new MedicalStaff();
            SearchStaff                 = new MedicalStaff();
            SelectedDate                = DateTime.Now.Date;
            DutyDatePicker.SelectedDate = DateTime.Now.Date;
            SelectedDay                 = WorkingDays.GetDayNumber( SelectedDate );
            OnSelect                    = OnSelectWorkingDays;
            
            StaffIdBox.TextChanged              += (object o, TextChangedEventArgs e) => HideMessage();
            DutyDatePicker.SelectedDateChanged  += (object o, SelectionChangedEventArgs e) => HideMessage();
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            LoadDetails();
            PerformSearch();
        }

        public override string[] GetData( WorkingDays workingDays )
        {
            return new string[]
            {
                workingDays.Owner.StringId,
                String.Format( "{0} {1}" , workingDays.Owner.Details.FirstName , workingDays.Owner.Details.LastName ),
                workingDays.Owner.Details.FullAddress,
                workingDays.Owner.LicenseNo
            };
        }

        public override void SetEventHandler()
        { 
            EditStaff.OnConfirm = OnConfirmStaff;
            EditStaff.OnCancel  = () => App.GoToPage( this );
        }

        private void SetWorkingDay( int day )
        {
            SelectedDay = day;
            LoadDetails();
        }

        private void SetWorking( bool working )
        {
            if( SelectedWorkingDays != null )
            {
                SelectedWorkingDays.Set( SelectedDay , working );
                LoadDetails();
            }
        }

        private void LoadDetails()
        {
            StaffIdBox.Text = SearchStaff != null && SearchStaff.Valid ? SearchStaff.StringId : "";
            
            switch( SelectedDay )
            {
                case( WorkingDays.Day.SUNDAY    ):  DayPickerHeader.Header = "Sunday";      break;
                case( WorkingDays.Day.MONDAY    ):  DayPickerHeader.Header = "Monday";      break;
                case( WorkingDays.Day.TUESDAY   ):  DayPickerHeader.Header = "Tuesday";     break;
                case( WorkingDays.Day.WEDNESDAY ):  DayPickerHeader.Header = "Wednesday";   break;
                case( WorkingDays.Day.THURSDAY  ):  DayPickerHeader.Header = "Thursday";    break;
                case( WorkingDays.Day.FRIDAY    ):  DayPickerHeader.Header = "Friday";      break;
                case( WorkingDays.Day.SATURDAY  ):  DayPickerHeader.Header = "Saturday";    break;
            };
                
            IsWorkingPickerHeader.Header = SelectedWorkingDays == null || SelectedWorkingDays.WorkingOn( SelectedDay ) ? "Yes" : "No";

            // Find Mode
            if( !InEditMode )
            {
                if( SelectedDate != DatabaseObject.INVALID_DATETIME )
                {
                    DutyDatePicker.SelectedDate = SelectedDate;
                }

                DateText.Visibility         = Visibility.Visible;
                DutyDatePanel.Visibility    = Visibility.Visible;

                if( Permission.CanEditStaffWorkingDays )
                {
                    WorkingDayText.Visibility   = Visibility.Collapsed;
                    WorkingOnText.Visibility    = Visibility.Collapsed;
                    DayPicker.Visibility        = Visibility.Collapsed;
                    IsWorkingPicker.Visibility  = Visibility.Collapsed;
                    ModeButton.IsEnabled        = true;
                    ModeButtonText.Foreground   = Brushes.Black;
                }
                else
                {
                    IsWorkingPicker.IsEnabled   = false;
                    ModeButton.IsEnabled        = false;
                    ModeButtonText.Foreground   = Brushes.Gray;
                }
                
                ModeButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/main_menu.png" ) );
                ModeButtonText.Text  = "Edit";

                ConfirmButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/search.png" ) );
                ConfirmButtonText.Text  = "Find";
            }
            // Edit Mode
            else
            {
                WorkingDayText.Visibility   = Visibility.Visible;
                WorkingOnText.Visibility    = Visibility.Visible;
                DayPicker.Visibility        = Visibility.Visible;
                IsWorkingPicker.Visibility  = Visibility.Visible;
                
                if( !Permission.CanEditStaffWorkingDays )
                {
                    DateText.Visibility         = Visibility.Visible;
                    DutyDatePanel.Visibility    = Visibility.Visible;
                    DayPicker.IsEnabled         = true;
                    IsWorkingPicker.IsEnabled   = false;
                    ModeButton.IsEnabled        = false;
                    ModeButtonText.Foreground   = Brushes.Gray;
                }
                else
                {
                    DateText.Visibility         = Visibility.Collapsed;
                    DutyDatePanel.Visibility    = Visibility.Collapsed;
                    DayPicker.IsEnabled         = true;
                    IsWorkingPicker.IsEnabled   = true;
                    ModeButton.IsEnabled        = true;
                    ModeButtonText.Foreground   = Brushes.Black;
                }
                
                ModeButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/cross.png" ) );
                ModeButtonText.Text  = "Cancel";

                ConfirmButtonImg.Source = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/save.png" ) );
                ConfirmButtonText.Text  = "Save";
            }
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
            SelectedWorkingDays = ( ( MedicalStaff ) staff ).WorkingDays;
            SearchStaff         = SelectedWorkingDays.Owner;
            App.GoToPage( this );
        }

        public void OnSelectWorkingDays( WorkingDays workingDays )
        {
            SelectedWorkingDays = workingDays;
            SearchStaff         = workingDays.Owner;
            LoadDetails();
        }

        private void RecordFields()
        {
            if( DutyDatePicker.SelectedDate != null )
            {
                SelectedDate = DutyDatePicker.SelectedDate.Value;
                if( !Permission.CanEditStaffWorkingDays )
                {
                    // User without global edit permission will see both date and days together.
                    // So, in this case, we will not change the day of week.
                    switch( SelectedDate.DayOfWeek )
                    {
                        case( DayOfWeek.Sunday       ): SelectedDay = WorkingDays.Day.SUNDAY;       break;
                        case( DayOfWeek.Monday       ): SelectedDay = WorkingDays.Day.MONDAY;       break;
                        case( DayOfWeek.Tuesday      ): SelectedDay = WorkingDays.Day.TUESDAY;      break;
                        case( DayOfWeek.Wednesday    ): SelectedDay = WorkingDays.Day.WEDNESDAY;    break;
                        case( DayOfWeek.Thursday     ): SelectedDay = WorkingDays.Day.THURSDAY;     break;
                        case( DayOfWeek.Friday       ): SelectedDay = WorkingDays.Day.FRIDAY;       break;
                        case( DayOfWeek.Saturday     ): SelectedDay = WorkingDays.Day.SATURDAY;     break;
                    }
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

        private void DoChangeMode()
        {
            // Swap the modes 
            InEditMode = !InEditMode;
            
            // Reload the selected day when we switch modes because we don't want to persist the changes.
            if( SelectedWorkingDays != null )
            {
                try
                {
                    SelectedWorkingDays.Load();

                    // Check permission and reset to find mode if the user have on permission to edit.
                    if( InEditMode && !Permission.CanEditStaffWorkingDays )
                    {
                        InEditMode                  = false;
                        ModeButton.IsEnabled        = false;
                        ModeButtonText.Foreground   = Brushes.Gray;
                    }
                    else
                    {
                        ModeButton.IsEnabled        = true;
                        ModeButtonText.Foreground   = Brushes.Black;
                    }
                }
                catch
                {
                    ShowMessage( "Failed to load data. Please check your connection." );
                }
            }

            // Enable/Disable selecting further staffs.
            if( SearchResult.Count > 0 )
            {
                foreach( UIElement element in ResultsList.Children )
                {
                    element.IsEnabled = !InEditMode;
                }
            }

            LoadDetails();
        }

        private void PerformSearch()
        {
            try
            {
                List<MedicalStaff> availableStaffs = StaffsManager.GetAllMedicalStaffs();

                SearchResult.Clear();
                foreach( MedicalStaff staff in availableStaffs )
                    SearchResult.Add( staff.WorkingDays );

                if( SearchStaff != null && SearchStaff.Valid    ) SearchResult = ManagerHelper.Filter( SearchResult , e => e.Owner.Id == SearchStaff.Id             );
                if( DutyDatePicker.SelectedDate != null         ) SearchResult = ManagerHelper.Filter( SearchResult , e => e.Owner.IsFullyAvailable( SelectedDate ) );

                SearchResult.Sort( (c,o) => c.Owner.Details.FullName.ToUpper().CompareTo( o.Owner.Details.FullName.ToUpper() ) );

                Populate( ResultsList );
            }
            catch
            {
                ShowMessage( "Failed to load data. Please check your connection." );
            }
        }

        private void DoConfiirm()
        {
            // Edit Mode
            RecordFields();
            if( InEditMode )
            {
                if( SelectedWorkingDays == null )
                {
                    ShowMessage( "Please select a staff." );
                    return;
                }
                
                try
                {
                    SelectedWorkingDays.Save();
                }
                catch
                {
                    ShowMessage( "Failed to save data. Please check your connection." );
                }

                ShowMessage( "Duty information saved." );
            }
            // Find Mode
            else
            {
                if( DutyDatePicker.SelectedDate != null && DutyDatePicker.SelectedDate.Value < DateTime.Now.Date )
                {
                    ShowMessage( "Please select a future date." );
                    return;
                }
                
                SelectedWorkingDays = null;
                LoadDetails();
                PerformSearch();
            }
        }

        private void DoCancel()
        {
            App.GoToMainMenu();
            OnCancel?.Invoke();
        }
    }
}
