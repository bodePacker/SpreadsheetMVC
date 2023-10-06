namespace FormulaTests;

using SpreadsheetUtilities;
/// <summary>
/// This class is used to test The Formula Class. This class includes tests
/// that determine if the public methods are the formula class act as they should
///
/// By Timothy Blamires
/// Last Updated 9/15/23
/// 
/// </summary>
[TestClass]
public class FormulaTests
{

    //construtor Tests
    [TestMethod]
    public void EmptyConstructorTest()
    {
        Formula f = new("1+ 1");
        Assert.AreEqual(2d, (double)f.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ConstructorTestThrowsParsing()
    {
        _ = new Formula("1 + #");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ThrowsWithInvalidVariable()
    {
        _ = new Formula("1 + _242jadfie / 4ad + 42");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ThrowsWithInvalidVariable2()
    {
        _ = new Formula("1 + _242jadfie / adkjaeif_235823? + 42");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ThrowsInvalidVariablesWithNormalizer()
    {
        Formula f = new("1 + _242jadfie / 4ad + 42", s => "_hi", s => true);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ThrowsAfterNormalizerMakesVariableInvalid()
    {
        Formula f = new("1 + _242jadfie / _ad + 42", s => "6hi", s => true);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ThrowsWithValidator()
    {
        Formula f = new("1 + _242jadfie / _ad + dc", s => s, s => s.Equals("_ad"));
    }



    [TestMethod]
    public void VariableValidatesAfterNormalization()
    {
        Formula f = new("1 + _242jadfie / _ad + dc", s => "_ad", s => s.Equals("_ad"));

    }

    [TestMethod]
    public void DoubleParsedCorrectly()
    {
        Formula f = new("1.5 + 1.3");
        Assert.AreEqual(2.8, (double)f.Evaluate(s => 0), 1e-9);

    }

    [TestMethod]
    public void DoubleParsedCorrectlyScientificNotation()
    {
        Formula f = new("1.5 + 13e-1");
        Assert.AreEqual(2.8, (double)f.Evaluate(s => 0), 1e-9);

    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void ImproperDoubleThrows()
    {
        Formula f = new("1.5352a + 103e-235");
    }


    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void EmptyFromulaThrows()
    {
        _ = new Formula("");
    }

    [TestMethod]
    public void OneNumberTest()
    {
        Formula f = new("1234.5678");
        Assert.AreEqual(1234.5678, (double)f.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OneParenthesisThrowsOpen()
    {
        _ = new Formula("(");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OneParenthesisThrowsClosed()
    {
        _ = new Formula(")");
    }
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OneOperatorThrowsAdd()
    {
        _ = new Formula("+");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OneOperatorThrowsMinus()
    {
        _ = new Formula("-");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OneOperatorThrowsMultiply()
    {
        _ = new Formula("*");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OneOperatorThrowsDivide()
    {
        _ = new Formula("/");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingOpeningParenthesisThrows()
    {
        _ = new Formula("((1) + (1)))");
    }


    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingClosingParenthesisThrows()
    {
        _ = new Formula("((1) + (1)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void NonBalancedParenthesisThrows()
    {
        _ = new Formula("())1(");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void NonBalancedParenthesisThrowsLarge()
    {
        _ = new Formula("(3 +2) * (((3))) -(1+(2 /1) +(1)) - (2*(4-(3))");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void OpeningParenthesisLastThrows()
    {
        _ = new Formula("1+ (");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void AddLastThrows()
    {
        _ = new Formula("1 + ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MinusLastThrows()
    {
        _ = new Formula("1 - ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MulitplyLastThrows()
    {
        _ = new Formula("1 * ");
    }


    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void DivideLastThrows()
    {
        _ = new Formula("1 / ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesAddThrows1()
    {
        _ = new Formula(" + 5 + 92 ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesAddThrows2()
    {
        _ = new Formula(" 5 + 92 + ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesSubtractThrows1()
    {
        _ = new Formula(" - 5 - 92  ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesSubtractThrows2()
    {
        _ = new Formula("5 - 92 - ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesMultiplicationThrows1()
    {
        _ = new Formula("* 5 * 92  ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesMultiplicationThrows2()
    {
        _ = new Formula(" 5 * 92  *");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesDivisionThrows1()
    {
        _ = new Formula(" / 10/  2  ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void MissingValuesDivisionThrows2()
    {
        _ = new Formula("10  /2 / ");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void EmptyParenthesisThrows()
    {
        _ = new Formula("()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void EmptyParenthesisThrows2()
    {
        _ = new Formula("1 + ()");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void DoubleOperatorsThrows1()
    {
        _ = new Formula("1 + 5 -( 3 */ 2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void DoubleOperatorsThrows2()
    {
        _ = new Formula("1 +- 3");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TwoNumbersThrows()
    {
        _ = new Formula("(a * 5.2 7.23 - _var1 / 3.7) + (b + 8.9) * (_var2 - 2.6)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void NumberOpeningThrows()
    {
        _ = new Formula("(a * 5.2 + 7.23 - _var1 / 3.7) + 8 (8.9) * (_var2 - 2.6)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void NumberVariableThrows()
    {
        _ = new Formula("(a * 5.2 - _var1 / 3.7) + 8 variable * (_var2 - 2.6)");
    }

    [TestMethod]
    public void ComplexDoesNotThrow()
    {
        _ = new Formula("    (_var1* 3.5 + 6.7) / (c - (_var2 + 7.5)) + (3.2 * 1.5 - _var1) + (_var2 / 3.14)");
    }

    [TestMethod]
    public void ComplexDoesNotThrow2()
    {
        _ = new Formula(" (a + b) * (_var1 - 3.5) + (c / (_var2 + 1.8) - (_var1 * _var2 + 7.9))");
    }


    //EvaluateTests
    [TestMethod]
    public void DivideByZeroReturnError()
    {
        Formula f = new("8/0");
        object ret = f.Evaluate(s => 0);
        Assert.IsTrue(ret is FormulaError);
    }

    [TestMethod]
    public void DivideByZeroReturnErrorParenthesis()
    {
        Formula f = new("8/(1- 1)");
        object ret = f.Evaluate(s => 0);
        Assert.IsTrue(ret is FormulaError);
    }

    [TestMethod]
    public void DivideByZeroReturnErrorVariable()
    {
        Formula f = new("8/x");
        object ret = f.Evaluate(s => 0);
        Assert.IsTrue(ret is FormulaError);
    }

    [TestMethod]
    public void DivideByZeroReturnErrorVariableParenthesis()
    {
        Formula f = new("8/0-x");
        object ret = f.Evaluate(s => 0);
        Assert.IsTrue(ret is FormulaError);
    }

    [TestMethod]
    public void LookupThrowingReturnsError()
    {
        Formula f = new("8/1-x");
        object ret = f.Evaluate(s => throw new ArgumentException(""));
        Assert.IsTrue(ret is FormulaError);
    }

    [TestMethod]
    public void EvaluateAddition()
    {
        Formula f = new("8.1 + 10.5");
        Assert.AreEqual(18.6, (double)f.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    public void EvaluateAdditionWithVariables()
    {
        Formula f = new("8.1 + x");
        Assert.AreEqual(18.6, (double)f.Evaluate(s => 10.5), 1e-9);
    }

    [TestMethod]
    public void EvaluateSubtraction()
    {
        Formula f = new("8.1 - 2.2");
        Assert.AreEqual(5.9, (double)f.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    public void EvaluateSubtractionWithVariables()
    {
        Formula f = new("8.1 - x");
        Assert.AreEqual(5.9, (double)f.Evaluate(s => 2.2), 1e-9);
    }

    [TestMethod]
    public void EvaluateMultiplication()
    {
        Formula f = new("8.1 * 10.5");
        Assert.AreEqual(85.05, (double)f.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    public void EvaluateMultiplicationWithVariables()
    {
        Formula f = new("8.1 * x");
        Assert.AreEqual(85.05, (double)f.Evaluate(s => 10.5), 1e-9);
    }

    [TestMethod]
    public void EvaluateDivision()
    {
        Formula f = new("8.1 / 2.5");
        Assert.AreEqual(3.24, (double)f.Evaluate(s => 0), 1e-9);
    }

    [TestMethod]
    public void EvaluateDivisionWithVariables()
    {
        Formula f = new("8.1 / x");
        Assert.AreEqual(3.24, (double)f.Evaluate(s => 2.5), 1e-9);
    }

    [TestMethod]
    public void EvaluateNormalizesVariables()
    {
        Formula f = new("1 + x", s => s.ToUpper(), x => true);
        Assert.AreEqual(2, (double)f.Evaluate(s => s == "X" ? 1 : 0), 1e-9);

    }

    [TestMethod]
    public void EvaluateComplex()
    {
        Formula f = new("(897 * 34 + ( 3 * afi532 + (2 * 34 + jale532) )) / (14 -ab)");
        Assert.AreEqual(2549.25, (double)f.Evaluate(s => s.Length), 1e-9);

    }

    //get Variables Tests

    [TestMethod]
    public void GetVarables()
    {
        Formula f = new("A + B + C");
        var expected = new HashSet<String> { "A", "B", "C" };
        Assert.IsTrue(expected.SetEquals(f.GetVariables()));

    }

    [TestMethod]
    public void GetVarablesNotDupes()
    {
        Formula f = new("A + A+ B + C");
        var expected = new HashSet<String>();
        foreach (var v in f.GetVariables())
            Assert.IsTrue(expected.Add(v));
    }

    [TestMethod]
    public void GetVariablesNormalizes()
    {
        Formula f = new("A + B + C", s => s.ToLower(), s=> true);
        var expected = new HashSet<String> { "a", "b", "c" };
        Assert.IsTrue(expected.SetEquals(f.GetVariables()));
    }

    //HashcodeTests
    [TestMethod]
    public void GetHashCodeIsEqualAfterNormalize()
    {
        Formula f1 = new("A + B + C", s => s.ToLower(), s => true);
        Formula f2 = new("a + b + c", s => s.ToLower(), s => true);

        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeIsEqualWithWhiteSpace()
    {
        Formula f1 = new("a     + b + c", s => s.ToLower(), s => true);
        Formula f2 = new("a + b   + c   ", s => s.ToLower(), s => true);

        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }


    //Equals
    [TestMethod]
    public void EqualsTestTrue()
    {
        Formula f1 = new("A + B + C", s => s.ToLower(), s => true);
        Formula f2 = new("a + b + c", s => s.ToLower(), s => true);

        Assert.IsTrue(f1.Equals(f2));
    }

    [TestMethod]
    public void EqualsTestFalseForOrder()
    {
        Formula f1 = new("2 + 1");
        Formula f2 = new("1 + 2");

        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    public void EqualsTestScientificNoation()
    {
        Formula f1 = new("1e1");
        Formula f2 = new("10");
        Assert.IsTrue(f1.Equals(f2));
    }


    [TestMethod]
    public void EqualsTestNull()
    {
        Formula f1 = new("A + B + C", s => s.ToLower(), s => true);


        Assert.IsFalse(f1.Equals(null));
    }

    [TestMethod]
    public void EqualsOverloadTestTrue()
    {
        Formula f1 = new("A + B + C", s => s.ToLower(), s => true);
        Formula f2 = new("a + b + c", s => s.ToLower(), s => true);

        Assert.IsTrue(f1 == f2);
    }
    [TestMethod]
    public void EqualsOverloadTestFalseForOrder()
    {
        Formula f1 = new("2 + 1");
        Formula f2 = new("1 + 2");

        Assert.IsFalse(f1 == (f2));
    }

    [TestMethod]
    public void NotEqualsOverloadTestFalse()
    {
        Formula f1 = new("A + B + C", s => s.ToLower(), s => true);
        Formula f2 = new("a + b + c", s => s.ToLower(), s => true);

        Assert.IsFalse(f1 != f2);
    }

    [TestMethod]
    public void NotEqualsOverloadTestTrueForOrder()
    {
        Formula f1 = new("2 + 1");
        Formula f2 = new("1 + 2");

        Assert.IsTrue(f1 != (f2));
    }

    //ToStringTests
    [TestMethod]
    public void ToStringNormalize()
    {
        Formula f1 = new("A+B+C", s => s.ToLower(), s => true);


        Assert.AreEqual("a+b+c", f1.ToString());
    }

    [TestMethod]
    public void ToStringRemoveWhiteSpace()
    {
        Formula f1 = new("   1.2 +a  +b+   c", s => s.ToLower(), s => true);


        Assert.AreEqual("1.2+a+b+c", f1.ToString());
        
    }



}