using SmartStroke.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

/*
 *  TODO: measure other metrics such as if the "hands" are drawn to the correct time, if all 12 numbers are present,
 *      if more than 12 numbers are present or more than 2 "hands" are drawn, ...
 */

namespace SmartStroke
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ReyoTest : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        InfoPasser passer = new InfoPasser();

        private TestReplay testReplay;
        bool testStarted;

        //general globals
        private const double DRAW_WIDTH = 4.0;
        private const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Color.FromArgb(255, 50, 50, 50);
        private Color ERASE_COLOR = Colors.White;

        //boolean program state flags
        private bool erasing;
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
        private List<InkStroke> drawingOrder;

        //private List<int> quadWeights;

        //TrailNode handling members
        private List<Line> currentEdge;

        //Size of screen
        double screenWidth;
        double screenHeight;

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


        public ReyoTest()
        {
            this.InitializeComponent();

            inkManager = new Windows.UI.Input.Inking.InkManager();

            // Create the trails test background. The test image is 117X917 px but to fit on a screen (surface) it is 686 X 939
            currentLine = new List<Line>();
            allLines = new Dictionary<InkStroke, List<Line>>();
            drawingOrder = new List<InkStroke>();

            testStarted = false;
   
            //add all the event handlers for touch/pen/mouse input (pointer handles all 3)
            MyCanvas.PointerPressed += new PointerEventHandler(MyCanvas_PointerPressed);
            MyCanvas.PointerMoved += new PointerEventHandler(MyCanvas_PointerMoved);
            MyCanvas.PointerReleased += new PointerEventHandler(MyCanvas_PointerReleased);
            MyCanvas.PointerExited += new PointerEventHandler(MyCanvas_PointerReleased);

            erasing = false;
            currentEdge = new List<Line>();

            screenHeight = Window.Current.Bounds.Height;
            screenWidth = Window.Current.Bounds.Width;

            //Set the ink to not use bezeir curves
            drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            // True is the Default value for fitToCurve.
            drawingAttributes.FitToCurve = false;
            inkManager.SetDefaultDrawingAttributes(drawingAttributes);
        }

        private bool eraserHitTest(InkStroke s, Point testPoint)
        {
            foreach (var p in s.GetRenderingSegments())
            {
                if (Math.Abs(testPoint.X - p.Position.X) < 10 && Math.Abs(testPoint.Y - p.Position.Y) < 10)
                    return true;
            }
            return false;
        }

        //distance between two points: used to determine if a line drawn is long enough to draw
        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }


        #region PointerEvents

        private void MyCanvas_PointerReleased(object sender, PointerRoutedEventArgs e) //TODO: Need a way to ensure that pointerMoved has been executed and handled before now
        {
            if (e.Pointer.PointerId == penId)
            {
                Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(MyCanvas);

                // Pass the pointer information to the InkManager. 
                inkManager.ProcessPointerUp(pt);

                if (!erasing)
                {
                    //create the link from the completed stroke to its list of lines on the canvas
                    allLines.Add(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1], currentLine);
                    drawingOrder.Add(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1]);
                    //cant just clear the list cuz its c#, have to point to a new list, not a memory leak
                    currentLine = new List<Line>();

                    testReplay.endStroke();
                }
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

                    if (!testStarted)
                    {
                        testStarted = true;
                        testReplay.startTest();
                    }

                    if (erasing)
                    {
                        //check if the pressed cursor has collided with any strokes
                        var strokes = inkManager.GetStrokes();
                        for (int i = 0; i < strokes.Count; i++)
                        {
                            if (eraserHitTest(strokes[i], new Point(x2, y2)))
                            {
                                strokes[i].Selected = true;

                                //remove each of the lines associated with this single stroke from canvas
                                foreach (Line line in allLines[strokes[i]])
                                {
                                    MyCanvas.Children.Remove(line);
                                }

                                testReplay.deleteStroke(drawingOrder.IndexOf(strokes[i]));
                            }
                        }


                        //tell the ink manager to stop tracking the strokes that were erased
                        inkManager.DeleteSelected();
                    }
                    else //if drawing
                    {
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
                    }

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

        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            if (testReplay.getTestActions().Count == 0)
            {
                testReplay.endTest();
                this.Frame.Navigate(typeof(MainMenu), passer);
            }

            testReplay.endTest();
            testReplay.saveTestReplay();
            this.Frame.Navigate(typeof(MainMenu), passer);
        }

        private void MyCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(MyCanvas);
            previousContactPt = pt.Position;

            // Accept input only from a pen or mouse with the left button pressed. 
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen || pointerDevType == PointerDeviceType.Mouse)
            {
                //first check if the stylus' eraser is being used
                if (pt.Properties.IsEraser || pt.Properties.IsRightButtonPressed)
                {
                    inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Erasing;
                    erasing = true;
                    //selCanvas.style.cursor = "url(images/erase.cur), auto"; 
                }
                else //if left click
                {
                    inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Inking;
                    testReplay.beginStroke();
                    erasing = false;
                }

                // Pass the pointer information to the InkManager.
                inkManager.ProcessPointerDown(pt);
                penId = pt.PointerId;

                e.Handled = true;
                pressed = true;
            }
            else if (pointerDevType == PointerDeviceType.Touch)
            {
                // Process touch input (from finger)
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            passer = e.Parameter as InfoPasser;
            testReplay = new TestReplay(passer.currentPatient, TEST_TYPE.REY_OSTERRIETH);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        #endregion

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            testReplay.endTest();
            this.Frame.Navigate(typeof(MainMenu), passer);
        }


    }
}
