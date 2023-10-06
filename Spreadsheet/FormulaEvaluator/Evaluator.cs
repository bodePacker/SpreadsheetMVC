using System.Text.RegularExpressions;

namespace FormulaEvaluator;
/// <summary>
/// The "Evaluator" class is a static utility class designed to perform arithmetic
/// evaluations of mathematical expressions represented as strings. This class
/// belongs to the "FormulaEvaluator" namespace and provides a method named
/// "Evaluate" for calculating the result of an arithmetic expression. The
/// expression can involve integers, variables, and basic arithmetic operators
/// (+, -, *, /). The class utilizes regular expressions to tokenize the input
/// expression and then evaluates the expression using two stacks - one for
/// values and another for operators. It supports variable evaluation through a
/// delegate called "Lookup," which allows users to define how variables are resolved. 
/// </summary>
public static class Evaluator
{
    public delegate int Lookup(string v);



    /// <summary>
    /// This method will evaluate an expresion with infix notation that contains
    /// positive integers, Variables, and Basic arithmatic (+, _ , / , *). The
    /// variables will be converted into integers using the delagate provided.
    /// varaibles must be in the form of 1 or more letters followed by 1 or more
    /// numbers ex. a1, A25 or Ga51.
    /// Note: the "-" sign should only be used as the subtraction and not as an
    /// unary negative
    /// Also Note: The / sign will be perform integer division meaning that 
    /// </summary>
    /// <param name="exp">The expresion to be evaluated</param>
    /// <param name="variableEvaluator">A Delegate used to lookup the value
    /// of any variables</param>
    /// <returns>The an integer representing the evalutaion of the equation
    /// given</returns>
    /// <exception cref="ArgumentException">If the expresion is formated
    /// incorectly of if the user attempts to divide by 0</exception>
    public static int Evaluate(string exp, Lookup variableEvaluator)
    {
        exp = exp.Trim();  //Removing extra white space
        string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

        //Removing whitespace from each token
        for (int i = 0; i < tokens.Length; i++)
        {
            tokens[i] = tokens[i].Trim();
        }

        Stack<int> valueStack = new();
        Stack<string> oprStack = new();

        //Used to catch errors relating to removing from an empty stack
        try
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                //skip white space
                if (tokens[i] == "")
                    continue;

                //Branch for numbers
                if (IsInteger(tokens[i]))
                {
                    int val = int.Parse(tokens[i]);
                    if (oprStack.Count > 0 && (oprStack.Peek() == "*" || oprStack.Peek() == "/"))
                    {
                        valueStack.Push(Calculate(valueStack.Pop(), val, oprStack.Pop()));
                    }
                    else
                        valueStack.Push(val);
                    continue;
                }

                //Branch for variables
                if (IsVariable(tokens[i]))
                {
                    int val = variableEvaluator(tokens[i]);
                    if (oprStack.Count > 0 && (oprStack.Peek() == "*" || oprStack.Peek() == "/"))
                    {
                        valueStack.Push(Calculate(valueStack.Pop(), val, oprStack.Pop()));
                    }
                    else
                        valueStack.Push(val);
                    continue;
                }

                //Branch for addition and subtraction
                if (tokens[i].Equals("+") || tokens[i].Equals("-"))
                {
                    if (oprStack.Count > 0 && (oprStack.Peek().Equals("+") || oprStack.Peek().Equals("-")))
                    {
                        int v2 = valueStack.Pop();
                        int v1 = valueStack.Pop();
                        valueStack.Push(Calculate(v1, v2, oprStack.Pop()));
                    }
                    oprStack.Push(tokens[i]);
                    continue;
                }

                //Branch for multiplication, division and opening parethesis
                if (tokens[i].Equals("*") || tokens[i].Equals("/") ||
                    tokens[i].Equals("("))
                {
                    oprStack.Push(tokens[i]);
                    continue;
                }

                //Branch for closing parenthesis
                if (tokens[i].Equals(")"))
                {
                    if (oprStack.Peek().Equals("+") || oprStack.Peek().Equals("-"))
                    {
                        int v2 = valueStack.Pop();
                        int v1 = valueStack.Pop();
                        valueStack.Push(Calculate(v1, v2, oprStack.Pop()));
                    }
                    if (!oprStack.Peek().Equals("("))
                    {
                        throw new ArgumentException("The Token: " + oprStack.Peek() +
                        " must be a ( in order for the expresion to be formated correctly");
                    }
                    oprStack.Pop();
                    if (oprStack.Count > 0 && (oprStack.Peek().Equals("*") || oprStack.Peek().Equals("/")))
                    {
                        int v2 = valueStack.Pop();
                        int v1 = valueStack.Pop();
                        valueStack.Push(Calculate(v1, v2, oprStack.Pop()));
                    }
                    continue;
                }

                //If none of the branches are executed then the token is invalid
                throw new ArgumentException("The Token: " + tokens[i] + " is not valid");
            }
          
