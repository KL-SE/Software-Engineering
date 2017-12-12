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
            SetAsAdmin.Click           += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new Receptionist() { Admin = true   } );
            SetAsReceptionist.Click    += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new Receptionist() { Admin = false  } );
            SetAsGP.Click              += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new MedicalStaff() { Nurse = false  } );
            SetAsNurse.Click           += (object o, RoutedEventArgs e) => ReplaceCurrentItem( o , new MedicalStaff() { Nurse = true   } );
            SetAsActive.Click          += (object o, RoutedEventArgs e) => { CurrentItem.Active = true;  StaffModeHeader.Header = "Yes"; };
            SetAsInactive.Click        += (object o, RoutedEventArgs e) => { CurrentItem.Active = false; StaffModeHeader.Header = "No";  };
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
                StaffMode.IsEnabled     = IsFind;
                CurrentItem             = newStaff;
            }
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
                MedicalLicenceBox.IsEnabled = true;
                MedicalLicenceBox.Text      = ( ( MedicalStaff ) CurrentItem ).LicenseNo;
                StaffTypeHeader.Header      = ( ( MedicalStaff ) CurrentItem ).Nurse ? "Nurse" : "General Practitioner";
            }

            EditDetails.Setup( CurrentDetail , Mode );
            StaffIdBox.IsEnabled        = IsFind;
            StaffType.IsEnabled         = ( IsFind || !CurrentItem.Valid ) && !IsRestricted;
            StaffMode.IsEnabled         = ( IsFind ||  CurrentItem.Valid ) && !IsRestricted;
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

                CurrentItem.Save();
                CurrentEditor.ShowMessage( "Staff saved." );
                LoadDetails();
            }
            else
            {
                CurrentItem.Id = Staff.GetIdFromString( StaffIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
            return 0;
        }

        private void DoCancel()
        {
            if( CurrentItem.Valid   ) CurrentItem.Load();
            if( CurrentDetail.Valid ) CurrentDetail.Load();
            OnCancel?.Invoke();
        }
    }
}
