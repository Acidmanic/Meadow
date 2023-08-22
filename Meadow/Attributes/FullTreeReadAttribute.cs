using System;
using System.Collections.Generic;

namespace Meadow.Attributes;

public class FullTreeReadAttribute:Attribute
{
    private List<string> MarkedMethods { get; } = new List<string>();
    protected bool AcceptsAll { get; set; }
    public FullTreeReadAttribute(params string[] markedMethods)
    {
        MarkedMethods.Clear();
        
        MarkedMethods.AddRange(markedMethods);

        AcceptsAll = markedMethods.Length == 0;
    }

    public bool AcceptsAsMarked(string name)
    {
        if (AcceptsAll)
        {
            return true;
        }
        
        foreach (var method in MarkedMethods)
        {
            if (name == method)
            {
                return true;
            }
        }

        return false;
    }

}