            //oprStack is Empty
            if (oprStack.Count == 0 && valueStack.Count == 1)
                return valueStack.Pop();

            //Addition or Subtraction left
            if (oprStack.Count == 1 && valueStack.Count == 2 &&
               (oprStack.Peek().Equals("+") || oprStack.Peek().Equals("-")))
            {
                int v2 = valueStack.Pop();
                int v1 = valueStack.Pop();
                return Calculate(v1, v2, oprStack.Pop());
            }

            //if neither of the 2 end conditions are met then the expresion is
            //formated incoreclty
            throw new ArgumentException("The Expresion: " + exp +
                                        " is formated incorectly ");

        }
        catch (InvalidOperationException) //thrown when a emtpy stack accessed
        {
            throw new ArgumentException("The Expresion: " + exp +
                                        " is formated incorectly ");
        }
    }


    /// <summary>
    /// This method will evaluate an operation between 2 numbers and return the
    /// result. The 4 operations supported are addition ("+"), subtraction ("-"),
    /// multiplication ("*"), and division ("/"). Any other strings provided
    /// will cause an ArgumentException to be thrown. An Argument Exception will
    /// also be thrown if a "/" operator is provated and the value of v2 is 0.
    /// </summary>
    /// <param name="v1">The first value of the equation</param>
    /// <param name="v2">The second value of the equation</param>
    /// <param name="opr">The operorator of the equation, must be one of the
    /// following {"+", "-", "*", "/" </param>
    /// <returns>The result of the mathematical equation v1 opr v2 </returns>
    /// <exception cref="ArgumentException">If a invalid operator is provided
    /// or if the user attempts ot divide by 0
    /// </exception>
    private static int Calculate(int v1, int v2, string opr)
    {
        if (opr.Equals("+"))
            return v1 + v2;
        if (opr.Equals("-"))
            return v1 - v2;
        if (opr.Equals("*"))
            return v1 * v2;
        if (opr.Equals("/"))
        {
            if (v2 == 0)
                throw new ArgumentException("Cannot divide by 0");
            else
                return v1 / v2;
        }

        throw new ArgumentException(opr + " is not a valid operator.");
    }

    /// <summary>
    /// This method will return a boolean value indicationg weather a string is
    /// an integer value. A integer is difined by this method to be string that
    /// only contains the charcheters 0 - 9.
    /// </summary>
    /// <param name="s">The string to be tested</param>
    /// <returns>True if it is an integer, false if not</returns>d
    private static bool IsInteger(string s)
    {
        if (s.Length == 0)
            return false;
        foreach (char c in s)
        {
            if (c < 48 || c > 57)
                return false;
        }
        return true;
    }

    /// <summary>
    /// This method will return a boolean value determining if the string s is
    /// formated as a variable. The proper format for a variable is sequence of
    /// at least one letter (upper or lower case), followed by a sequence of at
    /// least one number
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsVariable(string s)
    {
        int i = 0;
        for (; i < s.Length; i++)
        {
            //checking if current char is a non letter
            if (s.ElementAt(i) < 65 || s.ElementAt(i) > 122 ||
              s.ElementAt(i) > 90 && s.ElementAt(i) < 97)
            {
                //if the token contains no letters it is not a variable
                if (i == 0)
                    return false;
                else
                    break;
            }
        }

        //check that the remaining substring is a sequence of digits
        return IsInteger(s[i..]);

    }


}

