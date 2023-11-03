.NET MAUI Spreadsheet README
Author: Tim Blamires & Bode Packer
Date: 10/23/23

Project Overview
This project is a basic Excel spreasheet clone written in C# and MAUI. Bode Packer and Tim Blamires worked on this project over the course of 7 weeks as a class project for Software Practice 1. We implemented a GUI interface following MVC architecture as well as different backing classes to represent formulas and basic calculator functions. 

Project Description
Our .NET MAUI Spreadsheet project is designed to provide a user-friendly interface for managing spreadsheets. The application features several functions to make working with spreadsheets more efficient and user-friendly. This includes editing, file saving and opening, calculator functions, undo/redo functionality, and adjusting themes. Below are some of the key functionalities:

Editing Spreadsheet

To edit a cell's content:
Click on the cell you want to modify.
Input the desired content into the text box on the top right of the window.
Press "Enter" to update the values in the spreadsheet.
The current cell name will be displayed in the top-left box, and the current cell value will be in the middle box.
To enter formulas into the sheet, they must start with an equal sign, e.g., (=A1+5).

File Management

Creating a New Spreadsheet: 
When selecting "New" in the File Menu, a dialogue box will ask if you want to erase unsaved work.
Click "Cancel" to back out and prevent creating a new sheet.
Click "Accept" to create a new sheet, deleting unsaved work.

Opening an Existing Spreadsheet:
When selecting "Open" in the File Menu, a similar dialogue box as in "Creating a New Spreadsheet" will appear.
If "Accept" is clicked, a File Selector window will open, allowing you to select a file.
The file should have a .sprd extension, and its contents should adhere to the proper JSON format as specified in the API for spreadsheet.cs.
The opened file will update both the backing spreadsheet and the visual representation in the SpreadsheetGrid.

Saving a Spreadsheet:
When selecting "Save" in the File Menu, a dialogue text box will pop up, asking you to specify the file path where the current spreadsheet should be saved.
After saving the file, clicking "Save" again will automatically overwrite the sheet at the same location.

Undo and Redo Actions

Undo:
Select "Undo" in the Edit Menu to revert the most recently altered cell to its previous content.
Note: On Mac, Menu Items cannot be disabled, so clicking "Undo" when no actions are remaining will throw an error.

Redo:
Select "Redo" in the Edit Menu to reverse the most recent undone action.
Note: On Mac, Menu Items cannot be disabled, so clicking "Redo" when no actions have been undone will throw an error.

Cell Value Truncation:
If a cell's content contains more than 10 characters, the spreadsheet will visually truncate the values to fit inside the cell.

Special Feature Notes:
Our special features include a undo and redo buttons, a menu to change font themes, and auto truncation of strings longer then 10 characters. This truncation ensures values do not pill over into other cells. The full cell value and content can still be viewed in the Entry menu items at the top of the sheet. The use of all of these features is explained in more depth above. 

Implementation Notes
To view data in the spreadsheet we used entries as a simple menu bar to view the current selcted cell, the current cell's value, and the current cell's content. We also added menuBar elements for spreadsheet actions such as Save or Undo. This design decison allowed us to maintain a single content page and increases usability, as this follows larger spreadsheet design decisions. 

Known Issues
Menu Items are not allowed to be disabled on Mac, so when clicking a button that should be disabled, a pop up is displayed saying button can not be clicked. 

Future Enhancements
When changing themes, we were unable to make the Column and Row labels update to the repective font theme slected by the user. We would like to add that functionality as well as the the font of all text the user enters into the spreadsheet cells.

Author Information
Bode Packer
bjackpacker@gmail.com
Tim Blamires
blamires.timothy@gmail.com
