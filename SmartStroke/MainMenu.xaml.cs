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
        private NavigationEventArgs args;
        public MainMenu()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            args = e;
            passer = e.Parameter as InfoPasser;

            // Check to disable any buttons
            List<string> fileNames = passer.currentPatient.getTestFilenames();
            // Check if the file name includes trailsA, trailsB, or Clock and allow button press
            foreach (string name in fileNames)
            {
                if (name.Contains("TRAILS_A"))
                {
                    viewTrailsA.IsEnabled = true;
                }
                else if (name.Contains("TRAILS_B"))
                {
                    viewTrailsB.IsEnabled = true;
                }
                else if (name.Contains("CLOCK"))
                {
                    viewClock.IsEnabled = true;
                }
                else if (name.Contains("REY_OSTERRIETH"))
                {
                    viewReyo.IsEnabled = true;
                }
            }
        }

        private void trailsA_click(object sender, RoutedEventArgs e)
        {
            InfoPasser passer = args.Parameter as InfoPasser;
            passer.trailsTestVersion = 'A';
            this.Frame.Navigate(typeof(TrailsTestInstruction), passer);
        }

        private void trailsB_click(object sender, RoutedEventArgs e)
        {
            InfoPasser passer = args.Parameter as InfoPasser;
            passer.trailsTestVersion = 'B';
            this.Frame.Navigate(typeof(TrailsTestInstruction), passer);
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
            this.Frame.Navigate(typeof(ClockTestInstruction), passer);
        }

        private void reyo_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ReyOInstruction), passer);
        }

        private void trailsAOld(object sender, RoutedEventArgs e)
        {
            passer.trailsTestVersion = 'A';
            this.Frame.Navigate(typeof(TrailsTestTimeViz), passer);
        }

        private void trailsBOld(object sender, RoutedEventArgs e)
        {
            passer.trailsTestVersion = 'B';
            this.Frame.Navigate(typeof(TrailsTestTimeViz), passer);
        }

        private void view_old_clock_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ClockTestReplay), passer);
        }

        private void view_old_reyo_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ReyoTestReplay), passer);
        }


    }
}
