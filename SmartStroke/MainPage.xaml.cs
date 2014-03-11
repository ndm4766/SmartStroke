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

        DispatcherTimer timer;  // Timer to fire after 1 second to send to the next screen
        public MainPage()
        {
            //2B | !2B
            //Laramie's test commit
            //the best comment
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        // Auto-send the user to the User login page.. used for WIPPTE so people dont need to sign in
        private void tick(object sender, object e)
        {
            timer.Stop();
            this.Frame.Navigate(typeof(PatientSelection), "WIPPTE");
        }

        private async void protect()
        {
            // Initialize function arguments.
            String strMsg = userId.Text + " " + userPassword.Password;
            String strDescriptor = "LOCAL=user";
            BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;

            // Protect a message to the local user.
            IBuffer buffProtected = await this.SampleProtectAsync(
                strMsg,
                strDescriptor,
                encoding);

            // Decrypt the previously protected message.
            String strDecrypted = await this.SampleUnprotectData(
                buffProtected,
                encoding);
        }

        // Log in button clicked.
        // Authenticate the doctor.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the patient selection screen - send the doctor
            // name for patient authentication
            string user = userId.Text;
            string pass = userPassword.Password;
            if (user == "User Id" || pass == userId.PlaceholderText) { return; }

            protect();

            this.Frame.Navigate(typeof (PatientSelection), userId.Text);
        }


        // Encrypt the user name and password combination
        public async Task<IBuffer> SampleProtectAsync(String strMsg, String strDescriptor,BinaryStringEncoding encoding)
        {
            // Create a DataProtectionProvider object for the specified descriptor.
            DataProtectionProvider Provider = new DataProtectionProvider(strDescriptor);

            // Encode the plaintext input message to a buffer.
            encoding = BinaryStringEncoding.Utf8;
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(strMsg, encoding);

            // Encrypt the message.
            IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);

            // Execution of the SampleProtectAsync function resumes here
            // after the awaited task (Provider.ProtectAsync) completes.
            return buffProtected;
        }

        // Un-encrypt the data
        public async Task<String> SampleUnprotectData(IBuffer buffProtected, BinaryStringEncoding encoding)
        {
            // Create a DataProtectionProvider object.
            DataProtectionProvider Provider = new DataProtectionProvider();

            // Decrypt the protected message specified on input.
            IBuffer buffUnprotected = await Provider.UnprotectAsync(buffProtected);

            // Execution of the SampleUnprotectData method resumes here
            // after the awaited task (Provider.UnprotectAsync) completes
            // Convert the unprotected message from an IBuffer object to a string.
            String strClearText = CryptographicBuffer.ConvertBinaryToString(encoding, buffUnprotected);

            // Return the plaintext string.
            return strClearText;
        }

        // ToDo: Create a new account for a doctor
        private void createAccount_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
