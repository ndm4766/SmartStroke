using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;
using System.Collections;

namespace SmartStroke
{
    public abstract class TestAction
    {
        protected DateTime startTime;
        protected DateTime endTime;
        public TestAction() { }
        public TestAction(DateTime StartTime, DateTime EndTime)
        {
            startTime = StartTime;
            endTime = EndTime;
        }
        public abstract void executeAction();
    }
    public sealed class Stroke : TestAction
    {
        InkStrokeRenderingSegment strokeData;
        public Stroke(DateTime StartTime, DateTime EndTime,
            InkStrokeRenderingSegment StrokeData)
            : base(StartTime, EndTime)
        {
            strokeData = StrokeData;
        }
        public override void executeAction() { }
    }
    public sealed class ClearScreen : TestAction
    {
        public ClearScreen(){}
        public override void executeAction() { }
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
        public void addAction(TestAction action) { testActions.Add(action); }
    }
}
