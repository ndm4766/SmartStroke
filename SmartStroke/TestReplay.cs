using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using Windows.Data;
using Windows.Storage;
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
        public abstract string getActionTypeString();
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
        public override string getActionTypeString()
        {
            return "Stroke";
        }
    }
    public sealed class DeletePreviousStroke : TestAction
    {
        public DeletePreviousStroke() { finished = true; }
        public override ACTION_TYPE getActionType()
        {
            return ACTION_TYPE.DEL_PREV_STROKE;
        }
        public override string getActionTypeString()
        {
            return "DeletePreviousStroke";
        }
    }

    public enum TEST_TYPE { TRAILS_A, TRAILS_B, REY_OSTERRIETH, CLOCK }
    public sealed class TestReplay
    {
        private TEST_TYPE testType;
        private Patient patient;
        private DateTime startTime;
        private DateTime endTime;
        private List<TestAction> testActions;
        private Stroke currentStroke;
        public TestReplay(Patient _patient, TEST_TYPE TestType)
        {
            patient = _patient;
            testActions = new List<TestAction>();
            testType = TestType;
        }
        public void startTest() {
            startTime = DateTime.Now; 
        }
        public void endTest() { 
            endTime = DateTime.Now;
            saveTestReplay();
        }
        public void beginStroke()
        {
            if (getCurrentTestAction() != null)
            {
                if (!getCurrentTestAction().isFinished()) return;
            }
            currentStroke = new Stroke();
            testActions.Add(currentStroke);
        }
        public void endStroke()
        {
            if (getCurrentTestAction() != null)
            {
                if (getCurrentTestAction().isFinished()) return;
                if (!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
            }
            currentStroke.endAction();
        }
        public void addLine(Line line)
        {
            if (getCurrentTestAction() != null)
            {
                if(getCurrentTestAction().isFinished()) return;
                if(!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
                currentStroke.addLine(line);
            }
        }
        public void deletePreviousStroke()
        {
            if(getCurrentTestAction() == null) return;
            if (!getCurrentTestAction().isFinished()) return;
            if (!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
            testActions.Add(new DeletePreviousStroke());
        }
        public bool checkCurrentTestAction(ACTION_TYPE act)
        {
            if (getCurrentTestAction() == null) return false;
            return getCurrentTestAction().getActionType() == act;
        }
        public TestAction getCurrentTestAction()
        {
            if (testActions.Count == 0) return null;
            else return testActions[testActions.Count - 1];
        }
        private string getTestType()
        {
            switch (testType){
                case TEST_TYPE.TRAILS_A: { return "trailsA"; }
                case TEST_TYPE.TRAILS_B: { return "trailsB"; }
                case TEST_TYPE.REY_OSTERRIETH: { return "reyOsterrieth"; }
                case TEST_TYPE.CLOCK: { return "clock"; }
                default: { return "NOT_SUPPORTED"; }
            }
        }
        private string getFileSuffix()
        {
            string fileExtension = ".xml";
            string testType = getTestType();
            if (testType == "NOT_SUPPORTED") return testType;
            else return testType + fileExtension;
        }
        private string getFileName()
        {
            return "test.txt";
            string fileSuffix = getFileSuffix();
            if (fileSuffix=="NOT_SUPPORTED") return "TEST_TYPE_NOT_SUPPORTED";
            string filename = patient.getName() 
                + patient.getBirthDate().ToString() + fileSuffix;
            filename = filename.Replace(":","");
            filename = filename.Replace("/", "");
            return filename.Replace(" ","");
        }
        public async void loadTestReplay() {
            string testFilename = getFileName();
            if (testFilename == "TEST_TYPE_NOT_SUPPORTED") return;
            Windows.Storage.StorageFile testStorageFile;
            string testData;
            try
            {
                testStorageFile = await Windows.Storage.ApplicationData
                        .Current.LocalFolder.GetFileAsync(testFilename);
                testData = await 
                    Windows.Storage.FileIO.ReadTextAsync(testStorageFile);
            } 
            catch { return; }
        }
        public async void saveTestReplay() {
            string testFilename = getFileName();
            if (testFilename == "TEST_TYPE_NOT_SUPPORTED") return;
            string testString = "";
            Windows.Storage.StorageFile testStorageFile;
            try
            {
                testStorageFile = await Windows.Storage.ApplicationData
                    .Current.LocalFolder.CreateFileAsync(testFilename, 
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(testStorageFile
                    , testString);
                int i = 0; i++;
            }
            catch { return; }
        }
        public string toXmlString()
        {
            using (var sw = new StringWriter()) { 
            using (var xw = XmlWriter.Create(sw)){
                xw.WriteStartDocument();
                for(int i = 0; i < testActions.Count; i++) {
                    xw.WriteStartElement(testActions[i].getActionTypeString());
                    switch (testActions[i].getActionType()) {
                        case ACTION_TYPE.STROKE: { break; }
                        case ACTION_TYPE.DEL_PREV_STROKE: { break; }
                        default: { break; }
                    }
                }
            }
            return sw.ToString();
            }
        }
    }
}