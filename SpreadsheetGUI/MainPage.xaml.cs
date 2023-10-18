using SS;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
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
        currentCellValue.Text = value;
        currentCellName.Text = cordsToString(col, row);
        var content = spreadsheet.GetCellContents(cordsToString(col, row));
        currentCellContents.Text = content is Formula ? "=" + content : content.ToString();
        currentCellContents.Completed += CurrentCellContents_Completed;
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
            currentCellContents.Text = spreadsheet.GetCellContents(currentCellName.Text).ToString();
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

    private void NewClicked(Object sender, EventArgs e)
    {
        spreadsheetGrid.Clear();
    }

    /// <summary>
    /// Opens any file as text and prints its contents.
    /// Note the use of async and await, concepts we will learn more about
    /// later this semester.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {
        try
        {
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
        Console.WriteLine( "Successfully chose file: " + fileResult.FileName );
        // for windows, replace Console.WriteLine statements with:
        //System.Diagnostics.Debug.WriteLine( ... );

        string fileContents = File.ReadAllText(fileResult.FullPath);
                Console.WriteLine("First 100 file chars:\n" + fileContents.Substring(0, 100));
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error opening file:");
            Console.WriteLine(ex);
        }
    }

    private string cordsToString(int col, int row)
    {
        return $"{(char)(col + 65)}{row + 1}";
    }


    
}
