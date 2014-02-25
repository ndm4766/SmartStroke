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

namespace SmartStroke
{
    public sealed partial class LoginPage : Page
    {
        private bool loginUsernameReady;
        private bool loginPasswordReady;
        private bool registerUsernameReady;
        private bool registerPasswordReady;
        private bool registerConfirmPasswordReady;
        private string loginUsername;
        private string loginPassword;
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
        public LoginPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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
        #endregion
        #region Event Handling
        private void loginUsernameChanged(object sender, RoutedEventArgs e)
        {
            loginUsername = loginUsernameInputText.Text;
        }
        private void loginPasswordChanged(object sender, RoutedEventArgs e)
        {
            loginPassword = loginPasswordInputText.Text;
        }
        private void loginButtonClicked(object sender, RoutedEventArgs e)
        {
            //TODO (not currently supported, priority is saving test data)
            //Josh 2/25/2014
        }
        private void registerButtonClicked(object sender, RoutedEventArgs e)
        {
            //TODO (not currently supported, priority is saving test data)
            //Josh 2/25/2014
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
    }
}
