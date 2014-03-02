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

/*
 *   Current issue, having trouble with asynchronous saving events not always working.
 *   In C#, asnychronous function errors can sometimes not be caught by catch blocks.
 *   It is 4AM right now, and I will fix this, add file searching, and add documentation
 *   after some sleep - JR
 */

namespace SmartStroke
{
    public enum ACTION_TYPE { STROKE, DEL_PREV_STROKE, NONE }
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
        public DateTime getStartTime() { return startTime; }
        public DateTime getEndTime() { return endTime; }
        public abstract ACTION_TYPE getActionType();
        public abstract string getActionTypeString();
    }
    public sealed class LineData
    {
        private DateTime startTime;
        private Line line;
        public LineData(DateTime StartTime, Line _Line)
        {
            startTime = StartTime;
            line = _Line;
        }
        public Line getLine() { return line; }
        public DateTime getDateTime() { return startTime; }
    }
    public sealed class Stroke : TestAction
    {
        private List<LineData> lines;
        public Stroke() { lines = new List<LineData>(); }
        public Stroke(DateTime startTime, DateTime endTime)
            : base(startTime, endTime) { lines = new List<LineData>(); }
        public void addLine(Line _Line)
        {
            lines.Add(new LineData(DateTime.Now, _Line));
        }
        public void addLineData(LineData lineData)
        {
            lines.Add(lineData);
        }
        public List<LineData> getLines() { return lines; }
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
        public DeletePreviousStroke(DateTime startTime, DateTime endTime)
            : base(startTime, endTime) { }
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
        public void startTest()
        {
            startTime = DateTime.Now;
        }
        public void endTest()
        {
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
                if (getCurrentTestAction().isFinished()) return;
                if (!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
                currentStroke.addLine(line);
            }
        }
        public void deletePreviousStroke()
        {
            if (getCurrentTestAction() == null) return;
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
            switch (testType)
            {
                case TEST_TYPE.TRAILS_A: { return "trailsA"; }
                case TEST_TYPE.TRAILS_B: { return "trailsB"; }
                case TEST_TYPE.REY_OSTERRIETH: { return "reyOsterrieth"; }
                case TEST_TYPE.CLOCK: { return "clock"; }
                default: { return "NOT_SUPPORTED"; }
            }
        }
        private string getFileSuffix()
        {
            string fileExtension = ".txt";
            string testType = getTestType();
            if (testType == "NOT_SUPPORTED") return testType;
            else return testType + fileExtension;
        }
        private void setPatient(Patient _patient) { patient = _patient; }
        private string getFileName()
        {
            string fileSuffix = getFileSuffix();
            if (fileSuffix == "NOT_SUPPORTED") return "TEST_TYPE_NOT_SUPPORTED";
            string filename = patient.getName()
                + patient.getBirthDate().ToString() + fileSuffix;
            filename = filename.Replace(":", "");
            filename = filename.Replace("/", "");
            return filename.Replace(" ", "");
        }
        public async void loadTestReplay(string testFilename)
        {
            if (testFilename == "TEST_TYPE_NOT_SUPPORTED") return;
            StorageFile testStorageFile;
            string testReplayString = "";
            try
            {
                Task<StorageFile> fileTask = ApplicationData
                        .Current.LocalFolder.GetFileAsync(testFilename).AsTask<StorageFile>();
                fileTask.Wait();
                testStorageFile = fileTask.Result;
                testReplayString = await FileIO.ReadTextAsync(testStorageFile);
            }
            catch { return; }
            parseTestReplay(testReplayString);
        }
        public async void saveTestReplay()
        {
            string testFilename = getFileName();
            if (testFilename == "TEST_TYPE_NOT_SUPPORTED") return;
            StorageFile testStorageFile;
            string stringToSave = convertToString();
            try
            {
                Task<StorageFile> fileTask = ApplicationData
                    .Current.LocalFolder.CreateFileAsync(testFilename,
                    CreationCollisionOption.ReplaceExisting).AsTask<StorageFile>();
                fileTask.Wait();
                testStorageFile = fileTask.Result;
                await FileIO.WriteTextAsync(testStorageFile, stringToSave);
            }
            catch { return; }
        }
        public string formatLineString(LineData lineData)
        {
            string lineString = "";
            Line line = lineData.getLine();
            lineString += line.X1.ToString();
            lineString += ("," + line.Y1.ToString());
            lineString += ("," + line.X2.ToString());
            lineString += ("," + line.Y2.ToString());
            lineString += (" " + lineData.getDateTime().ToString());
            return lineString;
        }
        public string convertToString()
        {
            string testReplayString = "";
            testReplayString += (patient.convertToString() + "\n");
            for (int i = 0; i < testActions.Count; i++)
            {
                testReplayString += testActions[i].getActionTypeString() + " ";
                testReplayString
                    += testActions[i].getStartTime().ToString() + " ";
                testReplayString
                    += testActions[i].getEndTime().ToString() + "\n";
                switch (testActions[i].getActionType())
                {
                    case ACTION_TYPE.STROKE:
                    {
                        Stroke stroke = (Stroke)testActions[i];
                        List<LineData> lineData = stroke.getLines();
                        for (int j = 0; j < lineData.Count; j++)
                            testReplayString +=
                                ("line " + formatLineString(lineData[j]) + "\n");
                        break;
                    }
                    case ACTION_TYPE.DEL_PREV_STROKE: { break; }
                    default: { break; }
                }
            }
            return testReplayString;
        }
        public void parseTestReplay(string testReplayString)
        {
            List<string> testStrings = 
                testReplayString.Split('\n').Cast<string>().ToList<string>();
            for(int i = 1; i < testStrings.Count; i++)
            {
                List<string> lineWords =
                   testStrings[i].Split(' ').Cast<string>().ToList<string>();
                if(lineWords[0] == "line") {
                    ((Stroke)testActions[testActions.Count - 1])
                        .addLineData(parseLineLine(lineWords));
                } else if (lineWords[0] == "Stroke") {
                    testActions.Add(parseLineStroke(lineWords));
                } else if (lineWords[0] == "DeletePreviousStroke") {
                    testActions.Add(parseLineDelPrevStroke(lineWords));
                }
                int jazzman = 0; jazzman++;
            }
        }
        public Stroke parseLineStroke(List<string> line)
        {
            DateTime startTime = DateTime.Parse(
                line[1] + " " + line[2] + " " + line [3]);
            DateTime endTime = DateTime.Parse(
                line[4] + " " + line[5] + " " + line[6]);
            return new Stroke(startTime, endTime);
        }
        public DeletePreviousStroke parseLineDelPrevStroke(List<string> line)
        {
            DateTime startTime = DateTime.Parse(
                line[1] + " " + line[2] + " " + line[3]);
            DateTime endTime = DateTime.Parse(
                line[4] + " " + line[5] + " " + line[6]);
            return new DeletePreviousStroke(startTime, endTime);
        }
        public LineData parseLineLine(List<string> line)
        {
            Line newLine = new Line();
            List<string> coords = 
                line[1].Split(',').Cast<string>().ToList<string>();
            newLine.X1 = Convert.ToDouble(coords[0]);
            newLine.Y1 = Convert.ToDouble(coords[1]);
            newLine.X2 = Convert.ToDouble(coords[2]);
            newLine.Y2 = Convert.ToDouble(coords[3]);
            DateTime startTime = DateTime.Parse(
                line[2] + " " + line[3] + " " + line[4]);
            return new LineData(startTime, newLine);
        }
    }
}