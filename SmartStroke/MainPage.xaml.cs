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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //2B | !2B
            //Laramie's test commit
            //the best comment
            this.InitializeComponent();
            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Portrait;

        }

        private void trails_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TrailsMenu));
        }

        private void info_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage));
        }
        private void test_selection_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TestSelection));
        }
    }
}
