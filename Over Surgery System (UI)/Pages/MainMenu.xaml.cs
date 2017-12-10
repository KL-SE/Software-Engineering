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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        object passed_parameter;
        public MainMenu( object passed_parameter )
        {
            InitializeComponent();
            Loaded += OnLoad;
            this.passed_parameter = passed_parameter;
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
            TestBox.Text = (string) passed_parameter;
        }
    }
}
