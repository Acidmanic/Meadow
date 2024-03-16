using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.Casting;

namespace Meadow.Casting;

public class MeadowBuiltinCastList : List<ICast>
{
    public MeadowBuiltinCastList()
    {
        Initialize();
    }

    public MeadowBuiltinCastList(IEnumerable<ICast> collection) : base(collection)
    {
        Initialize();
    }

    public MeadowBuiltinCastList(int capacity) : base(capacity)
    {
        Initialize();
    }


    private void Initialize()
    {
        Add(new ExplicitCast<byte[], Guid>(b => new Guid(b)));

        Add(new ExplicitCast<string, Guid>(s =>
        {
            if (string.IsNullOrWhiteSpace(s)) return Guid.Empty;

            return Guid.Parse(s);
        }));
    }
}