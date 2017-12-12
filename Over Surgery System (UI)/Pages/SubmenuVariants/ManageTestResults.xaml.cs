using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.TestResults;
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

namespace OverSurgerySystem.UI.Pages
{
    /// <summary>
    /// Interaction logic for ManageTestResults.xaml
    /// </summary>
    public partial class ManageTestResults : Page
    {
        public ManageTestResults()
        {
            InitializeComponent();
            MainMenuButton.Click        += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddTestResultButton.Click   += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToAddTestResultPage();
                EditTestResult.OnConfirm   = null;
                EditTestResult.OnCancel    = OnCancel;
            };

            FindTestResultButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindTestResultPage();
                FindTestResult.OnFind   = OnFindTestResult;
                FindTestResult.OnFound  = null;
                FindTestResult.OnCancel = OnCancel;
                FindTestResult.OnSelect = ( TestResult test ) => App.GoToEditTestResultPage( test , EditTestResult.Edit );
            };
        }
        
        public static void OnCancel()
        {
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += ( object i , RoutedEventArgs a ) => App.GoToManageTestResults();
        }
        
        public static void OnFindTestResult( TestResult test )
        {
            FindTestResult.FindFromPrototype( test );
            App.GoToFindTestResultListPage();
        }
    }
}
