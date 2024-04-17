using System.Collections.Generic;
using Meadow.Scaffolding.Translators.Utilities;

namespace Meadow.Scaffolding.Translators.Contracts.Translatabels;

public class IfTranslatable:ITranslatable
{

    public record ElseIf(ITranslatable Condition, ITranslatable Content);
    
    public ITranslatable Condition { get; set; } = Translatable.Empty;

    public ITranslatable Content { get; set; } = Translatable.Empty;
    
    public ITranslatable? ElseContent { get; set; } =null;
    
    public List<ElseIf> ElseIfs { get; set; } = new();


    
    
    public virtual string Translate(int indent)
    {
        var ifScript = $"{S.Indent(indent)}IF ({Condition.Translate()}) THEN\n";

        ifScript += Content.Translate(indent+1)+"\n";

        foreach (var elseIf in ElseIfs)
        {
            ifScript += $"{S.Indent(indent)}ELSEIF {elseIf.Condition.Translate()}\n";

            ifScript += elseIf.Content.Translate(indent + 1)+"\n";
        }

        if (ElseContent != null)
        {
            ifScript += $"{S.Indent(indent)}ELSE\n";

            ifScript += ElseContent.Translate(indent + 1);
        }

        ifScript += $"END IF;";

        return ifScript;

    }
}