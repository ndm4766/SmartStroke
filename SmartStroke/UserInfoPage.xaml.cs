using SmartStroke.Common;
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

        public UserInfoPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        async void saveUserData()
        {
            try
            {
                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);

                //read
                String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);

                //save
                string allUserDataString = name + ", " + birthday + ", " + age + ", " + sex + ", " + educationLevel;
                await Windows.Storage.FileIO.WriteTextAsync(myFile, allUserDataString);

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
                NavigationHelper.GoBack();
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
