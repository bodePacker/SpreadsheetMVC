﻿// Skeleton implementation by: Joe Zachary, Daniel Kopta, Travis Martin for CS 3500
// Last updated: August 2023 (small tweak to API)
//Assignment completed 9/8/23 by Timothy Blamires

namespace SpreadsheetUtilities;

/// <summary>
/// (s1,t1) is an ordered pair of strings
/// t1 depends on s1; s1 must be evaluated before t1
/// 
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
/// set, and the element is already in the set, the set remains unchanged.
/// 
/// Given a DependencyGraph DG:
/// 
///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///        (The set of things that depend on s)    
///        
///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///        (The set of things that s depends on) 
//
// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
//     dependents("a") = {"b", "c"}
//     dependents("b") = {"d"}
//     dependents("c") = {}
//     dependents("d") = {"d"}
//     dependees("a") = {}
//     dependees("b") = {"a"}
//     dependees("c") = {"a"}
//     dependees("d") = {"b", "d"}
/// </summary>
public class DependencyGraph
{
    private Dictionary<string, HashSet<string>> dependents;
    private Dictionary<string, HashSet<string>> dependees;
    private int p_NumDependencies;

    
    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
        dependents = new();
        dependees = new();
        p_NumDependencies = 0;
    }


    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// This is an example of a property.
    /// </summary>
    public int NumDependencies
    {
        get { return p_NumDependencies; }
    }


    /// <summary>
    /// Returns the size of dependees(s),
    /// that is, the number of things that s depends on.
    /// </summary>
    public int NumDependees(string s)
    {
        if(HasDependees(s))
            return dependees[s].Count;
        return 0;
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
        if(dependents.ContainsKey(s))
            return dependents[s].Count > 0;
        return false;
                
    }


    /// <summary>
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
        if (dependees.ContainsKey(s))
            return dependees[s].Count > 0;
        return false;

    }


    /// <summary>
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
        if (dependents.ContainsKey(s))
            return dependents[s];
        return Array.Empty<string>();
    }


    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
        if (dependees.ContainsKey(s))
            return dependees[s];

        //return an empty array if it is not in the graph
        return Array.Empty<string>();
    }


    /// <summary>
    /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
    /// 
    /// <para>This should be thought of as:</para>   
    /// 
    ///   t depends on s
    ///
    /// </summary>
    /// <param name="s"> s must be evaluated first. T depends on S</param>
    /// <param name="t"> t cannot be evaluated until s is</param>
    public void AddDependency(string s, string t)
    {
        //adding to dependents
        if (!dependees.ContainsKey(t))
        {
            dependees[t] = new();
        }

        if (dependees[t].Add(s))
            p_NumDependencies++;


        //adding to dependents
        if (!dependents.ContainsKey(s))
        {
            dependents[s] = new();
        }

        dependents[s].Add(t);
    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
        //remvoing from dependees
        if (dependees.ContainsKey(t))
        {
            if (dependees[t].Remove(s))
                p_NumDependencies--;
        }

        //remvoing from dependents
        if (dependents.ContainsKey(s))
        {
            dependents[s].Remove(t);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
        //removing old dependencies
        foreach(string r in GetDependents(s))
        {
            dependees[r].Remove(s);

            p_NumDependencies--;
        }

        //cleared after loop to avoid concurent modification bugs
        dependents[s] = new();

        foreach (string t in newDependents)
        {
            AddDependency(s, t);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
        //removing old dependencies
        foreach (string r in GetDependees(s))
        {
            dependents[r].Remove(s);
            p_NumDependencies--;
        }

        //cleared after loop to avoid concurent modification bugs
        dependees[s] = new();

        foreach (string t in newDependees)
        {
            AddDependency(t, s);
        }
    }

}


