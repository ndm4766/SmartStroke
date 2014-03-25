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
    public sealed partial class CreateAccountPage : Page
    {
        private bool registerUsernameReady;
        private bool registerPasswordReady;
        private bool registerConfirmPasswordReady;
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
        private void registerButtonClicked(object sender, RoutedEventArgs e)
        {
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
