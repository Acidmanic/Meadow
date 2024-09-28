using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets.Builtin.Models;

namespace Meadow.Scaffolding.Snippets.Builtin;

public class ReadAllProcedureSnippet : ISnippet
{

    private readonly SelectSnippetParameters _parameters;
    private readonly string _procedureName;

    public ReadAllProcedureSnippet(SelectSnippetParameters parameters, string procedureName)
    {
        _parameters = parameters;
        _procedureName = procedureName;
    }

    public ISnippetToolbox Toolbox { get; set; } = ISnippetToolbox.Null;

    private ISnippetToolbox T
    {
        get
        {
            var t = Toolbox.CloneFor(_parameters.EntityType);
    
            var b = new SnippetConfigurationBuilder(t.Configurations);
    
            _parameters.ManipulateToolbox(b);
    
            return new SnippetToolbox(t.Construction, b.Build());
        }
    }
    public string Procedure(string body) => Toolbox.Procedure(T.Configurations.RepetitionHandling,_procedureName,
        body,string.Empty,T.SourceName(),_parameters.InputParameters.ToArray());

    public ISnippet Select => new ReadAllSelectSnippet(_parameters);
    
    public string Template => @"
{Procedure}
    {Select}
{/Procedure}
";


}