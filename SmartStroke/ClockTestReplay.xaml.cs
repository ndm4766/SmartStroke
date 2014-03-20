using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartStroke
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClockTestReplay : Page
    {
        InfoPasser passer = new InfoPasser();

        //test replay container
        private TestReplay testReplay;
        DispatcherTimer timer;
        Stopwatch stopwatch;
        int actionIndex;
        int linesIndex;
        List<Line> previousStroke;
        Dictionary<Stroke, List<Line>> linesRedrawn; //necessary bec the lines are already children of the other page

        string currentlySelectedDate;

        private const double DRAW_WIDTH = 4.0;
        private const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Color.FromArgb(255, 50, 50, 50);

        public ClockTestReplay()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += timer_tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            stopwatch = new Stopwatch();
            actionIndex = 0;
            linesIndex = 0;
            previousStroke = new List<Line>();
            linesRedrawn = new Dictionary<Stroke, List<Line>>();
        }

        // Load a previous clock test from a test replay object.
        // Need to create a new TestReplay object so that you do not add
        // more lines - test replay load will currently append information.
        async private void loadTest()
        {
            testReplay = new TestReplay();
            List<String> filenames = passer.currentPatient.getTestFilenames();
            foreach (String filename in filenames)
            {
                if(filename.Contains(currentlySelectedDate))
                {
                    await testReplay.loadTestReplay(filename);
                    break;
                }
            }
        }

        private bool actionTimeHasPassed(TestAction action)
        {
            TimeSpan span = action.getStartTime().Subtract(testReplay.getStartTime());
            double seconds = span.TotalSeconds;
            bool b = seconds < stopwatch.Elapsed.TotalSeconds;
            return b;
        }

        private bool lineDataTimeHasPassed(LineData lineData)
        {
            TimeSpan span = lineData.getDateTime().Subtract(testReplay.getStartTime());
            double seconds = span.TotalSeconds;
            bool b = seconds < stopwatch.Elapsed.TotalSeconds;
            return b;
        }

        private void timer_tick(object sender, object e)
        {
            //get all actions
            var allActions = testReplay.getTestActions(); //TODO: bug - this list does not include deletions

            if (actionTimeHasPassed(allActions[actionIndex]))
            {
                //that is, if the action is a stroke, draw a line on the canvas for each line in that stroke (use getRenderingSegments to get lines)
                if (allActions[actionIndex].getActionType() == ACTION_TYPE.STROKE)
                {
                    Stroke stroke = allActions[actionIndex] as Stroke;
                    List<LineData> lines = stroke.getLines();

                    if (lineDataTimeHasPassed(lines[linesIndex]))
                    {
                        Line line = new Line();
                        line.X1 = lines[linesIndex].getLine().X1;
                        line.Y1 = lines[linesIndex].getLine().Y1;
                        line.X2 = lines[linesIndex].getLine().X2;
                        line.Y2 = lines[linesIndex].getLine().Y2;
                        line.StrokeThickness = DRAW_WIDTH;
                        line.Stroke = new SolidColorBrush(DRAW_COLOR);

                        MyCanvas.Children.Add(line);
                        previousStroke.Add(line);
                        linesIndex++;
                    }

                    if (linesIndex >= lines.Count)
                    {
                        linesIndex = 0;
                        linesRedrawn.Add(stroke, previousStroke);
                        previousStroke = new List<Line>();
                        actionIndex++;
                    }
                }
                //if the action is an erasure, remove all the lines that were part of the stroke that was erased
                else if (allActions[actionIndex].getActionType() == ACTION_TYPE.DEL_STROKE)
                {
                    Stroke deletedStroke = allActions[(allActions[actionIndex] as DeleteStroke).getIndex()] as Stroke;

                    foreach (Line line in linesRedrawn[deletedStroke])
                    {
                        //MyCanvas.Children.Remove(line);
                        MyCanvas.Children[MyCanvas.Children.IndexOf(line)].Opacity = .2;
                    }
                    actionIndex++;
                }

                if (actionIndex >= allActions.Count)
                {
                    stopwatch.Stop();
                    timer.Stop();

                    granularReplayButton.IsEnabled = true;
                }
            }
        }

        private void renderGranularTestReplay(object sender, RoutedEventArgs e)
        {
            loadTest();
            granularReplayButton.IsEnabled = false;

            //reset all the things
            foreach (KeyValuePair<Stroke, List<Line>> item in linesRedrawn)
            {
                foreach(Line line in item.Value)
                {
                    MyCanvas.Children.Remove(line);
                }
            }
            stopwatch = new Stopwatch();
            actionIndex = 0;
            linesIndex = 0;
            previousStroke.Clear();
            linesRedrawn.Clear();

            stopwatch.Start();
            timer.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            passer = e.Parameter as InfoPasser;
            testReplay = new TestReplay(passer.currentPatient, TEST_TYPE.CLOCK);

            if (passer.currentPatient.getTestFilenames().Count > 0)
            {
                currentlySelectedDate = passer.currentPatient.getTestFilenames()[0];
            }
            else
            {
                granularReplayButton.IsEnabled = false;
            }

            var stuff = passer.currentPatient.getTestFilenames();
            foreach(string filename in stuff)
            {
                if (filename.Contains(testReplay.getTestType().ToString()))
                {
                    testDatesBox.Items.Add(filename.Substring(filename.Length - (14 + 4), 14));
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            timer.Stop();
            stopwatch.Stop();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedDate = testDatesBox.SelectedItem.ToString();
        }

        private void menuClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu), passer);
        }

    }
}
