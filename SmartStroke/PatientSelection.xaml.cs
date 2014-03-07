using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartStroke
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientSelection : Page
    {
        private InfoPasser passer;
        public PatientSelection()
        {
            this.InitializeComponent();
        }

        // Add a new Patient
        private void newPatient(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage), passer);
        }

        // Doctor has started typing a patient's name in.. try to match the name
        // and output possible patients for him/her to select
        private void searchPatients(object sender, TextChangedEventArgs e)
        {

        }
        
        // View the norms without a patient data.
        private void viewNorms(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NormComparison), "patientName:none, doctorName:" + passer.doctorId);
        }

        // Get the doctor's name
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as InfoPasser;
            greeting.Text = "Howdy, " + passer.doctorId;
        }

        
    }
}
