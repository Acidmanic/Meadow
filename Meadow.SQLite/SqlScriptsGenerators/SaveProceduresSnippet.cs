using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;

namespace Meadow.SQLite.SqlScriptsGenerators;


[CommonSnippet(CommonSnippets.SaveProcedure)]
public class SaveProceduresSnippet : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }


    public List<ISnippet> SaveByCollectionNames => CreateSaveSnippets();
   

    public string Template => $"{{{nameof(SaveByCollectionNames)}}}";
    
    
    
    private List<ISnippet> CreateSaveSnippets()
    {
        return new List<ISnippet>()
        {
            new Kido(),
            new Kido(),
            new Kido(),
            new Kido(),
            new Kido(),
        };
    }
}

public class Kido : ISnippet
{
    public SnippetToolbox? Toolbox { get; set; }
    public string Template => "-- --------------";
}