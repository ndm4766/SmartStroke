using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Input;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting.DirectUIControls;
using Microsoft.VisualStudio.TestTools.UITesting.WindowsRuntimeControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace CodedUITestProject1
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest(CodedUITestType.WindowsStore)]
    public class CodedUITest1
    {
        public CodedUITest1()
        {
        }

        [DeploymentItem("MOCK_DATA.csv"),
        DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\MOCK_DATA.csv",
            "MOCK_DATA#csv",
            DataAccessMethod.Sequential), TestMethod]

        public void CodedUITestMethod1()
        {
            
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
           
            // Launch the app
            XamlWindow.Launch("5d90a074-2671-4811-8156-60e16c3fd5e3_ywvksadvjkdhr!App");
            
            // Insert username
            this.UIMap.AssertMethod1ExpectedValues.UIUserIdEditText = TestContext.DataRow["username"].ToString();
            //this.UIMap.AssertMethod1();
            
            // Insert password
            string pass = Playback.EncryptText(TestContext.DataRow["Username"].ToString());
            this.UIMap.UISmartStrokeWindow.UIUserPasswordEdit.Text = pass;

            // Click the login button
            Gesture.Tap(this.UIMap.UISmartStrokeWindow.UILogInButton);

            // Click the add new patient button
            Gesture.Tap(this.UIMap.UISmartStrokeWindow.UIAddnewPatientButton);

            // Delete name in box
            Gesture.Tap(this.UIMap.UISmartStrokeWindow.UIPatientNameEdit);
            Gesture.Tap(this.UIMap.UISmartStrokeWindow.UIPatientNameEdit.UIDeleteButton);

            // Insert patient name
            this.UIMap.UISmartStrokeWindow.UIPatientNameEdit.Text = TestContext.DataRow["name"].ToString();
            //this.UIMap.UISmartStrokeWindow.UIPatientNameEdit = TestContext.DataRow["name"].ToString();

            // Select gender
            string gender = TestContext.DataRow["gender"].ToString();
            if (gender == "Male")
            {
                this.UIMap.UISmartStrokeWindow.UIMaleRadioButton.Selected = true;
                //Gesture.Tap(this.UIMap.UISmartStrokeWindow.UIMaleRadioButton);
            }
            else
            {
                this.UIMap.UISmartStrokeWindow.UIFemaleRadioButton.Selected = true;
                //Gesture.Tap(this.UIMap.UISmartStrokeWindow.UIFemaleRadioButton);
            }

            // Select Month
            DateTime bday = Convert.ToDateTime(TestContext.DataRow["birthday"].ToString());
            string birthday = "NULL";
            switch (Convert.ToInt32(bday.Month.ToString()))
            {
                case 1:
                    birthday = "January";
                    break;
                case 2:
                    birthday = "February";
                    break;
                case 3:
                    birthday = "March";
                    break;
                case 4:
                    birthday = "April";
                    break;
                case 5:
                    birthday = "May";
                    break;
                case 6:
                    birthday = "June";
                    break;
                case 7:
                    birthday = "July";
                    break;
                case 8:
                    birthday = "Augest";
                    break;
                case 9:
                    birthday = "September";
                    break;
                case 10:
                    birthday = "October";
                    break;
                case 11:
                    birthday = "November";
                    break;
                case 12:
                    birthday = "December";
                    break;
                default:
                    birthday = "END_OF_CASE";
                    break;
            }
            this.UIMap.UISmartStrokeWindow.UIDatepickerGroup.UIMonthComboBox.SelectedItem = birthday;
            
            // Select day
            this.UIMap.UISmartStrokeWindow.UIDatepickerGroup.UIDayComboBox.SelectedIndex = (bday.Day - 1);

            // Select year
            this.UIMap.UISmartStrokeWindow.UIDatepickerGroup.UIYearComboBox.SelectedItem = bday.Year.ToString();
            
            // Select Education Level
            string education = TestContext.DataRow["education"].ToString();
            Gesture.Tap(UIMap.UISmartStrokeWindow.UIEducationComboBox);

            if (education == "True")
            {
                this.UIMap.UISmartStrokeWindow.UIEducationComboBox.SelectedItem = "College Degree";
            }
            else
            {
                this.UIMap.UISmartStrokeWindow.UIEducationComboBox.SelectedItem = "Highschool Diploma";
            }

            // Click submit button
            Gesture.Tap(this.UIMap.UISmartStrokeWindow.UISubmitButton);

        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
