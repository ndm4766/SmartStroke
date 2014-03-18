﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartStroke
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClockTestReplay : Page
    {
        InfoPasser passer = new InfoPasser();

        //test replay container
        private TestReplay testReplay;
        DispatcherTimer timer;
        Stopwatch stopwatch;
        int actionIndex;
        int linesIndex;
        List<Line> previousStroke;
        List<Line> linesRedrawn; //necessary bec the lines are already children of the other page

        bool granular;
        string currentlySelectedDate;

        private const double DRAW_WIDTH = 4.0;
        private const double ERASE_WIDTH = 30.0;
        private Color DRAW_COLOR = Colors.Blue;

        public ClockTestReplay()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += timer_tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            stopwatch = new Stopwatch();
            actionIndex = 0;
            linesIndex = 0;
            previousStroke = new List<Line>();
            linesRedrawn = new List<Line>();
            granular = false;
            
        }

        async private void loadTest()
        {
            List<String> filenames = passer.currentPatient.getTestFilenames();
            foreach (String filename in filenames)
            {
                if(filename.Contains(currentlySelectedDate))
                {
                    await testReplay.loadTestReplay(filename);
                    break;
                }
            }
        }

        private bool actionTimeHasPassed(TestAction action)
        {
            TimeSpan span = action.getStartTime().Subtract(testReplay.getStartTime());
            double seconds = span.TotalSeconds;
            bool b = seconds < stopwatch.Elapsed.TotalSeconds;
            return b;
        }

        private bool lineDataTimeHasPassed(LineData lineData)
        {
            TimeSpan span = lineData.getDateTime().Subtract(testReplay.getStartTime());
            double seconds = span.TotalSeconds;
            bool b = seconds < stopwatch.Elapsed.TotalSeconds;
            return b;
        }

        private void timer_tick(object sender, object e)
        {
            //get all actions
            var allActions = testReplay.getTestActions(); //TODO: bug - this list does not include deletions

            if (!granular)
            {
                //as the time for the next action arrives, do the action
                if (actionTimeHasPassed(allActions[actionIndex]))
                {
                    //that is, if the action is a stroke, draw a line on the canvas for each line in that stroke (use getRenderingSegments to get lines)
                    if (allActions[actionIndex].getActionType() == ACTION_TYPE.STROKE)
                    {
                        Stroke stroke = allActions[actionIndex] as Stroke;
                        List<LineData> lines = stroke.getLines();

                        foreach (LineData lineData in lines)
                        {
                            Line line = new Line();
                            line.X1 = lineData.getLine().X1;
                            line.Y1 = lineData.getLine().Y1;
                            line.X2 = lineData.getLine().X2;
                            line.Y2 = lineData.getLine().Y2;
                            line.StrokeThickness = DRAW_WIDTH;
                            line.Stroke = new SolidColorBrush(DRAW_COLOR);

                            MyCanvas.Children.Add(line);
                            previousStroke.Add(line);
                            linesRedrawn.Add(line);
                        }

                        actionIndex++;
                    }
                    //if the action is an erasure, remove all the lines that were part of the stroke that was erased
                    else if (allActions[actionIndex].getActionType() == ACTION_TYPE.DEL_PREV_STROKE)
                    {
                        DeletePreviousStroke stroke = allActions[actionIndex] as DeletePreviousStroke;

                        foreach (Line line in previousStroke)
                        {
                            MyCanvas.Children.Remove(line);
                        }
                    }
                }
            }
            else if (granular)
            {
                if (actionTimeHasPassed(allActions[actionIndex]))
                {
                    //that is, if the action is a stroke, draw a line on the canvas for each line in that stroke (use getRenderingSegments to get lines)
                    if (allActions[actionIndex].getActionType() == ACTION_TYPE.STROKE)
                    {
                        Stroke stroke = allActions[actionIndex] as Stroke;
                        List<LineData> lines = stroke.getLines();

                        if (lineDataTimeHasPassed(lines[linesIndex]))
                        {
                            Line line = new Line();
                            line.X1 = lines[linesIndex].getLine().X1;
                            line.Y1 = lines[linesIndex].getLine().Y1;
                            line.X2 = lines[linesIndex].getLine().X2;
                            line.Y2 = lines[linesIndex].getLine().Y2;
                            line.StrokeThickness = DRAW_WIDTH;
                            line.Stroke = new SolidColorBrush(DRAW_COLOR);

                            MyCanvas.Children.Add(line);
                            previousStroke.Add(line);
                            linesRedrawn.Add(line);
                            linesIndex++;
                        }

                        if (linesIndex >= lines.Count)
                        {
                            actionIndex++;
                            linesIndex = 0;
                        }
                    }
                    //if the action is an erasure, remove all the lines that were part of the stroke that was erased
                    else if (allActions[actionIndex].getActionType() == ACTION_TYPE.DEL_PREV_STROKE)
                    {
                        DeletePreviousStroke stroke = allActions[actionIndex] as DeletePreviousStroke;

                        foreach (Line line in previousStroke)
                        {
                            MyCanvas.Children.Remove(line);
                        }
                    }
                }
            }

            if (actionIndex >= allActions.Count)
            {
                stopwatch.Stop();
                timer.Stop();

                replayButton.IsEnabled = true;
                granularReplayButton.IsEnabled = true;
            }
        }

        private void renderTestReplay(object sender, object e)
        {
            replayButton.IsEnabled = false;
            granularReplayButton.IsEnabled = false;

            //reset all the things
            foreach (Line line in linesRedrawn)
            {
                MyCanvas.Children.Remove(line);
            }
            previousStroke.Clear();
            linesRedrawn.Clear();
            actionIndex = 0;
            stopwatch.Reset();

            granular = false;

            stopwatch.Start();
            timer.Start();
        }

        private void renderGranularTestReplay(object sender, RoutedEventArgs e)
        {
            loadTest();
            replayButton.IsEnabled = false;
            granularReplayButton.IsEnabled = false;

            //reset all the things
            foreach (Line line in linesRedrawn)
            {
                MyCanvas.Children.Remove(line);
            }
            previousStroke.Clear();
            linesRedrawn.Clear();
            actionIndex = 0;
            stopwatch.Reset();

            granular = true;

            stopwatch.Start();
            timer.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            passer = e.Parameter as InfoPasser;
            testReplay = new TestReplay(passer.currentPatient, TEST_TYPE.CLOCK);

            if (passer.currentPatient.getTestFilenames().Count > 0)
            {
                currentlySelectedDate = passer.currentPatient.getTestFilenames()[0];
            }
            else
            {
                granularReplayButton.IsEnabled = false;
            }

            var stuff = passer.currentPatient.getTestFilenames();
            foreach(string filename in stuff)
            {
                if (filename.Contains(testReplay.getTestType().ToString()))
                {
                    testDatesBox.Items.Add(filename.Substring(filename.Length - (14 + 4), 14));
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            timer.Stop();
            stopwatch.Stop();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedDate = testDatesBox.SelectedItem.ToString();
        }

    }
}
