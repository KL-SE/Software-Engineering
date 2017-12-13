using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.UI.Core;
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
            Loaded                     += OnLoad;
            MainMenuButton.Click       += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddTestResultButton.Click  += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToEditTestResultPage( null , EditTestResult.Edit | EditTestResult.Restricted );
                EditTestResult.OnConfirm    = null;
                EditTestResult.OnCancel     = OnCancel;
                App.SetTitle( "Manage Test Results | Add" );
            };

            FindTestResultButton.Click += HandleFindTest;
            App.SetTitle( "Manage Test Results" );
        }

        public void OnLoad( object sender , EventArgs e )
        {
            if( !Permission.CanAddTestResults )
            {
                HandleFindTest( null , null );
            }
        }

        public static void HandleFindTest( object sender , RoutedEventArgs e )
        {
            App.GoToFindTestResultPage();
            FindTestResult.OnFind   = OnFindTestResult;
            FindTestResult.OnFound  = null;
            FindTestResult.OnCancel = OnCancel;
            FindTestResult.OnSelect = HandleSelectTestResult;
            App.SetTitle( "Manage Test Results | Find" );
        }
        
        public static void HandleSelectTestResult( TestResult test )
        {
            if( Permission.CanEditTestResults )
            {
                App.SetTitle( "Manage Test Results | Edit" );
                App.GoToEditTestResultPage( test , EditTestResult.Edit | EditTestResult.Restricted );
            }
            else
            {
                App.SetTitle( "Manage Test Results | View" );
                App.GoToEditTestResultPage( test , EditTestResult.View | EditTestResult.BackOnly );
            }
        }
        
        public static void OnCancel()
        {
            EditTestResult.Reset();
            App.GoToMainMenu();
            if( Permission.CanAddTestResults )
            {
                MainMenu.Instance.Loaded += NavigateToMenu;
            }
        }

        public static void NavigateToMenu( object i , RoutedEventArgs a )
        { 
            App.GoToManageTestResults();
            MainMenu.Instance.Loaded -= NavigateToMenu;
        }
        
        public static void OnFindTestResult( TestResult test )
        {
            FindTestResult.FindFromPrototype( test );
            App.GoToFindTestResultListPage();
        }
    }
}
