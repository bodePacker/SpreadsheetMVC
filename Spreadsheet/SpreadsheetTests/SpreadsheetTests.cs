
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests;
/// <summary>
/// This class is used to test The Spreadsheet Class. This class includes tests
/// that determine if the public methods are the Spreadsheet class act as they should
///
/// By Timothy Blamires
/// Last Updated 10/2/23
/// 
/// </summary>
[TestClass]
public class SpreadsheetTests
{

    /// <summary>
    /// Used to determine if 2 spreadsheets contain the same contents
    /// </summary>
    /// <param name="s1">the first Spreadsheet object</param>
    /// <param name="s2">the second Spreadsheet object</param>
    /// <returns>true if they are the same, false if they are not</returns>
    private static bool SpreadsheetEquals(Spreadsheet s1, Spreadsheet s2)
    {
        if (s1.Cells.Count != s2.Cells.Count)
            return false;
        foreach (string key in s1.Cells.Keys)
        {
            if (!(s2.Cells.ContainsKey(key) && s1.Cells[key].Content.Equals(s2.Cells[key].Content)))
                return false;
        }

        return true;
    }

    [TestMethod]
    public void SetCellsEmptyDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        var list = s.SetContentsOfCell("a1", 1d.ToString());

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

    }

    [TestMethod]
    public void SetCellsEmptyText()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        var list = s.SetContentsOfCell("a1", "hi");

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

    }

    [TestMethod]
    public void SetCellsEmptyFormula()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        var list = s.SetContentsOfCell("a1", "=1");

        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

    }

    [TestMethod]
    public void GetCellContentsDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "1");

        Assert.AreEqual(1d, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsText()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "hi");

        Assert.AreEqual("hi", s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsFormula()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        var expected = new Formula("1");
        _ = s.SetContentsOfCell("a1", "=1");

        Assert.AreEqual(expected, s.GetCellContents("a1"));
    }


    [TestMethod]
    public void GetCellContentsDoubleAfterSetDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "2");

        var list= s.SetContentsOfCell("a1", "1");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual(1d, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsDoubleAfterSetString()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", ";)");

        var list = s.SetContentsOfCell("a1", "1");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual(1d, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsDoubleAfterSetFormula()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "=2 + A2");

        var list = s.SetContentsOfCell("a1", "1");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual(1d, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsStringAfterSetDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "1");

        var list = s.SetContentsOfCell("a1", "hi");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual("hi", s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsStringAfterSetString()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", ";)");

        var list = s.SetContentsOfCell("a1", "hi");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual("hi", s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsStringAfterSetFormula()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "=2 + A2");

        var list = s.SetContentsOfCell("a1", "hi");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual("hi", s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsFormulaAfterSetDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "1");

        var expected = new Formula("1");
        var list = s.SetContentsOfCell("a1", "=1");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual(expected, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsFormulaAfterSetString()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", ";)");

        var expected = new Formula("1");
        var list = s.SetContentsOfCell("a1", "=1");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual(expected, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void GetCellContentsFormulaAfterSetFormula()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        _ = s.SetContentsOfCell("a1", "=2 + A2");

        var expected = new Formula("1");
        var list = s.SetContentsOfCell("a1", "=1");
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual("a1", list[0]);

        Assert.AreEqual(expected, s.GetCellContents("a1"));
    }

    [TestMethod]
    public void SimpleDependencyGraphAfterSetCellDouble()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=B1 + 1");
        s.SetContentsOfCell("B1", "=C1 + 1");
        s.SetContentsOfCell("C1", "=D1 + 1");
        s.SetContentsOfCell("D1", "=E1 + 1");
        s.SetContentsOfCell("E1", "=F1 + 1");
        var actual = s.SetContentsOfCell("F1", "1");

        Assert.AreEqual(6, actual.Count);
        Assert.AreEqual("F1", actual[0]);
        Assert.AreEqual("E1", actual[1]);
        Assert.AreEqual("D1", actual[2]);
        Assert.AreEqual("C1", actual[3]);
        Assert.AreEqual("B1", actual[4]);
        Assert.AreEqual("A1", actual[5]);
    }

    [TestMethod]
    public void SimpleDependencyGraphAfterSetCellText()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=B1 + 1");
        s.SetContentsOfCell("B1", "=C1 + 1");
        s.SetContentsOfCell("C1", "=D1 + 1");
        s.SetContentsOfCell("D1", "=E1 + 1");
        s.SetContentsOfCell("E1", "=F1 + 1");
        var actual = s.SetContentsOfCell("F1", "hi");

        Assert.AreEqual(6, actual.Count);
        Assert.AreEqual("F1", actual[0]);
        Assert.AreEqual("E1", actual[1]);
        Assert.AreEqual("D1", actual[2]);
        Assert.AreEqual("C1", actual[3]);
        Assert.AreEqual("B1", actual[4]);
        Assert.AreEqual("A1", actual[5]);
    }

    [TestMethod]
    public void SimpleDependencyGraphAfterSetCellFormula()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=B1 + 1");
        s.SetContentsOfCell("B1", "=C1 + 1");
        s.SetContentsOfCell("C1", "=D1 + 1");
        s.SetContentsOfCell("D1", "=E1 + 1");
        s.SetContentsOfCell("E1", "=F1 + 1");
        var actual = s.SetContentsOfCell("F1", "=1");

        Assert.AreEqual(6, actual.Count);
        Assert.AreEqual("F1", actual[0]);
        Assert.AreEqual("E1", actual[1]);
        Assert.AreEqual("D1", actual[2]);
        Assert.AreEqual("C1", actual[3]);
        Assert.AreEqual("B1", actual[4]);
        Assert.AreEqual("A1", actual[5]);
    }

    [TestMethod]
    public void GetNamesOfNonEmptyCellsSmall()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=B1 + 1");
        s.SetContentsOfCell("B1", "=C1 + 1");
        s.SetContentsOfCell("C1", "=D1 + 1");
        s.SetContentsOfCell("D1", "=E1 + 1");
        s.SetContentsOfCell("E1", "=F1 + 1");
        s.SetContentsOfCell("F1", "=1");

        var expected = new List<string>() { "A1", "B1", "C1", "D1", "E1", "F1" };
        var actual = new List<string>(s.GetNamesOfAllNonemptyCells());

        //Ordering does not matter
        expected.Sort();
        actual.Sort();

        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void GetNamesOfNonEmptyCellsAfterSetToEmptyString()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=B1 + 1");
        s.SetContentsOfCell("B1", "=C1 + 1");
        s.SetContentsOfCell("C1", "=D1 + 1");
        s.SetContentsOfCell("D1", "=E1 + 1");
        s.SetContentsOfCell("E1", "=F1 + 1");
        s.SetContentsOfCell("F1", "=1");

        //removing items
        s.SetContentsOfCell("A1", "");
        s.SetContentsOfCell("B1", "");
        s.SetContentsOfCell("C1", "");

        var expected = new List<string>() { "D1", "E1", "F1" };
        var actual = new List<string>(s.GetNamesOfAllNonemptyCells());

        //Ordering does not matter
        expected.Sort();
        actual.Sort();

        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void EmptyCellsReturnEmptyStrings()
    {
        Spreadsheet s = new();
        Assert.AreEqual("", s.GetCellContents("A1"));
        _ = s.SetContentsOfCell("A1", "2");
        Assert.AreEqual("", s.GetCellContents("A2"));

    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsGetContents1()
    {
        Spreadsheet s = new();
        s.GetCellContents("1F");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsGetContents2()
    {
        Spreadsheet s = new();
        s.GetCellContents("#134");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsSetDouble1()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1F", "1");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsSetDouble2()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("#adf", "1");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsSetString1()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1F", ";)");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsSetString2()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("#adf", ";)");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsSetFormula1()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1F", "=1");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void InvalidNameThrowsSetFormula2()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("#adf", "=1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void CircularExeptionThrowsDirect()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("a1", "=a1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void CircularExceptionThrowsSimple()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("a1", "=a2 + 1");
        s.SetContentsOfCell("a2", "=a1 + 1");
    }

    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void CircularExceptionThrowsComplex()
    {
        Spreadsheet s = new();
        for (int i = 1; i <= 100; i++)
            s.SetContentsOfCell($"A{i}", $"=A{i - 1} + 1 /2 - z{i}");
        s.SetContentsOfCell("A0", "=A100 + 95235 /2 -4");
        
    }

    [TestMethod]
    public void DoesNotAddCircularException()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=Z1 + 5");
        s.SetContentsOfCell("A2", "=A1 + 5");


        try
        {
            s.SetContentsOfCell("A1", "=A2 + 5"); //should have no effect
        }
        catch (CircularException) {} //Do nothing

        Assert.AreEqual(new Formula("Z1 + 5"), s.GetCellContents("A1"));
    }

    [TestMethod]
    public void DoesNotAddDoubleCircularException()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=A3 + 5");
        s.SetContentsOfCell("A2", "=A3 + 10");
        s.SetContentsOfCell("A3", "1");


        try
        {
            s.SetContentsOfCell("A3", "=A1 + A2"); //should have no effect
        }
        catch (CircularException) { } //Do nothing

        Assert.AreEqual(1d, s.GetCellContents("A3"));
        Assert.AreEqual(new Formula("A3 + 10"), s.GetCellContents("A2"));
        Assert.AreEqual(new Formula("A3 + 5"), s.GetCellContents("A1"));
    }

    [TestMethod]
    public void StressTestNonEmptyCells()
    {
        Random rng = new(1);
        Spreadsheet s = new();

        ISet<string> expected = new HashSet<string>();
        for (int i = 0; i < 10_000; i++) //adding 10,000 cells to the spreadsheet
        {
            s.SetContentsOfCell($"A{i}", $"{i}");
            expected.Add($"A{i}");
        }

        for (int i = 0; i < 10_000; i++)
        {
            if (rng.Next() % 2 == 0) //randomly removing half
            {
                s.SetContentsOfCell($"A{i}", "");
                expected.Remove($"A{i}");
            }
        }

        var expectedList = new List<string>(expected);
        var actual = new List<string>(s.GetNamesOfAllNonemptyCells());

        expectedList.Sort();
        actual.Sort();

        Assert.IsTrue(expectedList.SequenceEqual(actual));
    }
    
    //new for PS5

    [TestMethod]
    public void JsonTestSimple()
    {
        Spreadsheet spreadsheet = new();
        for (int i = 0; i < 10; i++)
            spreadsheet.SetContentsOfCell($"A{i}", $"{i}");
        
        spreadsheet.Save("save.txt");
        
        
        
        Spreadsheet newSpreadsheet  = new Spreadsheet("save.txt", s1 => true, s1 => s1, "default");
        Assert.IsTrue(SpreadsheetEquals(spreadsheet, newSpreadsheet));
    }
    
    [TestMethod]
    public void JsonTestWithFormulas()
    {
        Spreadsheet spreadsheet = new();
        spreadsheet.SetContentsOfCell("A0", "0");
        for (int i = 1; i <= 10; i++)
            spreadsheet.SetContentsOfCell($"A{i}", $"={i} + A{i - 1}");
        
        spreadsheet.Save("save.txt");
        Console.WriteLine(File.ReadAllText("save.txt"));
        
        
        Spreadsheet newSpreadsheet  = new Spreadsheet("save.txt", s1 => true, s1 => s1, "default");
        Assert.IsTrue(SpreadsheetEquals(spreadsheet, newSpreadsheet));
        Assert.AreEqual(55d, spreadsheet.GetCellValue("A10"));
        Assert.AreEqual(55d, newSpreadsheet.GetCellValue("A10"));

    }
    
    [TestMethod]
    public void JsonStressTest()
    {
        Random rng = new Random(0);
        Spreadsheet spreadsheet = new();
        for (int i = 1; i <= 10; i++)
            spreadsheet.SetContentsOfCell($"A{i}", $"={i}");

        for (int i = 11; i < 10_000; i++) // adding ~10,000 random formulas
        {
            string formula = "=";
            for (int j = 0; j < rng.Next(1, 100); j++)
            {
                formula += $"a{rng.Next(1, i)}+";
            }

            formula = formula.Substring(0, formula.Length - 1);
            spreadsheet.SetContentsOfCell($"A{i}", formula);
        }

        for (int i = 1; i < 10_000; i++) //adding 10,000 strings
        {
            spreadsheet.SetContentsOfCell($"B{i}", $"String number {i}");
        }
        
        for (int i = 1; i < 10_000; i++) //adding 10,000 doubles
        {
            spreadsheet.SetContentsOfCell($"C{i}", $"{rng.Next()}");
        }
        
        spreadsheet.Save("save.txt");
        Spreadsheet newSpreadsheet  = new Spreadsheet("save.txt", s1 => true, s1 => s1, "default");
        
        Assert.IsTrue(SpreadsheetEquals(spreadsheet, newSpreadsheet));
        for (int i = 1; i < 10_000; i++)
        {
            Assert.IsTrue(spreadsheet.GetCellContents($"A{i}") is Formula);
            Assert.IsTrue(spreadsheet.GetCellContents($"B{i}") is string);
            Assert.IsTrue(spreadsheet.GetCellContents($"C{i}") is double);
            Assert.IsTrue(newSpreadsheet.GetCellContents($"A{i}") is Formula);
            Assert.IsTrue(newSpreadsheet.GetCellContents($"B{i}") is string);
            Assert.IsTrue(newSpreadsheet.GetCellContents($"C{i}") is double);
        }
    }

    [TestMethod]
    public void GetCellValueSimpleString()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ";)");
        Assert.AreEqual(";)", s.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void GetCellValueSimpleDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        Assert.AreEqual(1d, s.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void GetCellValueSimpleFormulaAfterJ()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        Assert.AreEqual(1d, s.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void GetCellValueSimpleStringAfterJson()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", ";)");
        var path = "file.txt";
        s.Save(path);
        s = new Spreadsheet(path, s1 => true, s1 => s1, "default");
        Assert.AreEqual(";)", s.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void GetCellValueSimpleDoubleAfterJson()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "1");
        var path = "file.txt";
        s.Save(path);
        s = new Spreadsheet(path, s1 => true, s1 => s1, "default");
        Assert.AreEqual(1d, s.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void GetCellValueSimpleFormulaAfterJson()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        var path = "file.txt";
        s.Save(path);
        s = new Spreadsheet(path, s1 => true, s1 => s1, "default");
        
        Assert.AreEqual(1d, s.GetCellValue("A1"));
    }
    
    [TestMethod]
    public void GetValueOfEmptyCell()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        
        Assert.AreEqual("", s.GetCellValue("A2"));
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetValueInvalidNameThrows1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        
        Assert.AreEqual("", s.GetCellValue("1A"));
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetValueInvalidNameThrows2()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1");
        
        Assert.AreEqual("", s.GetCellValue("#1234"));
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetValueInvalidNameThrowsWithValidator()
    {
        AbstractSpreadsheet s = new Spreadsheet(s1 => s1 != "A2", s1 => s1, "default");
        s.SetContentsOfCell("A1", "=1");
        
        Assert.AreEqual("", s.GetCellValue("A2"));
    }

    [TestMethod]
    public void NormalizeTest()
    {
        AbstractSpreadsheet s = new Spreadsheet(s1 => true, s1 => s1.ToLower(), "default");
        s.SetContentsOfCell("A1", "10");
        Assert.AreEqual(10d, s.GetCellValue("a1"));
        Assert.AreEqual(10d, s.GetCellContents("a1"));
        Assert.AreEqual(10d, s.GetCellValue("A1"));
        Assert.AreEqual(10d, s.GetCellContents("a1"));
    }
    
    [TestMethod]
    public void NormalizeAndValidateTest()
    {
        AbstractSpreadsheet s = new Spreadsheet(s1 => s1 == "a1", s1 => s1.ToLower(), "default");
        s.SetContentsOfCell("A1", "10");
        Assert.AreEqual(10d, s.GetCellValue("a1"));
        Assert.AreEqual(10d, s.GetCellContents("a1"));
        Assert.AreEqual(10d, s.GetCellValue("A1"));
        Assert.AreEqual(10d, s.GetCellContents("a1"));
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void NormalizeToInvalidThrows()
    {
        AbstractSpreadsheet s = new Spreadsheet(s1 => s1 == "A1", s1 => s1.ToLower(), "default");
        s.SetContentsOfCell("A1", "10");
    }
    
    [TestMethod]
    public void ChangedIsFalseAfterConstruction()
    {
        AbstractSpreadsheet s1 = new Spreadsheet(s1 => true, s1 => s1, "default");
        AbstractSpreadsheet s2 = new Spreadsheet();
        new Spreadsheet().Save("file.txt");
        AbstractSpreadsheet s3 = new Spreadsheet("file.txt",s1 => true, s1 => s1, "default");
        
        Assert.IsFalse(s1.Changed);
        Assert.IsFalse(s2.Changed);
        Assert.IsFalse(s3.Changed);
    }
    
    [TestMethod]
    public void ChangedIsTrueAfterSetContents()
    {
        Spreadsheet s1 = new();
        s1.SetContentsOfCell("A1", "1");
        Assert.IsTrue(s1.Changed);


        s1 = new Spreadsheet();
        s1.SetContentsOfCell("A1", "=1");
        Assert.IsTrue(s1.Changed);

        s1 = new Spreadsheet();
        s1.SetContentsOfCell("A1", "string");
        Assert.IsTrue(s1.Changed);
    }
    
    
    [TestMethod]
    public void ChangedIsFalseAfterSave()
    {
        Spreadsheet s1 = new();
        s1.SetContentsOfCell("A1", "1");
        s1.Save("file.txt");
        Assert.IsFalse(s1.Changed);

        s1 = new Spreadsheet();
        s1.SetContentsOfCell("A1", "=1");
        s1.Save("file.txt");
        Assert.IsFalse(s1.Changed);

        s1 = new Spreadsheet();
        s1.SetContentsOfCell("A1", "string");
        s1.Save("file.txt");
        Assert.IsFalse(s1.Changed);
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveInvalidPathThrows()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "10");
        s.Save("Not/A/Valid/Path.txt");
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void DifferentVersionThrows()
    {
        AbstractSpreadsheet s = new Spreadsheet(); //Version is "default"
        s.SetContentsOfCell("A1", "10");
        s.Save("file.txt");

        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("file.txt", s1 => true, s1 => s1, "NotDefault");
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void InvalidNameInJsonThrows()
    {
        AbstractSpreadsheet s = new Spreadsheet(); //Version is "default"
        s.SetContentsOfCell("A1", "10");
        s.Save("file.txt");

        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("file.txt", s1 => s1 != "A1", s1 => s1, "default");
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void CircularExceptionInJsonThrows()
    {
        const string invalidJson = """{"Cells":{"A1":{"AsString":"=A2"},"A2":{"AsString":"=A1"}},"Version":"default"}""";

        File.WriteAllText("file.txt", invalidJson);
        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("file.txt", s1 => true, s1 => s1, "default");
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void InvalidFormulaInJsonThrows()
    {
        const string invalidJson = """{"Cells":{"A1":{"AsString":"=A2 + "}},"Version":"default"}""";

        File.WriteAllText("file.txt", invalidJson);
        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("file.txt", s1 => true, s1 => s1, "default");
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void InvalidJsonThrows()
    {
        const string invalidJson = """{"WrongName":{"A1":{"AsString":"=A2"}},"WrongName2":"default"}""";

        File.WriteAllText("file.txt", invalidJson);
        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("file.txt", s1 => true, s1 => s1, "default");
    }
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void InvalidJsonThrows2()
    {
        const string invalidJson = "Random text that is not in JSON format";

        File.WriteAllText("file.txt", invalidJson);
        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("file.txt", s1 => true, s1 => s1, "default");
    }
    
    
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void InvalidPathThrows()
    {
        AbstractSpreadsheet newSpreadsheet = new Spreadsheet("some/random/path/that/does/not/exist/file.txt", s1 => true, s1 => s1, "default");
    }

    [TestMethod]
    public void GetValueStressTest()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A0", "0");
        for (int i = 1; i < 500; i++)
        {
            s.SetContentsOfCell($"A{i}", $"=1 + A{i - 1}");
        }

        for (int i = 0; i < 500; i++)
        {
            Assert.AreEqual((double)i, s.GetCellValue($"A{i}"));
        }
        s.SetContentsOfCell("A0", "1");

        for (int i = 0; i < 500; i++)
        {
            Assert.AreEqual((double)i + 1, s.GetCellValue($"A{i}"));
        }

    }

}
