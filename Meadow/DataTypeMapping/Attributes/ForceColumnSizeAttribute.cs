using System;

namespace Meadow.DataTypeMapping.Attributes;


public class ForceColumnSizeAttribute:Attribute
{
    public ForceColumnSizeAttribute(long columnSize)
    {
        ColumnSize = columnSize;
    }

    public long ColumnSize { get; }
}