using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;
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
    public sealed partial class MainPage : Page
    {
        Dictionary<string, string> usernameHashDictionary;
        public MainPage()
        {
            this.InitializeComponent();
            usernameHashDictionary = new Dictionary<string, string>();
            loadAccounts();
        }
        // Debug as WIPTTE clicked
        private void WIPTTE_Click(object sender, RoutedEventArgs e)
        {
            InfoPasser passer = new InfoPasser("WIPPTE");
            this.Frame.Navigate(typeof(PatientSelection), passer);
        }

        // Login button clicked
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string user = userId.Text;
            string pass = userPassword.Password;
            if (user == "" || pass == "") return;
            loadAccounts();
            foreach (KeyValuePair<string, string> K in usernameHashDictionary)
            {
                if(K.Key == user && 
                    K.Value == pass.GetHashCode().ToString())
                    this.Frame.Navigate(typeof(PatientSelection), 
                        new InfoPasser(user));
            }
            usernameHashDictionary.Clear();
        }
        async private void loadAccounts()
        {
            await loadRegisteredUsersFile("registeredUsers.txt");
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
        private void createAccount_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateAccountPage));
        }
    }
}
