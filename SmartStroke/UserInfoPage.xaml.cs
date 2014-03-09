using SmartStroke.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SmartStroke
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        //Windows.Storage.StorageFile file;
        const string filename = "userInfo.txt";
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private string docName;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private bool sexReady;
        private bool educationReady;
        public char sex;
        public int age;
        public DateTime birthday;
        public string educationLevel;
        public string name;
        public Windows.Data.Json.JsonArray patients = new Windows.Data.Json.JsonArray();
        public const int medicalIdLength = 16;

        public UserInfoPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            loadJson();
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
        
        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            var name = e.Parameter as string;
            docName = name;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        // Return an id with the last x characters being the number of the patient.
        private string newID( int numberDigits)
        {
            Random r = new Random();
            string s = "";
            for (int i = 0; i < numberDigits; i++)
            {
                s += r.Next(10).ToString();
            }
            return s;
        }

        async void loadJson()
        {
            try
            {
                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                //read
                String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);

                patients = Windows.Data.Json.JsonArray.Parse(data);
                int numDigits = medicalIdLength - (patients.Count.ToString().Length);
                string prefix = newID(numDigits);
                patientName.Text = prefix + patients.Count.ToString();
            }
            catch
            {
                //json load failed
            }
        }

        async void saveUserData()
        {
            try
            {
                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);

                //read
                //String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);

                //save
                //string allUserDataString = name + ", " + birthday + ", " + age + ", " + sex + ", " + educationLevel;
                Windows.Data.Json.JsonObject newPatient = new Windows.Data.Json.JsonObject();
                newPatient["Doctor"] = Windows.Data.Json.JsonValue.CreateStringValue(docName);
                newPatient["Name"] = Windows.Data.Json.JsonValue.CreateStringValue(name);
                newPatient["Birthday"] = Windows.Data.Json.JsonValue.CreateStringValue(birthday.ToString());
                newPatient["Age"] = Windows.Data.Json.JsonValue.CreateNumberValue(age);
                newPatient["Sex"] = Windows.Data.Json.JsonValue.CreateStringValue(sex.ToString());
                newPatient["Education"] = Windows.Data.Json.JsonValue.CreateStringValue(educationLevel);
                //check if user is already saved
                bool addNew = true;
                for (uint i=0; i < patients.Count; i++){
                    if (patients.GetObjectAt(i).GetNamedValue("Name").GetString() == newPatient["Name"].GetString() && patients.GetObjectAt(i).GetNamedValue("Birthday").GetString() == newPatient["Birthday"].GetString())
                    {
                        //patient info already saved
                        // Create the message dialog and set its content
                        var messageDialog = new MessageDialog("We already have a patient with that name and birthday. Do you want to replace the existing patient?");

                        // Add commands and set their callbacks
                        string jsonPatient = newPatient.Stringify();
                        messageDialog.Commands.Add(new UICommand("Replace Old Patient", new UICommandInvokedHandler(this.replacePatient), jsonPatient));
                        messageDialog.Commands.Add(new UICommand("Keep Original Patient", new UICommandInvokedHandler(this.doNothing)));

                        // Set the command that will be invoked by default
                        messageDialog.DefaultCommandIndex = 0;

                        // Set the command to be invoked when escape is pressed
                        messageDialog.CancelCommandIndex = 1;

                        // Show the message dialog
                        await messageDialog.ShowAsync();
                        addNew = false;
                    }
                }

   
                if(addNew)
                {
                    patients.Add(newPatient);
                }
                string jsonAllPatientsString = patients.Stringify();
                await Windows.Storage.FileIO.WriteTextAsync(myFile, jsonAllPatientsString);
                //await Windows.Storage.FileIO.WriteTextAsync(myFile, ""); //uncomment this to erase database

            }
            catch (FileNotFoundException)
            {
                //file did not exist so create it
                create_file();
            }
        }
        async void create_file()
        {
            //initialize file
            Windows.Storage.StorageFile myFile2 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }

        private void replacePatient(IUICommand command)
        {
            Windows.Data.Json.JsonObject updatedPatient = new Windows.Data.Json.JsonObject();
            string patString = command.Id.ToString();
            updatedPatient = Windows.Data.Json.JsonObject.Parse(patString);
            for (uint i = 0; i < patients.Count; i++)
            {
                if (updatedPatient["Name"].GetString() == patients.GetObjectAt(i).GetNamedValue("Name").GetString() && updatedPatient["Birthday"].GetString() == patients.GetObjectAt(i).GetNamedValue("Birthday").GetString())
                {
                    patients.GetObjectAt(i).SetNamedValue("Education", updatedPatient["Education"]);
                    patients.GetObjectAt(i).SetNamedValue("Sex", updatedPatient["Sex"]);
                }
            }
        }


        private void doNothing(IUICommand command)
        {
        }

        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sexReady && educationReady)//make sure all info is valid
            {
                birthday = birthdayPicker.Date.Date;
                age = DateTime.Today.Year-birthday.Year;
                if (birthday > DateTime.Today.AddYears(-age))
                {
                    age--;
                }
                name = patientName.Text;
                saveUserData();

                // Send the patient data (JSON array) moving forward to the test page
                this.Frame.Navigate(typeof(MainMenu), this);
            }
        }
        private void radioButtonClicked(object sender, RoutedEventArgs e)
        {
            sexReady = true;
            //set sex
            if ((bool)sexM.IsChecked)
            {
                sex = 'M';
            }
            else if ((bool)sexF.IsChecked)
            {
                sex = 'F';
            }
            if (educationReady && patientName.Text!="Patient Name")
            {
                SubmitButton.IsEnabled = true;
            }
        }
        
        private void educationChanged(object sender, RoutedEventArgs e)
        {
            educationReady = true;
            educationLevel = (string)education.SelectedValue;
            if (sexReady && patientName.Text != "Patient Name")
            {
                SubmitButton.IsEnabled = true; 
            }
        }
        private void nameBeingChanged(object sender, RoutedEventArgs e)
        {
            if (sexReady && educationReady && patientName.Text!="")
            {
                SubmitButton.IsEnabled = true;
            }
        }
    }
}
