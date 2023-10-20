﻿using SS;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
    Spreadsheet spreadsheet;
    History history;



    /// <summary>
    /// Constructor for the demo
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        // This an example of registering a method so that it is notified when
        // an event happens.  The SelectionChanged event is declared with a
        // delegate that specifies that all methods that register with it must
        // take a SpreadsheetGrid as its parameter and return nothing.  So we
        // register the displaySelection method below.
        //currentCellValue.Text= $"\"\"";
        spreadsheet = new(x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(), "ps6");
        history = new();
        currentCellContents.Completed += CurrentCellContents_Completed;
        spreadsheetGrid.SelectionChanged += displaySelection;
        spreadsheetGrid.SetSelection(0, 0);
        ForwardButton.IsEnabled = false;
        BackButton.IsEnabled = false;
        displaySelection(spreadsheetGrid);
        

    }
    /// <summary>
    /// Whenever the spreadsheetGrid detects a selection has been changed, run this event using the spreadsheetGrid that notified it. 
    /// </summary>
    /// <param name="grid">SpreadsheetGrid that called this event</param>
    private void displaySelection(ISpreadsheetGrid grid)
    {
        spreadsheetGrid.GetSelection(out int col, out int row);

        spreadsheetGrid.GetValue(col, row, out string value);
        if (value.Equals(""))
            currentCellValue.Text = "\"\"";
        else
            currentCellValue.Text = value;

        currentCellName.Text = CordsToString(col, row);
        var content = spreadsheet.GetCellContents(CordsToString(col, row));
        currentCellContents.Text = content is Formula ? "=" + content : content.ToString();
        currentCellContents.Focus();
       
    }

    /// <summary>
    /// When the user presses enter in the CellContent Entry, both the backing spreadsheet and the spreadsheetGrid GUI object are updated.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CurrentCellContents_Completed(object sender, EventArgs e)
    {
        IList<String> cellsToUpdate;
        object oldContentsObject = spreadsheet.GetCellContents(currentCellName.Text);
        string oldContent = oldContentsObject is Formula ? $"={oldContentsObject}" : oldContentsObject.ToString();
        try
        {

            cellsToUpdate = spreadsheet.SetContentsOfCell(currentCellName.Text, currentCellContents.Text);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error:",$"Threw exception {ex.Message}", "Undo");
            currentCellContents.Text = oldContent;
            return;
        }

        history.addCell(currentCellName.Text, oldContent, currentCellContents.Text);
        currentCellValue.Text = spreadsheet.GetCellValue(cellsToUpdate[0]).ToString();

        BackButton.IsEnabled = history.canGoBack();
        ForwardButton.IsEnabled = history.canGoForward();
        foreach (var s in cellsToUpdate)
        {
            int col = s[0] - 65;
            int row = int.Parse(s[1..]) -1;

            var p = spreadsheet.GetCellValue(s).ToString();
            if(p.Length > 10) 
            {
                p = p.Substring(0,8) + "...";
            }
            spreadsheetGrid.SetValue(col, row, p);
        }
    }

        /// <summary>
        /// Creates a new empty spreadsheet when clicked. If any unsaved content is in the current sheet, 
        /// gives the user a pop up menu asking if they want to proceed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewClicked(Object sender, EventArgs e)
        {
            if (spreadsheet.Changed)
            {
                var b = DisplayAlert("Warning:", "Action will erase previous spreadsheet data", "Accept", "Cancel");
                //Wait for the user to make a decision.
                await b;
                if (b.Result)
                { 
                    spreadsheetGrid.Clear();
                    currentCellContents.Text = "";

                    //Effectively clears the spreadsheet and all of its cells using the same validator on intial construction
                    spreadsheet = new(x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(), "ps6");
                }
            }
        //If there has not been a change to the spreadsheet (Loaded or otherwise) just clear automatically. 
        else 
            spreadsheetGrid.Clear();
            currentCellContents.Text = "";
            //Effectively clears the spreadsheet and all of its cells using the same validator on intial construction
            spreadsheet = new(x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(), "ps6");
    }
    /// <summary>
    /// Saves the current spreadsheet file as a JSON text file at the location given by the user in the prompt. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
     private async void SaveClickedAsync(object sender, EventArgs e)
     {
        //Displays a text prompt for the user to put in a file path to save the given spreadsheet to.
        string path = await DisplayPromptAsync("Saving", "Enter Full File Save Path (Example: C:/SavedSheets/saveName.sprd)");

        //If there is no input or action is canceled, don't display popup window. 
        if (path is null)
            return; 
        //Interesting method call. Might be useful to let the user only put in part of a directory. 
        //path = Path.GetFullPath(path);
        try
        {
            spreadsheet.Save(Path.GetFullPath(path));
        }
        catch
        {
            //Lets the user know there was a problem when attempting to save to the given filepath.
            await DisplayAlert("Error:", "There was problem saving the given file.", "Okay");
        }

     }

     

    /// <summary>
    /// Opens any file as text and prints its contents.
    /// Note the use of async and await, concepts we will learn more about
    /// later this semester.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {
       var tempsheet = new Spreadsheet();
        if (spreadsheet.Changed)
        {
            //Gives a pop up notifing the user of a potentially unsafe action
            var b = DisplayAlert("Warning:", "Action will erase previous spreadsheet data", "Accept", "Cancel");
            await b;
            //If the user cancels the warning, return immediately, ottherwise attempt to open the file.
            if (!b.Result)
                return;
        }    
            try
            {
                FileResult fileResult = await FilePicker.Default.PickAsync();
                if (fileResult != null)
                {
                    //creates a new backing spreadsheet from the given file and uses the same validator and normalizer decided on for this
                    //SpreashseetGUI application 
                    tempsheet = spreadsheet;
                    spreadsheet = new(fileResult.FullPath, x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(),"ps6");
                    spreadsheetGrid.Clear();
                    //iterate through every cell that has content, and update the visual value in spreadsheetGrid. 
                    foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                        spreadsheetGrid.SetValue(StringToCoords(cell).Item1, StringToCoords(cell).Item2, spreadsheet.GetCellValue(cell).ToString());
                }
                else
                {
                    await DisplayAlert("Failure:", "No file selected.", "Okay");
                }
            }
            catch (Exception ex)
            {
            await DisplayAlert("Error:", $"There was problem opening the given file. {ex.Message}" , "Okay");
            spreadsheet = tempsheet;
            spreadsheetGrid.Clear();
            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                spreadsheetGrid.SetValue(StringToCoords(cell).Item1, StringToCoords(cell).Item2, spreadsheet.GetCellValue(cell).ToString());
            }
    }

    private void EditingSpreadheetHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("How To Use The Spreadsheet:", 
            "1. To change the value of a cell, click on it and then input the desired content into the text box on the top right of the window. \n" +
            "2. Once you have the desired text in the box, press enter to update the values in the spreadsheet. \n" +
            "3. The current cell name will be in the top left box, and the current cell value will be in the middle box. \n" +
            "4. In order to enter formulas into the sheet, they must start with an equal sign, otherwise it will be taken as a string literal. EX:(=A1+5) ", "Okay");
    }

    private void FileHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("How To Save, Load, Or Create New Spreadsheet:",
            "1. When \"New\" is clicked in the File Menu, a dialogue box asking if you want to erase unsaved work will pop up. If you want to back out, press \"Cancel\" and a new sheet will not be created. " +
            "If a new sheet is desired press \"Accept\" and unsaved work will be deleted and a new sheet will be displayed. \n" +
            "2. When \"Open\" is clicked in the File Menu, a dialogue box asking about deleting unsaved work, similar to \"New\", will appear. Following that action, if \"Accept\" is " +
            "clicked, a File Selector window will open where the user will select a file. If the file does not end with .sprd the load will fail. Opened files should be in proper JSON format as listed in " +
            "the API for spreadsheet.cs . All values in both the backing spreadsheet and visually on the SpreadsheetGrid will be updated from the opened file.  \n" +
            "3.  When \"Save\" is clicked in the File Menu, a dialogue text box will pop up asking you to input the file path where the current spreadsheet should be saved. Once a file has been saved" +
            "using this button, clicking this button again will automatically save the sheet to the same location.", "Okay");
    }

    private void UndoRedoHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("How to Undo and Redo Actions:",
            "1. When \"Undo\" is clicked in the Edit Menu, the last most recently altered cell will revert to its previous content." +
            " *Note: On Mac, Menu Items can not be disabled, so clicking Undo when no actions are remaining will throw an error.*  \n" +
            "2. When \"Redo\" is clicked in the Edit Menu, it reverses the most recent Undone action. " +
            "*Note: On Mac, Menu Items can not be disabled, so clicking Redo when no actions have been Undone will throw an error.* \n" +
            "3. Finally, as a simple Special Feature, we added the functionality where if there are more than 10 characters in a cell, the spreadsheet will visually truncate the values so they fit inside the cell." +
            "", "Okay");
    }


    private void UndoClicked(object sender, EventArgs e)
    {
        var oldData = history.Back();
        spreadsheet.SetContentsOfCell(oldData.Item1, oldData.Item2);
        var coords = StringToCoords(oldData.Item1);
        spreadsheetGrid.SetValue(coords.Item1, coords.Item2, oldData.Item2);
        ForwardButton.IsEnabled = history.canGoForward();
        BackButton.IsEnabled = history.canGoBack(); 
    }

    private void RedoClicked(object sender, EventArgs e)
    {
        var newData = history.Forward();
        spreadsheet.SetContentsOfCell(newData.Item1, newData.Item2);
        var coords = StringToCoords(newData.Item1);
        spreadsheetGrid.SetValue(coords.Item1, coords.Item2, newData.Item2);
        ForwardButton.IsEnabled = history.canGoForward();
        BackButton.IsEnabled = history.canGoBack();
    }

    /// <summary>
    /// Takes two ints and returns the desired Cell name for this spreadsheet (0,0) -> (A1) or (1,4) -> (B5)
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private static string CordsToString(int col, int row)
    {
        return $"{(char)(col + 65)}{row + 1}";
    }

    /// <summary>
    /// Takes in a cell name  and converts it to desired zero based coordinates. (A1) -> (0,0) or (B5) -> (1,4)
    /// </summary>
    /// <param name="cellName"></param>
    /// <returns></returns>
    private static (int, int) StringToCoords(string cellName)
    {
        //int coord2 = 0;
        //for (int i = 0; i < cellName.Length; i++)
        //{
        //    if (Char.IsDigit(cellName[i]))
        //    {
        //        cellName = cellName.Remove(i);
        //        coord2 = cellName[i];
        //    }
        //}
        
        int columnIndex = cellName[0] - 'A';
        int rowIndex = int.Parse(cellName[1..]) - 1; 
        return (columnIndex, rowIndex);
    }
}
