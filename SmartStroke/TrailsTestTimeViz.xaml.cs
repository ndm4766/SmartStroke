﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.UI.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Devices.Input;
using Windows.UI.ApplicationSettings;
using System.Diagnostics;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using System.Runtime.InteropServices.WindowsRuntime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartStroke
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrailsTestTimeViz : Page
    {
        //this comment is to force github to resync
        //test replay container
        private TestReplay testReplay;

        private bool finished = false;

        private string currentlySelectedDate;

        //general globals
        private InfoPasser passer;
        private const double DRAW_WIDTH = 4.0;
        private const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Color.FromArgb(255, 50, 50, 50);
        private Color ERASE_COLOR = Colors.White;

        //boolean program state flags
        private bool pressed = false;

        //ink drawing members
        private InkManager inkManager;
        private InkDrawingAttributes drawingAttributes;
        private uint penId;
        private uint touchId;
        private Point previousContactPt;
        private Point currentContactPt;
        private List<Line> currentLine;
        private Dictionary<InkStroke, List<Line>> allLines;

        //TrailNode handling members
        private List<TrailNode> nodes;
        private int nextIndex;
        private int currentIndex;
        private Queue<int> incorrectNodes;
        private List<Line> currentEdge;

        //Size of screen
        double screenWidth;
        double screenHeight;

        public TrailsTestTimeViz()
        {
            this.InitializeComponent();

            inkManager = new Windows.UI.Input.Inking.InkManager();

            // Create the trails test background. The test image is 117X917 px but to fit on a screen (surface) it is 686 X 939
            nodes = new List<TrailNode>();
            //populateNodes(testVersion, nodes);
            currentLine = new List<Line>();
            allLines = new Dictionary<InkStroke, List<Line>>();

            nextIndex = 0;
            currentIndex = 0;
            incorrectNodes = new Queue<int>();
            currentEdge = new List<Line>();


            screenHeight = Window.Current.Bounds.Height;
            screenWidth = Window.Current.Bounds.Width;

            //Set the ink to not use bezeir curves
            drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            // True is the Default value for fitToCurve.
            drawingAttributes.FitToCurve = false;
            inkManager.SetDefaultDrawingAttributes(drawingAttributes);
        }

        private void populateNodes(char kind, List<TrailNode> nodes)
        {
            #region TrailsA
            if (kind == 'A')
            {
                nodes.Add(new TrailNode(1, new Point(485, 503), MyCanvas, passer.trailsVertical));
                TextBlock begin = new TextBlock()
                {
                    Text = "Begin",
                    Margin = new Thickness(485, 473, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 25
                };
                RotateTransform r = new RotateTransform();
                r.Angle = 90;
                if (passer.trailsVertical)
                {
                    begin.Margin = new Thickness(575, 503, 0, 0);
                    begin.RenderTransform = r;
                }
                MyCanvas.Children.Add(begin);
                nodes.Add(new TrailNode(2, new Point(375, 341), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(3, new Point(277, 549), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(4, new Point(693, 547), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(5, new Point(677, 287), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(6, new Point(589, 423), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(7, new Point(495, 273), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(8, new Point(301, 151), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(9, new Point(111, 187), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(10, new Point(343, 251), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(11, new Point(81, 447), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(12, new Point(47, 65), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(13, new Point(593, 151), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(14, new Point(455, 61), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(15, new Point(1085, 55), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(16, new Point(861, 157), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(17, new Point(1089, 383), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(18, new Point(813, 343), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(19, new Point(955, 581), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(20, new Point(967, 455), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(21, new Point(1087, 673), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(22, new Point(697, 675), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(23, new Point(47, 685), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(24, new Point(503, 603), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(25, new Point(79, 573), MyCanvas, passer.trailsVertical));
                TextBlock end = new TextBlock()
                {
                    Text = "End",
                    Margin = new Thickness(79, 543, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 25
                };
                if (passer.trailsVertical)
                {
                    end.RenderTransform = r;
                    end.Margin = new Thickness(169, 573, 0, 0);
                }

                MyCanvas.Children.Add(end);
            }
            #endregion
            #region TrailsB
            else if (kind == 'B')
            {
                nodes.Add(new TrailNode(1, new Point(607, 369), MyCanvas, passer.trailsVertical));
                TextBlock begin = new TextBlock()
                {
                    Text = "Begin",
                    Margin = new Thickness(607, 339, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 25
                };
                RotateTransform r = new RotateTransform();
                r.Angle = 90;
                if (passer.trailsVertical)
                {
                    begin.Margin = new Thickness(697, 369, 0, 0);
                    begin.RenderTransform = r;
                }
                MyCanvas.Children.Add(begin);
                nodes.Add(new TrailNode('A', new Point(333, 509), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(2, new Point(251, 195), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('B', new Point(859, 317), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(3, new Point(717, 331), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('C', new Point(501, 491), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(4, new Point(881, 401), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('D', new Point(905, 587), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(5, new Point(571, 575), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('E', new Point(181, 571), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(6, new Point(243, 317), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('F', new Point(153, 137), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(7, new Point(625, 223), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('G', new Point(419, 135), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(8, new Point(917, 109), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('H', new Point(547, 149), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(9, new Point(919, 231), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('I', new Point(927, 479), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(10, new Point(1059, 661), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('J', new Point(325, 607), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(11, new Point(63, 649), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('K', new Point(65, 69), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(12, new Point(459, 69), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('L', new Point(219, 101), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(13, new Point(1057, 63), MyCanvas, passer.trailsVertical));
                TextBlock end = new TextBlock()
                {
                    Text = "End",
                    Margin = new Thickness(nodes[nodes.Count - 1].getLocation().X + 10,
                        nodes[nodes.Count - 1].getLocation().Y - 30, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 25
                };

                if (passer.trailsVertical)
                {
                    end.Margin = new Thickness(nodes[nodes.Count - 1].getLocation().X + 80,
                        nodes[nodes.Count - 1].getLocation().Y, 0, 0);
                    end.RenderTransform = r;
                }

                MyCanvas.Children.Add(end);
            }
            #endregion
        }


        // Return which index of node the user has hit
        private int stylusHitTest(double x, double y)
        {
            int radius = 25;
            double left;
            double top;
            double first;
            double second;

            //calculate whether the stylus x and y are within the circle defined by the node[i] using definition of circle
            for (int i = 0; i < nodes.Count; i++)
            {
                left = nodes[i].getEllipse().Margin.Left;
                top = nodes[i].getEllipse().Margin.Top;
                first = Math.Pow(x - (left + radius), 2);
                second = Math.Pow(y - (top + radius), 2);
                if ((first + second <= radius * radius) == true)
                {
                    return i;
                }
            }

            //if no node was intersected with, return -1
            return -1;
        }

        private int getColorTimeInterval()
        {
            double testDuration = (testReplay.getEndTime() - testReplay.getStartTime()).TotalSeconds;
            if (testDuration <= 8) { return 1; }
            else if (testDuration <= 40) { return 5; }
            else if (testDuration <= 120) { return 15; }
            else { return 30; }

        }

        private void removeAllStrokes()
        {
            MyCanvas.Children.Clear();
        }

        // Place a Textarea on the screen which will display the total time taken for the test.
        private void createTime()
        {
            // Determine the total time taken for the test from the file.
            TimeSpan testTime = testReplay.getEndTime() - testReplay.getStartTime();

            // Add a Textarea for time taken
            TextBlock text = new TextBlock();
            text.Margin = new Thickness(550, 25, 0, 0);
            text.Text = "Total Time: " + testTime.Seconds + " Seconds";
            text.FontSize = 20;
            MyCanvas.Children.Add(text);
        }

        // Place a Textarea on the screen which will display the number of errors on the test.
        private void createErrors()
        {
            // Determine the errors for the test from the file.
            List<TestError> errors = testReplay.getErrors();

            // Add a Textarea for errors
            TextBlock text = new TextBlock();
            text.Margin = new Thickness(550, 50, 0, 0);
            text.Text = "Total Errors: " + errors.Count;
            text.FontSize = 20;
            MyCanvas.Children.Add(text);
        }

        // Display statistics for the test such as test time, errors, left-right analysis
        private void viewStatistics()
        {
            // Grab all node completions
            List<NodeCompletion> completions = testReplay.getCompletions();

            // Grab all test errors
            List<TestError> errors = testReplay.getErrors();

            // If completions do not exist for this file, return right away.
            // Otherwise it will crash
            if(completions.Count < 25)
            {
                return;
            }

            // Start with a clean screen
            removeAllStrokes();

            createTime();
            createErrors();

            // Add a Textarea for time taken between nodes
            #region paths
            ListBox nodes = new ListBox();
            nodes.Margin = new Thickness(400, 100, 0,0);
            nodes.Height = 575;
            nodes.Width = 300;
            nodes.FontSize = 17;

            if (passer.trailsTestVersion == 'A')
            {
                for(int i = 1; i < 25; i++)
                {
                    string s = "";
                    s += i.ToString();
                    s += "   ->   ";
                    s += (i+1).ToString();
                    s += "   =   ";
                    
                    
                    // Calculate how much time between each completion
                    DateTime end = completions[i].getTime();
                    DateTime begin = completions[i - 1].getTime();
                    TimeSpan span = end-begin;
                    Double time =  (span.Milliseconds / 100.0) + span.Seconds;
                    s += time.ToString();
                    s += "   seconds";
                    nodes.Items.Add(s);
                }
            }
            else if (passer.trailsTestVersion == 'B')
            {
                int num = 1;
                char let = 'A';
                for (int i = 1; i < 25; i++)
                {
                    DateTime end = completions[i].getTime();
                    DateTime begin = completions[i - 1].getTime();
                    TimeSpan span = end - begin;
                    if(i % 2 == 1)
                    {
                        string s = num.ToString() + "   ->   " + ((char)(let)).ToString();
                        Double time = (span.Milliseconds / 100.0) + span.Seconds;
                        s += "   =   ";
                        s += time.ToString();
                        s += "   seconds";
                        nodes.Items.Add(s);
                        num += 1;
                    }
                    else
                    {
                        string s = let.ToString() + "   ->   " + (num).ToString();
                        Double time = (span.Milliseconds / 100.0) + span.Seconds;
                        s += "   =   ";
                        s += time.ToString();
                        s += "   seconds";
                        nodes.Items.Add(s);
                        let = (char)(1+let);
                    }
                }
            }
            MyCanvas.Children.Add(nodes);
            #endregion

            // Add a Textarea for the test errors
            #region errors
            ListBox ers = new ListBox();
            ers.Margin = new Thickness(750, 100, 0, 0);
            ers.Height = 575;
            ers.Width = 300;
            ers.FontSize = 17;

            TextBlock errTxt = new TextBlock();
            errTxt.Margin = new Thickness(750, 75, 0, 0);
            errTxt.FontSize = 17;
            errTxt.Text = "Errors";

            for (int i = 0; i < errors.Count; i++ )
            {
                ers.Items.Add(errors[i].getBegin().getNodeText() + "      To     " + errors[i].getActualEnd().getNodeText());
            }
            MyCanvas.Children.Add(errTxt);
            MyCanvas.Children.Add(ers);
            #endregion
        }

        private void viewColorTimeMode()
        {
            removeAllStrokes();

            createTime();

            populateNodes(passer.trailsTestVersion, nodes);
            
            var allActions = testReplay.getTestActions();
            var smartStrokes = allActions.OfType<SmartStroke.Stroke>();
            var strokes = inkManager.GetStrokes();
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
            DateTime prevTime = testReplay.getStartTime();
            strokeNum = 0;
            foreach (var stroke in smartStrokes)
            {

                var smartLines = smartStrokes.ElementAt(strokeNum).lines;
                int lineNum = 0;
                foreach (LineData lineData in smartLines)
                {

                    Line l = lineData.getLine();
                    int secondsSinceStartOfTest = (int)Math.Floor((smartLines[lineNum].getDateTime() - testReplay.getStartTime()).TotalSeconds);
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


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as InfoPasser;    // This is the type of the trails test.


            nodes.Clear();
            
            TEST_TYPE type = TEST_TYPE.TRAILS_A;
            if (passer.trailsTestVersion == 'A')
                type = TEST_TYPE.TRAILS_A;
            else if (passer.trailsTestVersion == 'B')
                type = TEST_TYPE.TRAILS_B;

            testReplay = new TestReplay(passer.currentPatient, type);

            if (passer.currentPatient.getTestFilenames().Count > 0)
            {
                currentlySelectedDate = passer.currentPatient.getTestFilenames()[0];
            }

            var stuff = passer.currentPatient.getTestFilenames();
            foreach (string filename in stuff)
            {
                if (filename.Contains(testReplay.getTestType().ToString()))
                {
                    testDatesBox.Items.Add(testReplay.getDisplayedDatetime(filename));
                }
            }
        }

        async void loadTest()
        {
            currentlySelectedDate = testReplay.getFilenameString(testDatesBox.SelectedItem.ToString());
            List<string> fileNames = passer.currentPatient.getTestFilenames();
            foreach (string name in fileNames)
            {
                if (name.Contains(currentlySelectedDate))
                {
                    await testReplay.loadTestReplay(name);
                    break;
                }
            }
        }

        void testSelected(object sender, SelectionChangedEventArgs e)
        {
            loadTest();
        }

        private void gotoMenu(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu), passer);
        }

        private void displayTimeElapsed(object sender, RoutedEventArgs e)
        {
            viewColorTimeMode();
        }

        // Button clicked to display the statistics for the test
        private void displayStatistics(object sender, RoutedEventArgs e)
        {
            viewStatistics();
        }
    }
}
