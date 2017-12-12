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

namespace OverSurgerySystem.UI.Pages.Prescriptions
{
    /// <summary>
    /// Interaction logic for FindPrescription.xaml
    /// </summary>
    public partial class FindPrescription : FinderPage<Prescription>
    {
        
        public FindPrescription()
        {
            InitializeComponent();
            Loaded             += OnLoad;
            BackButton.Click   += (object o, RoutedEventArgs e) => OnCancel?.Invoke();
            ResetButton.Click  += (object o, RoutedEventArgs e) =>
            {
                App.GoToFindPrescriptionPage( LastPrototype );
                EditPrescription.OnConfirm   = OnFind;
                EditPrescription.OnCancel    = () => App.GoToPage( this );
            };
        }
        
        public override string[] GetData( Prescription prescription )
        {
            return new string[4]
            {
                prescription.StringId,
                prescription.Patient.StringId,
                prescription.Remark == null ? "No Remark" : prescription.Name,
                prescription.EndDate.ToString()
            };
        }

        public override void SetEventHandler()
        {
            EditPrescription.OnConfirm   = OnFound;
            EditPrescription.OnCancel    = () => App.GoToPage( this );
        }

        public void OnLoad( object o , RoutedEventArgs e )
        {
            Populate( ResultsList );
        }

        public static List<Prescription> FindFromPrototype( Prescription protoPrescription )
        {
            LastPrototype = protoPrescription;
            if( protoPrescription.Valid )
            {
                SearchResult.Clear();
                Prescription prescription = PatientsManager.GetPrescription( protoPrescription.Id );

                if( prescription != null && prescription.Valid )
                {
                    SearchResult.Add( prescription );
                }

                return SearchResult;
            }
            
            SearchResult = PatientsManager.GetAllPrescriptions();
            if( protoPrescription.Name.Length > 0                               ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Name.ToUpper().Contains( protoPrescription.Name.ToUpper()     ) );
            if( protoPrescription.Remark.Length > 0                             ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Remark.ToUpper().Contains( protoPrescription.Remark.ToUpper() ) );
            if( protoPrescription.Patient != null                               ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Patient.Id == protoPrescription.Patient.Id                    );
            if( protoPrescription.Prescriber != null                            ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Prescriber.Id == protoPrescription.Prescriber.Id              );
            if( EditPrescription.EndBefore != DatabaseObject.INVALID_DATETIME   ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.EndDate.Date < EditPrescription.EndBefore.Date                );
            if( EditPrescription.StartAfter != DatabaseObject.INVALID_DATETIME  ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.StartDate.Date > EditPrescription.StartAfter.Date             );
            if( protoPrescription.Medications.Count > 0 )
            {
                // Make sure that all medications are in the target
                IList<Medication> medications = protoPrescription.Medications;
                SearchResult  = ManagerHelper.Filter( SearchResult , e =>
                {
                    int found = 0;

                    foreach( Medication protoMed in medications )
                        foreach( Medication med in e.Medications )
                            if( protoMed.Id == med.Id )
                                found++;

                    return found == medications.Count;
                });
            };

            return SearchResult;
        }
    }
}
