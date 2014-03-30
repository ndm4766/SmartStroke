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
    /// The page for the trails test to be performed
    /// </summary>
    public sealed partial class TrailsTest : Page
    {
        //test replay container
        private TestReplay testReplay;

        private bool finished = false;

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

        //Members to handle total test time
        private Stopwatch timer;
        private DispatcherTimer disp;

        //Size of screen
        double screenWidth;
        double screenHeight;

        public TrailsTest()
        {
            this.InitializeComponent();

            inkManager = new Windows.UI.Input.Inking.InkManager();

            

            // Create the trails test background. The test image is 117X917 px but to fit on a screen (surface) it is 686 X 939
            nodes = new List<TrailNode>();
            //populateNodes(testVersion, nodes);
            currentLine = new List<Line>();
            allLines = new Dictionary<InkStroke, List<Line>>();

            //add all the event handlers for touch/pen/mouse input (pointer handles all 3)
            MyCanvas.PointerPressed += new PointerEventHandler(MyCanvas_PointerPressed);
            MyCanvas.PointerMoved += new PointerEventHandler(MyCanvas_PointerMoved);
            MyCanvas.PointerReleased += new PointerEventHandler(MyCanvas_PointerReleased);
            MyCanvas.PointerExited += new PointerEventHandler(MyCanvas_PointerReleased);

            nextIndex = 0;
            currentIndex = 0;
            incorrectNodes = new Queue<int>();
            currentEdge = new List<Line>();

            timer = new Stopwatch();
            disp = new DispatcherTimer();
            disp.Interval = new TimeSpan(0, 0, 0, 0, 100);
            disp.Tick += timer_tick;
            //disp.Start();  //comment out this line to not display the timer
            
            screenHeight = Window.Current.Bounds.Height;
            screenWidth = Window.Current.Bounds.Width;

            //Set the ink to not use bezeir curves
            drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            // True is the Default value for fitToCurve.
            drawingAttributes.FitToCurve = false;
            inkManager.SetDefaultDrawingAttributes(drawingAttributes);

            determineScreenSize();
        }

        #region NodeCreation

        private void populateNodes(char kind, List<TrailNode> nodes)
        {
            #region TrailsA
            if (kind == 'A')
            {
                nodes.Add(new TrailNode(1, new Point(257, 421), MyCanvas, passer.trailsVertical));
                TextBlock begin = new TextBlock()
                {
                    Text = "Begin",
                    Margin = new Thickness(330, 425, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                RotateTransform r = new RotateTransform();
                r.Angle = 90;
                if (passer.trailsVertical)
                {
                    begin.RenderTransform = r;
                }
                MyCanvas.Children.Add(begin);
                nodes.Add(new TrailNode(2, new Point(150, 322), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(3, new Point(150, 491), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(4, new Point(584, 501), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(5, new Point(480, 312), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(6, new Point(382, 402), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(7, new Point(320, 279), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(8, new Point(163, 127), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(9, new Point(76, 155), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(10, new Point(163, 241), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(11, new Point(52, 317), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(12, new Point(42, 48), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(13, new Point(446, 97), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(14, new Point(358, 44), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(15, new Point(829, 43), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(16, new Point(671, 109), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(17, new Point(890, 227), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(18, new Point(670, 273), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(19, new Point(745, 434), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(20, new Point(754, 316), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(21, new Point(900, 363), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(22, new Point(798, 618), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(23, new Point(79, 643), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(24, new Point(452, 565), MyCanvas, passer.trailsVertical));
                TextBlock end = new TextBlock()
                {
                    Text = "End",
                    Margin = new Thickness(520, 575, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                if(passer.trailsVertical)
                {
                    end.RenderTransform = r;
                }
                
                MyCanvas.Children.Add(end);
            }
            #endregion
            #region TrailsB
            else if (kind == 'B')
            {
                nodes.Add(new TrailNode(1, new Point(530, 355), MyCanvas, passer.trailsVertical));
                TextBlock begin = new TextBlock()
                {
                    Text = "Begin",
                    Margin = new Thickness(600, 360, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };
                RotateTransform r = new RotateTransform();
                r.Angle = 90;
                if (passer.trailsVertical)
                {
                    begin.RenderTransform = r;
                }
                MyCanvas.Children.Add(begin);
                nodes.Add(new TrailNode('A', new Point(240, 488), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(2, new Point(265, 249), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('B', new Point(766, 318), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(3, new Point(654, 394), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('C', new Point(453, 486), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(4, new Point(812, 488), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('D', new Point(797, 586), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(5, new Point(389, 582), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('E', new Point(168, 544), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(6, new Point(189, 373), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('F', new Point(103, 205), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(7, new Point(518, 162), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('G', new Point(402, 103), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(8, new Point(882, 83), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('H', new Point(681, 182), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(9, new Point(816, 185), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('I', new Point(892, 428), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(10, new Point(881, 638), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('J', new Point(302, 613), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(11, new Point(87, 642), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode('K', new Point(56, 54), MyCanvas, passer.trailsVertical));
                nodes.Add(new TrailNode(12, new Point(478, 45), MyCanvas, passer.trailsVertical));
                TextBlock end = new TextBlock()
                {
                    Text = "End",
                    Margin = new Thickness(550, 55, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 15
                };

                if(passer.trailsVertical)
                    end.RenderTransform = r;

                MyCanvas.Children.Add(end);
            }
            #endregion
        }

        #endregion

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
            if(xdpi < 50)
            {
                MyCanvas.Height = 768;
                MyCanvas.Width = 1366;
                timer_box.Text = "PIXEL";
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
            //adds a buffer zone surrounding each node so that the patient can be off by a little but it will still count
            int buffer = 3;
            int radius = 25 + buffer;
            double left;
            double top;
            double first;
            double second;

            //calculate whether the stylus x and y are within the circle defined by the node[i] using definition of circle
            for (int i = 0; i < nodes.Count; i++)
            {
                left = nodes[i].getEllipse().Margin.Left - buffer;
                top = nodes[i].getEllipse().Margin.Top - buffer;
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

        private void timer_tick(object sender, object e)
        {
            //update the textbox with the current time in the stopwatch
            timer_box.Text = String.Format("{0}:{1}:{2}",
                timer.Elapsed.Minutes.ToString(),
                timer.Elapsed.Seconds.ToString("D2"),
                (timer.Elapsed.Milliseconds / 10).ToString("D2"));
        }

        //distance between two points: used to determine if a line drawn is long enough to draw
        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        // Go through and set anything that was yellow previously to Green
        // If a node was red, reset it to be blank. There should just be
        // One of each.
        private void resetIncorrectNodes(int node)
        {
            if (incorrectNodes.Count > 0)
            {
                int index;
                index = incorrectNodes.Dequeue();
                nodes[index].getEllipse().Fill = new SolidColorBrush(Colors.Green);

                while(incorrectNodes.Count > 0)
                {
                    index = incorrectNodes.Dequeue(); //remove from 0 size queue should be fixed now, contact nick if errors here
                    nodes[index].getEllipse().Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
           }
        }

        #region PointerEvents

        private void MyCanvas_PointerReleased(object sender, PointerRoutedEventArgs e) //TODO: need a way to ensure that pointerMoved has been handled before now
        {
            if (e.Pointer.PointerId == penId)
            {
                Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(MyCanvas);

                // Pass the pointer information to the InkManager. 
                inkManager.ProcessPointerUp(pt);
                testReplay.endStroke();

                allLines.Add(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1], currentLine);
                currentLine = new List<Line>();
            }

            else if (e.Pointer.PointerId == touchId)
            {
                // Process touch input (finger input)
            }

            touchId = 0;
            penId = 0;

            e.Handled = true;
            pressed = false;
        }

        private void MyCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            double x1, y1, x2, y2;
            int indexHit = -1;

            if (e.Pointer.PointerId == penId)
            {
                PointerPoint pt = e.GetCurrentPoint(MyCanvas);

                currentContactPt = pt.Position;
                x1 = previousContactPt.X;
                y1 = previousContactPt.Y;
                x2 = currentContactPt.X;
                y2 = currentContactPt.Y;

                //test whether the pointer has moved far enough to warrant drawing a new line
                if (Distance(x1, y1, x2, y2) > 2.0) 
                {
                    if (!pressed)
                    {
                        return;
                    }

                    //check if the stylus has collided with the "current" node to reset any error colors
                    indexHit = stylusHitTest(x2, y2);
                    if(indexHit == currentIndex)
                    {
                        nodes[currentIndex].setFillColor(new SolidColorBrush(Colors.Green));
                        resetIncorrectNodes(currentIndex + 1);
                    }

                    // The stylus has made contact with the correct next node.
                    if (indexHit == nextIndex)
                    {
                        // Start the timer keeping track of total time
                        if (!timer.IsRunning)
                        {
                            timer.Start();
                            testReplay.startTest();
                        }

                        // Set the node completed value equal to true and change the color to Green
                        nodes[nextIndex].setFillColor(new SolidColorBrush(Colors.Green));
                        nodes[nextIndex].setComplete(true);

                        // Change the index of the next node to look for and the current index
                        currentIndex = nextIndex;
                        nextIndex++;

                        //reset the list of lines so that if an error is made, the lines just drawn do not get erased
                        currentEdge.Clear();

                        //if the test is done, this code is executed
                        if (nextIndex >= nodes.Count)
                        {
                            
                            timer.Stop();
                            MyCanvas.PointerPressed -= MyCanvas_PointerPressed;
                            MyCanvas.PointerMoved -= MyCanvas_PointerMoved;
                            MyCanvas.PointerReleased -= MyCanvas_PointerReleased;
                            MyCanvas.PointerExited -= MyCanvas_PointerReleased;

                            inkManager.ProcessPointerUp(pt);

                            testReplay.endStroke();
                            testReplay.endTest();
                            
                            submitButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            submitButton.IsHitTestVisible = true;

                            saveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            saveButton.IsHitTestVisible = true;
                            
                            allLines.Add(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1], currentLine);
                            currentLine = new List<Line>();
                            finished = true;
                            return;
                        }
                    }
                    // Stylus did not contact the next node in the correct order.
                    // Need to change the color of the corrected node to Yellow and the
                        // incorrect node hit to red to notify the user this is not correct.
                    else if ((indexHit >= 0) && indexHit > currentIndex)
                    {
                        // If the user ran over a node that was already completed, ignore executing
                            // this code
                        //if (!nodes[indexHit].getCompleted() && nodes[indexHit].getEllipse().Fill != null)
                        //{

                        //TODO: need to take out the successive red nodes(only the first err should be red)
                        //TODO: after an err, no lines should be able to be drawn if they are still holding down the pen (when you collide with ANOTHER node after the first wrong one, it continues drawing)

                        //set error colors
                        if(incorrectNodes.Count < 1)
                            nodes[indexHit].setFillColor(new SolidColorBrush(Colors.Red));
                        nodes[currentIndex].setFillColor(new SolidColorBrush(Colors.Yellow));

                        //reset the index back 1
                        nextIndex = currentIndex;

                        if (!incorrectNodes.Contains(currentIndex))
                            incorrectNodes.Enqueue(currentIndex);

                        if (!incorrectNodes.Contains(indexHit))
                            incorrectNodes.Enqueue(indexHit);

                        //erase the line just drawn from previous node to the incorrect node
                        foreach (Line l in currentEdge)
                        {
                            MyCanvas.Children.Remove(l);
                        }
                        testReplay.endStroke();
                        testReplay.deleteStroke(testReplay.getTestActions().Count-1);
                        //}
                    }
                    
                        Line line = new Line()
                        {
                            X1 = x1,
                            X2 = x2,
                            Y1 = y1,
                            Y2 = y2,
                            StrokeThickness = DRAW_WIDTH,
                            Stroke = new SolidColorBrush(DRAW_COLOR)
                        };
                        currentLine.Add(line);
                        currentEdge.Add(line);
                        MyCanvas.Children.Add(line);

                        testReplay.addLine(line);
                    
                    if(!finished)
                        inkManager.ProcessPointerUpdate(pt);
                    previousContactPt = currentContactPt;
                }
            }

            else if (e.Pointer.PointerId == touchId)
            {
                // Process touch input (finger input)
            }
            e.Handled = true;
        }

        private void MyCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(MyCanvas);
            previousContactPt = pt.Position;

            // Accept input only from a pen or mouse with the left button pressed. 
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen || (pointerDevType == PointerDeviceType.Mouse && pt.Properties.IsLeftButtonPressed))
            {
                testReplay.beginStroke();
                // Pass the pointer information to the InkManager.
                inkManager.ProcessPointerDown(pt);
                penId = pt.PointerId;

                e.Handled = true;
            }
            else if (pointerDevType == PointerDeviceType.Touch)
            {
                // Process touch input (from finger)
            }

            pressed = true;
        }

        #endregion

        #region ButtonClicks

        // Need to cancel the test for some reason
        private void cancelTest(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            disp.Stop();
            this.Frame.Navigate(typeof(MainMenu), passer);
        }

        // Test is finished.. take a picture of the screen.
        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            passer.currentPatient = null;
            testReplay.saveTestReplay();
            this.Frame.Navigate(typeof(MainPage), passer);

        }
        private void TimeButtonClicked(object sender, RoutedEventArgs e)
        {
            passer.addTestToPasser(testReplay);
            this.Frame.Navigate(typeof(TrailsTestTimeViz), passer);

        }

        private void viewColorTimeMode()
        {
            var allActions = testReplay.getTestActions();
            var smartStrokes = allActions.OfType<SmartStroke.Stroke>();
            var strokes = inkManager.GetStrokes();
            int strokeNum = 0;
            int timeInterval = getColorTimeInterval();
            double msLineDuration = 0;
            SolidColorBrush color = new SolidColorBrush(Colors.Black);
            foreach (InkStroke stroke in strokes)
            {

                var smartLines = smartStrokes.ElementAt(strokeNum).lines;
                int lineNum = 0;
                DateTime prevTime = testReplay.getStartTime();
                
                foreach (Line l in allLines[stroke])
                {
                    //msLineDuration = (smartLines[lineNum].getDateTime()-prevTime).TotalMilliseconds;
                    MyCanvas.Children.Remove(l);
                    //l.StrokeThickness = msLineDuration * 1;//change one to a constant (less than 1) that makes it look okay
                    l.StrokeThickness = 10;
                    MyCanvas.Children.Add(l);
                    lineNum++;
                    //we probably want to render the line as a bezier curve. look at bezierCurveTo
                }
                strokeNum++;
                
            }
            foreach (InkStroke stroke in strokes)
            {
                
                var smartLines = smartStrokes.ElementAt(strokeNum).lines;
                int lineNum = 0;
                foreach (Line l in allLines[stroke])
                {
                    int secondsSinceStartOfTest = (int)Math.Floor((smartLines[lineNum].getDateTime() - testReplay.getStartTime()).TotalSeconds);
                    //assign 1 of 8 colors
                    if ((secondsSinceStartOfTest/timeInterval +1) % 8 == 1) { color = new SolidColorBrush(Color.FromArgb(255, 255,192,0)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 2) { color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 3) { color = new SolidColorBrush(Color.FromArgb(255, 146, 208, 80)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 4) { color = new SolidColorBrush(Color.FromArgb(255, 0, 176, 80)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 5) { color = new SolidColorBrush(Color.FromArgb(255, 0, 176, 240)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 6) { color = new SolidColorBrush(Color.FromArgb(255, 0, 112, 192)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 7) { color = new SolidColorBrush(Color.FromArgb(255, 0, 32, 96)); }
                    else if ((secondsSinceStartOfTest / timeInterval + 1) % 8 == 0) { color = new SolidColorBrush(Color.FromArgb(255, 112, 48, 160)); }
                    MyCanvas.Children.Remove(l);
                    l.Stroke = color;
                    MyCanvas.Children.Add(l);
                    lineNum++;
                }
                strokeNum++;
            }
            
        }

        //this is experimental code
        /*private void makeBezier()
        {
            //drawingAttributes.FitToCurve = true;
            //inkManager.SetDefaultDrawingAttributes(drawingAttributes);
            RenderStroke(50);
        }

        private void RenderStroke(double width)
        {
            var strokes = inkManager.GetStrokes();
      
            foreach (InkStroke stroke in strokes)
            {
                // Each stroke might have more than one segments
                var renderingStrokes = stroke.GetRenderingSegments();

                //
                // Set up the Path to insert the segments
                var path = new Windows.UI.Xaml.Shapes.Path();
                path.Data = new PathGeometry();
                ((PathGeometry)path.Data).Figures = new PathFigureCollection();

                var pathFigure = new PathFigure();
                pathFigure.StartPoint = renderingStrokes.First().Position;
                ((PathGeometry)path.Data).Figures.Add(pathFigure);

                //
                // Foreach segment, we add a BezierSegment
                foreach (var renderStroke in renderingStrokes)
                {
                    pathFigure.Segments.Add(new BezierSegment()
                    {
                        Point1 = renderStroke.BezierControlPoint1,
                        Point2 = renderStroke.BezierControlPoint2,
                        Point3 = renderStroke.Position
                    });
                }

                // Set the general options (i.e. Width and Color)
                path.StrokeThickness = width;



                MyCanvas.Children.Add(path);
            }
        }*/

        private int getColorTimeInterval()
        {
            double testDuration = (testReplay.getEndTime() - testReplay.getStartTime()).TotalSeconds;
            if (testDuration <= 8) { return 1; }
            else if (testDuration <= 40) { return 5; }
            else if (testDuration <= 120) { return 15; }
            else { return 30; }
      
        }

        #endregion

        #region navigationToAndFromPage

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as InfoPasser;    // This is the type of the trails test.

            nodes.Clear();
            populateNodes(passer.trailsTestVersion, nodes);      // Populate the list of trail nodes (no longer occurs in constructor bec need to have passer to know which test to load)

            //TODO must have real patient info here
            TEST_TYPE type = TEST_TYPE.TRAILS_A;
            if(passer.trailsTestVersion == 'A')
                type = TEST_TYPE.TRAILS_A;
            else if(passer.trailsTestVersion == 'B')
                type = TEST_TYPE.TRAILS_B;
            testReplay = new TestReplay(passer.currentPatient, type);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            timer.Stop();
        }

        #endregion

        #region SaveTest

        // Create a new screenshot of the test. Can save as a jpg, png, etc.
        async void btnScreenshot_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = await new ScreenExporter().StartAsync(MyCanvas);
        }
        #endregion
    }
}
