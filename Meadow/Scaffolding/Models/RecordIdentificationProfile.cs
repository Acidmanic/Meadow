using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Scaffolding.Models;

/// <summary>
/// Each Unique Field (Unique Members and AutoValued Members) is a singular record identifier,
/// and each collection of fields marked with CollectiveIdentifier attribute, form a CollectiveIdentification set
/// </summary>
public class RecordIdentificationProfile
{
    public static readonly string DefaultCollection = "Values";

    public Dictionary<string, List<FieldKey>> CollectiveIdentifiersByName { get; } = new();

    public Dictionary<string, FieldKey> SingularIdentifiersByName { get; } = new();

    internal void AddCollectiveIdentifierItem(string name, FieldKey value)
    {
        if (!CollectiveIdentifiersByName.ContainsKey(name))
        {
            CollectiveIdentifiersByName.Add(name, new List<FieldKey>());
        }

        CollectiveIdentifiersByName[name].RemoveAll(k => k.Equals(value));

        CollectiveIdentifiersByName[name].Add(value);
    }

    internal void AddSingularIdentifierItem(string name, FieldKey value)
    {
        if (SingularIdentifiersByName.ContainsKey(name))
        {
            SingularIdentifiersByName.Remove(name);
        }

        SingularIdentifiersByName.Add(name, value);
    }
}