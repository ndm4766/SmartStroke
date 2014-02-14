using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartStroke
{
    /// <summary>
    /// The page for the trails test to be performed
    /// </summary>
    public sealed partial class TrailsTest : Page
    {
        string testVersion;
        const double DRAW_WIDTH = 4.0;
        const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Colors.Blue;
        private Color ERASE_COLOR = Colors.White;
        //class member variables
        InkManager ink_manager = new Windows.UI.Input.Inking.InkManager();
        private InkDrawingAttributes drawingAttributes;
        private uint pen_id;
        private uint touch_id;
        private Point previous_contact_pt;
        private Point current_contact_pt;
        private bool erasing;
        Rectangle e;
        List<TrailNode> nodes;
        string nextItem = "2";                  // This will be the next item to look for - next node
        string currentItem = "1";         // This is the item/node the user has most recently found
        int nextIndex = 1;
        List<InkStroke> nodeToNode; // Keep a list of the strokes from node to node.

        DispatcherTimer timer;

        public TrailsTest()
        {
            this.InitializeComponent();
            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Portrait;
            
            // Create the trails test background. The test image is 117X917 px but to fit on a screen (surface) it is 686 X 939
            nodes = new List<TrailNode>();
            populateNodes(testVersion, nodes);

            //add all the event handlers for touch/pen/mouse input (pointer handles all 3)
            MyCanvas.PointerPressed += new PointerEventHandler(MyCanvas_PointerPressed);
            MyCanvas.PointerMoved += new PointerEventHandler(MyCanvas_PointerMoved);
            MyCanvas.PointerReleased += new PointerEventHandler(MyCanvas_PointerReleased);
            MyCanvas.PointerExited += new PointerEventHandler(MyCanvas_PointerReleased);

            erasing = false;

            //initialize timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_tick;
            //timer.Start();

            //Set the ink to not use bezeir curves
            drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            // True is the Default value for fitToCurve.
            drawingAttributes.FitToCurve = false;
            ink_manager.SetDefaultDrawingAttributes(drawingAttributes);

            var windowHeight = Windows.UI.Xaml.Window.Current.Bounds.Height;
            var windowWidth = Windows.UI.Xaml.Window.Current.Bounds.Width;
        }

        private void populateNodes(string kind, List<TrailNode> nodes)
        {
            if (kind == "A")
            {
                nodes.Add(new TrailNode(1, new Point(257, 421), MyCanvas));
                nodes[0].setFillColor(new SolidColorBrush(Colors.Green));
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
            }
            else if(kind == "B")
            {
                nodes.Add(new TrailNode(1, new Point(530, 355), MyCanvas));
                nodes[0].setFillColor(new SolidColorBrush(Colors.Green));
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
            }

            // Define a PointerEntered and a PointerExited event handler for each node.
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].getEllipse().PointerEntered += new Windows.UI.Xaml.Input.PointerEventHandler(pointerEnteredCircle);
                nodes[i].getEllipse().PointerExited += new Windows.UI.Xaml.Input.PointerEventHandler(pointerLeftCircle);
            }
        }

        // The pointer (stylus) entered a circle. Change the color and update next circle
        // Currently the program crashes on line 156
        private void pointerEnteredCircle(object sender, PointerRoutedEventArgs e)
        {
            // Pointer Entered a Circle. Check if it is the correct cirlce they were expected to go to
            Ellipse circleEntered = (Ellipse)sender;

            if (nodes[nextIndex].getEllipse() == circleEntered) //check if the next circle is the one entered
            {
                //nextItem = (tn.getNumber() + 1).ToString();
                nodes[nextIndex].setFillColor(new SolidColorBrush(Colors.Green));
                nextIndex++;
            }
        }

        // Pointer left a node. Restart the next stroke.
        private void pointerLeftCircle(object sender, PointerRoutedEventArgs e)
        {

        }
        private bool hit_test(InkStroke s, Point test)
        {
            foreach(var p in s.GetRenderingSegments())
            {
                if (Math.Abs(test.X - p.Position.X) < 10 && Math.Abs(test.Y - p.Position.Y) < 10)
                //if (test.X == p.Position.X && test.Y == p.Position.Y)
                    return true;
            }
                return false;
        }

        private void timer_tick(object sender, object e)
        {
            //called every 100 ms
            //timer_box.Text = "time should be here";
        }

        private void clear_clicked(object sender, RoutedEventArgs e)
        {
            ink_manager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Erasing;

            var strokes = ink_manager.GetStrokes();

            foreach (var stroke in strokes)
            {
                stroke.Selected = true;
            }

            ink_manager.DeleteSelected();
            MyCanvas.Children.Clear();
        }


        #region PointerEvents
        private void MyCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerId == pen_id)
            {
                Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(MyCanvas);

                // Pass the pointer information to the InkManager. 
                ink_manager.ProcessPointerUp(pt);
            }

            else if (e.Pointer.PointerId == touch_id)
            {
                // Process touch input (finger input)
            }

            touch_id = 0;
            pen_id = 0;

            e.Handled = true;
        }

        private void MyCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            double x1, y1, x2, y2;

            if (e.Pointer.PointerId == pen_id)
            {
                PointerPoint pt = e.GetCurrentPoint(MyCanvas);

                current_contact_pt = pt.Position;
                x1 = previous_contact_pt.X;
                y1 = previous_contact_pt.Y; 
                x2 = current_contact_pt.X;
                y2 = current_contact_pt.Y;

                if (Distance(x1, y1, x2, y2) > 2.0) //test whether the pointer has moved far enough to warrant drawing a new line
                {
                    //Ellipse ellipse;
                    if (erasing)
                    {
                        /*
                        ellipse = new Ellipse()
                        {
                            Margin = new Thickness(x1, y1, x2, y2),
                            Height = ERASE_WIDTH,
                            Width = ERASE_WIDTH,
                            //Fill = new SolidColorBrush(ERASE_COLOR)
                        };
                         */

                        //Rect r;
                        var strokes = ink_manager.GetStrokes();
                        foreach(var stroke in strokes)
                        {
                            //r = stroke.BoundingRect;
                            if (hit_test(stroke, new Point(x2, y2)))
                            {
                                stroke.Selected = true;

                                // erase strokes. DOES NOT WORK. FIX ME
                                foreach (var child in MyCanvas.Children)
                                {
                                    //if child is a line object, check if its x2 y2 match the stroke's x2 y2
                                    if(child.GetType() == typeof(Line))
                                    {
                                        Line l = (Line)child;
                                        if (l.X1 == l.X2) continue;
                                        if ( ! (x2 >= l.X1 && x2 <= l.X2 && y2 >= l.Y1 && y2 <= l.Y2))
                                        {
                                            continue;
                                        }
                                        double realSlope = (l.Y2 - l.Y1)/(l.X2 - l.X1);
                                        double fakeSlope = (l.Y2 - y2) / (l.X1 - x1);

                                        //if(Math.Abs(l.X2 - x2) < 10 && Math.Abs(l.Y2 - y2) < 10)
                                        if(Math.Abs(realSlope - fakeSlope) < 5)
                                        {
                                            //actually remove the ink from the canvas
                                            MyCanvas.Children.Remove(child);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        //tell the ink manager to stop tracking the strokes that were erased
                        ink_manager.DeleteSelected();
                    }
                    else
                    {
                        Line line = new Line()
                        {
                            //Margin = new Thickness(x1, y1, x2, y2),
                            X1 = x1, X2 = x2, Y1 = y1, Y2 = y2,
                            //Height = DRAW_WIDTH,
                            //Width = DRAW_WIDTH,
                            StrokeThickness = DRAW_WIDTH,
                            Stroke = new SolidColorBrush(DRAW_COLOR)
                        };

                        MyCanvas.Children.Add(line);
                    }
                    
                    previous_contact_pt = current_contact_pt;

                    // Pass the pointer information to the InkManager.
                    ink_manager.ProcessPointerUpdate(pt);
                }
            }

            else if (e.Pointer.PointerId == touch_id)
            {
                // Process touch input (finger input)
            }

        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private void MyCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get information about the pointer location.
            PointerPoint pt = e.GetCurrentPoint(MyCanvas);
            previous_contact_pt = pt.Position;
            
            // Accept input only from a pen or mouse with the left button pressed. 
            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;
            if (pointerDevType == PointerDeviceType.Pen || (pointerDevType == PointerDeviceType.Mouse && pt.Properties.IsLeftButtonPressed))
            {
                //first check if the stylus' eraser is being used
                if (pt.Properties.IsEraser)
                {
                    ink_manager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Erasing;
                    erasing = true;
                    //selCanvas.style.cursor = "url(images/erase.cur), auto"; 
                }
                else
                {
                    erasing = false;
                }

                // Pass the pointer information to the InkManager.
                ink_manager.ProcessPointerDown(pt);
                pen_id = pt.PointerId;

                e.Handled = true;
            }
            else if (pointerDevType == PointerDeviceType.Touch)
            {
                // Process touch input (from finger)
            }
        }

        #endregion


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string vers = e.Parameter as string;    // This is the type of the trails test.
            testVersion = vers;

            nodes.Clear();
            populateNodes(testVersion, nodes);      // (Re)Populate the list of trail nodes
        }

    }
}
