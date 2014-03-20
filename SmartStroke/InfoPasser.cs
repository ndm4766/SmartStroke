using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStroke
{
    //used to pass information between pages
    class InfoPasser
    {
        public char trailsTestVersion;
        public Patient currentPatient;
        public String doctorId;
        public TestReplay testReplay;

        public InfoPasser()
        {
            doctorId = "0";
            trailsTestVersion = 'x';
            currentPatient = null;
        }

        public InfoPasser(String docId)
        {
            doctorId = docId;
            trailsTestVersion = 'x';
            currentPatient = null;
        }

        public void addTestToPasser(TestReplay tr)
        {
            testReplay = tr;
        }

    }
}
