using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using Meadow.Enums;

namespace Meadow.Models;

public class SelectField
{

    public static readonly SelectField All = new SelectField()
    {
        Code = "*",
        Alias = null,
        Type = SelectFieldType.All
    };

    public string Code { get; set; } = string.Empty;
    
    public string? Alias { get; set; }
    
    public SelectFieldType Type { get; set; }
    
}