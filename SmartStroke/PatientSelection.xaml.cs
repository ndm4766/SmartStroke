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
        private string docName = "";
        InfoPasser passer;
        private Windows.Data.Json.JsonArray patients;

        public PatientSelection()
        {
            this.InitializeComponent();
            patients = new Windows.Data.Json.JsonArray();
            loadJson("userInfo.txt");
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
            if (search.Text == "Search for a Patient")
            {
                search.Text = "";
                return;
            }

            // Remove all ids from the listbox
            while(MedicalID.Items.Count > 0)
            {
                MedicalID.Items.RemoveAt(MedicalID.Items.Count-1);
            }

            // Populate listbox with Possible suggestions
            string query = search.Text;
            for (uint i = 0; i < patients.Count; i++)
            {
                var name = patients.GetObjectAt(i).GetNamedString("Name");
                var doctor = patients.GetObjectAt(i).GetNamedString("Doctor");
                if (doctor == docName)
                {
                    if(name.StartsWith(query))
                        MedicalID.Items.Add(name);
                }
            }
        }
        
        // View the norms without a patient data.
        private void viewNorms(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NormComparison), passer);
        }

        // Get the doctor's name
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as InfoPasser;    // This is the name of the doctor.
            docName = passer.doctorId;
            greeting.Text = "Howdy, " + docName;
        }

        // If you click on a patient, send his/her ID and the doctor's name to the testing screen
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MedicalID.SelectedItem != null)
            {
                MedicalID.SelectionChanged -= ListBox_SelectionChanged;
                search.TextChanged -= searchPatients;

                //find the patient that was selected and recreate patient object using information (if patient is not found, it wont navigate but this SHOULD be impossible)
                for (uint i = 0; i < patients.Count; i++)
                {
                    String name = patients.GetObjectAt(i).GetNamedString("Name");
                    String doctor = patients.GetObjectAt(i).GetNamedString("Doctor");
                    if (doctor == docName && name == MedicalID.SelectedItem.ToString())
                    {
                        String birthdayString = patients.GetObjectAt(i).GetNamedString("Birthday");

                        DateTime birthday = Convert.ToDateTime(birthdayString);

                        String genderString = patients.GetObjectAt(i).GetNamedString("Sex");
                        GENDER gender;
                        if (genderString == "M")
                            gender = GENDER.MALE;
                        else
                            gender = GENDER.FEMALE;

                        String educationString = patients.GetObjectAt(i).GetNamedString("Education");
                        EDU_LEVEL edu;
                        if (educationString == "Highschool Diploma")
                            edu = EDU_LEVEL.HIGHSCHOOL;
                        else if (educationString == "Bachelors")
                            edu = EDU_LEVEL.BACHELORS;
                        else if (educationString == "Associates")
                            edu = EDU_LEVEL.ASSOCIATES;
                        else if (educationString == "Masters")
                            edu = EDU_LEVEL.MASTERS;
                        else if (educationString == "PHD")
                            edu = EDU_LEVEL.PHD;
                        else
                            edu = EDU_LEVEL.OTHER;
                        

                        passer.currentPatient = new Patient(name, docName, birthday, gender, edu);
                        passer.currentPatient.loadFiles();
                        this.Frame.Navigate(typeof(MainMenu), passer);
                        return;
                    }
                }
            }
        }

        // Load the file with all the patients and display them for a doctor to pick
        async void loadJson(string filename)
        {
            try
            {
                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                //read
                String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);

                Windows.Data.Json.JsonArray.TryParse(data, out patients);
            }
            catch
            {
                //json load failed
            }
        }

    }
}
