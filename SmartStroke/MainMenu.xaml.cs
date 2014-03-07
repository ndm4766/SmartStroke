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

        UserInfoPage infoPage;

        public MainMenu()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            infoPage = e.Parameter as UserInfoPage;
        }

        private void trails_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TrailsMenu), infoPage);
        }
        private void test_selection_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TestSelection), infoPage);
        }

        private void clock_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ClockTest), infoPage);
        }
    }
}
