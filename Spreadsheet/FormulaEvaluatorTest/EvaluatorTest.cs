using FormulaEvaluator;
namespace FormulaEvaluatorTest;
/// <summary>
/// This Class is used to tests the the static Evaluate method inside the
/// FormulaEvaluator.Evaluator.cs class. This testing is done though the main
/// method which runs a series of tests to ensure that the Evaluate method works
/// as expected.
/// </summary>
public static class EvaluatorTest
{
    //used to define the type of method used in the testing
    public delegate string Test();


    public static void Main()
    {

        //list containing all tests
        List<Test> tests = new();

        //used to store the failed tests
        List<string> failed = new();

        //adding all the tests to the list
        tests.Add(SimpleAddTest);
        tests.Add(SimpleSubtractTest);
        tests.Add(SimpleMultiplyTest);
        tests.Add(SimpleDivideTest);
        tests.Add(SimpleParenthesisTest);
        tests.Add(SimpleVariableTest);
        tests.Add(ComplexAdditionTest);
        tests.Add(ComplexSubtractionTest);
        tests.Add(ComplexMultiplicationTest);
        tests.Add(ComplexDivisionTest);
        tests.Add(ComplexParenthesisTest);
        tests.Add(DivideByZeroThrows);
        tests.Add(MissingValuesAddThrows1);
        tests.Add(MissingValuesAddThrows2);
        tests.Add(MissingValuesSubtractThrows1);
        tests.Add(MissingValuesSubtractThrows2);
        tests.Add(MissingValuesMultiplicationThrows1);
        tests.Add(MissingValuesMultiplicationThrows2);
        tests.Add(MissingValuesDivisionThrows1);
        tests.Add(MissingValuesDivisionThrows2);
        tests.Add(EmptyExpresionThrows);
        tests.Add(EmptyParenthesisThrows);
        tests.Add(EmptyParenthesisThrows2);
        tests.Add(ComplexVariableTest);
        tests.Add(ComplexVariableTestWithNegative);
        tests.Add(ComplexEquation1);
        tests.Add(ComplexEquation2);
        tests.Add(ImproperVariableThrows1);
        tests.Add(ImproperVariableThrows2);
        tests.Add(ImproperVariableThrows3);
        tests.Add(ImproperVariableThrows4);
        tests.Add(ImproperVariableThrows5);
        tests.Add(ImproperVariableThrows6);
        tests.Add(ImproperVariableThrows7);
        tests.Add(InvalidCharsThrows1);
        tests.Add(InvalidCharsThrows2);
        tests.Add(InvalidCharsThrows3);




        foreach (Test test in tests)
        {
            string result = test();
            if (result.Length > 0)
                failed.Add(result);
        }

        Console.WriteLine(tests.Count - failed.Count + "/" + tests.Count +
                           " Tests passed");
        Console.WriteLine();

        foreach (string s in failed)
            Console.WriteLine(s);
    }


