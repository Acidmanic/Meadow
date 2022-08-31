using System;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.RelationalTranslation
{
    public interface IRelationalTranslator
    {


        TableDouble GetFullTreeTable(Type type);
        
        TableDouble GetFullTreeTable<TEntity>();

        IRelationalTranslator Initialize(IDataOwnerNameProvider dataOwnerNameProvider, char delimiter);
    }
}