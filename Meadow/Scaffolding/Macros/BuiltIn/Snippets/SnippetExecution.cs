using Meadow.Contracts;
using Meadow.DataTypeMapping;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class SnippetExecution
{
    public ISqlTranslator SqlTranslator { get; set; }
    
    public IDbTypeNameMapper TypeNameMapper { get; set; }
}