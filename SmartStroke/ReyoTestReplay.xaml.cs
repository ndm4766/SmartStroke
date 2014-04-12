using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace SmartStroke
{
    public sealed partial class ReyoTestReplay : Page
    {
        InfoPasser passer = new InfoPasser();

        private TestReplay testReplay1;
        private TestReplay testReplay2;

        string currentlySelectedDate1;
        string currentlySelectedDate2;

        DispatcherTimer timer;
        Stopwatch stopwatch;
        int actionIndex;
        int linesIndex;
        List<List<Line>> allLines;
        List<Line> currentLine;

        private const double DRAW_WIDTH = 4.0;
        private const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Color.FromArgb(255, 50, 50, 50);

        public ReyoTestReplay()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += timer_tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            stopwatch = new Stopwatch();
            actionIndex = 0;
            linesIndex = 0;
            allLines = new List<List<Line>>();
            currentLine = new List<Line>();
        }

        // Load a previous clock test from a test replay object.
        // Need to create a new TestReplay object so that you do not add
        // more lines - test replay load will currently append information.
        async private Task loadTest()
        {
            //testReplay = new TestReplay();
            List<String> filenames = passer.currentPatient.getTestFilenames();
            foreach (String filename in filenames)
            {
                if (filename.Contains(currentlySelectedDate1))
                {
                    await testReplay1.loadTestReplay(filename);
                    break;
                }
                if (filename.Contains(currentlySelectedDate2))
                {
                    await testReplay2.loadTestReplay(filename);
                    break;
                }
            }
        }

        private bool actionTimeHasPassed(TestAction action, TestReplay TR)
        {
            TimeSpan span = action.getStartTime().Subtract(TR.getStartTime());
            double seconds = span.TotalSeconds;
            bool b = seconds < stopwatch.Elapsed.TotalSeconds;
            return b;
        }

        private bool lineDataTimeHasPassed(LineData lineData, TestReplay TR)
        {
            TimeSpan span = lineData.getDateTime().Subtract(TR.getStartTime());
            double seconds = span.TotalSeconds;
            bool b = seconds < stopwatch.Elapsed.TotalSeconds;
            return b;
        }

        private void timer_tick(object sender, object e)
        {
            //get all actions
            var allActions = testReplay1.getTestActions(); //TODO: bug - this list does not include deletions

            if (actionTimeHasPassed(allActions[actionIndex], testReplay1))
            {
                //that is, if the action is a stroke, draw a line on the canvas for each line in that stroke (use getRenderingSegments to get lines)
                if (allActions[actionIndex].getActionType() == ACTION_TYPE.STROKE)
                {
                    Stroke stroke = allActions[actionIndex] as Stroke;
                    List<LineData> lines = stroke.getLines();

                    if (lineDataTimeHasPassed(lines[linesIndex], testReplay1))
                    {
                        Line line = new Line();
                        line.X1 = lines[linesIndex].getLine().X1;
                        line.Y1 = lines[linesIndex].getLine().Y1;
                        line.X2 = lines[linesIndex].getLine().X2;
                        line.Y2 = lines[linesIndex].getLine().Y2;
                        line.StrokeThickness = DRAW_WIDTH;
                        line.Stroke = new SolidColorBrush(DRAW_COLOR);

                        MyCanvas.Children.Add(line);
                        currentLine.Add(line);
                        linesIndex++;
                    }

                    if (linesIndex >= lines.Count)
                    {
                        linesIndex = 0;
                        allLines.Add(currentLine);
                        currentLine = new List<Line>();
                        actionIndex++;
                    }
                }
                //if the action is an erasure, remove all the lines that were part of the stroke that was erased
                else if (allActions[actionIndex].getActionType() == ACTION_TYPE.DEL_STROKE)
                {
                    int strokeDeletedIndex = (allActions[actionIndex] as DeleteStroke).getIndex();

                    foreach (Line line in allLines[strokeDeletedIndex])
                    {
                        MyCanvas.Children[MyCanvas.Children.IndexOf(line)].Opacity = .2;
                    }
                    actionIndex++;
                }

                if (actionIndex >= allActions.Count)
                {
                    stopwatch.Stop();
                    timer.Stop();

                    replayButton.IsEnabled = true;
                }
            }
        }

        async private void renderTestReplay(object sender, RoutedEventArgs e)
        {
            removeAllStrokes();
            stopwatch.Reset();
            timer.Stop();

            await loadTest();
            replayButton.IsEnabled = false;

            //reset all the things
            foreach (var stroke in allLines)
            {
                foreach(Line line in stroke)
                {
                    MyCanvas.Children.Remove(line);
                }
            }
            actionIndex = 0;
            linesIndex = 0;
            allLines.Clear();

            stopwatch.Start();
            timer.Start();
        }



        private void showTimeViz(object sender, RoutedEventArgs e)
        {
            viewColorTimeMode();
        }

        async private void viewColorTimeMode()
        {
            removeAllStrokes();
            await loadTest();
            var allActions = testReplay1.getTestActions();
            var smartStrokes = allActions.OfType<SmartStroke.Stroke>();
            //var strokes = inkManager.GetStrokes();
            int strokeNum = 0;
            int timeInterval = getColorTimeInterval();
            if (timeInterval == 1)
            {
                timeIntervalBlock.Text = "Color changes every second";
            }
            else
            {
                timeIntervalBlock.Text = "Color changes every " + timeInterval.ToString() + " seconds";
            }
            double msLineDuration = 0;
            SolidColorBrush color = new SolidColorBrush(Colors.Black);
            foreach (var stroke in smartStrokes)
            {

                var smartLines = smartStrokes.ElementAt(strokeNum).lines;
                int lineNum = 0;
                //DateTime prevTime = testReplay.getStartTime();

                foreach (LineData lineData in smartLines)
                {
                    Line l = lineData.getLine();
                    //msLineDuration = (smartLines[lineNum].getDateTime()-prevTime).TotalMilliseconds;
                    MyCanvas.Children.Remove(l);
                    //l.StrokeThickness = msLineDuration * 1;//change one to a constant (less than 1) that makes it look okay
                    Line line = new Line();
                    line.X1 = l.X1;
                    line.Y1 = l.Y1;
                    line.X2 = l.X2;
                    line.Y2 = l.Y2;


                    line.StrokeThickness = msLineDuration * 1;
                    MyCanvas.Children.Add(line);
                    lineNum++;
                    //we probably want to render the line as a bezier curve. look at bezierCurveTo
                }
                strokeNum++;

            }
            DateTime prevTime = testReplay1.getStartTime();
            strokeNum = 0;
            foreach (var stroke in smartStrokes)
            {

                var smartLines = smartStrokes.ElementAt(strokeNum).lines;
                int lineNum = 0;
                foreach (LineData lineData in smartLines)
                {

                    Line l = lineData.getLine();
                    int secondsSinceStartOfTest = (int)Math.Floor((smartLines[lineNum].getDateTime() - testReplay1.getStartTime()).TotalSeconds);
                    //assign 1 of 8 colors
                    if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 1) { color = new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 2) { color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 3) { color = new SolidColorBrush(Color.FromArgb(255, 146, 208, 80)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 4) { color = new SolidColorBrush(Color.FromArgb(255, 0, 176, 80)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 5) { color = new SolidColorBrush(Color.FromArgb(255, 0, 176, 240)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 6) { color = new SolidColorBrush(Color.FromArgb(255, 0, 112, 192)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 7) { color = new SolidColorBrush(Color.FromArgb(255, 0, 32, 96)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 0) { color = new SolidColorBrush(Color.FromArgb(255, 112, 48, 160)); }
                    MyCanvas.Children.Remove(l);

                    Line line = new Line();
                    line.X1 = l.X1;
                    line.Y1 = l.Y1;
                    line.X2 = l.X2;
                    line.Y2 = l.Y2;

                    line.Stroke = color;
                    msLineDuration = (smartLines[lineNum].getDateTime() - prevTime).TotalMilliseconds;
                    prevTime = prevTime.AddMilliseconds(msLineDuration);
                    line.StrokeThickness = 4;
                    //line.StrokeThickness = msLineDuration * 1; //uncomment this to see unrefined thickness variation view
                    MyCanvas.Children.Add(line);
                    lineNum++;
                }
                strokeNum++;
            }

        }
        private void removeAllStrokes()
        {
            MyCanvas.Children.Clear();
        }

        private int getColorTimeInterval()
        {
            double testDuration = (testReplay1.getEndTime() - testReplay1.getStartTime()).TotalSeconds;
            if (testDuration <= 8) { return 1; }
            else if (testDuration <= 40) { return 5; }
            else if (testDuration <= 120) { return 15; }
            else { return 30; }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            passer = e.Parameter as InfoPasser;
            testReplay1 = new TestReplay(passer.currentPatient, TEST_TYPE.REY_OSTERRIETH);

            if (passer.currentPatient.getTestFilenames().Count > 0)
            {
                currentlySelectedDate1 = passer.currentPatient.getTestFilenames()[0];
                currentlySelectedDate2 = passer.currentPatient.getTestFilenames()[0];
            }
            else
            {
                replayButton.IsEnabled = false;
            }

            var stuff = passer.currentPatient.getTestFilenames();
            foreach(string filename in stuff)
            {
                if (filename.Contains(testReplay1.getTestType().ToString()))
                {
                    testDatesBox1.Items.Add(testReplay1.getDisplayedDatetime(filename));
                    testDatesBox2.Items.Add(testReplay1.getDisplayedDatetime(filename));
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            timer.Stop();
            stopwatch.Stop();
        }


        private void ListBox_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedDate1 = testReplay1.getFilenameString(testDatesBox1.SelectedItem.ToString());
        }
        private void ListBox_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedDate2 = testReplay1.getFilenameString(testDatesBox2.SelectedItem.ToString());
        }
        private void menuClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu), passer);
        }

    }
}
