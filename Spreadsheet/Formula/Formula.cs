// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!
// Last updated: August 2023 (small tweak to API)

using System.Text.RegularExpressions;


namespace SpreadsheetUtilities;

/// <summary>
/// Represents formulas written in standard infix notation using standard precedence
/// rules.  The allowed symbols are non-negative numbers written using double-precision
/// floating-point syntax (without unary preceeding '-' or '+');
/// variables that consist of a letter or underscore followed by
/// zero or more letters, underscores, or digits; parentheses; and the four operator
/// symbols +, -, *, and /.
///
/// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
/// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
/// and "x 23" consists of a variable "x" and a number "23".
///
/// Associated with every formula are two delegates: a normalizer and a validator.  The
/// normalizer is used to convert variables into a canonical form. The validator is used to
/// add extra restrictions on the validity of a variable, beyond the base condition that
/// variables must always be legal: they must consist of a letter or underscore followed
/// by zero or more letters, underscores, or digits.
/// Their use is described in detail in the constructor and method comments.
/// </summary>
public class Formula
{

    //Contains strings and Doubles to represent each token in the formula
    private readonly object[] tokens;


    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically invalid,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer is the identity function, and the associated validator
    /// maps every string to true.
    /// </summary>
    public Formula(string formula) :
        this(formula, s => s, s => true)
    {
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    ///
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.
    ///
    /// If the formula contains a variable v such that normalize(v) is not a legal variable,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    ///
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    ///
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
        IEnumerable<string> givenTokens = GetTokens(formula);
        tokens = new object[givenTokens.Count()];

        //Parsing the string
        int i = 0; 
        foreach (string t in givenTokens)
        {

            //Branch for variables
            if (IsValidVariable(t))
            {
                //Normalized variable must be
                if (!IsValidVariable(normalize(t)))
                    throw new FormulaFormatException("Variable Became invald after" +
                        " being normalized");
                if (!isValid(normalize(t)))
                    throw new FormulaFormatException("The Variable is not valid " +
                        "according to provided validator");
                tokens[i] = normalize(t);
            }

            //branch for numbers
            else if (double.TryParse(t, out double result))
                tokens[i] = result;
            

            //branch for operators
            else if (t == "(" || t == ")" || t == "+" || t == "-" ||
                    t == "*" || t == "/")
            {
                tokens[i] = t;
            }
            
            //only variables, numbers and operators are allowed
            else
                throw new FormulaFormatException("Token: " + t + " is not valid." +
                    "Only numbers, variables, +, -, *, and / are valid tokens.");
            i++;
        }

        //One token rule
        if (tokens.Length < 1)
            throw new FormulaFormatException("Formula must contain at least one " +
                "Token");

        //Parenthesis rule
        //b represents the number of left perenthesis seen so far subtracted
        //by the number of left parenthesis seen so far
        int balance = 0;
        foreach (var t in tokens)
        {
            if (t is string s)
            {
                if (s.Equals("("))
                    balance++;
                if (s.Equals(")"))
                    balance--;
            }

            if (balance < 0)
            {
                throw new FormulaFormatException("The parenthesis in the formula" +
                    " are not balanced");
            }
        }
        if (balance > 0)
        {
            throw new FormulaFormatException("The formula given is missing " + balance
                + " closing parenthesis");
        }


        //Starting Token Rule
        if (tokens[0] is string first)
        {
            if (first == ")" || first == "+" || first == "-" || first == "*" || first == "/")
            {
                throw new FormulaFormatException("Formula must start with a" +
                    " number, variable or open parenthesis");
            }
        }

        //Ending Token Rule
        if (tokens[^1] is string last)
        {
            if (last == "(" || last == "+" || last == "-" || last == "*" || last == "/")
            {
                throw new FormulaFormatException("Formula must end with a" +
                    " number, variable or closing parenthesis");
            }
        }

        //Parenthesis/Operator Following rule
        for (int j = 0; j < tokens.Length - 1; j++)
        {
            if (tokens[j] is string s1 &&
                Regex.IsMatch(s1, @"^[\(\+\-\*\/]$") && //determine if operator or (
                tokens[j + 1] is string s2 &&
                Regex.IsMatch(s2, @"^[\)\+\-\*\/]$")) //determine if followed by operator or )
            {

                //throw error if a operator or ( is followed by another operator or )
                throw new FormulaFormatException("Every token following a " +
                    "opening parenthesis or operator must be followed by " +
                    "a number, variable, or opening paraenthesis");
            }
        }

        //Extra Following Rule
        for (int k = 0; k < tokens.Length -1; k++)
        {
            if(tokens[k] is double ||
               (tokens[k] is string s &&
               (IsValidVariable(s) || tokens[k].Equals(")"))))
            {
                if (!(tokens[k + 1] is string nextString &&
                    Regex.IsMatch(nextString, @"^[\)\+\-\*\/]$")))
                    throw new FormulaFormatException("Token: " + tokens[k] +
                        " Must be followed by a operator or a closing parenthesis");

            }


        }

    }


