using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Contracts
{
    public interface IRelationalIdentifierToStandardFieldMapper
    {

        Dictionary<string, FieldKey> MapAddressesByIdentifier<TModel>(bool fullTree=true);
        
        Dictionary<string, FieldKey> MapAddressesByIdentifier(Type type,bool fullTree=true);
    }
}