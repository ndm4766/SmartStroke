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
        //private bool registerUsernameReady;
        //private bool registerPasswordReady;
        //private bool registerConfirmPasswordReady;
        private string registerUsername;
        private string registerPassword;
        private string registerConfirmPassword;

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
            saveToRegisteredUsersFile(registeredUsersFile);
        }
        async private void loadAccounts()
        {
            await loadRegisteredUsersFile(registeredUsersFile);
        }
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
            if (registerPassword == null 
                || registerUsername == null 
                || registerConfirmPassword == null) return;
            if (registerPassword != registerConfirmPassword) return;
            bool usernameAlreadyExists = false;
            foreach (KeyValuePair<string, string> K in usernameHashDictionary)
            {
                if (K.Key == registerUsername) usernameAlreadyExists = true;
            }
            if (usernameAlreadyExists) return;
            usernameHashDictionary.Add(
                registerUsername,registerPassword.GetHashCode().ToString());
            saveAccounts();
            InfoPasser passer = new InfoPasser(registerUsernameInputText.Text);
            this.Frame.Navigate(typeof(PatientSelection), passer);
        }
        private void registerUsernameChanged(object sender, RoutedEventArgs e)
        {
            registerUsername = registerUsernameInputText.Text;
        }
        private void registerPasswordChanged(object sender, RoutedEventArgs e)
        {
            registerPassword = registerPasswordInputText.Text;
        }
        private void registerConfirmPasswordChanged
            (object sender, RoutedEventArgs e)
        {
            registerConfirmPassword = registerConfirmPasswordInputText.Text;
        }
        #endregion

        private void pageTitle_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
