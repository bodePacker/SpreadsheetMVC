using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SS;

/// <summary>
/// This is the Implementation of the AbstractSpreadsheet abstract class that represents
/// a spreadsheet using a Dictionary to represent the Data stored in the cells of
/// the spreadsheet.
///
/// A string is a valid cell name if and only if:
///   (1) its first character is an underscore or a letter
///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
/// Note that this is the same as the definition of valid variable from the PS3 Formula class.
/// 
/// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
/// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
/// different cell names.
/// 
/// A spreadsheet contains a cell corresponding to every possible cell name.  (This
/// means that a spreadsheet contains an infinite number of cells.)  In addition to 
/// a name, each cell has a contents and a value.  The distinction is important.
/// 
/// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
/// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
/// of a cell in Excel is what is displayed on the editing line when the cell is selected).
/// 
/// In a new spreadsheet, the contents of every cell is the empty string.
/// 
/// Spreadsheets are never allowed to contain a combination of Formulas that establish
/// a circular dependency.  A circular dependency exists when a cell depends on itself.
/// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
/// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
/// dependency.
/// </summary>
/// 
public class Spreadsheet : AbstractSpreadsheet
{
    public Dictionary<string, Cell> Cells { get; }

    private readonly DependencyGraph dependencyGraph;
    private readonly Func<string, bool> validator;
    private readonly Func<string, string> normalizer;

    /// <summary>
    /// Given to the Formula class in order to determine the values variables
    /// </summary>
    /// <param name="name">The name of the cell</param>
    /// <returns>The value of the cell at the position name</returns>
    /// <exception cref="ArgumentException">Thrown if the value of the cell in not a number</exception>
    private double Lookup(string name)
    { 
        object val = GetCellValue(name);
        if (val is double d)
            return d;

        throw new ArgumentException($"The value of the cell {name} is not a number");
    }
    
    /// <summary>
    /// Builds an empty Spreadsheet object 
    /// </summary>
    public Spreadsheet() : base("default")
    {
        Changed = false;
        dependencyGraph = new();
        Cells = new Dictionary<string, Cell>();
        validator = s => true;
        normalizer = s => s;
    }

    /// <summary>
    /// Builds a spreadsheet with the given cellDictionary and the version string. Note This method does not
    /// guarantee that the built spreadsheet will be valid and should only be used By the JsonSerializer object.
    /// </summary>
    /// <param name="cells">The dictionary specifying which cell addresses map to what cell objects</param>
    /// <param name="version">the version of the spreadsheet </param>
    [JsonConstructor]
    public Spreadsheet(Dictionary<string, Cell> cells, string version) : base(version)
    {
        Changed = false;
        dependencyGraph = new DependencyGraph();
        this.Cells = cells;
        validator = s => true;
        normalizer = s => s;
    }

    /// <summary>
    /// Builds a spreadsheet emptry spreadsheet with the given validator and normalizer
    /// </summary>
    /// <param name="validator"> used to validate variable names</param>
    /// <param name="normalizer"></param>
    /// <param name="version"></param>
    public Spreadsheet(Func<string, bool> validator, Func<string, string> normalizer, string version) : base(version)
    {
        Changed = false;
        dependencyGraph = new DependencyGraph();
        Cells = new Dictionary<string, Cell>();
        this.validator = validator;
        this.normalizer = normalizer;
    }
    
    public Spreadsheet(string path, Func<string, bool> validator, Func<string, string> normalizer, string version) : base(version)
    {
        Changed = false;
        dependencyGraph = new DependencyGraph();
        Cells = new Dictionary<string, Cell>();
        this.validator = validator;
        this.normalizer = normalizer;

        Spreadsheet? oldSpreadsheet;
        try
        { 
            oldSpreadsheet = JsonSerializer.Deserialize<Spreadsheet>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            throw DetermineErrorMessage(e);
        }
        if (oldSpreadsheet == null)
            throw new SpreadsheetReadWriteException($"File Path: {path} could not be converted From JSON to a Spreadsheet object");
        if (oldSpreadsheet.Version != Version)
            throw new SpreadsheetReadWriteException(
                $"The version given: {Version}, did not match the version of the spreadsheet at the given path: {oldSpreadsheet.Version}");
        
        foreach (var kvPair in oldSpreadsheet.Cells)
        {
            string? s = kvPair.Value.StringForm;
            if (s == null)
                throw new SpreadsheetReadWriteException("Spreadsheet contained a null value as the contents of a cell");
            try
            {
                SetContentsOfCell(kvPair.Key, s);
            }
            catch (Exception e)
            {
                throw DetermineErrorMessage(e);
            }

            Changed = false;
        }
    }

