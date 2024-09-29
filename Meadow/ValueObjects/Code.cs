namespace Meadow.ValueObjects;

public struct Code
{
    public string Value { get; }
    
    public Code(string? value)
    {
        Value = value ?? string.Empty;
    }


    public static implicit operator string(Code value) => value.Value;
    public static implicit operator Code(string? value) => new Code(value);
    
}