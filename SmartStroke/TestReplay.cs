using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Shapes;

namespace SmartStroke
{
    public enum ACTION_TYPE { STROKE }
    public abstract class TestAction
    {
        protected DateTime startTime;
        protected DateTime endTime;
        public TestAction() { }
        public TestAction(DateTime StartTime)
        {
            startTime = StartTime;
        }
        public TestAction(DateTime StartTime, DateTime EndTime)
        {
            startTime = StartTime;
            endTime = EndTime;
        }
        public void startAction(DateTime StartTime) 
        {
            startTime = StartTime;
        }
        public void endAction(DateTime EndTime)
        {
            endTime = EndTime;
        }
        public abstract ACTION_TYPE getActionType();
    }
    public sealed class Stroke : TestAction
    {
        Line line;
        public Stroke(DateTime StartTime) : base(StartTime) { }
        public override ACTION_TYPE getActionType() 
        { 
            return ACTION_TYPE.STROKE; 
        }
    }
    public sealed class TestReplay
    {
        private DateTime startTime;
        private DateTime endTime;
        private List<TestAction> testActions;
        public TestReplay()
        {
            testActions = new List<TestAction>();
        }
        public void startTest() { startTime = DateTime.Now; }
        public void endTest() { endTime = DateTime.Now; }
        public void startStroke(DateTime StartTime)
        {
            Stroke stroke = new Stroke(StartTime);
            testActions.Add(stroke);
        }
        public void endStroke(DateTime EndTime)
        {
            for (int i = testActions.Count - 1; i >= 0; i++)
            {
                if (testActions[i].getActionType() == ACTION_TYPE.STROKE)
                {
                    testActions[i].endAction(EndTime);
                }
            }
        }
    }
}