    /// <summary>
    /// Helper method used to throw SpreadsheetReadWriteException's while creating a spreadsheet from a JSON file
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    private SpreadsheetReadWriteException DetermineErrorMessage(Exception exception)
    {
        if (exception is InvalidNameException)
            return new SpreadsheetReadWriteException("A Name of a cell in the Spreadsheet at the provided path was not valid and a" +
                                                    $"InvalidNameException was throw with the following message. \n {exception.Message}");
        if (exception is CircularException)
        {
            return new SpreadsheetReadWriteException("The Spreadsheet provided contained a circular dependency and a" +
                                                    $" CircularException was throw with the following message. \n {exception.Message}");
        }
        
        if (exception is FormulaFormatException)
        {
            return new SpreadsheetReadWriteException("The Spreadsheet provided contained a invalid formula and a" +
                                                    $"FormulaFormatException was throw with the following messages \n {exception.Message}");
        }

        return new SpreadsheetReadWriteException(
            $"An {exception.GetType()} was thrown while creating the Spreadsheet at the provided" +
            $"path, with the following message. \n {exception.Message}");
    }

    /// <summary>
    /// Writes the contents of this spreadsheet to the named file using a JSON format.
    /// The JSON object should have the following fields:
    /// "Version" - the version of the spreadsheet software (a string)
    /// "Cells" - a data structure containing 0 or more cell entries
    ///           Each cell entry has a field (or key) named after the cell itself 
    ///           The value of that field is another object representing the cell's contents
    ///               The contents object has a single field called "StringForm",
    ///               representing the string form of the cell's contents
    ///               - If the contents is a string, the value of StringForm is that string
    ///               - If the contents is a double d, the value of StringForm is d.ToString()
    ///               - If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    /// 
    /// For example, if this spreadsheet has a version of "default" 
    /// and contains a cell "A1" with contents being the double 5.0 
    /// and a cell "B3" with contents being the Formula("A1+2"), 
    /// a JSON string produced by this method would be:
    /// 
    /// {
    ///   "Cells": {
    ///     "A1": {
    ///       "StringForm": "5"
    ///     },
    ///     "B3": {
    ///       "StringForm": "=A1+2"
    ///     }
    ///   },
    ///   "Version": "default"
    /// }
    /// 
    /// If there are any problems opening, writing, or closing the file, the method should throw a
    /// SpreadsheetReadWriteException with an explanatory message.
    /// </summary>
    public override void Save(string filename)
    {
        try
        {
            File.WriteAllText(filename, JsonSerializer.Serialize(this));
        }
        catch (Exception e)
        {
            throw new SpreadsheetReadWriteException($"The Exception: {e.GetType()} was thrown while trying to save" + 
                                                    $"to the Path: {filename}\nBelow is the error message \n{e.Message}");
        }
        
        Changed = false;
    }

    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
    /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
    /// </summary>
    public override object GetCellValue(string name)
    {
        if (!IsValidName(name))
        {
            throw new InvalidNameException(); 
        }
        return Cells.TryGetValue(normalizer(name), out var cell) ? cell.Value : "";
    }

