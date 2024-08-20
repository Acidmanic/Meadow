using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Results;

namespace Meadow.Scaffolding.Models;

public class CollectiveIdentificationProfile
{
    public Dictionary<string, List<FieldKey>> IdentifiersByCollectionName { get; } = new();

    public Result<FieldKey> AutoValuedIdentifier { get; set; } = new Result<FieldKey>().FailAndDefaultValue();


    internal void AddPartialIdentifier(string collectionName, FieldKey value)
    {
        if (!IdentifiersByCollectionName.ContainsKey(collectionName))
        {
            IdentifiersByCollectionName.Add(collectionName, new List<FieldKey>());
        }

        IdentifiersByCollectionName[collectionName].RemoveAll(k => k.Equals(value));
        
        IdentifiersByCollectionName[collectionName].Add(value);
    }
}