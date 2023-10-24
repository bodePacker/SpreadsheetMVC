using SS;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;


namespace SpreadsheetGUI;

/// <summary>
/// This Class prespresents the main page for the Spreadsheet application. This
/// Page contains 3 boxes at the top which contian the current cell name, the
/// current cell value, and the current cell contents. Below these boxes are a
/// SpreadSheetGrid object which draws the cells of the spreadsheet. This page
/// also has menu items for opening and saving spreadsheets. It also contains menu
/// items to help the user understand the diffrent aspects of the app. For speacial
/// features we added undo and redo functionality along with a menu item to change
/// the fonts of the top 3 boxes. 
/// </summary>
public partial class MainPage : ContentPage
{
    //Used to store the data of the spreadsheet
    Spreadsheet spreadsheet;

    //Used to keep track of previous actions
    History history;

    //Used to keep track of saving functionalty
    private bool beenSaved;
    private List<string> pathsToSaveTo;


    /// <summary>
    /// This method creates the Main page building the nessecary instance varaibles,
    /// and adding event listeners. 
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        //Setting default values for fields
        spreadsheet = new(x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(), "ps6");
        history = new();
        ForwardButton.IsEnabled = false;
        BackButton.IsEnabled = false;
        beenSaved = false;
        pathsToSaveTo = new();

        //Adding action listeners
        currentCellContents.Completed += CurrentCellContents_Completed;
        spreadsheetGrid.SelectionChanged += DisplaySelection;

        //seting the selection to a1
        spreadsheetGrid.SetSelection(0, 0);

