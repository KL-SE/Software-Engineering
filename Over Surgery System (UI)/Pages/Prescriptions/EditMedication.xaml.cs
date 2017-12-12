using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.Manager;
using OverSurgerySystem.UI.Pages.Core;
using OverSurgerySystem.Core.Patients;

namespace OverSurgerySystem.UI.Pages.Patients
{
    /// <summary>
    /// Interaction logic for EditMedication.xaml
    /// </summary>
    public partial class EditMedication : EditorPage<Medication>
    {
        public static List<Medication> MedicationList = new List<Medication>();

        public EditMedication()
        {
            InitializeComponent();
            Loaded += OnLoad;

            CodeBox.TextChanged    += (object o, TextChangedEventArgs e) => HideMessage();
            NameBox.TextChanged    += (object o, TextChangedEventArgs e) => HideMessage();
            CodeBox.StoppedTyping  += (object o, EventArgs e) => UpdateMedications();
            NameBox.StoppedTyping  += (object o, EventArgs e) => UpdateMedications();
            CodeClearButton.Click  += (object o, RoutedEventArgs a) => CodeBox.Text = "";
            NameClearButton.Click  += (object o, RoutedEventArgs a) => NameBox.Text = "";
            ConfirmButton.Click    += (object o, RoutedEventArgs a) => DoConfirm();
            CancelButton.Click     += (object o, RoutedEventArgs a) => DoCancel();

            if( IsView )
            {
                CodeBox.Visibility          = Visibility.Collapsed;
                NameBox.Visibility          = Visibility.Collapsed;
                ConfirmButton.Visibility    = Visibility.Collapsed;

                CancelButtonImg.Source      = new BitmapImage( new Uri( "pack://application:,,,/Over Surgery System (UI);component/Resources/main_menu.png" ) );
                CancelButtonText.Text       = "Back";
            }
        }

        private void OnLoad( object o , EventArgs e )
        {
            if( CurrentItem != null )
            {
                CodeBox.Text = CurrentItem.Code;
                NameBox.Text = CurrentItem.Name;
            }
            UpdateMedications();
        }

        private void HideMessage()
        {
            MsgBox.Visibility = Visibility.Collapsed;
        }

        private void ShowMessage( string error_message )
        {
            MsgBox.Visibility = Visibility.Visible;
            MsgBox.Text       = error_message;
        }

        private void UpdateMedications()
        {
            if( CodeBox.Text.Length > 0 )
            {
                List<Medication> exactMeds = PatientsManager.GetMedicationByCode( CodeBox.Text );
                MedicationList.Clear();

                if( exactMeds.Count == 1 )
                {
                    MedicationList.Add( exactMeds[0] );
                }
            }
            else
            {
                MedicationList = PatientsManager.GetMedicationByName( NameBox.Text );
            }
            
            if( MedicationList.Count == 0 )
            {
                MedicationResult.IsEnabled          = false;
                MedicationResultHeader.Header       = "- Not Found -";
                MedicationResultHeader.ItemsSource  = new List<string>();
                CurrentItem                         = null;
            }
            else
            {
                List<MenuItem> list         = new List<MenuItem>();
                MedicationResult.IsEnabled  = MedicationList.Count > 1;
                
                foreach( Medication medication in MedicationList )
                {
                    string itemName     = String.Format( "{0} - {1}" , medication.Code , medication.Name );
                    MenuItem menuItem   = new MenuItem();
                    menuItem.Header     = itemName;
                    menuItem.Click     += ( object o , RoutedEventArgs a ) =>
                    {
                        MedicationResultHeader.Header   = String.Format( "{0} - {1}" , medication.Code , medication.Name );
                        CurrentItem                     = medication;
                    };

                    list.Add( menuItem );
                }
                
                MedicationResultHeader.ItemsSource  = list;
                MedicationResultHeader.Header       = String.Format( "{0} - {1}" , MedicationList[0].Code , MedicationList[0].Name );
                CurrentItem                         = MedicationList[0];
            }
        }

        private void DoConfirm()
        {
            if( IsEdit )
            {
                if( CurrentItem == null || !CurrentItem.Valid )
                {
                    CurrentItem         = new Medication();
                    CurrentItem.Code    = CodeBox.Text;
                    CurrentItem.Name    = NameBox.Text;

                    foreach( Medication medication in PatientsManager.GetAllMedications() )
                    {
                        if( medication.Code.Equals( CurrentItem.Code ) )
                        {
                            ShowMessage( "There is already a medication with this code." );
                            return;
                        }
                    }
                }
            
                if( CurrentItem.Code.Length == 0 ) { ShowMessage( "Please enter the medication code."   ); return; }
                if( CurrentItem.Name.Length == 0 ) { ShowMessage( "Please enter the medication name."   ); return; }
            
                CurrentItem.Save();
            }

            OnConfirm?.Invoke( CurrentItem );
        }

        private void DoCancel()
        {
            OnCancel?.Invoke();
        }
    }
}
