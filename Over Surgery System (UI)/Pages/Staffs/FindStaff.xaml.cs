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
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.Manager;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.UI.Pages;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.Staffs
{
    /// <summary>
    /// Interaction logic for FindStaff.xaml
    /// </summary>
    public partial class FindStaff : FinderPage<Staff>
    {
        public FindStaff()
        {
            InitializeComponent();
            Loaded             += OnLoad;
            BackButton.Click   += (object o, RoutedEventArgs e) => OnCancel?.Invoke();
            ResetButton.Click  += (object o, RoutedEventArgs e) =>
            {
                App.GoToFindStaffPage( LastPrototype );
                EditStaff.OnConfirm = OnFind;
                EditStaff.OnCancel  = () => App.GoToPage( this );
            };
        }

        public void OnLoad( object o , RoutedEventArgs e )
        {
            Populate( ResultsList );
        }

        public override void SetEventHandler()
        {
            EditStaff.OnConfirm = OnFound;
            EditStaff.OnCancel  = () => App.GoToPage( this );
        }

        public override string[] GetData( Staff staff )
        {
            return new string[4]
            {
                staff.StringId,
                String.Format( "{0} {1}" , staff.Details.FirstName , staff.Details.LastName ),
                staff.Details.FullAddress,
                staff.Details.DateOfBirth.ToShortDateString()
            };
        }

        public static List<Staff> FindFromPrototype( Staff protoStaff )
        {
            LastPrototype = protoStaff;
            if( protoStaff.Valid )
            {
                SearchResult.Clear();
                Staff staff = StaffsManager.GetStaff( protoStaff.Id );

                if( staff != null && staff.Valid && protoStaff.GetType() == staff.GetType() )
                {
                    SearchResult.Add( staff );
                }

                return SearchResult;
            }
            
            SearchResult = protoStaff.Active ? StaffsManager.GetActiveStaffs() : StaffsManager.GetInactiveStaffs();
            if( protoStaff.Details.FirstName.Length > 0                             ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.FirstName.ToUpper().Contains( protoStaff.Details.FirstName.ToUpper()  ) );
            if( protoStaff.Details.LastName.Length > 0                              ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.LastName.ToUpper().Contains(  protoStaff.Details.LastName.ToUpper()   ) );
            if( protoStaff.Details.Address.Length > 0                               ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.Address.ToUpper().Contains(   protoStaff.Details.Address.ToUpper()    ) );
            if( protoStaff.Details.DateOfBirth != DatabaseObject.INVALID_DATETIME   ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.DateOfBirth.Equals(           protoStaff.Details.DateOfBirth          ) );

            // Match the identifications
            if( protoStaff.Details.Identifications.Count > 0 )
            {
                SearchResult = ManagerHelper.Filter( SearchResult , e =>
                {
                    foreach( Identification iden in e.Details.Identifications )
                    {
                        if( iden.Value.Equals( protoStaff.Details.Identifications[0].Value ) )
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }

            // Match the contact number
            if( protoStaff.Details.ContactNumbers.Count > 0 )
            {
                SearchResult = ManagerHelper.Filter( SearchResult , e =>
                {
                    foreach( ContactNumber no in e.Details.ContactNumbers )
                    {
                        if( no.Number.Equals( protoStaff.Details.ContactNumbers[0].Number ) )
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }
            
            // Match the postcode.
            if( protoStaff.Details.Postcode!= null && protoStaff.Details.Postcode.Valid )
            {
                SearchResult = ManagerHelper.Filter( SearchResult , e => e.Details.Postcode.Id == protoStaff.Details.Postcode.Id );
            }
            
            // Match staff specfic details
            if( protoStaff is Receptionist )
            {
                Receptionist protoReceptionist = ( Receptionist )( protoStaff );
                SearchResult = ManagerHelper.FilterType<Receptionist,Staff>( SearchResult );
                SearchResult = ManagerHelper.Filter( SearchResult , e => ( ( Receptionist ) e ).Admin == protoReceptionist.Admin );
            }
            else if( protoStaff is MedicalStaff )
            {
                MedicalStaff protoMedicalStaff = ( MedicalStaff )( protoStaff );
                SearchResult = ManagerHelper.FilterType<MedicalStaff,Staff>( SearchResult );
                SearchResult = ManagerHelper.Filter( SearchResult , e => ( ( MedicalStaff ) e ).Nurse == protoMedicalStaff.Nurse );

                // Compare the license number
                if( protoMedicalStaff.LicenseNo.Length > 0 )
                {
                    int first = SearchResult.Count;
                    SearchResult = ManagerHelper.Filter( SearchResult , e => ( ( MedicalStaff ) e ).LicenseNo.Equals( protoMedicalStaff.LicenseNo ) );
                }
            }

            return SearchResult;
        }
    }
}
