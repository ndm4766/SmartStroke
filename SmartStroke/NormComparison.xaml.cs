﻿using SmartStroke.Common;
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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SmartStroke
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NormComparison : Page
    {

        const string filename = "userInfo.txt";
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public class Performance
        {
            public int Age { get; set; }
            public double Time { get; set; }
        }

        public Windows.Data.Json.JsonArray patients = new Windows.Data.Json.JsonArray();

        public NormComparison()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.Loaded += NormComparisonPage_Loaded;
            //loadJson();
        }

        

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
        }

        void NormComparisonPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
        }

        private void LoadChartContents()
        {

            loadJson();

            Random rand = new Random();
            
            List<Performance> lowEducationResults = new List<Performance>();
            List<Performance> highEducationResults = new List<Performance>();
            List<Performance> uniquePoints = new List<Performance>();

            for (int i = 0; i < 100; i++)
            {
                int j = rand.Next(15,90);
                double y = j * 0.98 + 204;
                lowEducationResults.Add(new Performance() { Age = j, Time = rand.NextDouble() * 14 + y });
            }

            for (int i = 0; i < 100; i++)
            {
                int j = rand.Next(15, 90);
                double y = j * 0.82 + 200;
                highEducationResults.Add(new Performance() { Age = j, Time = rand.NextDouble() * 11 + y });
            }

            uniquePoints.Add(new Performance() { Age = 55, Time = 260 });

            (ScatterChart.Series[0] as ScatterSeries).ItemsSource = lowEducationResults;
            (ScatterChart.Series[1] as ScatterSeries).ItemsSource = highEducationResults;
            (ScatterChart.Series[2] as ScatterSeries).ItemsSource = uniquePoints;

        }

        public class PatientPlot {

            public string patientName;
            public string patientBirthday;
            public string patientAge;
            public string patientSex;
            public string patientEducation;

            public PatientPlot(string name, string birthday, string age, string sex, string education)
            {

                patientName = name;
                patientBirthday = birthday;
                patientAge = age;
                patientSex = sex;
                patientEducation = education;

            }

        };

        async void loadJson()
        {
            try
            {
                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                //read
                String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);

                patients = Windows.Data.Json.JsonArray.Parse(data);

                for (uint i = 0; i < patients.Count; i++)
                {

                    string patientName = patients.GetObjectAt(i).GetNamedValue("Name").GetString();
                    string patientBirthday = patients.GetObjectAt(i).GetNamedValue("Birthday").GetString();
                    string patientAge = patients.GetObjectAt(i).GetNamedValue("Age").GetString();
                    string patientSex = patients.GetObjectAt(i).GetNamedValue("Sex").GetString();
                    string patientEducation = patients.GetObjectAt(i).GetNamedValue("Education").GetString();

                    PatientPlot patient = new PatientPlot(patientName, patientBirthday, patientAge, patientSex, patientEducation);
                
                }

            }
            catch
            {
                //json load failed
            }
        }

        #endregion
    }
}
