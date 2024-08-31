using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.DataBound)]
public class DataBoundSnippet:ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public List<ISnippet> EntityDataBounds => CreateEntityDataBoundSnippets();

    private List<ISnippet> CreateEntityDataBoundSnippets()
    {
        if (Toolbox is { } toolbox)
        {
            var configuration = toolbox.Construction.MeadowConfiguration;

            var repetitionHandling = toolbox.Configurations.RepetitionHandling;
            
            var ev = new ObjectEvaluator(toolbox.ProcessedType.NameConvention.EntityType);

            var childrenTypes = ev.Map.Nodes
                .Where(n => !n.IsLeaf && !n.IsCollection && TypeCheck.IsModel(n.Type,true))
                .Select(n => n.Type).ToList();

            var snippets = childrenTypes.Select(t =>
                new EntityDataBoundSnippet(t,configuration,repetitionHandling));

            return new List<ISnippet>(snippets);
        }

        return new List<ISnippet>();
    }

    public string Template => $"{{{nameof(EntityDataBounds)}}}";
}