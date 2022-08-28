using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;

namespace Meadow.Contracts
{
    public interface IFieldAddressIdentifierTranslator
    {

        Dictionary<string, FieldKey> MapAddressesByIdentifier<TModel>(bool fullTree=true);
        
        Dictionary<string, FieldKey> MapAddressesByIdentifier(Type type,bool fullTree=true);
    }
}