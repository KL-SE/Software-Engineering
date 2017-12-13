using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.Core;
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

namespace OverSurgerySystem.UI.Pages.Appointments
{
    /// <summary>
    /// Interaction logic for EndAppointment.xaml
    /// </summary>
    public partial class EndAppointment : Page
    {
        public static Action OnConfirm;
        public static Action OnCancel;

        public EndAppointment()
        {
            InitializeComponent();
            CancelButton.Click  += (object o, RoutedEventArgs e) => OnCancel?.Invoke();
            ConfirmButton.Click += (object o, RoutedEventArgs e) => OnConfirm?.Invoke();
        }
    }
}
