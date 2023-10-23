.NET MAUI Spreadsheet Project README
Author: Tim Blamires & Bode Packer
Date: 10/23/23

Project Overview
This README file provides essential information about the .NET MAUI Spreadsheet project. It includes details about design decisions, external code resources, implementation notes, and any important instructions for general use.

Project Description
Our .NET MAUI Spreadsheet project is designed to provide a user-friendly interface for managing spreadsheets. The application features several functions to make working with spreadsheets more efficient and user-friendly. Below are some of the key functionalities:

Editing Spreadsheet
To edit a cell's content:

Click on the cell you want to modify.
Input the desired content into the text box on the top right of the window.
Press "Enter" to update the values in the spreadsheet.
The current cell name will be displayed in the top-left box, and the current cell value will be in the middle box.
To enter formulas into the sheet, they must start with an equal sign, e.g., (=A1+5).
File Management
Creating a New Spreadsheet
When selecting "New" in the File Menu, a dialogue box will ask if you want to erase unsaved work.
Click "Cancel" to back out and prevent creating a new sheet.
Click "Accept" to create a new sheet, deleting unsaved work.
Opening an Existing Spreadsheet
When selecting "Open" in the File Menu, a similar dialogue box as in "Creating a New Spreadsheet" will appear.
If "Accept" is clicked, a File Selector window will open, allowing you to select a file.
The file should have a .sprd extension, and its contents should adhere to the proper JSON format as specified in the API for spreadsheet.cs.
The opened file will update both the backing spreadsheet and the visual representation in the SpreadsheetGrid.
Saving a Spreadsheet
When selecting "Save" in the File Menu, a dialogue text box will pop up, asking you to specify the file path where the current spreadsheet should be saved.
After saving the file, clicking "Save" again will automatically overwrite the sheet at the same location.
Undo and Redo Actions
Undo
Select "Undo" in the Edit Menu to revert the most recently altered cell to its previous content.
Note: On Mac, Menu Items cannot be disabled, so clicking "Undo" when no actions are remaining will throw an error.
Redo
Select "Redo" in the Edit Menu to reverse the most recent undone action.
Note: On Mac, Menu Items cannot be disabled, so clicking "Redo" when no actions have been undone will throw an error.
Special Feature: Cell Value Truncation
If a cell's content contains more than 10 characters, the spreadsheet will visually truncate the values to fit inside the cell.

External Resources
[List any external libraries, frameworks, or APIs used in the project and provide relevant links.]
Implementation Notes
[Add any important implementation details, architectural choices, or technical notes here.]
Known Issues
[List any known issues, bugs, or limitations of the project.]
Future Enhancements
[Describe any planned future enhancements or features for the project.]
Author Information
[Your Name]
[Your Contact Information]

[Add any additional contact or author information as needed.]

Revision History
[Use this section to log updates and changes to the project. Include dates and descriptions of revisions.]