    /// <summary>
    /// This method tests that simple addition works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string SimpleAddTest()
    {
        try
        {
            int result = Evaluator.Evaluate("150 + 300", (s) => 0);
            int expected = 450;
            if (result != expected)
            {
                return "simpleAddTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "simpleAddTest failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that simple subtraction works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string SimpleSubtractTest()
    {
        try
        {
            int result = Evaluator.Evaluate("150 - 80", (s) => 0);
            int expected = 70;
            if (result != expected)
            {
                return "simpleSubtractTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "simpleSubtractTest failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that simple multiplication works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string SimpleMultiplyTest()
    {
        try
        {
            int result = Evaluator.Evaluate("5 * 25", (s) => 0);
            int expected = 125;
            if (result != expected)
            {
                return "simpleMultiplyTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "simpleMultiplyTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that simple division works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string SimpleDivideTest()
    {
        try
        {
            int result = Evaluator.Evaluate("10 / 2", (s) => 0);
            int expected = 5;
            if (result != expected)
            {
                return "simpleDivideTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "simpleDivideTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that simple equation with parenthesis works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string SimpleParenthesisTest()
    {
        try
        {
            int result = Evaluator.Evaluate("10 / (2+ 1)", (s) => 0);
            int expected = 3;
            if (result != expected)
            {
                return "SimpleParenthesisTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "SimpleParenthesisTest failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that simple equation with variables works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string SimpleVariableTest()
    {
        try
        {
            int result = Evaluator.Evaluate("10 + A6 - Bb540", (s) => s.Length);
            int expected = 7;
            if (result != expected)
            {
                return "SimpleVariableTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "SimpleVariableTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that complex addition works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexAdditionTest()
    {
        try
        {
            int result = Evaluator.Evaluate("57 + 52 +   (   12 + 5) +(4)", (s) => 0);
            int expected = 130;
            if (result != expected)
            {
                return "ComplexAdditionTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexAdditionTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that complex subtraction works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexSubtractionTest()
    {
        try
        {
            int result = Evaluator.Evaluate("100 - 4 - 16 -23     - (9-2  )", (s) => 0);
            int expected = 50;
            if (result != expected)
            {
                return "ComplexSubtractionTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexSubtractionTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that complex multiplication works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexMultiplicationTest()
    {
        try
        {
            int result = Evaluator.Evaluate("4*   2    * 1 * (4 * 3   ) * (2)", (s) => 0);
            int expected = 192;
            if (result != expected)
            {
                return "ComplexMultiplicationTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexMultiplicationTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that complex division works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexDivisionTest()
    {
        try
        {
            int result = Evaluator.Evaluate("101 /   (5) / (3 /  1  )", (s) => 0);
            int expected = 6;
            if (result != expected)
            {
                return "ComplexDivisionTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexDivisionTest failed: " + ex.Message;
        }
        return "";
    }


    /// <summary>
    /// This method tests that complex equations with parenthesis works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexParenthesisTest()
    {
        try
        {
            int result = Evaluator.Evaluate("1*(   ( (1  ) + 2) - 3 + 2) / (7/(4 -(1  ))   )", (s) => 0);
            int expected = 1;
            if (result != expected)
            {
                return "ComplexParenthesisTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexParenthesisTest failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that complex equation with variables works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexVariableTest()
    {
        try
        {
            int result = Evaluator.Evaluate("A6 * 5 / (3 *(g6646 - hhr8)) / (12 - (h3 + hh99 * 2 + 1)  )", (s) => s.Length);
            int expected = 3;
            if (result != expected)
            {
                return "ComplexVariableTest failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexVariableTest failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that complex equation with negative values for
    /// variables works correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexVariableTestWithNegative()
    {
        try
        {
            int result = Evaluator.Evaluate("A6 * 5 / (3 *(g6646 - hhr8)) / " +
                          "(12 + (h3 + hh99 * 2 + 1)  )", (s) => s.Length * -1);
            int expected = 1;
            if (result != expected)
            {
                return "ComplexVariableTestWithNegative1 failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexVariableTestWithNegative1 failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that complex equation evaluated correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexEquation1()
    {
        try
        {
            int result = Evaluator.Evaluate("(897 * 34 + ( 3 * afi532 + (2 * 34 + jale532) )) / ( 123 - 133 / ahr45)", (s) => s.Length);
            int expected = 315;
            if (result != expected)
            {
                return "ComplexEquation1 failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexEquation1 failed: " + ex.Message;
        }
        return "";
    }

    /// <summary>
    /// This method tests that complex equation evaluated correctly
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ComplexEquation2()
    {
        try
        { 
            int result = Evaluator.Evaluate("86532 + (32 + alGheiDHJGI99 / (12 + ad3 - A5)  ) * (3 -EjgiEd392)", (s) => s.Length);
            int expected = 86334;
            if (result != expected)
            {
                return "ComplexEquation2 failed: Expected: " + expected + " but got "
                       + result;
            }
        }
        catch (ArgumentException ex)
        {
            return "ComplexEquation2 failed: " + ex.Message;
        }
        return "";
    }



    /// <summary>
    /// This method tests that dividing by 0 throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string DivideByZeroThrows()
    {
        try
        {
            int result = Evaluator.Evaluate("54 / (10 - 5 *2", (s) => 0);

        }
        catch (ArgumentException)   
        {
            return ""; //Tests passed if error is thrown
        }
        return "DivideByZeroThrows failed: Expected: ArgumentException, but " +
            "no error was thrown";

    }

    /// <summary>
    /// This method tests that missing addition values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesAddThrows1()
    {
        try
        {
            int result = Evaluator.Evaluate(" + 5 + 92 ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesAddThrows1 failed: Expected: ArgumentException, but " +
            "no error was thrown";

    }

    /// <summary>
    /// This method tests that missing addition values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesAddThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate(" 5 + 92 + ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }           
        return "MissingValuesAddThrows2 failed: Expected: ArgumentException, but " +
            "no error was thrown";

    }


    /// <summary>
    /// This method tests that missing subtraction values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesSubtractThrows1()
    {
        try
        {
            int result = Evaluator.Evaluate(" - 5 - 92  ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesSubtractThrows1 failed: Expected: ArgumentException, but " +
            "no error was thrown";

    }


    /// <summary>
    /// This method tests that missing subtraction values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesSubtractThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate("5 - 92 - ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesSubtractThrows2 failed: Expected: ArgumentException, but " +
            "no error was thrown";

    }


    /// <summary>
    /// This method tests that missing multiplication values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesMultiplicationThrows1()
    {
        try
        {
            int result = Evaluator.Evaluate(" * 5 * 92  ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesMultiplicationThrows1 failed: Expected: " +
            "ArgumentException, but no error was thrown";

    }


    /// <summary>
    /// This method tests that missing multiplication values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesMultiplicationThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate("5 * 92 * ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesMultiplicationThrows2 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that missing division values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesDivisionThrows1()
    {
        try
        {
            int result = Evaluator.Evaluate(" / 10/  2  ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesMultiplicationThrows1 failed: Expected: " +
            "ArgumentException, but no error was thrown";

    }


    /// <summary>
    /// This method tests that missing division values throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string MissingValuesDivisionThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate("10  /2 / * ", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "MissingValuesMultiplicationThrows2 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }



    /// <summary>
    /// This method tests that an empty expresion throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string EmptyExpresionThrows()
    {
        try
        {
            int result = Evaluator.Evaluate("", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "EmptyExpresionThrows failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with empty parenthesis throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string EmptyParenthesisThrows()
    {
        try
        {
            int result = Evaluator.Evaluate("2 +  (2 / 1 + 2 - ())", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "EmptyParenthesisThrows failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }

    /// <summary>
    /// This method tests that an expresion with empty parenthesis throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string EmptyParenthesisThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate("() + 1", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "EmptyParenthesisThrows2 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows1()
    {
        try
        {
            int result = Evaluator.Evaluate("Ajfie6 + Ahi33a", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows1 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }

    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate("Ajfie6 + 33Aai33", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows2 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows3()
    {
        try
        {
            int result = Evaluator.Evaluate("@Hie66 + 0", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows3 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows4()
    {
        try
        {
            int result = Evaluator.Evaluate("Hi[e66 + 0", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows4 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }

    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows5()
    {
        try
        {
            int result = Evaluator.Evaluate("H`ie66 + 0", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows5 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows6()
    {
        try
        {
            int result = Evaluator.Evaluate("Hie{66 + 0", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows6 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }

    /// <summary>
    /// This method tests that an expresion with invalid variable name throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string ImproperVariableThrows7()
    {
        try
        {
            int result = Evaluator.Evaluate("Hieie + 0", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "ImproperVariableThrows7 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid characters  throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string InvalidCharsThrows1()
    {
        try
        {
            int result = Evaluator.Evaluate("305 + 352 ^ 324", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "InvalidCharsThrows1 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid characters  throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string InvalidCharsThrows2()
    {
        try
        {
            int result = Evaluator.Evaluate("$05 + 352 + 24", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "InvalidCharsThrows1 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }


    /// <summary>
    /// This method tests that an expresion with invalid characters  throws an error
    /// </summary>
    /// <returns>empty string if passed, error message if failed</returns>
    private static string InvalidCharsThrows3()
    {
        try
        {
            int result = Evaluator.Evaluate("5 + 3:52 + 24", (s) => 0);

        }
        catch (ArgumentException)
        {
            return ""; //Tests passed if error is thrown
        }
        return "InvalidCharsThrows3 failed: Expected: " +
            "ArgumentException, but no error was thrown";
    }
}