        //Sets the values of the 3 boxes 
        DisplaySelection(spreadsheetGrid);

    }

    /// <summary>
    /// Whenever the spreadsheetGrid detects a selection has been changed, run this event using the spreadsheetGrid that notified it. 
    /// </summary>
    /// <param name="grid">SpreadsheetGrid that called this event</param>
    private void DisplaySelection(ISpreadsheetGrid grid)
    {
        spreadsheetGrid.GetSelection(out int col, out int row);

        object value = spreadsheet.GetCellValue(CoordsToString(col, row));
        var stringValue = value is Formula ? $"={value}" : value.ToString();
        if (value.Equals(""))
            currentCellValue.Text = "\"\"";
        else
            currentCellValue.Text = stringValue;

        currentCellName.Text = CoordsToString(col, row);
        var content = spreadsheet.GetCellContents(CoordsToString(col, row));
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

        //storing old contents for history or in case of a exception
        object oldContentsObject = spreadsheet.GetCellContents(currentCellName.Text);
        string oldContent = oldContentsObject is Formula ? $"={oldContentsObject}" : oldContentsObject.ToString();
        try
        {
            cellsToUpdate = spreadsheet.SetContentsOfCell(currentCellName.Text, currentCellContents.Text);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error:", $"Threw exception {ex.Message}", "Undo");
            currentCellContents.Text = oldContent;
            return;
        }

        history.addCell(currentCellName.Text, oldContent, currentCellContents.Text);
        currentCellValue.Text = spreadsheet.GetCellValue(cellsToUpdate[0]).ToString();

        BackButton.IsEnabled = history.CanGoBack();
        ForwardButton.IsEnabled = history.canGoForward();
        foreach (var adress in cellsToUpdate)
        {
            UpdateGrid(adress);
        }
    }

    /// <summary>
    /// This method will update the value of the spreadsheetGrid object at the
    /// specified address to equal the value determined by the backing spreadsheet
    /// object
    /// </summary>
    /// <param name="adress"></param>
    private void UpdateGrid(string adress)
    {
        var coords = StringToCoords(adress);

        var cellValue = spreadsheet.GetCellValue(adress).ToString();

        //shortening string to avoid overflow
        if (cellValue.Length > 10)
        {
            cellValue = cellValue.Substring(0, 8) + "...";
        }

        spreadsheetGrid.SetValue(coords.Item1, coords.Item2, cellValue);
    }

    /// <summary>
    /// Creates a new empty spreadsheet when clicked. If any unsaved content is in the current sheet, 
    /// gives the user a pop up menu asking if they want to proceed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void NewClicked(Object sender, EventArgs e)
    {
        if(spreadsheet.Changed)
        {
            var b = DisplayAlert("Warning:", "Action will erase previous spreadsheet data", "Accept", "Cancel");
            //Wait for the user to make a decision.
            await b;

            //if they cancel do nothing to the current spreadsheet
            if (!b.Result)
                return;
        }

        spreadsheetGrid.Clear();
        spreadsheetGrid.SetSelection(0, 0);
        currentCellContents.Text = "";
        history.Clear();
        beenSaved = false;
        pathsToSaveTo.Clear();
        spreadsheet = new(x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(), "ps6");
        spreadsheetGrid.SetSelection(0, 0);
        DisplaySelection(spreadsheetGrid);
    }

    /// <summary>
    /// Saves the current spreadsheet file as a JSON text file at the location given by the user in the prompt. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveClickedAsync(object sender, EventArgs e)
    {
        if (!beenSaved)
        {
            SaveAsClickedAsync(sender, e);
            return;
        }

        foreach (var path in pathsToSaveTo)
        {
            try
            {
                spreadsheet.Save(path);
            }
            catch
            {
                //Lets the user know there was a problem when attempting to save to the given filepath.
                await DisplayAlert("Error:", "There was problem saving the given file.", "Okay");

            }
        }
    }
    /// <summary>
    /// Saves the current spreadsheet file as a JSON text file at the location given by the user in the prompt. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveAsClickedAsync(object sender, EventArgs e)
    {
        //Displays a text prompt for the user to put in a file path to save the given spreadsheet to.
        string path = await DisplayPromptAsync("Saving", "Enter Full File Save Path (Example: C:/SavedSheets/saveName.sprd)");

        //If there is no input or action is canceled, don't display popup window. 
        if (path is null)
            return;

        //Interesting method call. Might be useful to let the user only put in part of a directory. 
        path = Path.GetFullPath(path);
        try
        {
            spreadsheet.Save(path);
        }
        catch
        {
            //Lets the user know there was a problem when attempting to save to the given filepath.
            await DisplayAlert("Error:", "There was problem saving the given file.", "Okay");
            return;
        }
        pathsToSaveTo.Add(path);
        beenSaved = true;

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
                tempsheet = spreadsheet;
                spreadsheet = new(fileResult.FullPath, x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(), "ps6");
                spreadsheetGrid.Clear();
                history.Clear();
                beenSaved = true;
                pathsToSaveTo.Clear();
                pathsToSaveTo.Add(fileResult.FullPath);
                //iterate through every cell that has content, and update the visual value in spreadsheetGrid. 
                foreach (string cellAddress in spreadsheet.GetNamesOfAllNonemptyCells())
                    UpdateGrid(cellAddress);

                spreadsheetGrid.SetSelection(0, 0);
                DisplaySelection(spreadsheetGrid);
            }
            else
            {
                await DisplayAlert("Failure:", "No file selected.", "Okay");
            }
        }
        catch (Exception ex)
        {
            //If an exception is thrown tell the user and restore the old spreadsheet
            await DisplayAlert("Error:", $"There was problem opening the given file. {ex.Message}", "Okay");
            spreadsheet = tempsheet;
            spreadsheetGrid.Clear();
            foreach (string cellAddress in spreadsheet.GetNamesOfAllNonemptyCells())
                UpdateGrid(cellAddress);
        }
    }


    /// <summary>
    /// Shows helpfull info about the editing functionality of the spreadsheet
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditingSpreadheetHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("How To Use The Spreadsheet:",
            "1. To change the value of a cell, click on it and then input the desired content into the text box on the top right of the window. \n" +
            "2. Once you have the desired text in the box, press enter to update the values in the spreadsheet. \n" +
            "3. The current cell name will be in the top left box, and the current cell value will be in the middle box. \n" +
            "4. In order to enter formulas into the sheet, they must start with an equal sign, otherwise it will be taken as a string literal. EX:(=A1+5) ", "Okay");
    }

    /// <summary>
    /// Shows helpfull info about the File read/write functionality of the spreadsheet
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Shows helpfull info about the und/redo speaceal feture
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        (string, string) oldData;
        try
        {
            oldData = history.Back(); //Done to avoid a mac bug from crahsing program
        }
        catch (InvalidOperationException)
        {
            DisplayAlert("Error:", "There was going back", "Okay");
            return;
        }

        //Update the spreadsheets with the undo data
        spreadsheet.SetContentsOfCell(oldData.Item1, oldData.Item2);
        UpdateGrid(oldData.Item1);

        //Disable / Reable buttons
        ForwardButton.IsEnabled = history.canGoForward();
        BackButton.IsEnabled = history.CanGoBack();
    }

    private void RedoClicked(object sender, EventArgs e)
    {
        (string, string) newData;
        try
        { 
            newData = history.Forward(); //Done to avoid a mac bug from crahsing program
        }
        catch (InvalidOperationException)
        {
            DisplayAlert("Error:", "There was going forward", "Okay");
            return;
        }

        //Update the spreadsheets with the redo data
        spreadsheet.SetContentsOfCell(newData.Item1, newData.Item2);
        UpdateGrid(newData.Item1);

        //Disable / Reable buttons
        ForwardButton.IsEnabled = history.canGoForward();
        BackButton.IsEnabled = history.CanGoBack();
    }

    /// <summary>
    /// Changes the theme to Default (only affects text in the top 3 boxes)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DefaultThemeClicked(object sender, EventArgs e)
    {
        currentCellName.FontFamily = "Lobster-Regular";
        currentCellValue.FontFamily = "Lobster-Regular";
        currentCellContents.FontFamily = "Lobster-Regular";
    }

    /// <summary>
    /// Changes the theme to Modern (only affects text in the top 3 boxes)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ModernThemeClicked(object sender, EventArgs e)
    {
        currentCellName.FontFamily = "Futuren0tFoundRegular";
        currentCellValue.FontFamily = "Futuren0tFoundRegular";
        currentCellContents.FontFamily = "Futuren0tFoundRegular";
    }

    /// <summary>
    /// Changes the theme to OldEnglish (only affects text in the top 3 boxes)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewEnglishThemeClicked(object sender, EventArgs e)
    {
        currentCellName.FontFamily = "Canterbury";
        currentCellValue.FontFamily = "Canterbury";
        currentCellContents.FontFamily = "Canterbury";
    }

    /// <summary>
    /// Changes the theme to Spooky (only affects text in the top 3 boxes)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SpookyThemeClicked(object sender, EventArgs e)
    {
        currentCellName.FontFamily = "ScaryHalloweenFont";

        currentCellValue.FontFamily = "ScaryHalloweenFont";

        currentCellContents.FontFamily = "ScaryHalloweenFont";
        currentCellContents.FontSize = 25;
    }


    /// <summary>
    /// Takes two ints and returns the desired Cell name for this spreadsheet (0,0) -> (A1) or (1,4) -> (B5)
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private static string CoordsToString(int col, int row)
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
        int columnIndex = cellName[0] - 'A';
        int rowIndex = int.Parse(cellName[1..]) - 1;
        return (columnIndex, rowIndex);
    }
}
