using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStroke
{
    public enum GENDER { MALE, FEMALE }
    public enum EDU_LEVEL { HIGHSCHOOL, ASSOCIATES, BACHELORS, MASTERS, PHD }
    class Patient
    {
        private string name;
        private DateTime birthDate;
        private GENDER gender;
        private EDU_LEVEL eduLevel;
        public Patient(string Name, DateTime BirthDate,
            GENDER Gender, EDU_LEVEL EduLevel)
        {
            name = Name;
            birthDate = BirthDate;
            gender = Gender;
            eduLevel = EduLevel;
        }
        public string getName() { return name; }
        public DateTime getBirthDate() { return birthDate; }
        public GENDER getGender() { return gender; }
        public EDU_LEVEL getEduLevel() { return eduLevel; }
    }
}
