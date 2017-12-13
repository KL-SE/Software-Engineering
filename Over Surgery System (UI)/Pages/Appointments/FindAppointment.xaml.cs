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

namespace OverSurgerySystem.UI.Pages.Appointments
{
    /// <summary>
    /// Interaction logic for FindAppointment.xaml
    /// </summary>
    public partial class FindAppointment : FinderPage<Appointment>
    {
        
        public FindAppointment()
        {
            InitializeComponent();
            Loaded             += OnLoad;
            BackButton.Click   += (object o, RoutedEventArgs e) => OnCancel?.Invoke();
            ResetButton.Click  += (object o, RoutedEventArgs e) =>
            {
                App.GoToEditAppointmentPage( LastPrototype , LastEditMode );
                EditAppointment.OnConfirm   = OnFind;
                EditAppointment.OnCancel    = () => App.GoToPage( this );
            };

            LastEditMode = EditAppointment.Mode;
        }
        
        public override string[] GetData( Appointment appointment )
        {
            return new string[]
            {
                appointment.StringId,
                appointment.Patient.StringId,
                appointment.Remark == null ? "No Remark" : appointment.Remark,
                appointment.DateAppointed.ToString()
            };
        }

        public override void SetEventHandler()
        {
            EditAppointment.OnConfirm   = OnFound;
            EditAppointment.OnCancel    = () => App.GoToPage( this );
        }

        public void OnLoad( object o , RoutedEventArgs e )
        {
            Populate( ResultsList );
        }

        public static List<Appointment> FindFromPrototype( Appointment protoAppointment )
        {
            try
            {
                if( protoAppointment.Valid )
                {
                    SearchResult.Clear();
                    Appointment appointment = PatientsManager.GetAppointment( protoAppointment.Id );

                    if( appointment != null && appointment.Valid )
                    {
                        SearchResult.Add( appointment );
                    }

                    return SearchResult;
                }
            
                SearchResult = PatientsManager.GetAllAppointments();
                if( protoAppointment.Remark.Length > 0                              ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Remark.ToUpper().Contains(   protoAppointment.Remark.ToUpper()    ) );
                if( protoAppointment.Patient != null                                ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.Patient.Id == protoAppointment.Patient.Id                         );
                if( protoAppointment.MedicalStaff != null                           ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.MedicalStaff.Id == protoAppointment.MedicalStaff.Id               );
                if( !protoAppointment.Cancelled                                     ) SearchResult  = ManagerHelper.Filter( SearchResult , e => !e.Cancelled                                                        );
                if( EditAppointment.SelectedDate != DatabaseObject.INVALID_DATETIME ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.DateAppointed.Date == EditAppointment.SelectedDate.Date           );
                if( EditAppointment.DateAfter != DatabaseObject.INVALID_DATETIME    ) SearchResult  = ManagerHelper.Filter( SearchResult , e => e.DateAppointed.Date > EditAppointment.DateAfter.Date               );

                SearchResult.Sort( (c,o) => c.DateAppointed.CompareTo( o.DateAppointed ) );
                LastSearchError = false;
            }
            catch
            {
                LastSearchError = true;
            }

            LastPrototype = protoAppointment;
            return SearchResult;
        }
    }
}
