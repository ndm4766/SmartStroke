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
    public sealed partial class TestSelection : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private InfoPasser passer;


        public TestSelection()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            passer = e.Parameter as InfoPasser;

            // Check to disable any buttons
            List<string> fileNames = passer.currentPatient.getTestFilenames();
            // Check if the file name includes trailsA, trailsB, or Clock and allow button press
            foreach(string name in fileNames)
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        //private void Button_Click(object sender, RoutedEventArgs e){}

        private void select_trails_test(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TrailsMenu), passer);
        }

        private void norm_button_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NormComparison), passer);
        }

        private void select_clock_test(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ClockTest), passer);
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

        private void select_reyo_test(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ReyoTest), passer);
        }
    }
}
