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
    public sealed partial class ClockTest : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        //test replay container
        private TestReplay testReplay;

        //general globals
        private string testVersion;
        private const double DRAW_WIDTH = 4.0;
        private const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Colors.Blue;
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

        private List<int> quadWeights;

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


        public ClockTest()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            inkManager = new Windows.UI.Input.Inking.InkManager();

            // Create the trails test background. The test image is 117X917 px but to fit on a screen (surface) it is 686 X 939
            currentLine = new List<Line>();
            allLines = new Dictionary<InkStroke, List<Line>>();

            quadWeights = new List<int>();
            quadWeights.Add(0);
            quadWeights.Add(0);
            quadWeights.Add(0);
            quadWeights.Add(0);

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

            determineScreenSize();
        }


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
            }
            else
            {
                MyCanvas.Height = 768;
                MyCanvas.Width = 1366;
            }
        }

        private Rectangle getQuadrantCollision(Line line)
        {
            List<Rectangle> quadrants = new List<Rectangle>();
            quadrants.Add(box1);
            quadrants.Add(box2);
            quadrants.Add(box3);
            quadrants.Add(box4);
            foreach (Rectangle quadrant in quadrants)
            {
                if (line.X1 > Canvas.GetLeft(quadrant) &&
                    line.X1 < Canvas.GetLeft(quadrant) + quadrant.Width &&
                    line.Y1 > Canvas.GetTop(quadrant) &&
                    line.Y1 < Canvas.GetTop(quadrant) + quadrant.Height)
                {
                    line.Fill = new SolidColorBrush(Colors.Red);
                    return quadrant;
                }
            }

            return null;
        }

        private void analyzeClicked(object sender, RoutedEventArgs e)
        {
            //reset the data collection
            quadWeights[0] = 0;
            quadWeights[1] = 0;
            quadWeights[2] = 0;
            quadWeights[3] = 0;

            foreach (var stroke in inkManager.GetStrokes())
            {
                foreach (Line line in allLines[stroke])
                {
                    Rectangle quad = getQuadrantCollision(line);

                    //increment the counter of the rect with the collision if there was one
                    if (quad != null)
                    {
                        if (quad == box1)
                        {
                            quadWeights[0]++;
                        }
                        else if (quad == box2)
                        {
                            quadWeights[1]++;
                        }
                        else if (quad == box3)
                        {
                            quadWeights[2]++;
                        }
                        else if (quad == box4)
                        {
                            quadWeights[3]++;
                        }
                        dataTextblock.Text = "Top Right: " + quadWeights[0].ToString() + "\n" +
                            "Top Left: " + quadWeights[1].ToString() + "\n" +
                            "Bot Left: " + quadWeights[2].ToString() + "\n" +
                            "Bot Right: " + quadWeights[3].ToString();
                    }
                }
            }

            
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

        private void MyCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == penId)
            {
                Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(MyCanvas);

                // Pass the pointer information to the InkManager. 
                inkManager.ProcessPointerUp(pt);

                if (!erasing)
                {
                    //create the link from the completed stroke to its list of lines on the canvas
                    allLines.Add(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1], currentLine);//TODO: erasing causing outofrange err
                    //cant just clear the list cuz its c#, have to point to a new list, not a memory leak
                    currentLine = new List<Line>();

                    //testReplay.endStroke();
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

                    if (erasing)
                    {
                        //check if the pressed cursor has collided with any strokes
                        foreach (var stroke in inkManager.GetStrokes())
                        {
                            if (eraserHitTest(stroke, new Point(x2, y2)))
                            {
                                stroke.Selected = true;

                                //remove each of the lines associated with this single stroke from canvas
                                foreach (Line line in allLines[stroke])
                                {
                                    MyCanvas.Children.Remove(line);
                                    allLines.Remove(stroke);
                                }
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

                        //testReplay.addLine(line);
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
            this.Frame.Navigate(typeof(MainPage));
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
                //first check if the stylus' eraser is being used
                if (pt.Properties.IsEraser)
                {
                    inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Erasing;
                    erasing = true;
                    //selCanvas.style.cursor = "url(images/erase.cur), auto"; 
                }
                else
                {
                    inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Inking;
                    //testReplay.beginStroke();
                    erasing = false;
                }

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

        #endregion

        private async void recognizedWordClicked(object sender, RoutedEventArgs e)
        {
            //get the available strokes from ink manager and select the strokes 
            string numbers = "";
            var strokes = inkManager.GetStrokes();

            for (int i = 0; i < strokes.Count; i++)
            {
                strokes[i].Selected = true;
            }
            //set handwriting recognizer 
            var recname = "Microsoft English (US) Handwriting Recognizer";
            var recognizers = inkManager.GetRecognizers();
            for (int i = 0, len = recognizers.Count; i < len; i++)
            {
                if (recname == recognizers[i].Name)
                {
                    inkManager.SetDefaultRecognizer(recognizers[i]);

                }
            }
            //asynchronously recognize the word 
            IReadOnlyList<InkRecognitionResult> x = await inkManager.RecognizeAsync(InkRecognitionTarget.Selected);
            //IReadOnlyList<String> text; 
            // Doing a recognition does not update the storage of results (the results that are stored inside the ink manager). 
            // We do that ourselves by calling this below method 
            inkManager.UpdateRecognitionResults((x));

            foreach (InkRecognitionResult i in x)
            {
                var candidates = i.GetTextCandidates();
                foreach (var possibleWord in candidates)
                {
                    int num;
                    if (int.TryParse(possibleWord, out num))
                    {
                        numbers += num + " : ";
                    }
                    break;  // Why is the break outside the if? It will only try one possible word out of all the candidates then..
                }
            }

            Windows.UI.Popups.MessageDialog mg = new Windows.UI.Popups.MessageDialog(numbers + "");
            await mg.ShowAsync();
        }

    }
}
