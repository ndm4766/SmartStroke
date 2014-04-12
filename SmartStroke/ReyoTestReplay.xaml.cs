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

        int actionIndex1;
        int linesIndex1;
        List<List<Line>> allLines1;
        List<Line> currentLine1;

        int actionIndex2;
        int linesIndex2;
        List<List<Line>> allLines2;
        List<Line> currentLine2;

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
            actionIndex1 = 0;
            linesIndex1 = 0;
            allLines1 = new List<List<Line>>();
            currentLine1 = new List<Line>();
            actionIndex2 = 0;
            linesIndex2 = 0;
            allLines2 = new List<List<Line>>();
            currentLine2 = new List<Line>();
        }

        async private Task loadTest()
        {
            List<String> filenames = passer.currentPatient.getTestFilenames();
            foreach (String filename in filenames)
            {
                if (filename.Contains(currentlySelectedDate1))
                {
                    await testReplay1.loadTestReplay(filename);
                }
                if (filename.Contains(currentlySelectedDate2))
                {
                    await testReplay2.loadTestReplay(filename);
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
            var allActions1 = testReplay1.getTestActions();

            if(allActions1.Count > actionIndex1)
            if (actionTimeHasPassed(allActions1[actionIndex1], testReplay1))
            {
                if (allActions1[actionIndex1].getActionType() == ACTION_TYPE.STROKE)
                {
                    Stroke stroke = allActions1[actionIndex1] as Stroke;
                    List<LineData> lines = stroke.getLines();

                    if (lineDataTimeHasPassed(lines[linesIndex1], testReplay1))
                    {
                        Line line = new Line();
                        line.X1 = lines[linesIndex1].getLine().X1 * .45 + 20;
                        line.Y1 = lines[linesIndex1].getLine().Y1 * .45 + 197;
                        line.X2 = lines[linesIndex1].getLine().X2 * .45 + 20;
                        line.Y2 = lines[linesIndex1].getLine().Y2 * .45 + 197;
                        line.StrokeThickness = DRAW_WIDTH;
                        line.Stroke = new SolidColorBrush(DRAW_COLOR);

                        MyCanvas.Children.Add(line);
                        currentLine1.Add(line);
                        linesIndex1++;
                    }

                    if (linesIndex1 >= lines.Count)
                    {
                        linesIndex1 = 0;
                        allLines1.Add(currentLine1);
                        currentLine1 = new List<Line>();
                        actionIndex1++;
                    }
                }
                //if the action is an erasure, remove all the lines that were part of the stroke that was erased
                else if (allActions1[actionIndex1].getActionType() == ACTION_TYPE.DEL_STROKE)
                {
                    int strokeDeletedIndex = (allActions1[actionIndex1] as DeleteStroke).getIndex();

                    foreach (Line line in allLines1[strokeDeletedIndex])
                    {
                        MyCanvas.Children[MyCanvas.Children.IndexOf(line)].Opacity = .2;
                    }
                    actionIndex1++;
                }
            }
            var allActions2 = testReplay2.getTestActions();
            if (allActions2.Count > actionIndex2)
            if (actionTimeHasPassed(allActions2[actionIndex2], testReplay2))
            {
                if (allActions2[actionIndex2].getActionType() == ACTION_TYPE.STROKE)
                {
                    Stroke stroke = allActions2[actionIndex2] as Stroke;
                    List<LineData> lines = stroke.getLines();

                    if (lineDataTimeHasPassed(lines[linesIndex2], testReplay2))
                    {
                        Line line = new Line();
                        line.X1 = lines[linesIndex2].getLine().X1 * .45 + 597;
                        line.Y1 = lines[linesIndex2].getLine().Y1 * .45 + 197;
                        line.X2 = lines[linesIndex2].getLine().X2 * .45 + 597;
                        line.Y2 = lines[linesIndex2].getLine().Y2 * .45 + 197;
                        line.StrokeThickness = DRAW_WIDTH;
                        line.Stroke = new SolidColorBrush(DRAW_COLOR);

                        MyCanvas.Children.Add(line);
                        currentLine2.Add(line);
                        linesIndex2++;
                    }

                    if (linesIndex2 >= lines.Count)
                    {
                        linesIndex2 = 0;
                        allLines1.Add(currentLine2);
                        currentLine2 = new List<Line>();
                        actionIndex2++;
                    }
                }
                //if the action is an erasure, remove all the lines that were part of the stroke that was erased
                else if (allActions2[actionIndex2].getActionType() == ACTION_TYPE.DEL_STROKE)
                {
                    int strokeDeletedIndex = (allActions2[actionIndex2] as DeleteStroke).getIndex();

                    foreach (Line line in allLines2[strokeDeletedIndex])
                    {
                        MyCanvas.Children[MyCanvas.Children.IndexOf(line)].Opacity = .2;
                    }
                    actionIndex2++;
                }
            }
            if (actionIndex2 >= allActions2.Count && actionIndex1 >= allActions1.Count)
            {
                stopwatch.Stop();
                timer.Stop();

                replayButton.IsEnabled = true;
            }


        }

        async private void renderTestReplay(object sender, RoutedEventArgs e)
        {
            removeAllStrokes();
            stopwatch.Reset();
            timer.Stop();

            await loadTest();
            replayButton.IsEnabled = false;

            foreach (var stroke in allLines1)
            {
                foreach(Line line in stroke)
                {
                    MyCanvas.Children.Remove(line);
                }
            }
            actionIndex1 = 0;
            linesIndex1 = 0;
            allLines1.Clear();

            actionIndex2 = 0;
            linesIndex2 = 0;
            allLines2.Clear();

            stopwatch.Start();
            timer.Start();
        }



        private void showTimeViz(object sender, RoutedEventArgs e)
        {
            viewColorTimeMode1();
            viewColorTimeMode2();
        }

        async private void viewColorTimeMode1()
        {
            removeAllStrokes();
            await loadTest();
            var allActions = testReplay1.getTestActions();
            var smartStrokes = allActions.OfType<SmartStroke.Stroke>();
            //var strokes = inkManager.GetStrokes();
            int strokeNum = 0;
            int timeInterval = getColorTimeInterval(testReplay1);
            if (timeInterval == 1)
            {
                timeIntervalBlock1.Text = "Color changes every second";
            }
            else
            {
                timeIntervalBlock1.Text = "Color changes every " + timeInterval.ToString() + " seconds";
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
                    line.X1 = l.X1 * .45 + 20;
                    line.Y1 = l.Y1 * .45 + 197;
                    line.X2 = l.X2 * .45 + 20;
                    line.Y2 = l.Y2 * .45 + 197;


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
                    line.X1 = l.X1 * .45 + 20;
                    line.Y1 = l.Y1 * .45 + 197;
                    line.X2 = l.X2 * .45 + 20;
                    line.Y2 = l.Y2 * .45 + 197;

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

        async private void viewColorTimeMode2()
        {
            removeAllStrokes();
            await loadTest();
            var allActions = testReplay2.getTestActions();
            var smartStrokes = allActions.OfType<SmartStroke.Stroke>();
            //var strokes = inkManager.GetStrokes();
            int strokeNum = 0;
            int timeInterval = getColorTimeInterval(testReplay2);
            if (timeInterval == 1)
            {
                timeIntervalBlock2.Text = "Color changes every second";
            }
            else
            {
                timeIntervalBlock2.Text = "Color changes every " + timeInterval.ToString() + " seconds";
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
                    line.X1 = l.X1 * .45 + 597;
                    line.Y1 = l.Y1 * .45 + 197;
                    line.X2 = l.X2 * .45 + 597;
                    line.Y2 = l.Y2 * .45 + 197;


                    line.StrokeThickness = msLineDuration * 1;
                    MyCanvas.Children.Add(line);
                    lineNum++;
                    //we probably want to render the line as a bezier curve. look at bezierCurveTo
                }
                strokeNum++;

            }
            DateTime prevTime = testReplay2.getStartTime();
            strokeNum = 0;
            foreach (var stroke in smartStrokes)
            {

                var smartLines = smartStrokes.ElementAt(strokeNum).lines;
                int lineNum = 0;
                foreach (LineData lineData in smartLines)
                {

                    Line l = lineData.getLine();
                    int secondsSinceStartOfTest = (int)Math.Floor((smartLines[lineNum].getDateTime() - testReplay2.getStartTime()).TotalSeconds);
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
                    line.X1 = l.X1 * .45 + 597;
                    line.Y1 = l.Y1 * .45 + 197;
                    line.X2 = l.X2 * .45 + 597;
                    line.Y2 = l.Y2 * .45 + 197;

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

        private int getColorTimeInterval(TestReplay TR)
        {
            double testDuration = (TR.getEndTime() - TR.getStartTime()).TotalSeconds;
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
            testReplay2 = new TestReplay(passer.currentPatient, TEST_TYPE.REY_OSTERRIETH);

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