    /// <summary>
    /// Evaluates this Formula, using the lookup delegate to determine the values of
    /// variables.  When a variable symbol v needs to be determined, it should be looked up
    /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
    /// the constructor.)
    ///
    /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
    /// in a string to upper case:
    ///
    /// new Formula("x+7", N, s => true).Evaluate(L) is 11
    /// new Formula("x+7").Evaluate(L) is 9
    ///
    /// Given a variable symbol as its parameter, lookup returns the variable's value
    /// (if it has one) or throws an ArgumentException (otherwise).
    ///
    /// If no undefined variables or divisions by zero are encountered when evaluating
    /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
    /// The Reason property of the FormulaError should have a meaningful explanation.
    ///
    /// This method should never throw an exception.
    /// </summary>
    public object Evaluate(Func<string, double> lookup)
    {
        Stack<double> valueStack = new();
        Stack<string> oprStack = new();

        for (int i = 0; i < tokens.Count(); i++)
        {
            //Branch for numbers
            if (tokens[i] is double)
            {
                if (oprStack.Count > 0 && (oprStack.Peek() == "*" || oprStack.Peek() == "/"))
                {
                    //Checking for a divide by 0 error
                    if (oprStack.Peek() == "/" && (double)tokens[i] == 0)
                        return new FormulaError("Can not divide by 0");
                    valueStack.Push(Calculate(valueStack.Pop(), (double)tokens[i], oprStack.Pop()));
                }
                else
                    valueStack.Push((double)tokens[i]);
            }

            //Tokens[i] must be a string after this point
            //Branch for addition and subtraction
            else if (tokens[i].Equals("+") || tokens[i].Equals("-"))
            {
                if (oprStack.Count > 0 && (oprStack.Peek().Equals("+") || oprStack.Peek().Equals("-")))
                {
                    double v2 = valueStack.Pop();
                    double v1 = valueStack.Pop();
                    valueStack.Push(Calculate(v1, v2, oprStack.Pop()));
                }
                oprStack.Push((string)tokens[i]);
            }


            //Branch for multiplication, division and opening parethesis
            else if (tokens[i].Equals("*") || tokens[i].Equals("/") ||
                tokens[i].Equals("("))
            {
                oprStack.Push((string)tokens[i]);
            }


            //Branch for closing parenthesis
            else if (tokens[i].Equals(")"))
            {
                if (oprStack.Peek().Equals("+") || oprStack.Peek().Equals("-"))
                {
                    double v2 = valueStack.Pop();
                    double v1 = valueStack.Pop();
                    valueStack.Push(Calculate(v1, v2, oprStack.Pop()));
                }
                oprStack.Pop();

                if (oprStack.Count > 0 && (oprStack.Peek().Equals("*") || oprStack.Peek().Equals("/")))
                {
                    double v2 = valueStack.Pop();
                    double v1 = valueStack.Pop();

                    //Checking for a divide by 0 error
                    if (oprStack.Peek() == "/" && v2 == 0)
                        return new FormulaError("Can not divide by 0");
                    valueStack.Push(Calculate(v1, v2, oprStack.Pop()));

                }
            }


            //Branch for variables
            else
            {
                double val;
                try
                {
                    val = lookup((string)tokens[i]);    
                }
                catch (ArgumentException ex)  //return error is lookup fails
                {
                    return new FormulaError("Lookup Delegate threw ArgumentException " +
                        "while looking up value of " + (string)tokens[i] +" Below" +
                        "is the message \n" +  ex.Message);
                }
                if (oprStack.Count > 0 && (oprStack.Peek() == "*" || oprStack.Peek() == "/"))
                {
                    //Checking for a divide by 0 error
                    if (oprStack.Peek() == "/" && val == 0)
                        return new FormulaError("Can not divide by 0");
                    valueStack.Push(Calculate(valueStack.Pop(), val, oprStack.Pop()));
                }
                else
                    valueStack.Push(val);
                continue;
            }


        }


