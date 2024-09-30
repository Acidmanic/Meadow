using Meadow.Enums;
using Meadow.Extensions;

namespace Meadow.ValueObjects;

public struct Code
{
    public string Value { get; }
    
    public Code(string? value,KnownWraps wrap = KnownWraps.None)
    {
        Value = (value ?? string.Empty).Wrap(wrap);
    }


    public static implicit operator string(Code value) => value.Value;
    public static implicit operator Code(string? value) => new Code(value);
    
}