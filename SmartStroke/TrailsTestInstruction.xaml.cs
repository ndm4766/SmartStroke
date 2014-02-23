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
using SmartStroke.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SmartStroke
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TrailsTestInstruction : Page
    {
        private string version;
        private List<TrailNode> nodes;
        private Stopwatch timer;
        private DispatcherTimer disp;

        //ink drawing members
        private InkManager ink_manager;
        private InkDrawingAttributes drawingAttributes;
        private const double DRAW_WIDTH = 4.0;
        private Color DRAW_COLOR = Colors.Blue;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Point currentPoint;
        private Point previousPoint;
        private int currentNode;

        private double amountToMove;

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


        public TrailsTestInstruction()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            nodes = new List<TrailNode>();
            ink_manager = new Windows.UI.Input.Inking.InkManager();

            //Set the ink to not use bezeir curves
            drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            // True is the Default value for fitToCurve.
            drawingAttributes.FitToCurve = false;
            ink_manager.SetDefaultDrawingAttributes(drawingAttributes);

            timer = new Stopwatch();
            disp = new DispatcherTimer();
            disp.Interval = new TimeSpan(0, 0, 0, 0, 10);
            disp.Tick += timer_tick;
            disp.Start();

            amountToMove = 1;
        }

        private void populateNodes()
        {
            if(version == "A")
            {
                nodes.Add(new TrailNode(1, new Point(377, 244), MyCanvas));
                nodes.Add(new TrailNode(2, new Point(554, 68), MyCanvas));
                nodes.Add(new TrailNode(3, new Point(763, 463), MyCanvas));
                nodes.Add(new TrailNode(4, new Point(541, 225), MyCanvas));
                nodes.Add(new TrailNode(5, new Point(585, 400), MyCanvas));
                nodes.Add(new TrailNode(6, new Point(126, 408), MyCanvas));
                nodes.Add(new TrailNode(7, new Point(109, 155), MyCanvas));
                nodes.Add(new TrailNode(8, new Point(336, 137), MyCanvas));
            }
            else if(version == "B")
            {
                nodes.Add(new TrailNode(1, new Point(337, 260), MyCanvas));
                nodes.Add(new TrailNode('A', new Point(523, 76), MyCanvas));
                nodes.Add(new TrailNode(2, new Point(673, 288), MyCanvas));
                nodes.Add(new TrailNode('B', new Point(512, 218), MyCanvas));
                nodes.Add(new TrailNode(3, new Point(560, 419), MyCanvas));
                nodes.Add(new TrailNode('C', new Point(137, 397), MyCanvas));
                nodes.Add(new TrailNode(4, new Point(83, 81), MyCanvas));
                nodes.Add(new TrailNode('D', new Point(311, 109), MyCanvas));
            }

            //previousPoint = nodes[0].getLocation();
            //currentPoint = nodes[0].getLocation();
            previousPoint = new Point(nodes[0].getLocation().X+15, nodes[0].getLocation().Y+15);
            currentPoint = new Point(nodes[0].getLocation().X + 15, nodes[0].getLocation().Y + 15);
            currentNode = 0;
        }

        // Timer tick. When the timer starts ticking, begin 
        // 'animating' the ink strokes
        private void timer_tick(object sender, object e)
        {
           if(Math.Abs(previousPoint.X - (nodes[currentNode].getLocation().X+15)) < 1 && Math.Abs(previousPoint.Y - (nodes[currentNode].getLocation().Y+15)) < 1)
           {
               nodes[currentNode].getEllipse().Fill = new SolidColorBrush(Colors.Green);

               // Reached the end of the animation
               if(currentNode == nodes.Count-1)
               {
                   disp.Tick -= timer_tick;
                   return;
               }
               currentNode++;
               currentPoint = new Point(nodes[currentNode].getLocation().X+15, nodes[currentNode].getLocation().Y+15);
           }
           else
           {
               // Move toward the next node slowly
               // Find the slope and then create a line from 
               // previous point to the next point.

               // Imagine we are in the 2nd quadrant in math..

               double prevY = -1 * previousPoint.Y;
               double currY = -1 * currentPoint.Y;

               double deltaY = (currY - prevY);
               double deltaX = (currentPoint.X - previousPoint.X);
               double slope = deltaY / deltaX;

               if(Math.Abs(deltaX) > amountToMove)
               {
                   if (deltaX < 0)
                       deltaX = -1 * amountToMove;
                   else 
                        deltaX = amountToMove;
               }
               if (Math.Abs(deltaY) > amountToMove)
               {
                   if (deltaY < 0)
                       deltaY = -1 * amountToMove;
                   else
                       deltaY = amountToMove;
               }

               // (0,b)
               // y-y1 = m(x-x1)
               // y-y1 = mx - mx1
               // y-y1+mx1 = mx

               // (x1,y1) = (previousPoint.X,previousPoint.Y)
               // (x, y) = (currentPoint.X, currentPoint.Y)

               // y = mx+b
               // y-b = mx
               // y-b/m = x

               //     X1           Y1               X           Y
               // (previous.X, previou.Y), (previous.X+deltaX, Y), slope
               double newX = previousPoint.X + deltaX;
               double newY = (slope * (newX - previousPoint.X)) + prevY;
               newY *= -1;

               Line line = new Line()
               {
                   X1 = previousPoint.X,
                   X2 = newX,
                   Y1 = previousPoint.Y,
                   Y2 = newY,
                   StrokeThickness = DRAW_WIDTH,
                   Stroke = new SolidColorBrush(DRAW_COLOR)
               };
               MyCanvas.Children.Add(line);
               previousPoint = new Point(newX, newY);
           }
        }

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
            string vers = e.Parameter as string;    // This is the type of the trails test.
            version = vers;
            pageTitle.Text = "Instructions: Trails Test " + vers;
            populateNodes();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TrailsTest), version);
        }
    }
}
