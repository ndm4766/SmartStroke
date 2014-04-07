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

            determineScreenSize();
        }

        private void populateNodes(char kind, List<TrailNode> nodes)
        {
            SolidColorBrush backgroundColor = new SolidColorBrush(Colors.Transparent);
            if (kind == 'A')
            {
                nodes.Add(new TrailNode(1, new Point(257, 421), MyCanvas));
                TextBlock begin = new TextBlock()
                {
                    Text = "Begin",
                    Margin = new Thickness(330, 425, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                RotateTransform r = new RotateTransform();
                r.Angle = 90;
                begin.RenderTransform = r;
                MyCanvas.Children.Add(begin);

                nodes.Add(new TrailNode(2, new Point(150, 322), MyCanvas));
                nodes.Add(new TrailNode(3, new Point(150, 491), MyCanvas));
                nodes.Add(new TrailNode(4, new Point(584, 501), MyCanvas));
                nodes.Add(new TrailNode(5, new Point(480, 312), MyCanvas));
                nodes.Add(new TrailNode(6, new Point(382, 402), MyCanvas));
                nodes.Add(new TrailNode(7, new Point(320, 279), MyCanvas));
                nodes.Add(new TrailNode(8, new Point(163, 127), MyCanvas));
                nodes.Add(new TrailNode(9, new Point(76, 155), MyCanvas));
                nodes.Add(new TrailNode(10, new Point(163, 241), MyCanvas));
                nodes.Add(new TrailNode(11, new Point(52, 317), MyCanvas));
                nodes.Add(new TrailNode(12, new Point(42, 48), MyCanvas));
                nodes.Add(new TrailNode(13, new Point(446, 97), MyCanvas));
                nodes.Add(new TrailNode(14, new Point(358, 44), MyCanvas));
                nodes.Add(new TrailNode(15, new Point(829, 43), MyCanvas));
                nodes.Add(new TrailNode(16, new Point(671, 109), MyCanvas));
                nodes.Add(new TrailNode(17, new Point(890, 227), MyCanvas));
                nodes.Add(new TrailNode(18, new Point(670, 273), MyCanvas));
                nodes.Add(new TrailNode(19, new Point(745, 434), MyCanvas));
                nodes.Add(new TrailNode(20, new Point(754, 316), MyCanvas));
                nodes.Add(new TrailNode(21, new Point(900, 363), MyCanvas));
                nodes.Add(new TrailNode(22, new Point(798, 618), MyCanvas));
                nodes.Add(new TrailNode(23, new Point(79, 643), MyCanvas));
                nodes.Add(new TrailNode(24, new Point(452, 565), MyCanvas));

                foreach (var node in nodes)
                {
                    node.setFillColor(backgroundColor);
                }

                TextBlock end = new TextBlock()
                {
                    Text = "End",
                    Margin = new Thickness(520, 575, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                end.RenderTransform = r;
                MyCanvas.Children.Add(end);
            }
            else if (kind == 'B')
            {
                nodes.Add(new TrailNode(1, new Point(530, 355), MyCanvas));
                TextBlock begin = new TextBlock()
                {
                    Text = "Begin",
                    Margin = new Thickness(600, 360, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                RotateTransform r = new RotateTransform();
                r.Angle = 90;
                begin.RenderTransform = r;
                MyCanvas.Children.Add(begin);
                nodes.Add(new TrailNode('A', new Point(240, 488), MyCanvas));
                nodes.Add(new TrailNode(2, new Point(265, 249), MyCanvas));
                nodes.Add(new TrailNode('B', new Point(766, 318), MyCanvas));
                nodes.Add(new TrailNode(3, new Point(654, 394), MyCanvas));
                nodes.Add(new TrailNode('C', new Point(453, 486), MyCanvas));
                nodes.Add(new TrailNode(4, new Point(812, 488), MyCanvas));
                nodes.Add(new TrailNode('D', new Point(797, 586), MyCanvas));
                nodes.Add(new TrailNode(5, new Point(389, 582), MyCanvas));
                nodes.Add(new TrailNode('E', new Point(168, 544), MyCanvas));
                nodes.Add(new TrailNode(6, new Point(189, 373), MyCanvas));
                nodes.Add(new TrailNode('F', new Point(103, 205), MyCanvas));
                nodes.Add(new TrailNode(7, new Point(518, 162), MyCanvas));
                nodes.Add(new TrailNode('G', new Point(402, 103), MyCanvas));
                nodes.Add(new TrailNode(8, new Point(882, 83), MyCanvas));
                nodes.Add(new TrailNode('H', new Point(681, 182), MyCanvas));
                nodes.Add(new TrailNode(9, new Point(816, 185), MyCanvas));
                nodes.Add(new TrailNode('I', new Point(892, 428), MyCanvas));
                nodes.Add(new TrailNode(10, new Point(881, 638), MyCanvas));
                nodes.Add(new TrailNode('J', new Point(302, 613), MyCanvas));
                nodes.Add(new TrailNode(11, new Point(87, 642), MyCanvas));
                nodes.Add(new TrailNode('K', new Point(56, 54), MyCanvas));
                nodes.Add(new TrailNode(12, new Point(478, 45), MyCanvas));

                foreach (var node in nodes)
                {
                    node.setFillColor(backgroundColor);
                }

                TextBlock end = new TextBlock()
                {
                    Text = "End",
                    Margin = new Thickness(550, 55, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                end.RenderTransform = r;
                MyCanvas.Children.Add(end);
            }
        }

        /*
         * Perceptive Pixel - DPIX/DPIY = 40, DPILog = 96 Res=1920X1080 Scale = 100
         * Slate            - DPIX/DPIY = 135,DPILog = 96 Res=1920X1080 Scale = 100
         * Surface          - DPIX/DPIY = 120.DPILog = 207Res=1920X1080 Scale = 140
         */
        private void determineScreenSize()
        {
            DisplayInformation display = DisplayInformation.GetForCurrentView();
            float dpi = display.LogicalDpi;
            float xdpi = display.RawDpiX;
            float ydpi = display.RawDpiY;
            double dots = xdpi * Window.Current.Bounds.Width;
            ResolutionScale scale = display.ResolutionScale;

            //if large screen
            if (xdpi < 50)
            {
                MyCanvas.Height = 768;
                MyCanvas.Width = 1366;
                //timer_box.Text = "PIXEL";
            }
            else
            {
                MyCanvas.Height = 768;
                MyCanvas.Width = 1366;
            }
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
            populateNodes(passer.trailsTestVersion, nodes);
        }

        private void viewColorTimeMode()
        {
            removeAllStrokes();
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
            populateNodes(passer.trailsTestVersion, nodes);      // Populate the list of trail nodes (no longer occurs in constructor bec need to have passer to know which test to load)

            //TODO must have real patient info here
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
            testDatesBox.SelectionChanged -= testSelected;
            viewColorTimeMode();
            testDatesBox.SelectionChanged += testSelected;
        }

        void testSelected(object sender, SelectionChangedEventArgs e)
        {
            loadTest();
        }

        private void gotoMenu(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu), passer);
        }
    }
}
