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
using Windows.Storage;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SmartStroke
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NormComparison : Page
    {

        const string filename = "userInfo.txt";
        InfoPasser passer = new InfoPasser();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        List<PatientPlot> patientList = new List<PatientPlot>();
        List<string> fileNames;
        private TestReplay testReplay;

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

            fileNames = new List<string>();
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
            passer = e.Parameter as InfoPasser;
            testReplay = new TestReplay();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        void NormComparisonPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadChartContents();
        }

        private async void LoadChartContents()
        {
            await loadJson();
            Random rand = new Random();
            
            List<Performance> allResults = new List<Performance>();

            List<double> testTimes = new List<double>();
            List<double> patientAges = new List<double>();

            // Go through all the patients and display the complete data.
            // Do not separate into different categories.
            for (int i = 0; i < patientList.Count; i++)
            {
                testReplay = new TestReplay();
                // Find the file that corresponds with this patient name and load it
                foreach( string name in fileNames)
                {
                    if(name.Contains(patientList[i].patientName))
                    {
                        await testReplay.loadTestReplay(name);
                        var actions = testReplay.getTestActions();
                        if (actions.Count < 1) continue;

                        DateTime start = actions[0].getStartTime();
                        DateTime end = actions[actions.Count - 1].getEndTime();
                        TimeSpan TimeDifference = end - start;

                        double seconds = TimeDifference.Minutes * 60 + TimeDifference.Seconds + TimeDifference.Milliseconds / 100;
                        int tempAge = Convert.ToInt32(patientList[i].patientAge);

                        testTimes.Add(seconds);
                        patientAges.Add(tempAge);

                        allResults.Add(new Performance() { Age = tempAge, Time = seconds });
                        
                        break;
                    }
                }
                
            }

            testTimes.Sort();
            patientAges.Sort();

            double medTestTime = 0;
            double medAge = 0;

            if (testTimes.Count > 0)
            {

                if (testTimes.Count % 2 == 0)
                {
                    double tempDouble = Convert.ToDouble(testTimes.Count);
                    int topIndex = Convert.ToInt32(Math.Floor(tempDouble) / 2);
                    medTestTime = ((testTimes[topIndex - 1] + testTimes[topIndex]) / 2.0);
                }
                else
                {
                    medTestTime = testTimes[testTimes.Count / 2];
                }

                if (patientAges.Count % 2 == 0)
                {
                    double tempDouble = Convert.ToDouble(patientAges.Count);
                    int topIndex = Convert.ToInt32(Math.Floor(tempDouble) / 2);
                    medAge = ((patientAges[topIndex - 1] + patientAges[topIndex]) / 2.0);
                }
                else
                {
                    medAge = Convert.ToDouble(patientAges[patientAges.Count / 2]);
                }

                double avgTestTime = testTimes.Average();
                double avgAge = patientAges.Average();

                double minTestTime = testTimes[0];
                double minAge = patientAges[0];

                double maxTestTime = testTimes[testTimes.Count - 1];
                double maxAge = patientAges[patientAges.Count - 1];

            }

            else
            {
                //no data, do nothing
            }

            (ScatterChart.Series[0] as ScatterSeries).ItemsSource = allResults;

        }

        private void RecreateChart(string way)
        {

            Random rand = new Random();

            List<Performance> lowEducationResults = new List<Performance>();
            List<Performance> highEducationResults = new List<Performance>();
            List<Performance> uniquePoints = new List<Performance>();

            // Go through all the patients and display the complete data.
            // Do not separate into different categories.
            for (int i = 0; i < patientList.Count; i++)
            {

                int tempAge = Convert.ToInt32(patientList[i].patientAge);

                if (patientList[i].patientEducation == "Highschool Diploma")
                {

                    int j = rand.Next(tempAge - 5, tempAge + 5);
                    double y = j * 0.82 + 200;
                    lowEducationResults.Add(new Performance() { Age = tempAge, Time = rand.NextDouble() * 14 + y });

                }
                else
                {

                    int j = rand.Next(tempAge - 5, tempAge + 5);
                    double y = j * 0.98 + 204;
                    highEducationResults.Add(new Performance() { Age = tempAge, Time = rand.NextDouble() * 11 + y });

                }

            }


            for (int i = 0; i < 100; i++)
            {
                int j = rand.Next(15, 90);
                double y = j * 0.98 + 204;
                lowEducationResults.Add(new Performance() { Age = j, Time = rand.NextDouble() * 14 + y });
            }

            for (int i = 0; i < 100; i++)
            {
                int j = rand.Next(15, 90);
                double y = j * 0.82 + 200;
                highEducationResults.Add(new Performance() { Age = j, Time = rand.NextDouble() * 11 + y });
            }

            //uniquePoints.Add(new Performance() { Age = 55, Time = 260 });

            //(ScatterChart.Series[0] as ScatterSeries).ItemsSource = lowEducationResults;
            //(ScatterChart.Series[1] as ScatterSeries).ItemsSource = highEducationResults;
            //(ScatterChart.Series[2] as ScatterSeries).ItemsSource = uniquePoints;

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

        async Task loadJson()
        {
            try
            {
                // Clear all the fileNames in the directory
                fileNames.Clear();
                var names = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync();
                foreach ( var name in names )
                {
                    // The fileName starts with a number
                    if(name.Name[0] >= 48 && name.Name[0] <= 58)
                        fileNames.Add(name.Name);
                }

                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                //read
                String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);
                Windows.Data.Json.JsonArray.TryParse(data, out patients);

                for (uint i = 0; i < patients.Count; i++)
                {

                    string patientName = patients.GetObjectAt(i).GetNamedValue("Name").GetString();
                    string patientBirthday = patients.GetObjectAt(i).GetNamedValue("Birthday").GetString();
                    string patientBday = patients.GetObjectAt(i).GetNamedValue("Birthday").GetString();
                    string patientSex = patients.GetObjectAt(i).GetNamedValue("Sex").GetString();
                    string patientEducation = patients.GetObjectAt(i).GetNamedValue("Education").GetString();

                    DateTime today = DateTime.Today;
                    DateTime bday = Convert.ToDateTime(patientBday);

                    int age = today.Year - bday.Year;
                    if (bday > today.AddYears(-age)) age--;

                    string patientAge = Convert.ToString(age);

                    PatientPlot patient = new PatientPlot(patientName, patientBirthday, patientAge, patientSex, patientEducation);
                    patientList.Add(patient);
                    
                }

            }
            catch
            {
                //json load failed
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        #endregion

        private void chartOptions(object sender, RoutedEventArgs e)
        {

            var popup = new Windows.UI.Popups.PopupMenu();
            popup.ShowAsync(new Point(0,0));
        
        }
    }
}
