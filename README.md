SmartStroke
===========

Installing This Application
==========================

You have two different options for installing this application:

1) The applicaion resides in the SmartStroke.sln file (this is a Visual Studio 2013 solution file). To open our application and use it, you must have Windows 8.1 and Microsoft Visual Studio 2013 on your computer. 
To use, you must download this solution file and open it. If successful, Visual Studio will launch and the solution will be loaded. If you are running an incorrect version of Visual Studio (such as 2012), you may receive an error such as a failed 'Migration.' The csproj file in the SmartStroke.sln will not be able to migrate successfully to Visual Studio 2012 (which is a downgrade).

2) You can download the file Installer.zip file at the file location:

https://github.com/tracyhammond/mediCS/blob/master/SmartStroke/Installer.zip

Once extracted, you will find a file called Add-AppDevPackage.ps1 in the SmartStroke_1.1.0.2_AnyCPU_Test folder. Right click on this file and choose 'Run with Powershell.' Follow the onscreen instructions to allow it to automatically run the app installer (you need administrator access). This should install the SmartStrokes application as a tile on the metro screen of Windows 8.1.

How To Use This Application
===========================

Upon opening the app, you are greeted by a log-in screen that allows you to either enter an existing username/password combination.  If using the app for the first time, select the choice to make a new account and create a username and password for yourself.  If you are a returning user, just enter your existing credentials.  From there you will be directed to a screen allowing you to either select an existing patient ID by searching it in the textbox shown, or creating a new user.  If you create a new patient, you will have to enter the person's gender, birthday, and education level.  Once a patient is selected, the main screen is displayed.  From here you can either take a new test (Trails version A or B, clock-drawing, or Rey-Osterrieth tests), or analyze a past test.  Each test, when selected, will display an instructions screen first with the test directions, and then allows the user to take the test.  


Norm Comparison/Graphs
=======================

When first navigating to the Norm Comparison page, a loading ring will display showing that the data is being loaded and calculated to be displayed on the graphs. By default, Trails A vertical test data will be displayed along with for all age groups, all education levels, along with the mean and median plots. The Normative Data graphs will graph all normative data for Trails A & B separately. In addition to the normal Trails A & B tests, we also include the option to graph horizontal versisons of Trails A & B. To switch between which type of test to graph, select the dropdown box below the Options button to select which test you would like to graph. 

The Options button wil display a popup menu with several options on how to display the graphs. There are 2 dropdown boxes for Time Range. This sets the range of test times to be displayed on the graph. There is no specific order on the dropdown boxes. The left or right box can be either the minimum or maximum for the time range. There are also 2 dropdown boxes for the Age Range. This sets the range of test times by the ages of the subjects to be displayed on the graph. There is no specific order on the dropdown boxes. The left or right box can be either the minimum or maximum for the age range. The age range minimum is 18, anything below that will be displayed in the age group of -1 to signify an error in the data. This can happen if a subject is youger than 18 or if they failed to properly select a birthdate when being added. There are checkboxes for each level of education to be graphed. If an education level is checkmarked, then subjects of that education level will be included in the graphical display of the normative data. Unchecking a box will refresh the graph that does not include subjects with that education level. There are also checkboxes for male and female, these do the same thing as the education level checkboxes except based on gender. Finally, there are 2 more checkboxes, for displaying a line graph for the mean and median. Unchecking these boxes will remove the respective line graph.  

Time Elapsed View
==================

In order to see how the patient drew the test with color-based timing information, select a test date and time taken in the list and then tap "replay".  This will show the strokes of the test colored by a certain number of sectons per color (where a color is a section of the line).  The number of seconds each color represents is shown the bottom right corner.

