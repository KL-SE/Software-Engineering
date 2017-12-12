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
using OverSurgerySystem.Manager;
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.UI.Pages;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.TestResults
{
    /// <summary>
    /// Interaction logic for FindTestResult.xaml
    /// </summary>
    public partial class FindTestResult : FinderPage<TestResult>
    {
        public FindTestResult()
        {
            InitializeComponent();
            Loaded             += OnLoad;
            BackButton.Click   += (object o, RoutedEventArgs e) => OnCancel?.Invoke();
            ResetButton.Click  += (object o, RoutedEventArgs e) =>
            {
                App.GoToFindTestResultPage( LastPrototype );
                EditTestResult.OnConfirm    = OnFind;
                EditTestResult.OnCancel     = () => App.GoToPage( this );
            };
        }

        public override string[] GetData( TestResult test )
        {
            return new string[4]
            {
                test.StringId,
                test.Name,
                test.Description,
                test.CreatedOn.ToShortDateString()
            };
        }

        public override void SetEventHandler()
        { 
            EditTestResult.OnConfirm    = OnFound;
            EditTestResult.OnCancel     = () => App.GoToPage( this );
        }

        public void OnLoad( object o , RoutedEventArgs e )
        {
            Populate( ResultsList );
        }

        public static List<TestResult> FindFromPrototype( TestResult protoTestResult )
        {
            LastPrototype = protoTestResult;
            if( protoTestResult.Valid )
            {
                SearchResult.Clear();
                TestResult TestResult = PatientsManager.GetTestResult( protoTestResult.Id );

                if( TestResult != null && TestResult.Valid )
                {
                    SearchResult.Add( TestResult );
                }

                return SearchResult;
            }
            
            SearchResult = PatientsManager.GetAllTestResults();
            if( protoTestResult.Patient != null                                 ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Patient.Id == protoTestResult.Patient.Id                                          );
            if( protoTestResult.MedicalLicenseNo.Length > 0                     ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.MedicalLicenseNo.ToUpper().Contains(  protoTestResult.MedicalLicenseNo.ToUpper()  ) );
            if( protoTestResult.Name.Length > 0                                 ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Name.ToUpper().Contains(              protoTestResult.Name.ToUpper()              ) );
            if( protoTestResult.Description.Length > 0                          ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Description.ToUpper().Contains(       protoTestResult.Description.ToUpper()       ) );
            if( protoTestResult.Result.Length > 0                               ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Result.ToUpper().Contains(            protoTestResult.Result.ToUpper()            ) );
            if( protoTestResult.Remark.Length > 0                               ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Remark.ToUpper().Contains(            protoTestResult.Remark.ToUpper()            ) );
            if( EditTestResult.DateBefore != DatabaseObject.INVALID_DATETIME    ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.CreatedOn.Date <= EditTestResult.DateBefore                                       );

            return SearchResult;
        }
    }
}
