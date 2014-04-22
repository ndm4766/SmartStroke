﻿using System;
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
using Windows.Foundation;

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
Load a TestReplay:
    3)  Call loadTestReplay(string fileName)
*/

namespace SmartStroke
{
    // Helps determine what kind of action something is
    public enum ACTION_TYPE { STROKE, DEL_STROKE, NONE }
    public abstract class TestAction
    {
        // Time an action started and completed
        protected DateTime startTime;
        protected DateTime endTime;

        // Indicates when an action is completed. 
        // After this, it can no longer be edited.
        protected bool finished;
        
        // Create an unfinished TestAction with now as the start time
        public TestAction()
        {
            startTime = DateTime.Now;
            finished = false;
        }
        // Create an unfinished TestAction with a predefined start time
        public TestAction(DateTime StartTime)
        {
            startTime = StartTime;
            finished = false;
        }
        // Create a finished TestAction with a predefined start and end time
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

    /*  Not a TestAction, a representation of a single line in the context of
     *  a TestReplay.  A Stroke has many of these.
     */
    public sealed class LineData
    {
        // Lines are drawn instantaneously, therefore an end time is not needed
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
    public sealed class DeleteStroke : TestAction
    {
        private int actionIndex;
        public DeleteStroke() { finished = true; }
        public DeleteStroke(DateTime occuranceTime, int index)
            : base(occuranceTime, occuranceTime) 
        { 
            actionIndex = index;
        }
        public int getIndex() { return actionIndex; }
        public override ACTION_TYPE getActionType()
        {
            return ACTION_TYPE.DEL_STROKE;
        }
        public override string getActionTypeString()
        {
            return "DeleteStroke";
        }
    }

    // Class for test errors in trailsA and trailsB test
    // Allow user to see which line was an error - from and to
    public sealed class TestError
    {
        private TrailNode begin;
        private TrailNode expectedEnd;
        private TrailNode actualEnd;
        private DateTime time;

        public TestError() { }

        // Test error on trails was from a beginning node to an expected incremental
        // node. Instead, the user went to the actual node. Could come in handy for
        // analysis purposes.
        public TestError(TrailNode bgn, TrailNode expected, TrailNode actual)
        {
            begin = bgn;
            expectedEnd = expected;
            actualEnd = actual;
            time = DateTime.Now;
        }

        // Return the node the user started from which resulted in an error
        public TrailNode getBegin() { return begin; }

        // Return the expected next node. The user did not come to this node.
        public TrailNode getExpectedEnd() { return expectedEnd; }

        // Return which node the user actually went to. This will be useful
        // for analysis purposes of left-right impairment, etc.
        public TrailNode getActualEnd() { return actualEnd; }

        // Return the time of the error. This may not be useful information.
        public DateTime getTime() { return time; }

        public void setTime(DateTime t) { time = t; }
        public void setBegin(TrailNode node) { begin = node; }
        public void setExpected(TrailNode node) { expectedEnd = node; }
        public void setActual(TrailNode node) { actualEnd = node; }

        // Overload the operator == on two TestErrors. Useful to count how
        // many times the user made the same error in a test.
        /*public static bool operator ==(TestError te1, TestError te2)
        {
            return te1.getBegin() == te2.getBegin();
        }
        public static bool operator !=(TestError te1, TestError te2)
        {
            return te1.getBegin() != te2.getBegin();
        }*/

        // Convert the TestError object to a string to actually save it into
        // The TestReplay Object

        // Should be: 
        // beginning node
        // expected end node
        // actual end node
        // time of error
        public string convertToString()
        {
            string convert = "";
            convert += begin.convertToString();
            convert += expectedEnd.convertToString();
            convert += actualEnd.convertToString();
            convert += time.ToString();
            convert += "\n";

            return convert;
        }
    }

    // Allows TestReplay objects to detail what test they were created for
    public enum TEST_TYPE { TRAILS_A, TRAILS_A_H, TRAILS_B, TRAILS_B_H, REY_OSTERRIETH, CLOCK }
    public sealed class TestReplay
    {
        private const string fileExtension = ".txt";
        private TEST_TYPE testType;
        private Patient patient;
        private DateTime startTime;
        private DateTime endTime;
        private List<TestAction> testActions;
        private List<TestError> testErrors;
        private List<PatientNote> testNotes;
        private Stroke currentStroke;
        public TestReplay()
        {
            testActions = new List<TestAction>();
            testNotes = new List<PatientNote>();
            testErrors = new List<TestError>();
        }
        public TestReplay(Patient _patient, TEST_TYPE TestType)
        {
            patient = _patient;
            testActions = new List<TestAction>();
            testNotes = new List<PatientNote>();
            testErrors = new List<TestError>();
            testType = TestType;
        }
        public TEST_TYPE getTestType() { return testType; }
        public Patient getPatient() { return patient; }
        public DateTime getStartTime() { return startTime; }
        public DateTime getEndTime() { return endTime; }
        public List<TestAction> getTestActions() { return testActions; }
        public List<PatientNote> getPatientNotes() { return testNotes; }
        public List<TestError> getErrors() { return testErrors; }
        public void startTest() { startTime = DateTime.Now; }
        
