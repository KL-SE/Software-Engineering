using OverSurgerySystem.Core.Staffs;
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
            AddStaffButton.Click    += ( object sender , RoutedEventArgs e ) =>
            {
                // Add a new staff
                App.GoToAddStaffPage();
                EditStaff.OnConfirm = null;
                EditStaff.OnCancel  = OnCancel;
            };

            FindStaffButton.Click += ( object sender , RoutedEventArgs e ) =>
            {
                App.GoToFindStaffPage();
                FindStaff.OnFind    = OnFindStaff;
                FindStaff.OnFound   = null;
                FindStaff.OnCancel  = OnCancel;

                // Enter edit staff page when a staff is selected from the list.
                // After we stared interacting with inner pages, backing out from the editor page now goes back to the results page instead.
                FindStaff.OnSelect = ( Staff staff ) => App.GoToEditStaffPage( staff , EditStaff.Edit );
            };
        }
        
        // Go back to main menu if the action is cancelled 
        public static void OnCancel()
        {
            App.GoToMainMenu();
            MainMenu.Instance.Loaded += ( object i , RoutedEventArgs a ) => App.GoToManageStaffs();
        }
        
        // Navigate to result page after finding a staff.
        public static void OnFindStaff( Staff staff )
        {
            FindStaff.FindFromPrototype( staff );
            App.GoToFindStaffResultPage();
        }
    }
}
