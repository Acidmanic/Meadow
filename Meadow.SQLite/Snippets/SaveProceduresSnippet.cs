using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.Snippets;

/// <summary>
/// Analyzes the type for identifiers, produces the SaveProcedure snippets for each identifier set and
/// aggregates these snippets into one snippet (itself)
/// </summary>
[CommonSnippet(CommonSnippets.SaveProcedure)]
public class SaveProceduresSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public List<ISnippet> SaveByCollectionNames => CreateSaveSnippets();


    public string Template => "{SaveByCollectionNames}";


    private List<ISnippet> CreateSaveSnippets()
    {
        var snippets = new List<ISnippet>();
        
        if (Toolbox is { } toolbox)
        {
            snippets.Add(new TitleBarSnippet($"Save Procedures For Type: {toolbox.EntityType.Name}:{toolbox.EffectiveType.Name} Table: {toolbox.ProcessedType.NameConvention.TableName}"));
            
            var profile = toolbox.ProcessedType.RecordIdentificationProfile;

            foreach (var collectiveIdSet in profile.CollectiveIdentifiersByName)
            {
                var analysis = toolbox.ComponentsProcessor
                    .CreateSaveProcedureAnalysis(collectiveIdSet.Key, collectiveIdSet.Value.ToArray());
                
                snippets.Add(new SaveProcedureByCollectionNameSnippet(toolbox,analysis));
                
                snippets.Add(new CommentLineSnippet());
            }
            foreach (var singularIdSet in profile.SingularIdentifiersByName)
            {
                var analysis = toolbox.ComponentsProcessor
                    .CreateSaveProcedureAnalysis(singularIdSet.Key, singularIdSet.Value);
                
                snippets.Add(new SaveProcedureByCollectionNameSnippet(toolbox,analysis));
                
                snippets.Add(new CommentLineSnippet());
            }
        }

        return snippets;
    }
  
}

