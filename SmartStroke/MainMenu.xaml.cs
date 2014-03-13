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
    public sealed partial class MainMenu : Page
    {
        InfoPasser passer;

        public MainMenu()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            passer = e.Parameter as InfoPasser;
        }

        private void trails_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TrailsMenu), passer);
        }
        private void test_selection_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TestSelection), passer);
        }
        private void logout(object sender, RoutedEventArgs e)
        {
            passer = new InfoPasser();
            passer.currentPatient = null;
            this.Frame.Navigate(typeof(MainPage), passer);
        }
        private void clock_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ClockTest), passer);
        }
    }
}
