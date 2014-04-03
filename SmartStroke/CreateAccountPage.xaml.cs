using SmartStroke.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SmartStroke
{
    public sealed partial class CreateAccountPage : Page
    {
        private const string registeredUsersFile = "registeredUsers.txt";
        private Dictionary<string, string> usernameHashDictionary;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        public CreateAccountPage()
        {
            usernameHashDictionary = new Dictionary<string, string>();
            loadAccounts();
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }
        
        // Converts username/pass dictionary to string
        private string convertRegisteredUsersToString()
        {
            string toReturn = "";
            foreach (KeyValuePair<string, string> K in usernameHashDictionary)
            {
                toReturn += K.Key;
                toReturn += '\t';
                toReturn += K.Value;
                toReturn += '\n';
            }
            return toReturn;
        }

        // Writes current username/pass dictionary to file
        private async Task saveToRegisteredUsersFile(string testFilename)
        {
            StorageFile testStorageFile;
            string stringToSave = convertRegisteredUsersToString();
            try
            {
                Task<StorageFile> fileTask = ApplicationData
                    .Current.LocalFolder.CreateFileAsync(testFilename,
                    CreationCollisionOption.ReplaceExisting)
                    .AsTask<StorageFile>();
                fileTask.Wait();
                testStorageFile = fileTask.Result;
                await FileIO.WriteTextAsync(testStorageFile, stringToSave);
            }
            catch { return; }
        }
        async private void saveAccounts()
        {
            await saveToRegisteredUsersFile(registeredUsersFile);
        }

        // Clears username/password hash dictionary before loading users
        async private void loadAccounts()
        {
            usernameHashDictionary.Clear();
            await loadRegisteredUsersFile(registeredUsersFile);
        }

        // Loads and parses user/password combination file
        private async Task loadRegisteredUsersFile(string testFilename)
        {
            StorageFile registeredUsersStorageFile;
            string registeredUsersString = "";
            try
            {
                Task<StorageFile> fileTask = ApplicationData
                        .Current.LocalFolder
                        .GetFileAsync(testFilename).AsTask<StorageFile>();
                fileTask.Wait();
                registeredUsersStorageFile = fileTask.Result;
                registeredUsersString 
                    = await FileIO.ReadTextAsync(registeredUsersStorageFile);
            }
            catch { return; }
            parseRegisteredUsersFile(registeredUsersString);
        }

        // Loads user/password combinations from string
        private void parseRegisteredUsersFile(string registeredUsersText)
        {
            List<string> testStrings =
                registeredUsersText.Split('\n').Cast<string>()
                .ToList<string>();
            foreach (string line in testStrings)
            {
                if (line.Split('\t').Length > 1)
                {
                    string username = line.Split('\t')[0];
                    string passHash = line.Split('\t')[1];
                    usernameHashDictionary.Add(username, passHash);
                }
            }
        }
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }
        private void registerButtonClicked(object sender, RoutedEventArgs e)
        {
            if (registerPasswordInputText.Password == null
                || registerUsernameInputText.Text == null
                || registerConfirmPasswordInputText.Password == null) return;
            if (registerPasswordInputText.Password != 
                registerConfirmPasswordInputText.Password) return;
            bool usernameAlreadyExists = false;
            foreach (KeyValuePair<string, string> K in usernameHashDictionary)
            {
                if (K.Key == registerUsernameInputText.Text) 
                    usernameAlreadyExists = true;
            }
            if (usernameAlreadyExists) return;
            usernameHashDictionary.Add(
                registerUsernameInputText.Text, 
                registerPasswordInputText.Password.GetHashCode().ToString());
            saveAccounts();
            InfoPasser passer = new InfoPasser(registerUsernameInputText.Text);
            this.Frame.Navigate(typeof(PatientSelection), passer);
        }
        #endregion

        private void pageTitle_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
