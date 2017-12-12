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
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Manager;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.UI.Pages;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.Patients
{
    /// <summary>
    /// Interaction logic for FindPatient.xaml
    /// </summary>
    public partial class FindPatient : FinderPage<Patient>
    {
        public FindPatient()
        {
            InitializeComponent();
            Loaded             += OnLoad;
            BackButton.Click   += (object o, RoutedEventArgs e) => OnCancel?.Invoke();
            ResetButton.Click  += (object o, RoutedEventArgs e) =>
            {
                App.GoToFindPatientPage( LastPrototype );
                EditPatient.OnConfirm   = OnFind;
                EditPatient.OnCancel    = () => App.GoToPage( this );
            };
        }
        
        public override string[] GetData( Patient patient )
        {
            return new string[4]
            {
                patient.StringId,
                String.Format( "{0} {1}" , patient.Details.FirstName , patient.Details.LastName ),
                patient.Details.FullAddress,
                patient.Details.DateOfBirth.ToShortDateString()
            };
        }

        public override void SetEventHandler()
        {
            EditPatient.OnConfirm   = OnFound;
            EditPatient.OnCancel    = () => App.GoToPage( this );
        }

        public void OnLoad( object o , RoutedEventArgs e )
        {
            Populate( ResultsList );
        }
        
        public static List<Patient> FindFromPrototype( Patient protoPatient )
        {
            LastPrototype = protoPatient;
            if( protoPatient.Valid )
            {
                SearchResult.Clear();
                Patient patient = PatientsManager.GetPatient( protoPatient.Id );

                if( patient != null && patient.Valid )
                {
                    SearchResult.Add( patient );
                }

                return SearchResult;
            }
            
            SearchResult = PatientsManager.GetAllPatients();
            if( protoPatient.Details.FirstName.Length > 0                             ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.FirstName.ToUpper().Contains(   protoPatient.Details.FirstName.ToUpper()    ) );
            if( protoPatient.Details.LastName.Length > 0                              ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.LastName.ToUpper().Contains(    protoPatient.Details.LastName.ToUpper()     ) );
            if( protoPatient.Details.Address.Length > 0                               ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.Address.ToUpper().Contains(     protoPatient.Details.Address.ToUpper()      ) );
            if( protoPatient.Details.DateOfBirth != DatabaseObject.INVALID_DATETIME   ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Details.DateOfBirth.Equals(             protoPatient.Details.DateOfBirth            ) );

            // Match the identifications
            if( protoPatient.Details.Identifications.Count > 0 )
            {
                SearchResult = ManagerHelper.Filter( SearchResult , e =>
                {
                    foreach( Identification iden in e.Details.Identifications )
                    {
                        if( iden.Value.Equals( protoPatient.Details.Identifications[0].Value ) )
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }

            // Match the contact number
            if( protoPatient.Details.ContactNumbers.Count > 0 )
            {
                SearchResult = ManagerHelper.Filter( SearchResult , e =>
                {
                    foreach( ContactNumber no in e.Details.ContactNumbers )
                    {
                        if( no.Number.Equals( protoPatient.Details.ContactNumbers[0].Number ) )
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }
            
            // Match the postcode.
            if( protoPatient.Details.Postcode!= null && protoPatient.Details.Postcode.Valid )
            {
                SearchResult = ManagerHelper.Filter( SearchResult , e => e.Details.Postcode.Id == protoPatient.Details.Postcode.Id );
            }

            return SearchResult;
        }
    }
}
