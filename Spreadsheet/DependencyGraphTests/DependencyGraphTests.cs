using SpreadsheetUtilities;

namespace DevelopmentTests;

/// <summary>
///This is a test class for DependencyGraphTest and is intended
///to contain all DependencyGraphTest Unit Tests (once completed by the student)
///</summary>
[TestClass()]
public class DependencyGraphTest
{

    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void SimpleEmptyTest()
    {
        DependencyGraph t = new DependencyGraph();
        Assert.AreEqual(0, t.NumDependencies);
    }


    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void SimpleEmptyRemoveTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
        t.RemoveDependency("x", "y");
        Assert.AreEqual(0, t.NumDependencies);
    }


    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void EmptyEnumeratorTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
        Assert.IsTrue(e1.MoveNext());
        Assert.AreEqual("x", e1.Current);
        IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
        Assert.IsTrue(e2.MoveNext());
        Assert.AreEqual("y", e2.Current);
        t.RemoveDependency("x", "y");
        Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
        Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
    }


    /// <summary>
    ///Replace on an empty DG shouldn't fail
    ///</summary>
    [TestMethod()]
    public void SimpleReplaceTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(t.NumDependencies, 1);
        t.RemoveDependency("x", "y");
        t.ReplaceDependents("x", new HashSet<string>());
        t.ReplaceDependees("y", new HashSet<string>());
    }



    ///<summary>
    ///It should be possibe to have more than one DG at a time.
    ///</summary>
    [TestMethod()]
    public void StaticTest()
    {
        DependencyGraph t1 = new DependencyGraph();
        DependencyGraph t2 = new DependencyGraph();
        t1.AddDependency("x", "y");
        Assert.AreEqual(1, t1.NumDependencies);
        Assert.AreEqual(0, t2.NumDependencies);
    }




    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void SizeTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        Assert.AreEqual(4, t.NumDependencies);
    }


    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void EnumeratorTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        // This is one of several ways of testing whether your IEnumerable
        // contains the right values. This does not require any particular
        // ordering of the elements returned.
        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        String s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        String s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }


    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void ReplaceThenEnumerate()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", new HashSet<string>());
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", new HashSet<string>() { "c" });
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
        t.ReplaceDependees("d", new HashSet<string>() { "b" });

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        String s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        String s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }



    /// <summary>
    ///Using lots of data
    ///</summary>
    [TestMethod()]
    public void StressTest()
    {
        // Dependency graph
        DependencyGraph t = new DependencyGraph();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = ("" + (char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = new HashSet<string>();
            dees[i] = new HashSet<string>();
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }
    }


    //My tests
    [TestMethod()]
    public void NumDependenciesAfterRemove()
    {
        //Adding 9 dependents to 1
        DependencyGraph dg = new();
        for (int i = 2; i <= 10; i++)
        {
            dg.AddDependency("1", "" + i); 

        }
        Assert.AreEqual(9, dg.NumDependencies);

        dg.RemoveDependency("1", "2");
        Assert.AreEqual(8, dg.NumDependencies);

        dg.RemoveDependency("1", "3");
        dg.RemoveDependency("1", "4");

        //After removing 3 we should have 6 left in the graph
        Assert.AreEqual(6, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependenciesAfterRemoveDuplicates()
    {
        DependencyGraph dg = new();

        //Adding 9 dependents to 1

        for (int i = 2; i <= 10; i++)
        {
            dg.AddDependency("1", "" + i);

        }
        Assert.AreEqual(9, dg.NumDependencies);

        dg.RemoveDependency("1", "2");
        Assert.AreEqual(8, dg.NumDependencies);

        dg.RemoveDependency("1", "2");
        dg.RemoveDependency("1", "2");

        //Should only have removed 1 as 2 of the calls where redudent
        Assert.AreEqual(8, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependenciesAfterReplaceDependents()
    {
        //Adding 9 dependents to 1
        DependencyGraph dg = new();
        for (int i = 2; i <= 10; i++)
        {
            dg.AddDependency("1", "" + i);
        }

        Assert.AreEqual(9, dg.NumDependencies);

        dg.ReplaceDependents("1", new string[]{"-1", "-2"});
        Assert.AreEqual(2, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependenciesAfterReplaceDependentsEmpty()
    {
        DependencyGraph dg = new();
        for (int i = 2; i <= 10; i++)
        {
            dg.AddDependency("1", "" + i);

        }
        Assert.AreEqual(9, dg.NumDependencies);

        dg.ReplaceDependents("1", Array.Empty<string>());
        Assert.AreEqual(0, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependenciesAfterReplaceDependees()
    {
        DependencyGraph dg = new();
        for (int i = 2; i <= 10; i++)
        {
            dg.AddDependency("" + i, "1");

        }
        Assert.AreEqual(9, dg.NumDependencies);

        dg.ReplaceDependees("1", new string[] { "-1", "-2" });
        Assert.AreEqual(2, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependenciesAfterReplaceDependeesEmpty()
    {
        DependencyGraph dg = new();
        for (int i = 2; i <= 10; i++)
        {
            dg.AddDependency("" + i, "1");

        }
        Assert.AreEqual(9, dg.NumDependencies);

        dg.ReplaceDependees("1", Array.Empty<string>());
        Assert.AreEqual(0, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependiesEmpty()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(0, dg.NumDependees("a"));
        Assert.AreEqual(0, dg.NumDependees("1"));
        Assert.AreEqual(0, dg.NumDependees("jakl;dfj;a"));
    }


    [TestMethod()]
    public void NumDependiesSimple()
    {
        DependencyGraph dg = new();
        for (int i = 2; i < 10; i++)
        {
            dg.AddDependency("" + i, "0");
            dg.AddDependency("" + i, "1");
        }
        Assert.AreEqual(8, dg.NumDependees("0"));
        Assert.AreEqual(8, dg.NumDependees("1"));
        Assert.AreEqual(16, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependiesAfterRemove()
    {
        DependencyGraph dg = new();
        for (int i = 2; i <= 5; i++)
        {
            dg.AddDependency("" + i, "0");
            dg.AddDependency("" + i, "1");
        }

        dg.RemoveDependency("2", "0");
        dg.RemoveDependency("3", "0");
        dg.RemoveDependency("4", "0");
        dg.RemoveDependency("5", "0");

        dg.RemoveDependency("2", "1");
        dg.RemoveDependency("3", "1");


        Assert.AreEqual(0, dg.NumDependees("0"));
        Assert.AreEqual(2, dg.NumDependees("1"));
        Assert.AreEqual(2, dg.NumDependencies);
    }


    [TestMethod()]
    public void NumDependiesAfterReplace()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees("1", new string[] { "2", "3", "4", "5" });
        dg.ReplaceDependents("1", new string[] { "6", "7", "8", "9", "10" });

        Assert.AreEqual(4, dg.NumDependees("1"));
        for (int i = 6; i <= 10; i++)
            Assert.AreEqual(1, dg.NumDependees("" + i));
    }


    [TestMethod()]
    public void HasDependentsEmpty()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependents("1"));
    }


    [TestMethod()]
    public void HasDependentsAfterRemove()
    {               
        DependencyGraph dg = new();
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");
        dg.RemoveDependency("1", "2");
        dg.RemoveDependency("1", "3");
        Assert.IsFalse(dg.HasDependents("1"));
    }


    [TestMethod()]
    public void HasDependentsAfterReplaceTrue()
    { 
        DependencyGraph dg = new();
        dg.ReplaceDependents("1", new string[] { "2", "3", "4" });
        Assert.IsTrue(dg.HasDependents("1"));
    }


    [TestMethod()]
    public void HasDependentsAfterReplaceFalse()
    {
        DependencyGraph dg = new();
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");
        dg.AddDependency("1", "4");
        dg.ReplaceDependents("1", Array.Empty<string>());
        Assert.IsFalse(dg.HasDependents("1"));
    }


    [TestMethod()]
    public void HasDependeesEmpty()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependees("1"));
    }


    [TestMethod()]
    public void HasDependeesAfterRemove()
    {
        DependencyGraph dg = new();
        dg.AddDependency("2", "1");
        dg.AddDependency("3", "1");
        dg.RemoveDependency("2", "1");
        dg.RemoveDependency("3", "1");
        Assert.IsFalse(dg.HasDependees("1"));
    }


    [TestMethod()]
    public void HasDependeesAfterReplaceTrue()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees("1", new string[] { "2", "3", "4" });
        Assert.IsTrue(dg.HasDependees("1"));
    }


    [TestMethod()]
    public void HasDependeesAfterReplaceFalse()
    {
        DependencyGraph dg = new();
        dg.AddDependency("2", "1");
        dg.AddDependency("3", "1");
        dg.AddDependency("4", "1");
        dg.ReplaceDependees("1", Array.Empty<string>());
        Assert.IsFalse(dg.HasDependees("1"));
    }


    [TestMethod()]
    public void GetDependentsEmpty()
    {
        DependencyGraph dg = new();
        var e = dg.GetDependents("1").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");
        dg.AddDependency("1", "4");

        e = dg.GetDependents("2").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        e = dg.GetDependents("3").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        e = dg.GetDependents("4").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
    }


    [TestMethod()] 
    public void GetDependentsSimple()
    {
        DependencyGraph dg = new();

        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");
        dg.AddDependency("1", "4");
        HashSet<string> expected = new(){"2","3","4"};
        HashSet<string> actual = new();
        var e = dg.GetDependents("1").GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsFalse(e.MoveNext());


        //Assert.AreEqual(expected, actual);
        Assert.IsTrue(expected.SetEquals(actual));
    }


    [TestMethod()]
    public void GetDependentsComplex()
    {
        DependencyGraph dg = new();

        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");
        dg.AddDependency("1", "4");
        dg.AddDependency("1", "5");
        dg.AddDependency("1", "6");
        dg.AddDependency("1", "7");
        dg.AddDependency("1", "8");


        dg.ReplaceDependents("1", new string[] { "2", "3", "8" });
        dg.RemoveDependency("1", "8");
        dg.AddDependency("1", "4");

        dg.ReplaceDependents("4", new string[] { "7", "9" });
        dg.RemoveDependency("4", "9");
        dg.AddDependency("4", "8");

        HashSet<string> expected = new() { "2", "3", "4" };
        HashSet<string> actual = new();
        var e = dg.GetDependents("1").GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsFalse(e.MoveNext());


        //Assert.AreEqual(expected, actual);
        Assert.IsTrue(expected.SetEquals(actual));


        expected = new() { "7", "8" };
        actual = new();

        e = dg.GetDependents("4").GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsFalse(e.MoveNext());

        Assert.IsTrue(expected.SetEquals(actual));
    }


    [TestMethod()]
    public void GetDependeesEmpty()
    {
        DependencyGraph dg = new();
        var e = dg.GetDependees("1").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        dg.AddDependency("2", "1");
        dg.AddDependency("3", "1");
        dg.AddDependency("4", "1");

        e = dg.GetDependees("2").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        e = dg.GetDependees("3").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        e = dg.GetDependees("4").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
    }


    [TestMethod()]
    public void GetDependeesSimple()
    {
        DependencyGraph dg = new();
        dg.AddDependency("2", "1");
        dg.AddDependency("3", "1");
        dg.AddDependency("4", "1");

        HashSet<string> expected = new() { "2", "3", "4" };
        HashSet<string> actual = new();
        var e = dg.GetDependees("1").GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsFalse(e.MoveNext());


        //Assert.AreEqual(expected, actual);
        Assert.IsTrue(expected.SetEquals(actual));
    }


    [TestMethod()]
    public void GetDependeesComplex()
    {
        DependencyGraph dg = new();

        dg.AddDependency("3", "1");
        dg.AddDependency("2", "1");
        dg.AddDependency("4", "1");
        dg.AddDependency("5", "1");
        dg.AddDependency("6", "1");
        dg.AddDependency("7", "1");
        dg.AddDependency("8", "1");


        dg.ReplaceDependees("1", new string[] { "2", "3", "8" });
        dg.RemoveDependency("8", "1");
        dg.AddDependency("4", "1");

        dg.ReplaceDependees("4", new string[] { "7", "9" });
        dg.RemoveDependency("9", "4");
        dg.AddDependency("8", "4");

        HashSet<string> expected = new() { "2", "3", "4" };
        HashSet<string> actual = new();
        var e = dg.GetDependees("1").GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsFalse(e.MoveNext());


        //Assert.AreEqual(expected, actual);
        Assert.IsTrue(expected.SetEquals(actual));


        expected = new() { "7", "8" };
        actual = new();

        e = dg.GetDependees("4").GetEnumerator();

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsTrue(e.MoveNext());
        actual.Add(e.Current);

        Assert.IsFalse(e.MoveNext());

        Assert.IsTrue(expected.SetEquals(actual));
        Assert.AreEqual(5, dg.NumDependencies);
    }


    [TestMethod()]
    public void ReplaceDependentsEmpty()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependents("1", new string[] { "2", "3", "4" });
        HashSet<string> expected = new() {"2", "3", "4"};

        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));

        expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("2")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("3")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("4")));

        Assert.AreEqual(3, dg.NumDependencies);
    }


    [TestMethod()]
    public void ReplaceDependentsKeepsDependees()
    {
        DependencyGraph dg = new();
        dg.AddDependency("10", "1");
        dg.AddDependency("11", "1");
        dg.AddDependency("12", "1");
        dg.AddDependency("10", "1");
        dg.AddDependency("1", "100");
        dg.AddDependency("1", "101");

        dg.ReplaceDependents("1", new string[] { "2", "3", "4" });
        HashSet<string> expected = new() { "2", "3", "4" };

        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));

        expected = new() { "10", "11", "12",  };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("1")));

        Assert.AreEqual(6, dg.NumDependencies);
    }


    [TestMethod()]
    public void ReplaceDependentsTwice()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependents("1", new string[] { "2", "3", "4" });
        dg.ReplaceDependents("1", new string[] { "5", "6", "7" });

        HashSet<string> expected = new() { "5", "6", "7" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));

        expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("5")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("6")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("7")));

        //Checking that the 1 was cleared as a dependies in 2, 3, and 4
        Assert.IsFalse(dg.HasDependees("2"));
        Assert.IsFalse(dg.HasDependees("3"));
        Assert.IsFalse(dg.HasDependees("4"));

        Assert.AreEqual(3, dg.NumDependencies);
    }


    [TestMethod()]
    public void ReplaceDependeesEmpty()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees("1", new string[] { "2", "3", "4" });
        HashSet<string> expected = new() { "2", "3", "4" };

        Assert.IsTrue(expected.SetEquals(dg.GetDependees("1")));

        expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("2")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("3")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("4")));

        Assert.AreEqual(3, dg.NumDependencies);
    }


    [TestMethod()]
    public void ReplaceDependeesKeepsDependents()
    {
        DependencyGraph dg = new();
        dg.AddDependency("1", "10");
        dg.AddDependency("1", "11");
        dg.AddDependency("1", "12");
        dg.AddDependency("1", "10");
        dg.AddDependency("100", "1");
        dg.AddDependency("101", "1");

        dg.ReplaceDependees("1", new string[] { "2", "3", "4" });
        HashSet<string> expected = new() { "2", "3", "4" };

        Assert.IsTrue(expected.SetEquals(dg.GetDependees("1")));

        expected = new() { "10", "11", "12", };
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));

        Assert.AreEqual(6, dg.NumDependencies);
    }


    [TestMethod()]
    public void ReplaceDependeesTwice()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees("1", new string[] { "2", "3", "4" });
        dg.ReplaceDependees("1", new string[] { "5", "6", "7" });

        HashSet<string> expected = new() { "5", "6", "7" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("1")));

        expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("5")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("6")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("7")));

        //Checking that the 1 was cleared as a dependies in 2, 3, and 4
        Assert.IsFalse(dg.HasDependents("2"));
        Assert.IsFalse(dg.HasDependents("3"));
        Assert.IsFalse(dg.HasDependents("4"));

        Assert.AreEqual(3, dg.NumDependencies);
    }


    [TestMethod()]
    public void CycleWordsCorrectly()
    {
        DependencyGraph dg = new();
        dg.AddDependency("1", "2");
        dg.AddDependency("2", "1");
        Assert.AreEqual(2, dg.NumDependencies);

        Assert.IsTrue(dg.HasDependees("1"));
        Assert.IsTrue(dg.HasDependees("1"));
        Assert.IsTrue(dg.HasDependents("1"));
        Assert.IsTrue(dg.HasDependents("2"));

        HashSet<string> expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("2")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("2")));

        expected = new() { "2" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("1")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));
    }


    [TestMethod()]
    public void RemoveNonExistantDependenciesHasNoEffect()
    {
        DependencyGraph dg = new();
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");

        dg.RemoveDependency("1", "4");
        dg.RemoveDependency("2", "3");
        dg.RemoveDependency("10", "3");
        dg.RemoveDependency("3", "10");

        Assert.AreEqual(2, dg.NumDependencies);

        HashSet<string> expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("2")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("3")));

        expected = new() { "2", "3" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));
    }


    [TestMethod()]
    public void AddingDuplicateDependenciesHasNoEffect()
    {
        DependencyGraph dg = new();
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "3");
        dg.AddDependency("1", "2");
        dg.AddDependency("1", "2");


        Assert.AreEqual(2, dg.NumDependencies);

        HashSet<string> expected = new() { "1" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("2")));
        Assert.IsTrue(expected.SetEquals(dg.GetDependees("3")));

        expected = new() { "2", "3" };
        Assert.IsTrue(expected.SetEquals(dg.GetDependents("1")));
    }
}