        //Addition or Subtraction left
        if (oprStack.Count == 1 && valueStack.Count == 2 &&
           (oprStack.Peek().Equals("+") || oprStack.Peek().Equals("-")))
        {
            double v2 = valueStack.Pop();
            double v1 = valueStack.Pop();
            return Calculate(v1, v2, oprStack.Pop()); 
        }

        return valueStack.Pop();


    }


    /// <summary>
    /// Enumerates the normalized versions of all of the variables that occur in this
    /// formula.  No normalization may appear more than once in the enumeration, even
    /// if it appears more than once in this Formula.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
    /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
    /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
    /// </summary>
    public IEnumerable<string> GetVariables()
    {
        HashSet<string> ret = new();
        foreach (var t in tokens)
            if (t is string s && IsValidVariable(s))
                ret.Add(s);

        return ret;
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
        System.Text.StringBuilder stringBuilder = new();
        foreach (object t in tokens)
            stringBuilder.Append(t.ToString());
        return stringBuilder.ToString();
    }

    /// <summary>
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    ///
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" by
    /// using C#'s standard conversion from string to double (and optionally back to a string).
    /// Variable tokens are considered equal if their normalized forms are equal, as
    /// defined by the provided normalizer.
    ///
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Formula)
        {
            return false ;
        }

        var f = (Formula)obj;
        if (tokens.Length != f.tokens.Length)
            return false;

        for (int i = 0; i < tokens.Length; i++)
        {
            if (!tokens[i].Equals(f.tokens[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    /// Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// Note that f1 and f2 cannot be null, because their types are non-nullable
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !f1.Equals(f2);
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
        int ret = 0;
        foreach (var token in tokens)
        {
            ret += token.GetHashCode();
        }
        return ret;
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a legal variable token;
    /// a double literal; and anything that doesn't match one of those patterns.
    /// There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(string formula)
    {
        // Patterns for individual tokens
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                yield return s;
            }
        }

    }

    /// <summary>
    /// This method will determine if a varaible is formated correctly
    /// The correct format is a letter (upper or lowercase)  or underscore
    /// followed by any combination of letter numbers or underscores
    /// </summary>
    /// <param name="t">The string to be checked</param>
    /// <returns>True if it is in the proper format false if not</returns>
    private bool IsValidVariable(string t)
    {
        return Regex.IsMatch(t, "^[a-zA-Z_][a-zA-Z0-9_]*$");
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
    private static double Calculate(double v1, double v2, string opr)
    {
        if (opr.Equals("+"))
            return v1 + v2;
        if (opr.Equals("-"))
            return v1 - v2;
        if (opr.Equals("*"))
            return v1 * v2;

        //Must de division if it is not any of the others
        return  v1 / v2;    
    }
}


/// <summary>
/// Used to report syntactic errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(string message) : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public struct FormulaError
{
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(string reason) : this()
    {
        Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}
