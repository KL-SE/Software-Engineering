using OverSurgerySystem.Core.Staffs;
using OverSurgerySystem.UI.Core;
using OverSurgerySystem.UI.Pages.Staffs;
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
    /// Interaction logic for ManageStaffs.xaml
    /// </summary>
    public partial class ManageStaffs : Page
    {
        public ManageStaffs()
        {
            InitializeComponent();
            MainMenuButton.Click    += ( object sender , RoutedEventArgs e ) => App.GoToMainMenu();
            AddStaffButton.Click    += HandleAddStaff;

            FindStaffButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindStaffPage();
                FindStaff.OnFind    = OnFindStaff;
                FindStaff.OnFound   = null;
                FindStaff.OnCancel  = OnCancel;
                FindStaff.OnSelect  = HandleSelectStaff;
                App.SetTitle( "Manage Staffs | Find" );
            };

            if( !Receptionist.IsAdmin( App.LoggedInStaff ) )
            {
                AddStaffText.Text = "Edit Your Detail";
            }

            App.SetTitle( "Manage Staffs" );
        }

        public static void HandleAddStaff( object sender , RoutedEventArgs e )
        {
            if( Permission.CanAddStaffs )
            {
                // Add a new staff
                App.GoToAddStaffPage();
                EditStaff.OnConfirm = null;
                EditStaff.OnCancel  = OnCancel;
                App.SetTitle( "Manage Staffs | Add" );
            }
            else
            {
                // Edit the currently logged in staff.
                EditStaff.OnConfirm = null;
                EditStaff.OnCancel  = () => App.GoToMainMenu();
                App.GoToEditStaffPage( App.LoggedInStaff , EditStaff.Edit | EditStaff.Restricted );
                App.SetTitle( "Manage Staffs | Edit" );
            }
        }

        // Enter edit staff page when a staff is selected from the list.
        // After we stared interacting with inner pages, backing out from the editor page now goes back to the results page instead.
        public void HandleSelectStaff( Staff staff )
        {
            if( staff.Id == App.LoggedInStaff.Id || Permission.CanEditOtherStaffs )
            {
                App.SetTitle( "Manage Staffs | Edit" );
                App.GoToEditStaffPage( staff , EditStaff.Edit | EditStaff.Restricted );
            }
            else
            {
                App.SetTitle( "Manage Staffs | View" );
                App.GoToEditStaffPage( staff , EditStaff.View | EditStaff.BackOnly );
            }
        }
        
        // Go back to main menu if the action is cancelled 
        public static void OnCancel()
        {
            EditStaff.Reset();
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += NavigateToMenu;
        }

        public static void NavigateToMenu( object i , RoutedEventArgs a )
        { 
            App.GoToManageStaffs();
            MainMenu.Instance.Loaded -= NavigateToMenu;
        }

        // Navigate to result page after finding a staff.
        public static void OnFindStaff( Staff staff )
        {
            FindStaff.FindFromPrototype( staff );
            App.GoToFindStaffResultPage();
        }
    }
}
