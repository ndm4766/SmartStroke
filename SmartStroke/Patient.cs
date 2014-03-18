using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStroke
{
    public enum GENDER { MALE, FEMALE }
    public enum EDU_LEVEL { HIGHSCHOOL, ASSOCIATES, BACHELORS, MASTERS, PHD, OTHER }
    public class PatientNote
    {
        private string title;
        private string note;
        private DateTime time;
        public PatientNote(string Title, string Note) {
            title = Title;
            time = DateTime.Now;
            note = Note;
        }
        public PatientNote(string Title, string Note, DateTime Time)
        {
            title = Title;
            note = Note;
            time = Time;
        }
        public void changeTitle(string Title) { title = Title; }
        public string getTitle() { return title;  }
       
        public void changeNote(string Note) {
            note = Note;
        }
        public string getNote() { return note; }
        public void changeTime(DateTime Time) { time = Time; }
        public DateTime getTime() { return time; }
        public string convertToString()
        {
            string stringNote = time.ToString() + '\t';
            stringNote = stringNote.Replace(" ","-");
            stringNote += (title + '\t');
            stringNote += (note + '\n');
            return stringNote;
        }
    }
    public class Patient
    {
        private string name;
        private string doctor;
        private DateTime birthDate;
        private GENDER gender;
        private EDU_LEVEL eduLevel;
        private List<PatientNote> notes;
        private List<string> testFilenames;
        public Patient(string Name, string doc, DateTime BirthDate,
            GENDER Gender, EDU_LEVEL EduLevel)
        {
            name = Name;
            doctor = doc;
            birthDate = BirthDate;
            gender = Gender;
            eduLevel = EduLevel;
            notes = new List<PatientNote>();
            testFilenames = new List<string>();
        }
        public string getName() { return name; }
        public string getDoctor() { return doctor; }
        public DateTime getBirthDate() { return birthDate; }
        public GENDER getGender() { return gender; }
        public EDU_LEVEL getEduLevel() { return eduLevel; }
        public List<PatientNote> getNotes() { return notes; }
        public List<string> getTestFilenames() { return testFilenames; }
        public void addNote(PatientNote patNote) { notes.Add(patNote); }
        public void addFile(string testFile) { testFilenames.Add(testFile); }
        public string convertToString() {
            char genderChar = (gender == GENDER.MALE ? 'M' : 'F');
            return name + " - "
                + birthDate.ToString() + " - "
                + genderChar + " - "
                + eduLevel.ToString();
        }

        async public void loadFiles()
        {
            var filenames = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (var filename in filenames)
            {
                // The fileName starts with a number
                if (filename.Name[0] >= 48 && filename.Name[0] <= 58 && filename.Name.Contains(name))
                {
                    testFilenames.Add(filename.Name);
                }
            }
        }
    }
}
