using System;

namespace Meadow.DataTypeMapping;

public abstract class SqlDbTypeNameMapperBase:DbTypeNameMapperBase
{
    
    protected override string AdjustSize(Type type, string databaseTypeName, long columnSize)
    {
        if (columnSize > 0)
        {
            databaseTypeName=OverrideSizeInParentheses(databaseTypeName, columnSize);
        }
        
        return databaseTypeName;
    }
}