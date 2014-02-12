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
        const double DRAW_WIDTH = 4.0;
        const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Colors.Blue;
        private Color ERASE_COLOR = Colors.White;
        //class member variables
        InkManager ink_manager = new Windows.UI.Input.Inking.InkManager();
        private uint pen_id;
        private uint touch_id;
        private Point previous_contact_pt;
        private Point current_contact_pt;
        private bool erasing;

        DispatcherTimer timer;

        public TrailsTest()
        {
            this.InitializeComponent();

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
                    Ellipse ellipse;
                    if (erasing)
                    {
                        ellipse = new Ellipse()
                        {
                            Margin = new Thickness(x1, y1, x2, y2),
                            Height = ERASE_WIDTH,
                            Width = ERASE_WIDTH,
                            Fill = new SolidColorBrush(ERASE_COLOR)
                        };
                        MyCanvas.Children.Add(ellipse);
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

                    //MyCanvas.Children.Add(ellipse);

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
        }
    }
}
