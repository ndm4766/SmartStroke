using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStroke
{
    public enum GENDER { MALE, FEMALE }
    public enum EDU_LEVEL { HIGHSCHOOL, ASSOCIATES, BACHELORS, MASTERS, PHD }
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
    }
    public class Patient
    {
        private string name;
        private DateTime birthDate;
        private GENDER gender;
        private EDU_LEVEL eduLevel;
        private List<PatientNote> notes;
        private List<string> testFiles;
        public Patient(string Name, DateTime BirthDate,
            GENDER Gender, EDU_LEVEL EduLevel)
        {
            name = Name;
            birthDate = BirthDate;
            gender = Gender;
            eduLevel = EduLevel;
            notes = new List<PatientNote>();
            testFiles = new List<string>();
        }
        public string getName() { return name; }
        public DateTime getBirthDate() { return birthDate; }
        public GENDER getGender() { return gender; }
        public EDU_LEVEL getEduLevel() { return eduLevel; }
        public List<PatientNote> getNotes() { return notes; }
        public string convertToString() {
            char genderChar = (gender == GENDER.MALE ? 'M' : 'F');
            return name + " - "
                + birthDate.ToString() + " - "
                + genderChar + " - "
                + eduLevel.ToString();
        }
    }
}
