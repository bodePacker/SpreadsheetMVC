using SS;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Net.Security;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
    Spreadsheet spreadsheet;

    

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
        spreadsheetGrid.SelectionChanged += displaySelection;
        spreadsheetGrid.SetSelection(0, 0);
        displaySelection(spreadsheetGrid);
        
    }

    private void displaySelection(ISpreadsheetGrid grid)
    {
        spreadsheetGrid.GetSelection(out int col, out int row);
        spreadsheetGrid.GetValue(col, row, out string value);
        if(value.Equals(""))
            currentCellValue.Text = "\"\""; 
        else
            currentCellValue.Text = value;

        currentCellName.Text = CordsToString(col, row);
        var content = spreadsheet.GetCellContents(CordsToString(col, row));
        currentCellContents.Text = content is Formula ? "=" + content : content.ToString();
        currentCellContents.Completed += CurrentCellContents_Completed;
        currentCellContents.Focus();
        //if (currentCellContents.Focus())
        //{
        //    currentCellName.Text = "hello";
        //}
        //if (value == "")
        //{
        //    spreadsheetGrid.SetValue(col, row, DateTime.Now.ToLocalTime().ToString("T"));
        //    spreadsheetGrid.GetValue(col, row, out value);
        //    DisplayAlert("Selection:", "column " + col + " row " + row + " value " + value, "OK");
        //}
    }

    private void CurrentCellContents_Completed(object sender, EventArgs e)
    {
        IList<String> cellsToUpdate;
        try
        {
            cellsToUpdate = spreadsheet.SetContentsOfCell(currentCellName.Text, currentCellContents.Text);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error:",$"Threw exception {ex.Message}", "Undo");
            //currentCellContents.Text = spreadsheet.GetCellContents(currentCellName.Text).ToString();
            var content = spreadsheet.GetCellContents(currentCellName.Text);
            currentCellContents.Text = content is Formula ? "=" + content : content.ToString();
            return;
        }
        currentCellValue.Text = spreadsheet.GetCellValue(cellsToUpdate[0]).ToString();

        foreach(var s in cellsToUpdate)
        {
            int col = s[0] - 65;
            int row = int.Parse(s[1..]) -1;
            spreadsheetGrid.SetValue(col, row, spreadsheet.GetCellValue(s).ToString());
        }
    }

    private async void NewClicked(Object sender, EventArgs e)
    {
        if (spreadsheet.Changed)
        {
            var b = DisplayAlert("Warning:", "Action will erase previous spreadsheet data", "Accept", "Cancel");
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
            spreadsheet.Save(path);
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
                    //Console.WriteLine("Successfully chose file: " + fileResult.FileName);
                    // for windows, replace Console.WriteLine statements with:
                    //System.Diagnostics.Debug.WriteLine( ... );

                    //creates a new backing spreadsheet from the given file and uses the same validator and normalizer decided on for this
                    //SpreashseetGUI application 
                    spreadsheet = new(fileResult.FullPath, x => Regex.IsMatch(x, "^[A-Z][1-9][0-9]|[A-Z][1-9]$"), x => x.ToUpper(),"ps6");
                    
                    //iterate through every cell that has content, and update the visual value in spreadsheetGrid. 
                    foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                        spreadsheetGrid.SetValue(StringToCoords(cell).Item1, StringToCoords(cell).Item2, spreadsheet.GetCellValue(cell).ToString());

                    //string fileContents = File.ReadAllText(fileResult.FullPath);
                    //Console.WriteLine("First 100 file chars:\n" + fileContents.Substring(0, 100));
                }
                else
                {
                    //Console.WriteLine("No file selected.");
                    await DisplayAlert("Failure:", "No file selected.", "Okay");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error:", $"There was problem opening the given file. {ex.Message}" , "Okay");

                //Console.WriteLine("Error opening file:");
                //Console.WriteLine(ex);
            }
            
        
    }

    private void HelpClicked(object sender, EventArgs e)
     {

     }

    private void UndoClicked(object sender, EventArgs e)
    {

    }

    private void RedoClicked(object sender, EventArgs e)
    {

    }

    private static string CordsToString(int col, int row)
    {
        return $"{(char)(col + 65)}{row + 1}";
    }

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
        int rowIndex = int.Parse(cellName[1..]) - 1; ;
        return (columnIndex, rowIndex);
    }

    
}
