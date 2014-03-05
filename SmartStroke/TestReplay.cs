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
  /===========================================================================/
 /                        TestReplay.cs Documentation                        /
/===========================================================================/

Intro to Classes:
    TestReplay - Functions as container for a completed test, contains a list
                 of ordered TestActions.
    TestAction - Abstract class which represents a patient action during a 
                 test, such as a stroke drawn or a stroke erased.
    Stroke     - Inherits from TestAction, represents a stroke drawn.
                 Contains a list of LineData objects.
    DeletePreviousStroke - Inherits from TestAction, represents the deletion of
                           the previous stroke.
    LineData   - Contains a Line and a DateTime at which the line was drawn.

Intro to Enums:
    ACTION_TYPE - Represents the kind of TestAction (i.e. Stroke).  Contained by
                  a TestAction.
    TEST_TYPE   - Represents the kind of test (i.e. Trails A).  Contained by a 
                  TestReplay.

How to use TestReplay.  First, you must always:
    1)  Initialize a TestReplay object.
        Example: 
            Patient p = new Patient("Leeroy Jenkins", 
                                DateTime.Now, GENDER.MALE, EDU_LEVEL.PHD);
            TestReplay testReplay = new TestReplay(p, TEST_TYPE.TRAILS_A);

Then, would you like to -
Capture patient actions:
    2)  Call startTest()
    3)  Call startStroke(), addLine(line), and endStroke() when applicable.  
    4)  Call deletePreviousStroke() when applicable, 
        but be sure to call endStroke() first.
Save a current TestReplay:
    2)  Call saveTestReplay()
Load a previous TestReplay:  WIP
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
        public List<LineData> lines;
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
        private const string fileExtension = ".txt";
        private TEST_TYPE testType;
        private Patient patient;
        private DateTime startTime;
        private DateTime endTime;
        private List<TestAction> testActions;
        private List<PatientNote> testNotes;
        private Stroke currentStroke;
        public TestReplay(Patient _patient, TEST_TYPE TestType)
        {
            patient = _patient;
            testActions = new List<TestAction>();
            testNotes = new List<PatientNote>();
            testType = TestType;
        }
        public TEST_TYPE getTestType() { return testType; }
        public Patient getPatient() { return patient; }
        public DateTime getStartTime() { return startTime; }
        public DateTime getEndTime() { return endTime; }
        public List<TestAction> getTestActions() { return testActions; }
        public List<PatientNote> getPatientNotes() { return testNotes; }
        public void startTest() { startTime = DateTime.Now; }
        public void endTest()
        {
            endTime = DateTime.Now;
            saveTestReplay();
        }
        public void beginStroke()
        {
            if (getCurrentTestAction() != null)
                if (!getCurrentTestAction().isFinished()) return;
            currentStroke = new Stroke();
            testActions.Add(currentStroke);
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
        public void endStroke()
        {
            if (getCurrentTestAction() != null)
            {
                if (getCurrentTestAction().isFinished()) return;
                if (!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
            }
            currentStroke.endAction();
        }
        public void deletePreviousStroke()
        {
            if (getCurrentTestAction() == null) return;
            if (!getCurrentTestAction().isFinished()) return;
            if (!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
            testActions.Add(new DeletePreviousStroke());
        }
        public void addTestNote(PatientNote patientNote) 
        { 
            testNotes.Add(patientNote);
        }
        private bool checkCurrentTestAction(ACTION_TYPE act)
        {
            if (getCurrentTestAction() == null) return false;
            return getCurrentTestAction().getActionType() == act;
        }
        private TestAction getCurrentTestAction()
        {
            if (testActions.Count == 0) return null;
            else return testActions[testActions.Count - 1];
        }
        private string getFileName()
        {
            string fileSuffix = testType.ToString() + fileExtension;
            string filename = patient.getName()
                + patient.getBirthDate().ToString() + fileSuffix;
            filename = filename.Replace(":", "");
            filename = filename.Replace("/", "");
            return filename.Replace(" ", "");
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
                    CreationCollisionOption.ReplaceExisting)
                    .AsTask<StorageFile>();
                fileTask.Wait();
                testStorageFile = fileTask.Result;
                await FileIO.WriteTextAsync(testStorageFile, stringToSave);
            }
            catch { return; }
            patient.addFile(testFilename);
        }
        public async void loadTestReplay(string testFilename)
        {
            if (testFilename == "TEST_TYPE_NOT_SUPPORTED") return;
            StorageFile testStorageFile;
            string testReplayString = "";
            try
            {
                Task<StorageFile> fileTask = ApplicationData
                        .Current.LocalFolder
                        .GetFileAsync(testFilename).AsTask<StorageFile>();
                fileTask.Wait();
                testStorageFile = fileTask.Result;
                testReplayString = await FileIO.ReadTextAsync(testStorageFile);
            }
            catch { return; }
            parseTestReplayFile(testReplayString);
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
                                    ("line " + formatLineDataAsString(lineData[j]) + "\n");
                            break;
                        }
                    case ACTION_TYPE.DEL_PREV_STROKE: { break; }
                    default: { break; }
                }
            }
            testReplayString += "=====NOTES=====\n";
            for (int i = 0; i < testNotes.Count; i++ )
            {
                testReplayString += (testNotes[i].convertToString());
            }
            return testReplayString;
        }
        public string formatLineDataAsString(LineData lineData)
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
        public void parseTestReplayFile(string testReplayString)
        {
            bool inActionSection = true;
            List<string> testStrings = 
                testReplayString.Split('\n').Cast<string>().ToList<string>();
            for(int i = 1; i < testStrings.Count; i++)
            {
                if (inActionSection) {
                    List<string> lineWords = testStrings[i].Split(' ')
                        .Cast<string>().ToList<string>();
                    if (lineWords[0] == "line")
                        ((Stroke)testActions[testActions.Count - 1])
                            .addLineData(parseLineLineData(lineWords));
                    else if (lineWords[0] == "Stroke")
                        testActions.Add(parseLineStroke(lineWords));
                    else if (lineWords[0] == "DeletePreviousStroke")
                        testActions.Add(parseLineDelPrevStroke(lineWords));
                    else if (lineWords[0] == "=====NOTES=====")
                        inActionSection = false;
                } else {
                    List<string> lineWords = testStrings[i]
                        .Split('\t').Cast<string>().ToList<string>();
                    if (lineWords.Count > 0)
                    {
                        DateTime date = new DateTime();
                        date = Convert.ToDateTime(lineWords[0]);
                        testNotes.Add(
                            new PatientNote(lineWords[1], lineWords[2], date));
                    }
                }
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
        public LineData parseLineLineData(List<string> line)
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