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
        private string note;
        private DateTime time;
        public PatientNote(string Note) { note = Note; }
        public PatientNote(string Note, DateTime Time)  // There is only content and a time?! No subject, context? 
        {
            note = Note;
            time = Time;
        }
        public void changeNote(string Note) {       //REALLY?!! The doctor is going to change a note by inputting the entire text of the note and then the new one? I doubt it
            note = Note;
        }
        public string getNote() { return note; }
        public DateTime getTime() { return time; }
    }
    public class Patient
    {
        private string name;
        private DateTime birthDate;
        private GENDER gender;
        private EDU_LEVEL eduLevel;
        private List<PatientNote> notes;
        public Patient(string Name, DateTime BirthDate,
            GENDER Gender, EDU_LEVEL EduLevel)
        {
            name = Name;
            birthDate = BirthDate;
            gender = Gender;
            eduLevel = EduLevel;
            notes = new List<PatientNote>();
        }
        public string getName() { return name; }
        public DateTime getBirthDate() { return birthDate; }
        public GENDER getGender() { return gender; }
        public EDU_LEVEL getEduLevel() { return eduLevel; }
        public List<PatientNote> getNotes() { return notes; }
    }
}