    /// <summary>
    /// Enumerates the names of all the non-empty cells in the spreadsheet.
    /// </summary>
    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        return Cells.Keys;
    }

    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, returns the contents (as opposed to the value) of the named cell.
    /// The return value should be either a string, a double, or a Formula.
    /// </summary>
    public override object GetCellContents(string name)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();
        name = normalizer(name);
        
        return Cells.TryGetValue(name, out Cell? value) ? value.Content : "";
    }

    public override IList<string> SetContentsOfCell(string name, string content)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();
        name = normalizer(name);

        IList<string> returnList;
        if (double.TryParse(content, out var result))
        {
            returnList = SetCellContents(name, result);
        }
        else if (content.Length > 0 && content[0] == '=')
        {
            Formula f = new Formula(content[1..], normalizer, validator);
            returnList = SetCellContents(name, f);
        }
        else
        {
            returnList = SetCellContents(name, content);
        }
        
        Changed = true;
        UpdateValues(returnList);
        return returnList;
    }


    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, the contents of the named cell becomes number.  The method returns a
    /// list consisting of name plus the names of all other cells whose value depends, 
    /// directly or indirectly, on the named cell.
    /// 
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </summary>
    protected override IList<string> SetCellContents(string name, double number)
    {
        //remove all dependees from the cell
        dependencyGraph.ReplaceDependees(name, Array.Empty<string>());

        Cells[name] = new Cell(number, number);
        return new List<string>(GetCellsToRecalculate(name));
    }


    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, the contents of the named cell becomes text.  The method returns a
    /// list consisting of name plus the names of all other cells whose value depends, 
    /// directly or indirectly, on the named cell.
    /// 
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </summary>
    protected override IList<string> SetCellContents(string name, string text)
    {
        //remove all dependees from the cell
        dependencyGraph.ReplaceDependees(name, Array.Empty<string>());

        //if the text is the empty string remove it from the hashtable
        if (text != "")
            Cells[name] = new Cell(text, text);
        else
            Cells.Remove(name);
        return new List<string>(GetCellsToRecalculate(name));
    }


    /// <summary>
    /// If name is invalid, throws an InvalidNameException.
    /// 
    /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
    /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
    /// 
    /// Otherwise, the contents of the named cell becomes formula.  The method returns a
    /// list consisting of name plus the names of all other cells whose value depends,
    /// directly or indirectly, on the named cell.
    /// 
    /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    /// list {A1, B1, C1} is returned.
    /// </summary>
    protected override IList<string> SetCellContents(string name, Formula formula)
    {
        //keeping old dependees in case of error
        IEnumerable<string> oldDependees = dependencyGraph.GetDependees(name);

        //updating the dependency graph
        dependencyGraph.ReplaceDependees(name, formula.GetVariables());

        //checking for cycles
        IList<string> ret;
        try
        {
            ret = new List<string>(GetCellsToRecalculate(name));
        }
        catch (CircularException ex)
        {
            //if cycle is created, undo the effect on the dependency graph
            dependencyGraph.ReplaceDependees(name, oldDependees);
            throw ex;
        }
        //updating hashtable
        Cells[name] = new Cell(formula, formula.Evaluate(Lookup));

        return ret;
    }

    /// <summary>
    /// Returns an enumeration, without duplicates, of the names of all cells whose
    /// values depend directly on the value of the named cell.  In other words, returns
    /// an enumeration, without duplicates, of the names of all cells that contain
    /// formulas containing name.
    /// 
    /// For example, suppose that
    /// A1 contains 3
    /// B1 contains the formula A1 * A1
    /// C1 contains the formula B1 + A1
    /// D1 contains the formula B1 - C1
    /// The direct dependents of A1 are B1 and C1
    /// </summary>
    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        return dependencyGraph.GetDependents(name);
    }
    
    
    /// <summary>
    /// This method will determine if a name is formatted correctly
    /// The correct format is a letter (upper or lowercase)  or underscore
    /// followed by any combination of letter numbers or underscores
    /// </summary>
    /// <param name="t">The string to be checked</param>
    /// <returns>True if it is in the proper format false if not</returns>
    private bool IsValidName(string t)
    {
        return Regex.IsMatch(t, "^[a-zA-Z_][a-zA-Z0-9_]*$") &&
               Regex.IsMatch(normalizer(t), "^[a-zA-Z_][a-zA-Z0-9_]*$") && validator(normalizer(t));
    }
    
    private void UpdateValues(IEnumerable<string> cellsToUpdate)
    {
        bool first = true;
        foreach (var name in cellsToUpdate)
        {
            if (first) //first already updated
            {
                first = false;
                continue;
            }
            var currentCell = Cells[name];
            currentCell.Value = ((Formula)currentCell.Content).Evaluate(Lookup);
        }
    }

    /// <summary>
    /// Represents an immutable cell in the spreadsheet with a string address and
    /// a Content of type object. This content always be either a string, a double,
    /// or a Formula object. 
    /// </summary>
    public class Cell
    {
        public string StringForm { get; }
        [JsonIgnore]
        public object Content { get; }
        
        [JsonIgnore]
        public object Value { get; set; }
        public Cell(object content, object value)
        {
            Content = content;
            Value = value;
            if (content is double d)
                StringForm = d.ToString();
            else if (content is string s)
                StringForm = s;
            else
                StringForm = $"={content}";
        }
        [JsonConstructor]
        public Cell(string stringForm)
        {
            this.StringForm = stringForm;
            Content = stringForm;
            Value = stringForm;
        }
    }


}

