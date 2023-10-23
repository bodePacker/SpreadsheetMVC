
namespace SpreadsheetGUI;

/// <summary>
/// This class stores the previous actions of the spreadsheet in order to add undo
/// and redo functionality to the spreadsheet
/// </summary>
public class History
{
	//used to store previous and undone acitons
	private Stack<(string, string, string)> backwards;
	private Stack<(string, string, string)> forwards;

	/// <summary>
	/// Builds a new history object
	/// </summary>
    public History()
	{
		backwards = new();
		forwards = new();
	}

	/// <summary>
	/// This methods adds a cell to the history of actions. This should be called
	/// whenever a user updates a cells contents
	/// </summary>
	/// <param name="address">The address of the cell updates</param>
	/// <param name="oldContents">The contents that where previously in the cell</param>
	/// <param name="newContents">The contents just added to the cell</param>
	public void addCell(string address, string oldContents, string newContents)
	{
		backwards.Push((address, oldContents, newContents));
		forwards.Clear();
	}

    /// <summary>
    /// Tells you if there is an action to be undone
    /// </summary>
    /// <returns>True if Back can be called without an exception being thrown</returns>
    public bool CanGoBack()
	{
		return backwards.Count > 0;
	}

    /// <summary>
    /// Tells you if there is an action to be redone
    /// </summary>
    /// <returns>True if Forward can be called without an exception being thrown</returns>
    public bool canGoForward()
    {
        return forwards.Count > 0;
    }

	/// <summary>
	/// Returns which contents to be changed at which cell address in order to undo
	/// the previous action
	/// </summary>
	/// <returns>Item 1: Adress of cell to be updated
	///			 Iten 2: Contents to be updated to</returns>
	/// <exception cref="InvalidOperationException">If their is no aciton to be undone</exception>
	public (string, string) Back()
	{
		if (!CanGoBack())
			throw new InvalidOperationException();
		var prev = backwards.Pop();
		forwards.Push(prev);

		//only need the cell address and the old content
		return (prev.Item1, prev.Item2);
	}

    /// <summary>
    /// Returns which contents to be changed at which cell address in order to redo
    /// the previous undo action
    /// </summary>
    /// <returns>Item 1: Adress of cell to be updated
    ///			 Iten 2: Contents to be updated to</returns>
    /// <exception cref="InvalidOperationException">If their is no aciton to be redone</exception>
    public (string, string) Forward()
    {
        if (!canGoForward())
            throw new InvalidOperationException();
        var prev = forwards.Pop();
        backwards.Push(prev);

        //only need the cell address and the new content
        return (prev.Item1, prev.Item3);
    }

	/// <summary>
	/// Clears the history of the spreadsheet
	/// </summary>
    public void Clear()
    {
        backwards.Clear();
        forwards.Clear();
    }
}