        // Add a TestError to the list of errors in the TestReplay class
        public void addError(TestError e)
        {
            testErrors.Add(e);
        }
        public void endTest()
        {
            endTime = DateTime.Now;
        }
        // Create an unfinished stroke and add to it as the current action
        // First, test if there is not another current action
        public void beginStroke()
        {
            if (getCurrentTestAction() != null)
                if (!getCurrentTestAction().isFinished()) return;
            currentStroke = new Stroke();
            testActions.Add(currentStroke);
        }
        // Add a Line to a Stroke (must be unfinished and most recent action)
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
        // Adds a delete stroke object
        public void deleteStroke(int index)
        {
            if (getCurrentTestAction() == null) return;
            if (!getCurrentTestAction().isFinished()) return;
            //if (!checkCurrentTestAction(ACTION_TYPE.STROKE)) return;
            testActions.Add(new DeleteStroke(DateTime.Now, index));
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
        // Returns a filename to save the TestReplay object as
        public string getFileName()
        {
            string timeString = getStartTime().ToString().Replace("/", "-");
            string filename = patient.getName() + testType.ToString() + timeString + fileExtension;
            filename = filename.Replace(":", ";");
            return filename.Replace(" ", "_");
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
                patient.addFile(testFilename);
                await FileIO.WriteTextAsync(testStorageFile, stringToSave);
            }
            catch { return; }
            
        }
        // Converts the TestReplay object into a string for saving
        public string convertToString()
        {
            string testReplayString = "";
            // See Patient.convertToString() to see the intended format of the first line
            testReplayString += (patient.convertToString() + "\n");
            testReplayString += (startTime.ToString() + "\n");
            testReplayString += (endTime.ToString() + "\n");
            for (int i = 0; i < testActions.Count; i++)
            {
                switch (testActions[i].getActionType())
                {
                    case ACTION_TYPE.STROKE:
                        {
                            testReplayString += testActions[i].getActionTypeString() + " ";
                            testReplayString
                                += testActions[i].getStartTime().ToString() + " ";
                            testReplayString
                                += testActions[i].getEndTime().ToString() + "\n";

                            Stroke stroke = (Stroke)testActions[i];
                            List<LineData> lineData = stroke.getLines();
                            for (int j = 0; j < lineData.Count; j++)
                                testReplayString +=
                                    ("line " + formatLineDataAsString(lineData[j]) + "\n");
                            break;
                        }
                    case ACTION_TYPE.DEL_STROKE: 
                    {
                        testReplayString += (testActions[i].getActionTypeString() + " " +
                                             testActions[i].getStartTime().ToString() + " " +
                                             (testActions[i] as DeleteStroke).getIndex().ToString() + "\n");
                        break;
                    }
                    default: { break; }
                }
            }
            
            // Write out all the test errors on trails a into the file
            testReplayString += "=====ERRORS====\n";
            for (int i = 0; i < testErrors.Count; i++ )
            {
                testReplayString += testErrors[i].convertToString();
            }

            testReplayString += "=====NOTES=====\n";
            for (int i = 0; i < testNotes.Count; i++ )
            {
                testReplayString += (testNotes[i].convertToString());
            }
            return testReplayString;
        }
        // Converts LineData to string for saving TestReplay objects
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

        // Reads a file and attempts to convert it to a TestReplay object
        public async Task loadTestReplay(string testFilename)
        {
            testActions.Clear();
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
            if (testFilename.Contains("_H"))
            {
                if (this.testType == TEST_TYPE.TRAILS_A)
                {
                    this.testType = TEST_TYPE.TRAILS_A_H;
                }
                else
                {
                    this.testType = TEST_TYPE.TRAILS_B_H;
                }
            }
            parseTestReplayFile(testReplayString);
        }
        
