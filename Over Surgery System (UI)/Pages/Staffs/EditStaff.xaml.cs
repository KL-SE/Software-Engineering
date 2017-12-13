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
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;
using OverSurgerySystem.UI.Pages.Common;
using OverSurgerySystem.UI.Pages.Address;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.Staffs
{
    /// <summary>
    /// Interaction logic for EditStaff.xaml
    /// </summary>
    public partial class EditStaff : EditorPage<Staff>
    {
        public static bool RestrictAdmin        { set; get; }
        public static bool RestrictReceptionist { set; get; }
        public static bool RestrictGP           { set; get; }
        public static bool RestrictNurse        { set; get; }
        public static bool RestrictActive       { set; get; }

        private EditDetails CurrentEditor;
        private PersonalDetails CurrentDetail
        {
            get
            {
                return CurrentItem.Details;
            }
        }

        public EditStaff()
        {
            InitializeComponent();
            Loaded                     += OnLoad;
            EditDetails.OnConfirm       = e => DoConfirm( e );
            EditDetails.OnCancel        = DoCancel;
            EditDetails.OnNavigate      = RecordFields;
            EditDetails.OnReturn        = () => App.GoToPage( this );

            StaffIdBox.TextChanged         += (object o, TextChangedEventArgs e) => CurrentEditor?.HideMessage();
            MedicalLicenceBox.TextChanged  += (object o, TextChangedEventArgs e) => CurrentEditor?.HideMessage();
            PasswordField.TextInput        += (object o, TextCompositionEventArgs e) => CurrentEditor?.HideMessage();

            SetAsAdmin.Click           += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new Receptionist() { Admin = true   } );
            SetAsReceptionist.Click    += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new Receptionist() { Admin = false  } );
            SetAsGP.Click              += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new MedicalStaff() { Nurse = false  } );
            SetAsNurse.Click           += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new MedicalStaff() { Nurse = true   } );
            
            SetAsAdmin.Visibility           = IsRestricted && RestrictAdmin         ? Visibility.Collapsed : Visibility.Visible;
            SetAsReceptionist.Visibility    = IsRestricted && RestrictReceptionist  ? Visibility.Collapsed : Visibility.Visible;
            SetAsGP.Visibility              = IsRestricted && RestrictGP            ? Visibility.Collapsed : Visibility.Visible;
            SetAsNurse.Visibility           = IsRestricted && RestrictNurse         ? Visibility.Collapsed : Visibility.Visible;

            SetAsActive.Click          += (object o, RoutedEventArgs e) => { CurrentItem.Active = true;  StaffModeHeader.Header = "Yes"; CurrentEditor.HideMessage(); };
            SetAsInactive.Click        += (object o, RoutedEventArgs e) => { CurrentItem.Active = false; StaffModeHeader.Header = "No";  CurrentEditor.HideMessage(); };

            if( IsView )
            {
                StaffIdBox.IsEnabled        = false;
                StaffType.IsEnabled         = false;
                StaffTypeHeader.IsEnabled   = false;
                StaffMode.IsEnabled         = false;
                PasswordField.IsEnabled     = false;
                MedicalLicenceBox.IsEnabled = false;
            }
        }

        private void ReplaceCurrentItem( object sender ,  Staff newStaff )
        {
            if( !CurrentItem.Valid )
            {
                newStaff.Password   = CurrentItem.Password;
                newStaff.Details    = CurrentItem.Details;
                newStaff.DateJoined = CurrentItem.DateJoined;
                newStaff.Active     = CurrentItem.Active;

                if( newStaff is MedicalStaff )
                {
                    MedicalLicenceBox.IsEnabled = true;
                    if( CurrentItem is MedicalStaff )
                    {
                        MedicalStaff srcStaff   = ( MedicalStaff )( CurrentItem     );
                        MedicalStaff dstStaff   = ( MedicalStaff )( newStaff        );
                        dstStaff.LicenseNo      = srcStaff.LicenseNo;
                    }
                }
                else
                {
                    MedicalLicenceBox.IsEnabled = false;
                }
                
                MenuItem item           = ( MenuItem )( sender );
                StaffTypeHeader.Header  = item.Header;
                StaffMode.IsEnabled     = IsFind && !IsView && !( IsRestricted && RestrictActive );
                CurrentItem             = newStaff;
            }
            CurrentEditor.HideMessage();
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            if( !IsEdit || IsFind )
            {
                StaffIdBox.Text             = "";
                PasswordText.Visibility     = Visibility.Collapsed;
                PasswordField.Visibility    = Visibility.Collapsed;
            }
            else
            {
                StaffIdBox.Text = "- New Staff -";
            }
            
            LoadDetails();
            CurrentEditor = new EditDetails();
            DetailsEditor.Navigate( CurrentEditor );
        }

        private void LoadDetails()
        { 
            if( CurrentItem.Valid )
                StaffIdBox.Text = CurrentItem.StringId;

            PasswordField.Password  = CurrentItem.Password;
            StaffModeHeader.Header  = CurrentItem.Active ? "Yes" : "No";

            if( CurrentItem is Receptionist )
            {
                MedicalLicenceBox.IsEnabled = false;
                StaffTypeHeader.Header      = ( ( Receptionist ) CurrentItem ).Admin ? "Administrator" : "Receptionist";
            }

            if( CurrentItem is MedicalStaff )
            {
                MedicalLicenceBox.IsEnabled = !IsView;
                MedicalLicenceBox.Text      = ( ( MedicalStaff ) CurrentItem ).LicenseNo;
                StaffTypeHeader.Header      = ( ( MedicalStaff ) CurrentItem ).Nurse ? "Nurse" : "General Practitioner";
            }

            EditDetails.Setup( CurrentDetail , Mode );
            StaffIdBox.IsEnabled    = IsFind;
            StaffType.IsEnabled     = ( IsFind || !CurrentItem.Valid ) && !IsView;
            StaffMode.IsEnabled     = ( IsFind ||  CurrentItem.Valid ) && !IsView && !( IsRestricted && RestrictActive );
        }

        private void RecordFields()
        {
            CurrentItem.Password = PasswordField.Password;
            if( CurrentItem is MedicalStaff )
            {
                MedicalStaff staff  = ( MedicalStaff )( CurrentItem );
                staff.LicenseNo     = MedicalLicenceBox.Text;
            }
        }

        private int DoConfirm( PersonalDetails d )
        {
            if( !IsView )
                RecordFields();

            if( IsEdit )
            {
                if( CurrentItem.Password.Length == 0 )
                {
                    CurrentEditor.ShowMessage( "Enter a password." );
                    return 0;
                }

                if( CurrentItem is MedicalStaff )
                {
                    MedicalStaff staff          = ( MedicalStaff )( CurrentItem );
                    List<MedicalStaff> dupes    = StaffsManager.GetMedicalStaffWithLicenseNo( staff.LicenseNo );

                    if( staff.LicenseNo.Length == 0 )
                    {
                        CurrentEditor.ShowMessage( "Enter the medical licence number." );
                        return 0;
                    }
                    else if( dupes.Count > 0 )
                    {
                        if( dupes[0].Id != staff.Id )
                        {
                            CurrentEditor.ShowMessage( "The medical licence number already exists." );
                            return 0;
                        }
                    }
                }
                
                try
                {
                    CurrentItem.Save();
                    CurrentEditor.ShowMessage( "Staff saved." );
                    LoadDetails();
                }
                catch
                {
                    CurrentEditor.ShowMessage( "Failed to save data. Please check your connection." );
                }
            }
            else if( IsFind )
            {
                CurrentItem.Id = Staff.GetIdFromString( StaffIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
            return 0;
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
                CurrentEditor.ShowMessage( "Failed to load data. Please check your connection." );
            }
            OnCancel?.Invoke();
        }
    }
}
