using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Shapes;

namespace SmartStroke
{
    public enum ACTION_TYPE { STROKE, DEL_PREV_STROKE }
    public abstract class TestAction
    {
        protected DateTime startTime;
        protected DateTime endTime;
        protected bool finished;
        public TestAction() 
        {
            startTime = DateTime.Now;
            finished = false; 
        }
        public TestAction(DateTime StartTime)
        {
            startTime = StartTime;
            finished = false;
        }
        public TestAction(DateTime StartTime, DateTime EndTime)
        {
            startTime = StartTime;
            endTime = EndTime;
            finished = true;
        }
        public void startAction(DateTime StartTime) 
        {
            startTime = StartTime;
            finished = false;
        }
        public void startAction()
        {
            startTime = DateTime.Now;
            finished = false;
        }
        public void endAction(DateTime EndTime)
        {
            endTime = EndTime;
            finished = true;
        }
        public void endAction()
        {
            endTime = DateTime.Now;
            finished = true;
        }
        public bool isFinished() { return finished; }
        public abstract ACTION_TYPE getActionType();
    }
    public sealed class Stroke : TestAction
    {
        private class LineData
        {
            private DateTime startTime;
            private Line line;
            public LineData(DateTime StartTime, Line _Line) 
            {
                startTime = StartTime;
                line = _Line;
            }
        }
        private List<LineData> lines;
        public Stroke() { lines = new List<LineData>(); }
        public void addLine(Line _Line)
        {
            lines.Add(new LineData(DateTime.Now, _Line));
        }
        public override ACTION_TYPE getActionType() 
        { 
            return ACTION_TYPE.STROKE; 
        }
    }
    public sealed class DeletePreviousStroke : TestAction
    {
        public override ACTION_TYPE getActionType()
        {
            return ACTION_TYPE.DEL_PREV_STROKE;
        }
    }
    public sealed class TestReplay
    {
        private DateTime startTime;
        private DateTime endTime;
        private List<TestAction> testActions;
        private Stroke currentStroke;
        public TestReplay()
        {
            testActions = new List<TestAction>();
        }
        public void startTest() { startTime = DateTime.Now; }
        public void endTest() { endTime = DateTime.Now; }
        public void beginStroke()
        {
            if (getCurrentTestAction() != null)
            {
                Debug.Assert(getCurrentTestAction().isFinished()
                    , "Current test action is unfinished.");
            }
            currentStroke = new Stroke();
            testActions.Add(currentStroke);
        }
        public void endStroke()
        {
            if (getCurrentTestAction() != null)
            {
                Debug.Assert(!getCurrentTestAction().isFinished()
                    , "Current test action is already finished.");
                Debug.Assert(checkCurrentTestAction(ACTION_TYPE.STROKE)
                    , "Current test action is already ended."); 
            }
            currentStroke.endAction();
        }
        public void addLine(Line line)
        {
            if (getCurrentTestAction() != null)
            {
                Debug.Assert(!getCurrentTestAction().isFinished(), 
                    "Current test action is already finished.");
                Debug.Assert(checkCurrentTestAction(ACTION_TYPE.STROKE), 
                    "Current test action is not a stroke.");
                currentStroke.addLine(line);
            }
        }
        public void deletePreviousStroke()
        {
            Debug.Assert(getCurrentTestAction() != null, 
                "There is no previous stroke to delete.");
            Debug.Assert(getCurrentTestAction().isFinished(), 
                "Current test action is not finished.");
            Debug.Assert(checkCurrentTestAction(ACTION_TYPE.STROKE), 
                "Current test action is not a stroke.");
            testActions.Add(new DeletePreviousStroke());
        }
        public bool checkCurrentTestAction(ACTION_TYPE act)
        {
            if (getCurrentTestAction() == null) return false;
            else return getCurrentTestAction().getActionType() == act;
        }
        public TestAction getCurrentTestAction()
        {
            if (testActions.Count == 0) return null;
            else return testActions[testActions.Count - 1];
        }
    }
}