        // Converts a string to a TestReplay object
        public void parseTestReplayFile(string testReplayString)
        {
            bool inActionSection = true;
            List<string> testStrings = 
                testReplayString.Split('\n').Cast<string>().ToList<string>();

            List<string> firstLineWords = testStrings[0].Split(' ').Cast<string>().ToList<string>();
            parsePatientInfo(firstLineWords);

            string startTimeString = testStrings[1];
            startTime = Convert.ToDateTime(startTimeString);

            int foundEndTestLine = 0;
            List<string> LEGACY_endTestCheckLine = testStrings[2].Split(' ').Cast<string>().ToList<string>();
            if (LEGACY_endTestCheckLine[0].Contains('/')) {
                string endTimeString = testStrings[2];
                endTime = Convert.ToDateTime(endTimeString);
                foundEndTestLine = 1;
            }

            for(int i = 2 + foundEndTestLine; i < testStrings.Count; i++)
            {
                if (inActionSection) {
                    List<string> lineWords = testStrings[i].Split(' ')
                        .Cast<string>().ToList<string>();
                    if (lineWords[0] == "line")
                        ((Stroke)testActions[testActions.Count - 1])
                            .addLineData(parseLineLineData(lineWords));
                    else if (lineWords[0] == "Stroke")
                        testActions.Add(parseLineStroke(lineWords));
                    else if (lineWords[0] == "DeleteStroke")
                        testActions.Add(parseLineDelPrevStroke(lineWords));
                    else if (lineWords[0] == "=====ERRORS====")
                    {
                        inActionSection = false;
                    }     
                    else if (lineWords[0] == "=====NOTES=====") 
                    { 
                        inActionSection = false;
                    }     
                } else {
                    // Parse all the error objects. Each one should be three nodes plus a Date
                    List<string> lineWords = testStrings[i]
                        .Split('\t').Cast<string>().ToList<string>();
                    
                    // This is an error object.
                    // Should be read as:
                    //begin   node '\t' point
                    //exp end node '\t' point
                    //act end node '\t' point
                    //DateTime

                    if(lineWords.Count == 10)
                    {
                        TestError error = new TestError();
                        // Get each of the node strings and points from the line
                        for(int j = 0; j < 9; j += 3)
                        {
                            string beginText = lineWords[j];
                            Point point;
                            point.X = Convert.ToDouble(lineWords[j + 1]);
                            point.Y = Convert.ToDouble(lineWords[j + 2]);
                            bool flip = true;
                            if(testType.ToString().Contains("_H"))
                                flip = false;
                            TrailNode node = new TrailNode(beginText, point, flip);
                            
                            if(j == 0)
                                error.setBegin(node);
                            if (j == 3)
                                error.setExpected(node);
                            if (j == 6)
                                error.setActual(node);
                        }
                        
                        DateTime date = new DateTime();
                        date = Convert.ToDateTime(lineWords[9]);
                        error.setTime(date);

                        testErrors.Add(error);
                    }

                    else if (lineWords.Count >= 5)
                    {
                        DateTime date = new DateTime();
                        date = DateTime.Parse(lineWords[0] + " " + 
                            lineWords[1] + " " + lineWords[2]);
                        lineWords[3] = lineWords[3].Replace("[SPC]", "");
                        lineWords[4] = lineWords[4].Replace("[SPC]", "");
                        testNotes.Add(
                            new PatientNote(lineWords[3], lineWords[4], date));
                    }
                }
            }
        }

        // Populates the TestReplay patient using the split patient line
        public void parsePatientInfo(List<string> patientString)
        {
            GENDER gender;
            if (patientString[6] == "M") gender = GENDER.MALE;
            else gender = GENDER.FEMALE;
            EDU_LEVEL eduLevel = (EDU_LEVEL)Enum.Parse(typeof(EDU_LEVEL), patientString[8]);
            DateTime birthdate = Convert.ToDateTime(patientString[2] + ' ' + patientString[3] + ' ' + patientString[4]);
            string name = patientString[0];
            patient = new Patient(name, "", birthdate, gender, eduLevel);
        }

        // Converts a string into a Stroke object
        public Stroke parseLineStroke(List<string> line)
        {
            DateTime startTime = DateTime.Parse(
                line[1] + " " + line[2] + " " + line[3]);
            DateTime endTime = DateTime.Parse(
                line[4] + " " + line[5] + " " + line[6]);
            return new Stroke(startTime, endTime);
        }
        
        // Converts a string into a DeleteStroke object
        public DeleteStroke parseLineDelPrevStroke(List<string> line)
        {
            DateTime occuranceTime = DateTime.Parse(
                line[1] + " " + line[2] + " " + line[3]);
            int index = Convert.ToInt32(line[4]);
            return new DeleteStroke(occuranceTime, index);
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

        public string getDisplayedDatetime(string filename)
        {
            int testTypeLen = testType.ToString().Length;
            if(filename.Contains("_H"))
            {
                testTypeLen += 2;
            }
            
            int idLen = 15;
            int extensionLen = 4;

            string datetime = filename.Substring(idLen + testTypeLen + 1, filename.Length - (idLen + testTypeLen + 1) - extensionLen);
            return replaceWithPrettyChars(datetime);
        }

        public string replaceWithPrettyChars(string datetime)
        {
            datetime = datetime.Replace("-", "/");
            datetime = datetime.Replace("_", " ");
            datetime = datetime.Replace(";", ":");
            return datetime;
        }

        public string getFilenameString(string date)
        {
            date = date.Replace("/", "-");
            date = date.Replace(" ", "_");
            date = date.Replace(":", ";");
            return date;
        }
    }
}