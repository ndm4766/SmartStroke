﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 12.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace CodedUITestProject1
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITest.Input;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WindowsRuntimeControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = Microsoft.VisualStudio.TestTools.UITest.Input.MouseButtons;
    
    
    [GeneratedCode("Coded UITest Builder", "12.0.21005.1")]
    public partial class UIMap
    {
        
        /// <summary>
        /// AssertMethod1 - Use 'AssertMethod1ExpectedValues' to pass parameters into this method.
        /// </summary>
        public void AssertMethod1()
        {
            #region Variable Declarations
            XamlEdit uIUserIdEdit = this.UISmartStrokeWindow.UIUserIdEdit;
            XamlEdit uIUserPasswordEdit = this.UISmartStrokeWindow.UIUserPasswordEdit;
            XamlEdit uIPatientNameEdit = this.UISmartStrokeWindow.UIPatientNameEdit;
            XamlRadioButton uIMaleRadioButton = this.UISmartStrokeWindow.UIMaleRadioButton;
            XamlRadioButton uIFemaleRadioButton = this.UISmartStrokeWindow.UIFemaleRadioButton;
            XamlComboBox uIMonthComboBox = this.UISmartStrokeWindow.UIDatepickerGroup.UIMonthComboBox;
            XamlComboBox uIDayComboBox = this.UISmartStrokeWindow.UIDatepickerGroup.UIDayComboBox;
            XamlComboBox uIYearComboBox = this.UISmartStrokeWindow.UIDatepickerGroup.UIYearComboBox;
            XamlComboBox uIEducationComboBox = this.UISmartStrokeWindow.UIEducationComboBox;
            #endregion

            // Verify that the 'Text' property of 'userId' text box is not equal to 'User Id'
            Assert.AreNotEqual(this.AssertMethod1ExpectedValues.UIUserIdEditText, uIUserIdEdit.Text);

            // Verify that the 'Enabled' property of 'userPassword' text box equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIUserPasswordEditEnabled, uIUserPasswordEdit.Enabled);

            // Verify that the 'Text' property of 'patientName' text box is not equal to 'Patient Name'
            Assert.AreNotEqual(this.AssertMethod1ExpectedValues.UIPatientNameEditText, uIPatientNameEdit.Text);

            // Verify that the 'Enabled' property of 'Male' radio button equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIMaleRadioButtonEnabled, uIMaleRadioButton.Enabled);

            // Verify that the 'Enabled' property of 'Female' radio button equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIFemaleRadioButtonEnabled, uIFemaleRadioButton.Enabled);

            // Verify that the 'Enabled' property of 'Month' combo box equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIMonthComboBoxEnabled, uIMonthComboBox.Enabled);

            // Verify that the 'Enabled' property of 'Day' combo box equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIDayComboBoxEnabled, uIDayComboBox.Enabled);

            // Verify that the 'Enabled' property of 'Year' combo box equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIYearComboBoxEnabled, uIYearComboBox.Enabled);

            // Verify that the 'Enabled' property of 'education' combo box equals 'True'
            Assert.AreEqual(this.AssertMethod1ExpectedValues.UIEducationComboBoxEnabled, uIEducationComboBox.Enabled);
        }
        
        #region Properties
        public virtual AssertMethod1ExpectedValues AssertMethod1ExpectedValues
        {
            get
            {
                if ((this.mAssertMethod1ExpectedValues == null))
                {
                    this.mAssertMethod1ExpectedValues = new AssertMethod1ExpectedValues();
                }
                return this.mAssertMethod1ExpectedValues;
            }
        }
        
        public UISmartStrokeWindow UISmartStrokeWindow
        {
            get
            {
                if ((this.mUISmartStrokeWindow == null))
                {
                    this.mUISmartStrokeWindow = new UISmartStrokeWindow();
                }
                return this.mUISmartStrokeWindow;
            }
        }
        #endregion
        
        #region Fields
        private AssertMethod1ExpectedValues mAssertMethod1ExpectedValues;
        
        private UISmartStrokeWindow mUISmartStrokeWindow;
        #endregion
    }
    
    /// <summary>
    /// Parameters to be passed into 'AssertMethod1'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "12.0.21005.1")]
    public class AssertMethod1ExpectedValues
    {
        
        #region Fields
        /// <summary>
        /// Verify that the 'Text' property of 'userId' text box is not equal to 'User Id'
        /// </summary>
        public string UIUserIdEditText = "User Id";
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'userPassword' text box equals 'True'
        /// </summary>
        public bool UIUserPasswordEditEnabled = true;
        
        /// <summary>
        /// Verify that the 'Text' property of 'patientName' text box is not equal to 'Patient Name'
        /// </summary>
        public string UIPatientNameEditText = "Patient Name";
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'Male' radio button equals 'True'
        /// </summary>
        public bool UIMaleRadioButtonEnabled = true;
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'Female' radio button equals 'True'
        /// </summary>
        public bool UIFemaleRadioButtonEnabled = true;
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'Month' combo box equals 'True'
        /// </summary>
        public bool UIMonthComboBoxEnabled = true;
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'Day' combo box equals 'True'
        /// </summary>
        public bool UIDayComboBoxEnabled = true;
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'Year' combo box equals 'True'
        /// </summary>
        public bool UIYearComboBoxEnabled = true;
        
        /// <summary>
        /// Verify that the 'Enabled' property of 'education' combo box equals 'True'
        /// </summary>
        public bool UIEducationComboBoxEnabled = true;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "12.0.21005.1")]
    public class UISmartStrokeWindow : XamlWindow
    {
        
        public UISmartStrokeWindow()
        {
            #region Search Criteria
            this.SearchProperties[XamlControl.PropertyNames.Name] = "SmartStroke";
            this.SearchProperties[XamlControl.PropertyNames.ClassName] = "Windows.UI.Core.CoreWindow";
            this.WindowTitles.Add("SmartStroke");
            #endregion
        }
        
        #region Properties
        public XamlEdit UIUserIdEdit
        {
            get
            {
                if ((this.mUIUserIdEdit == null))
                {
                    this.mUIUserIdEdit = new XamlEdit(this);
                    #region Search Criteria
                    this.mUIUserIdEdit.SearchProperties[XamlEdit.PropertyNames.AutomationId] = "userId";
                    this.mUIUserIdEdit.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIUserIdEdit;
            }
        }
        
        public XamlEdit UIUserPasswordEdit
        {
            get
            {
                if ((this.mUIUserPasswordEdit == null))
                {
                    this.mUIUserPasswordEdit = new XamlEdit(this);
                    #region Search Criteria
                    this.mUIUserPasswordEdit.SearchProperties[XamlEdit.PropertyNames.AutomationId] = "userPassword";
                    this.mUIUserPasswordEdit.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIUserPasswordEdit;
            }
        }
        
        public XamlButton UILogInButton
        {
            get
            {
                if ((this.mUILogInButton == null))
                {
                    this.mUILogInButton = new XamlButton(this);
                    #region Search Criteria
                    this.mUILogInButton.SearchProperties[XamlButton.PropertyNames.Name] = "Log In";
                    this.mUILogInButton.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUILogInButton;
            }
        }
        
        public XamlButton UIAddnewPatientButton
        {
            get
            {
                if ((this.mUIAddnewPatientButton == null))
                {
                    this.mUIAddnewPatientButton = new XamlButton(this);
                    #region Search Criteria
                    this.mUIAddnewPatientButton.SearchProperties[XamlButton.PropertyNames.Name] = "Add new Patient";
                    this.mUIAddnewPatientButton.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIAddnewPatientButton;
            }
        }
        
        public UIPatientNameEdit UIPatientNameEdit
        {
            get
            {
                if ((this.mUIPatientNameEdit == null))
                {
                    this.mUIPatientNameEdit = new UIPatientNameEdit(this);
                }
                return this.mUIPatientNameEdit;
            }
        }
        
        public XamlRadioButton UIMaleRadioButton
        {
            get
            {
                if ((this.mUIMaleRadioButton == null))
                {
                    this.mUIMaleRadioButton = new XamlRadioButton(this);
                    #region Search Criteria
                    this.mUIMaleRadioButton.SearchProperties[XamlRadioButton.PropertyNames.AutomationId] = "sexM";
                    this.mUIMaleRadioButton.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIMaleRadioButton;
            }
        }
        
        public XamlRadioButton UIFemaleRadioButton
        {
            get
            {
                if ((this.mUIFemaleRadioButton == null))
                {
                    this.mUIFemaleRadioButton = new XamlRadioButton(this);
                    #region Search Criteria
                    this.mUIFemaleRadioButton.SearchProperties[XamlRadioButton.PropertyNames.AutomationId] = "sexF";
                    this.mUIFemaleRadioButton.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIFemaleRadioButton;
            }
        }
        
        public UIDatepickerGroup UIDatepickerGroup
        {
            get
            {
                if ((this.mUIDatepickerGroup == null))
                {
                    this.mUIDatepickerGroup = new UIDatepickerGroup(this);
                }
                return this.mUIDatepickerGroup;
            }
        }
        
        public XamlText UIItem2014Text
        {
            get
            {
                if ((this.mUIItem2014Text == null))
                {
                    this.mUIItem2014Text = new XamlText(this);
                    #region Search Criteria
                    this.mUIItem2014Text.SearchProperties[XamlText.PropertyNames.Name] = "‎2014";
                    this.mUIItem2014Text.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIItem2014Text;
            }
        }
        
        public XamlComboBox UIEducationComboBox
        {
            get
            {
                if ((this.mUIEducationComboBox == null))
                {
                    this.mUIEducationComboBox = new XamlComboBox(this);
                    #region Search Criteria
                    this.mUIEducationComboBox.SearchProperties[XamlComboBox.PropertyNames.AutomationId] = "education";
                    this.mUIEducationComboBox.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIEducationComboBox;
            }
        }
        
        public XamlButton UISubmitButton
        {
            get
            {
                if ((this.mUISubmitButton == null))
                {
                    this.mUISubmitButton = new XamlButton(this);
                    #region Search Criteria
                    this.mUISubmitButton.SearchProperties[XamlButton.PropertyNames.AutomationId] = "SubmitButton";
                    this.mUISubmitButton.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUISubmitButton;
            }
        }
        #endregion
        
        #region Fields
        private XamlEdit mUIUserIdEdit;
        
        private XamlEdit mUIUserPasswordEdit;
        
        private XamlButton mUILogInButton;
        
        private XamlButton mUIAddnewPatientButton;
        
        private UIPatientNameEdit mUIPatientNameEdit;
        
        private XamlRadioButton mUIMaleRadioButton;
        
        private XamlRadioButton mUIFemaleRadioButton;
        
        private UIDatepickerGroup mUIDatepickerGroup;
        
        private XamlText mUIItem2014Text;
        
        private XamlComboBox mUIEducationComboBox;
        
        private XamlButton mUISubmitButton;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "12.0.21005.1")]
    public class UIPatientNameEdit : XamlEdit
    {
        
        public UIPatientNameEdit(XamlControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[XamlEdit.PropertyNames.AutomationId] = "patientName";
            this.WindowTitles.Add("SmartStroke");
            #endregion
        }
        
        #region Properties
        public XamlButton UIDeleteButton
        {
            get
            {
                if ((this.mUIDeleteButton == null))
                {
                    this.mUIDeleteButton = new XamlButton(this);
                    #region Search Criteria
                    this.mUIDeleteButton.SearchProperties[XamlButton.PropertyNames.AutomationId] = "DeleteButton";
                    this.mUIDeleteButton.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIDeleteButton;
            }
        }
        #endregion
        
        #region Fields
        private XamlButton mUIDeleteButton;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "12.0.21005.1")]
    public class UIDatepickerGroup : XamlControl
    {
        
        public UIDatepickerGroup(XamlControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[UITestControl.PropertyNames.ControlType] = "Group";
            this.SearchProperties["AutomationId"] = "birthdayPicker";
            this.WindowTitles.Add("SmartStroke");
            #endregion
        }
        
        #region Properties
        public XamlComboBox UIMonthComboBox
        {
            get
            {
                if ((this.mUIMonthComboBox == null))
                {
                    this.mUIMonthComboBox = new XamlComboBox(this);
                    #region Search Criteria
                    this.mUIMonthComboBox.SearchProperties[XamlComboBox.PropertyNames.AutomationId] = "MonthPicker";
                    this.mUIMonthComboBox.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIMonthComboBox;
            }
        }
        
        public XamlComboBox UIDayComboBox
        {
            get
            {
                if ((this.mUIDayComboBox == null))
                {
                    this.mUIDayComboBox = new XamlComboBox(this);
                    #region Search Criteria
                    this.mUIDayComboBox.SearchProperties[XamlComboBox.PropertyNames.AutomationId] = "DayPicker";
                    this.mUIDayComboBox.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIDayComboBox;
            }
        }
        
        public XamlComboBox UIYearComboBox
        {
            get
            {
                if ((this.mUIYearComboBox == null))
                {
                    this.mUIYearComboBox = new XamlComboBox(this);
                    #region Search Criteria
                    this.mUIYearComboBox.SearchProperties[XamlComboBox.PropertyNames.AutomationId] = "YearPicker";
                    this.mUIYearComboBox.WindowTitles.Add("SmartStroke");
                    #endregion
                }
                return this.mUIYearComboBox;
            }
        }
        #endregion
        
        #region Fields
        private XamlComboBox mUIMonthComboBox;
        
        private XamlComboBox mUIDayComboBox;
        
        private XamlComboBox mUIYearComboBox;
        #endregion
    }
}