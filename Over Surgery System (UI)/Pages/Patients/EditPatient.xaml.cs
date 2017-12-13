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
using OverSurgerySystem.Core.Base;
using OverSurgerySystem.UI.Pages.Common;
using OverSurgerySystem.Core.Patients;
using OverSurgerySystem.UI.Pages.Core;

namespace OverSurgerySystem.UI.Pages.Patients
{
    /// <summary>
    /// Interaction logic for EditPatient.xaml
    /// </summary>
    public partial class EditPatient : EditorPage<Patient>
    {
        private EditDetails CurrentEditor;
        private PersonalDetails CurrentDetail
        {
            get
            {
                return CurrentItem.Details;
            }
        }

        public EditPatient()
        {
            InitializeComponent();
            Loaded                  += OnLoad;
            EditDetails.OnConfirm   = e => DoConfirm( e );
            EditDetails.OnCancel    = DoCancel;
            EditDetails.OnReturn    = () => App.GoToPage( this );
        }

        private void OnLoad( object o , RoutedEventArgs e )
        {
            PatientIdBox.Text = !IsEdit ? "" : "- New Patient -";
            LoadDetails();

            CurrentEditor = new EditDetails();
            DetailsEditor.Navigate( CurrentEditor );
        }

        private void LoadDetails()
        {
            if( CurrentItem != null && CurrentItem.Valid )
            {
                PatientIdBox.Text = CurrentItem.StringId;
            }

            EditDetails.Setup( CurrentDetail , Mode );
            PatientIdBox.IsEnabled = IsFind;
        }

        private int DoConfirm( PersonalDetails d )
        {
            if( IsEdit )
            { 
                try
                {
                    CurrentItem.Save();
                    LoadDetails();
                    CurrentEditor.ShowMessage( "Patient saved." );
                }
                catch
                {
                    CurrentEditor.ShowMessage( "Failed to load data. Please check your connection." );
                }
            }
            else if( IsFind )
            {
                CurrentItem.Id = Patient.GetIdFromString( PatientIdBox.Text );
            }

            OnConfirm?.Invoke( CurrentItem );
            return 0;
        }

        private void DoCancel()
        {
            try
            {
                if( CurrentItem.Valid )
                {
                    CurrentItem.Load();
                }
            }
            catch
            {
                CurrentEditor.ShowMessage( "Failed to load data. Please check your connection." );
            }
            OnCancel?.Invoke();
        }
    }
}
