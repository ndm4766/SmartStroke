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
using Windows.UI.Xaml.Media.Imaging;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SmartStroke
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TrailsTestInstruction : Page
    {
        private InfoPasser passer;
        private List<TrailNode> nodes;
        private DispatcherTimer disp;
        private DispatcherTimer instructionTimer;
        private int instructionNumber = 0;

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

        private Image pen;
        private const double penWidth = 187;
        private const double penHeight = 136; 

        private double amountToMove;

        private List<string> instructions;

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


        private void makeInstructions()
        {
            instructions.Clear();
            instructions.Add("Place the pen on the starting circle (1)");
            if (passer.trailsTestVersion == 'A')
                instructions.Add("Determine the next node in increasing order (2)");
            else if (passer.trailsTestVersion == 'B')
                instructions.Add("Determine the next node in increasing order (A)");
            instructions.Add("Draw a line connecting the nodes");
            instructions.Add("Stop when you have connected all the nodes");
            instructions.Add("If you make a mistake, the node will light up red");
            instructions.Add("If you make a mistake, start back at the previous node and retry");
        }

        public TrailsTestInstruction()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Landscape; //Landscape mode
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

            disp = new DispatcherTimer();
            disp.Interval = new TimeSpan(0, 0, 0, 0, 1);
            disp.Tick += timer_tick;

            instructionTimer = new DispatcherTimer();
            instructionTimer.Interval = new TimeSpan(0, 0, 3);
            instructionTimer.Tick += instruction_tick;
            instructionTimer.Start();

            pen = new Image();

            instructions = new List<string>();
            amountToMove = 3;
        }

        private void populateNodes()
        {
            nodes.Clear();
            if (passer.trailsTestVersion == 'A')
            {
                nodes.Add(new TrailNode(1, new Point(377, 244), MyCanvas, false));
                nodes.Add(new TrailNode(2, new Point(554, 68), MyCanvas, false));
                nodes.Add(new TrailNode(3, new Point(763, 463), MyCanvas, false));
                nodes.Add(new TrailNode(4, new Point(541, 225), MyCanvas, false));
                nodes.Add(new TrailNode(5, new Point(585, 400), MyCanvas, false));
                nodes.Add(new TrailNode(6, new Point(126, 408), MyCanvas, false));
                nodes.Add(new TrailNode(7, new Point(109, 155), MyCanvas, false));
                nodes.Add(new TrailNode(8, new Point(336, 137), MyCanvas, false));
            }
            else if(passer.trailsTestVersion == 'B')
            {
                nodes.Add(new TrailNode(1, new Point(337, 260), MyCanvas, false));
                nodes.Add(new TrailNode('A', new Point(523, 76), MyCanvas,false));
                nodes.Add(new TrailNode(2, new Point(673, 288), MyCanvas, false));
                nodes.Add(new TrailNode('B', new Point(512, 218), MyCanvas, false));
                nodes.Add(new TrailNode(3, new Point(560, 419), MyCanvas, false));
                nodes.Add(new TrailNode('C', new Point(137, 397), MyCanvas, false));
                nodes.Add(new TrailNode(4, new Point(83, 81), MyCanvas, false));
                nodes.Add(new TrailNode('D', new Point(311, 109), MyCanvas, false));
            }

            //previousPoint = nodes[0].getLocation();
            //currentPoint = nodes[0].getLocation();
            previousPoint = new Point(nodes[0].getLocation().X+15, nodes[0].getLocation().Y+15);
            currentPoint = new Point(nodes[0].getLocation().X + 15, nodes[0].getLocation().Y + 15);
            currentNode = 0;

            pen.Source = new BitmapImage(new Uri(this.BaseUri, "Properties/images/pen.png"));
            pen.Margin = new Thickness(nodes[0].getLocation().X-penWidth, nodes[0].getLocation().Y-penHeight, 0, 0);
            MyCanvas.Children.Add(pen);
        }

        private void instruction_tick(object sender, object e)
        {
            if(instructionNumber == 1)
            {
                disp.Start();
            }
            else if(instructionNumber == instructions.Count-1)
            {
                instructionTimer.Tick -= instruction_tick;
                instructionTimer.Stop();
            }
            inst.Text += "\n";
            inst.Text += instructions[instructionNumber];
            inst.FontSize = 24;
            instructionNumber += 1;
        }

        // restart tick. When the timer starts ticking, restart the animation
        private void restart_tick(object sender, object e)
        {
            disp.Tick -= restart_tick;
            disp.Stop();
            this.Frame.Navigate(typeof(TrailsTestInstruction), passer);
        }


        // Timer tick. When the timer starts ticking, begin 
        // 'animating' the ink strokes
        private void timer_tick(object sender, object e)
        {
           if(Math.Abs(previousPoint.X - (nodes[currentNode].getLocation().X+15)) < 1 && Math.Abs(previousPoint.Y - (nodes[currentNode].getLocation().Y+15)) < 1)
           {
               nodes[currentNode].getEllipse().Fill = new SolidColorBrush(Colors.Green);

               // Reached the end of the animation
               // Restart
               if(currentNode == nodes.Count-1)
               {
                   disp.Tick -= timer_tick;
                   disp.Stop();
                   disp.Interval = new TimeSpan(0, 0, 5);
                   disp.Tick += restart_tick;
                   disp.Start();
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
               

               // Radians
               double theta = (Math.Atan(deltaY / deltaX));

               // Total Distance from beginning to end
               double dist = Math.Sqrt(Math.Pow((currentPoint.X - previousPoint.X), 2) + Math.Pow((currentPoint.Y - previousPoint.Y), 2));
               
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
               
           
               //     X1           Y1               X           Y
               // (previous.X, previou.Y), (previous.X+deltaX, Y), slope
               double newX = previousPoint.X + deltaX * Math.Cos(theta);

               double newY;
               if(theta < 0)
                   newY = previousPoint.Y + (deltaY * Math.Sin(theta));
               else
                   newY = previousPoint.Y - (deltaY * Math.Sin(theta));

               Line line = new Line()
               {
                   X1 = previousPoint.X,
                   X2 = newX,
                   Y1 = previousPoint.Y,
                   Y2 = newY,
                   StrokeThickness = DRAW_WIDTH,
                   Stroke = new SolidColorBrush(DRAW_COLOR)
               };
               pen.Margin = new Thickness(newX - penWidth - 15, newY - penHeight-15, 0, 0);
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
            passer = e.Parameter as InfoPasser;    // This is the type of the trails test.
            pageTitle.Text = "Instructions: Trails Test " + passer.trailsTestVersion;
            populateNodes();
            makeInstructions();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
            disp.Stop();
            instructionTimer.Stop();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            disp.Stop();
            instructionTimer.Stop();
            this.Frame.Navigate(typeof(TrailsTest), passer);
        }
    }
}
