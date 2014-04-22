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
        //const string filename = "registeredUsers.txt";
        InfoPasser passer = new InfoPasser();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        List<PatientPlot> patientList = new List<PatientPlot>();
        List<string> fileNames;
        private TestReplay testReplay;
        int chartChoice = -1;

        //Initializes data structures for the different tests
        List<Performance> TrailsA = new List<Performance>();
        List<Performance> TrailsB = new List<Performance>();
        List<Performance> TrailsA_H = new List<Performance>();
        List<Performance> TrailsB_H = new List<Performance>();

        //Initializes the Lists that store the plot points for graphing
        List<PlotPoint> TrailsAGroup = new List<PlotPoint>();
        List<PlotPoint> TrailsBGroup = new List<PlotPoint>();
        List<PlotPoint> TrailsA_HGroup = new List<PlotPoint>();
        List<PlotPoint> TrailsB_HGroup = new List<PlotPoint>();

        List<PlotPoint> avgTrailsAGrouped = new List<PlotPoint>();
        List<PlotPoint> medTrailsAGrouped = new List<PlotPoint>();

        List<PlotPoint> avgTrailsBGrouped = new List<PlotPoint>();
        List<PlotPoint> medTrailsBGrouped = new List<PlotPoint>();

        List<PlotPoint> avgTrailsA_HGrouped = new List<PlotPoint>();
        List<PlotPoint> medTrailsA_HGrouped = new List<PlotPoint>();

        List<PlotPoint> avgTrailsB_HGrouped = new List<PlotPoint>();
        List<PlotPoint> medTrailsB_HGrouped = new List<PlotPoint>();
        //..............................................

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

        //Simplified class for storing patient test data
        public class Performance
        {
            public int Age { get; set; }
            public double Time { get; set; }
            public string Test { get; set; }
        }

        //Class for storing the minimal data for plotting a test
        public class PlotPoint
        {
            public string Age { get; set; }
            public double Time { get; set; }
            public string foo { get; set; }
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

        //Function for returning the age range of a Performance class data point, currently unused but still could be useful
        public string ageRange(Performance data)
        {
            string range = "";

            if (data.Age > 17 && data.Age <= 24)
            {
                range = "18-24";
            }
            else if (data.Age > 24 && data.Age <= 34)
            {
                range = "25-34";
            }
            else if (data.Age > 34 && data.Age <= 44)
            {
                range = "35-44";
            }
            else if (data.Age > 44 && data.Age <= 54)
            {
                range = "45-54";
            }
            else if (data.Age > 54 && data.Age <= 59)
            {
                range = "55-59";
            }
            else if (data.Age > 59 && data.Age <= 64)
            {
                range = "60-64";
            }
            else if (data.Age > 64 && data.Age <= 69)
            {
                range = "65-69";
            }
            else if (data.Age > 69 && data.Age <= 74)
            {
                range = "70-74";
            }
            else if (data.Age > 74 && data.Age <= 79)
            {
                range = "75-79";
            }
            else if (data.Age > 79 && data.Age <= 84)
            {
                range = "80-84";
            }
            else if (data.Age > 84 && data.Age <= 89)
            {
                range = "85-89";
            }
            else if (data.Age > 89)
            {
                range = "90+";
            }
            return range;
        }

        //Returns the average time of a list of Performance data points
        public double returnAverage(List<Performance> data)
        {
            List<double> times = new List<double>();

            for (int i = 0; i < data.Count; i++)
            {
                times.Add(data[i].Time);
            }
            return times.Average();
        }

        //Returns a string of the age ranges used by the graph
        public List<string> generateAgeStrings()
        {
            List<String> ageStrings = new List<string>();
            ageStrings.Add("18-24");
            ageStrings.Add("25-34");
            ageStrings.Add("35-44");
            ageStrings.Add("45-54");
            ageStrings.Add("55-59");
            ageStrings.Add("60-64");
            ageStrings.Add("65-69");
            ageStrings.Add("70-74");
            ageStrings.Add("75-79");
            ageStrings.Add("80-84");
            ageStrings.Add("85-89");
            ageStrings.Add("90+");

            return ageStrings;
        }

        //Transforms and returns the list of raw Performance data into a list of list of performance 
        //data, where the sublist contains performance data that falls within a certain age range
        public List<List<Performance>> generateAgeGroups(List<Performance> data)
        {
            List<Performance> first = new List<Performance>();
            List<Performance> second = new List<Performance>();
            List<Performance> third = new List<Performance>();
            List<Performance> fourth = new List<Performance>();
            List<Performance> fifth = new List<Performance>();
            List<Performance> sixth = new List<Performance>();
            List<Performance> seventh = new List<Performance>();
            List<Performance> eighth = new List<Performance>();
            List<Performance> ninth = new List<Performance>();
            List<Performance> tenth = new List<Performance>();
            List<Performance> eleventh = new List<Performance>();
            List<Performance> twelfth = new List<Performance>();

            first = data.FindAll(delegate(Performance s) { return (s.Age > 17 && s.Age <= 24); });
            second = data.FindAll(delegate(Performance s) { return (s.Age > 24 && s.Age <= 34); });
            third = data.FindAll(delegate(Performance s) { return (s.Age > 34 && s.Age <= 44); });
            fourth = data.FindAll(delegate(Performance s) { return (s.Age > 44 && s.Age <= 54); });
            fifth = data.FindAll(delegate(Performance s) { return (s.Age > 54 && s.Age <= 59); });
            sixth = data.FindAll(delegate(Performance s) { return (s.Age > 59 && s.Age <= 64); });
            seventh = data.FindAll(delegate(Performance s) { return (s.Age > 64 && s.Age <= 69); });
            eighth = data.FindAll(delegate(Performance s) { return (s.Age > 70 && s.Age <= 74); });
            ninth = data.FindAll(delegate(Performance s) { return (s.Age > 75 && s.Age <= 79); });
            tenth = data.FindAll(delegate(Performance s) { return (s.Age > 80 && s.Age <= 84); });
            eleventh = data.FindAll(delegate(Performance s) { return (s.Age > 85 && s.Age <= 89); });
            twelfth = data.FindAll(delegate(Performance s) { return (s.Age > 89); });

            List<List<Performance>> allAgeGroups = new List<List<Performance>>();
            allAgeGroups.Add(first);
            allAgeGroups.Add(second);
            allAgeGroups.Add(third);
            allAgeGroups.Add(fourth);
            allAgeGroups.Add(fifth);
            allAgeGroups.Add(sixth);
            allAgeGroups.Add(seventh);
            allAgeGroups.Add(eighth);
            allAgeGroups.Add(ninth);
            allAgeGroups.Add(tenth);
            allAgeGroups.Add(eleventh);
            allAgeGroups.Add(twelfth);

            return allAgeGroups;
        }

        //Takes the average of performance times within each age range
        public List<PlotPoint> averagize(List<Performance> data)
        {
            List<PlotPoint> output = new List<PlotPoint>();

            List<List<Performance>> allAgeGroups = new List<List<Performance>>();
            allAgeGroups = generateAgeGroups(data);

            List<string> ageStrings = new List<string>();
            ageStrings = generateAgeStrings();

            for (int i = 0; i < allAgeGroups.Count; i++)
            {
                try {
                    output.Add(new PlotPoint {Age = ageStrings[i], Time = returnAverage(allAgeGroups[i])} );
                } 
                catch {
                    //do nothing
                }
                
            }
            return output;
        }

        //Calculates and stores the median of each age group in a list of plotpoints
        public double returnMedian(List<Performance> data)
        {
            List<Performance> sortedData = new List<Performance>();
            sortedData = data;

            sortedData.Sort(delegate(Performance A, Performance B) { return A.Time.CompareTo(B.Time); });

            double output = -1.0;

            if (sortedData.Count % 2 == 0)
            {
                int pivot = sortedData.Count / 2;
                output = (sortedData[pivot].Time + sortedData[pivot - 1].Time) / 2;
            }
            else
            {
                int pivot = Convert.ToInt16(Math.Floor(sortedData.Count / 2.0));
                output = sortedData[pivot].Time;
            }

            return output;
        }

        //Calculates and stores the median of each age group in a list of plotpoints
        public List<PlotPoint> medianize(List<Performance> data)
        {
            List<PlotPoint> output = new List<PlotPoint>();

            List<List<Performance>> allAgeGroups = new List<List<Performance>>();
            allAgeGroups = generateAgeGroups(data);

            List<string> ageStrings = new List<string>();
            ageStrings = generateAgeStrings();

            for (int i = 0; i < allAgeGroups.Count; i++)
            {
                try
                {
                    output.Add(new PlotPoint { Age = ageStrings[i], Time = returnMedian(allAgeGroups[i]) } );
                }
                catch
                {
                    //do nothing
                }
            }

                return output;
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

        Performance performanceListMax(List<Performance> data)
        {
            Performance current = data[0];

            try
            {
                for (int i = 1; i < data.Count; i++)
                {
                    if (data[i].Time > current.Time)
                    {
                        current = data[i];
                    }
                    else
                    {
                        //do nothing
                    }
                }
            }
            catch
            {
                //do nothing, no data
            }
            return current;
        }

        Performance performanceListMin(List<Performance> data)
        {
            Performance current = data[0];

            try
            {
                for (int i = 1; i < data.Count; i++)
                {
                    if (data[i].Time < current.Time)
                    {
                        current = data[i];
                    }
                    else
                    {
                        //do nothing
                    }
                }
            }
            catch
            {
                //do nothing, no data
            }
            return current;
        }
        //Loads data and graphs points

        public List<PlotPoint> generatePlotPoints(List<Performance> data)
        {
            List<PlotPoint> output = new List<PlotPoint>();

            for (int i = 0; i < data.Count; i++)
            {
                output.Add(new PlotPoint { Age = ageRange(data[i]), Time = data[i].Time });
            }
            return output;
        }

        private async void LoadChartContents()
        {
            await loadJson();

            List<Performance> allResults = new List<Performance>();

            try
            {
                List<string> currentPatients = passer.currentPatient.getTestFilenames();
            }

            catch
            {
                //no passed data
            }

                // Go through all the patients and display the complete data.
                // Do not separate into different categories.

            foreach (string name in fileNames)
            
            //for (int i = 0; i < patientList.Count; i++)
                {
                    testReplay = new TestReplay();
                    // Find the file that corresponds with this patient name and load it
                    //foreach (string name in fileNames)
                    
                    for (int i = 0; i < patientList.Count; i++ )
                    {
                        if (name.Contains(patientList[i].patientName))
                        {
                            await testReplay.loadTestReplay(name);
                            var actions = testReplay.getTestActions();
                            if (actions.Count < 1) continue;

                            DateTime start = actions[0].getStartTime();
                            DateTime end = actions[actions.Count - 1].getEndTime();
                            TimeSpan TimeDifference = end - start;

                            //Get name of test
                            string testName = name.Trim(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                            int j = 0;
                            for (; !Char.IsNumber(testName[j]); j++)
                            {
                                //increase j
                            }
                            testName = testName.Substring(0, j);

                            //Get education level

                            //-------Open and read file----------------------------
                            Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(name);
                            //read
                            String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);
                            //-----------------------------------------------------

                            string educationLevel = "";
                            for (int charSpot = 47; charSpot < data.Count(); charSpot++)
                            {
                                if (data[charSpot] != '\n')
                                {
                                    educationLevel = educationLevel + data[charSpot];
                                }
                                else
                                {
                                    break;
                                }
                            }

                            string gender = data.Substring(43, 2);
                            //43

                            //Get performance time
                            double seconds = TimeDifference.Minutes * 60 + TimeDifference.Seconds + TimeDifference.Milliseconds / 100;
                            int tempAge = Convert.ToInt32(patientList[i].patientAge);

                            allResults.Add(new Performance() { Age = tempAge, Time = seconds, Test = testName });

                            break;
                        }
                    }
                }

            
            List<Performance> sortPatientAge = new List<Performance>(allResults);
            if (allResults.Count > 0)
            {
                sortPatientAge.Sort(delegate(Performance arg1, Performance arg2)
                {
                    return arg1.Age.CompareTo(arg2.Age);
                });
            }


            int minAgeIndex = sortPatientAge[0].Age;
            while (minAgeIndex < sortPatientAge[sortPatientAge.Count - 1].Age)
            {
                minAgeRange.Items.Add(Convert.ToDouble(minAgeIndex));
                maxAgeRange.Items.Add(Convert.ToDouble(minAgeIndex));
                minAgeIndex++;
            }
            minAgeRange.Items.Add(Convert.ToDouble(minAgeIndex));
            maxAgeRange.Items.Add(Convert.ToDouble(minAgeIndex));

            /*
            for (int i = sortPatientAge[0].Age; i < sortPatientAge[sortPatientAge.Count - 1].Age; i++)
            {
                minAgeRange.Items.Add(Convert.ToDouble(i));
                maxAgeRange.Items.Add(Convert.ToDouble(i));
            }*/
            
            for (int i = Convert.ToInt32(Math.Floor(performanceListMin(sortPatientAge).Time)); i <= Convert.ToInt32(Math.Ceiling(performanceListMax(sortPatientAge).Time)); i++)
            {
                minTimeRange.Items.Add(i);
                maxTimeRange.Items.Add(i);
            }

            minAgeRange.SelectedIndex = 0;
            maxAgeRange.SelectedIndex = (maxAgeRange.Items.Count - 1);

            minTimeRange.SelectedIndex = 0;
            maxTimeRange.SelectedIndex = (maxTimeRange.Items.Count - 1);

            //Adds each performance point to the appropiate data structure that is belongs
            //to based on the type of test 
            for (int i = 0; i < sortPatientAge.Count; i++)
            {
                switch (sortPatientAge[i].Test)
                {
                    case "TRAILS_A":
                        TrailsA.Add(sortPatientAge[i]);
                        break;
                    case "TRAILS_B":
                        TrailsB.Add(sortPatientAge[i]);
                        break;
                    case "TRAILS_A_H":
                        TrailsA_H.Add(sortPatientAge[i]);
                        break;
                    case "TRAILS_B_H":
                        TrailsB_H.Add(sortPatientAge[i]);
                        break;
                    default:
                        //error
                        break;
                }
            }

            TrailsAGroup = generatePlotPoints(TrailsA);
            TrailsBGroup = generatePlotPoints(TrailsB);
            TrailsA_HGroup = generatePlotPoints(TrailsA_H);
            TrailsB_HGroup = generatePlotPoints(TrailsB_H);

            //Stores the averages and medians of each test type
            avgTrailsAGrouped = averagize(TrailsA);
            avgTrailsA_HGrouped = averagize(TrailsA_H);
            avgTrailsBGrouped = averagize(TrailsB);
            avgTrailsB_HGrouped = averagize(TrailsB_H);

            medTrailsAGrouped = medianize(TrailsA);
            medTrailsA_HGrouped = medianize(TrailsA_H);
            medTrailsBGrouped = medianize(TrailsB);
            medTrailsB_HGrouped = medianize(TrailsB_H);

            //Reveals the chart after the data is loaded and calculated
            ScatterChart.Opacity = 100;
            ageAxis.Opacity = 100;
            timeAxis.Opacity = 100;
            progressNorm.IsActive = false;

            //Sets the Trails A vertical as the default test to be graphed
            dataSelection.SelectedIndex = 0;
        }

        //Older class that was used for plotting patient info
        //currently unused but still could be useful later
        public class PatientPlot
        {
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

                foreach (var name in names)
                {
                    // The fileName starts with a number
                    if (name.Name[0] >= 48 && name.Name[0] <= 58)
                        fileNames.Add(name.Name);
                }

                //get file
                Windows.Storage.StorageFile myFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                //read
                String data = await Windows.Storage.FileIO.ReadTextAsync(myFile);
                Windows.Data.Json.JsonArray.TryParse(data, out patients);

                for (uint i = 0; i < patients.Count; i++)
                {
                    string doctorName = patients.GetObjectAt(i).GetNamedValue("Doctor").GetString();
                    if (doctorName != passer.doctorId) continue;

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
            popup.ShowAsync(new Point(0, 0));
        }

        public List<Performance> filter(List<Performance> data)
        {
            List<Performance> output = new List<Performance>();

            int minAge = Convert.ToInt32(minAgeRange.SelectedItem.ToString());
            int maxAge = Convert.ToInt32(maxAgeRange.SelectedItem.ToString());

            int minTime = Convert.ToInt32(minTimeRange.SelectedItem.ToString());
            int maxTime = Convert.ToInt32(maxTimeRange.SelectedItem.ToString());

            if (minAge > maxAge)
            {
                int tempA = minAge;
                int tempB = maxAge;

                minAge = tempB;
                maxAge = tempA;
            }

            if (minTime > maxTime)
            {
                int tempA = minTime;
                int tempB = maxTime;

                minTime = tempB;
                maxTime = tempA;
            }

            output = data.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
            output = output.Where(x => x.Time >= minTime && x.Time <= maxTime).ToList();

            return output;
        }

        public void graphPoints()
        {
            chartChoice = dataSelection.SelectedIndex;
            
            //Simply switches to the appropiate data structures to be graphed
            //based on the selection in the combo box
            switch (chartChoice)
            {
                case 0:
                    List<Performance> filteredTrailsA = filter(TrailsA);
            
                    List<PlotPoint> plotTrailsA = new List<PlotPoint>(generatePlotPoints(filteredTrailsA));
                    List<PlotPoint> plotAvgTrailsAGroup = new List<PlotPoint>( averagize(filteredTrailsA));
                    List<PlotPoint> plotMedTrailsAGroup = new List<PlotPoint>(medianize(filteredTrailsA));

                    (ScatterChart.Series[0] as ScatterSeries).ItemsSource = plotTrailsA;
                    (ScatterChart.Series[1] as LineSeries).ItemsSource = plotAvgTrailsAGroup;
                    (ScatterChart.Series[2] as LineSeries).ItemsSource = plotMedTrailsAGroup;
                    break;
                case 1:
                    List<Performance> filteredTrailsB = filter(TrailsB);
            
                    List<PlotPoint> plotTrailsB = new List<PlotPoint>(generatePlotPoints(filteredTrailsB));
                    List<PlotPoint> plotAvgTrailsBGroup = new List<PlotPoint>( averagize(filteredTrailsB) );
                    List<PlotPoint> plotMedTrailsBGroup = new List<PlotPoint>( medianize(filteredTrailsB) );
            
                    (ScatterChart.Series[0] as ScatterSeries).ItemsSource = plotTrailsB;
                    (ScatterChart.Series[1] as LineSeries).ItemsSource = plotAvgTrailsBGroup;
                    (ScatterChart.Series[2] as LineSeries).ItemsSource = plotMedTrailsBGroup;
                    break;
                case 2:
                    List<Performance> filteredTrailsA_H = filter(TrailsA_H);
            
                    List<PlotPoint> plotTrailsA_H = new List<PlotPoint>(generatePlotPoints(filteredTrailsA_H));
                    List<PlotPoint> plotAvgTrailsA_HGroup = new List<PlotPoint>(averagize(filteredTrailsA_H) );
                    List<PlotPoint> plotMedTrailsA_HGroup = new List<PlotPoint>(medianize(filteredTrailsA_H) );

                    (ScatterChart.Series[0] as ScatterSeries).ItemsSource = plotTrailsA_H;
                    (ScatterChart.Series[1] as LineSeries).ItemsSource = plotAvgTrailsA_HGroup;
                    (ScatterChart.Series[2] as LineSeries).ItemsSource = plotMedTrailsA_HGroup;
                    break;
                case 3:
                    List<Performance> filteredTrailsB_H = filter(TrailsB_H);

                    List<PlotPoint> plotTrailsB_H = new List<PlotPoint>(generatePlotPoints(filteredTrailsB_H));
                    List<PlotPoint> plotAvgTrailsB_HGroup = new List<PlotPoint>(averagize(filteredTrailsB_H) );
                    List<PlotPoint> plotMedTrailsB_HGroup = new List<PlotPoint>(medianize(filteredTrailsB_H) );

                    (ScatterChart.Series[0] as ScatterSeries).ItemsSource = plotTrailsB_H;
                    (ScatterChart.Series[1] as LineSeries).ItemsSource = plotAvgTrailsB_HGroup;
                    (ScatterChart.Series[2] as LineSeries).ItemsSource = plotMedTrailsB_HGroup;
                    break;
                default:
                    //do nothing
                    break;
            }
        }

        //Function for handling the combo box that controls which type of test is graphed
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphPoints();
        }

        private void minTimeRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphPoints();
        }

        private void maxTimeRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphPoints();
        }

        private void minAgeRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphPoints();
        }

        private void maxAgeRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphPoints();
        }
    }
}
