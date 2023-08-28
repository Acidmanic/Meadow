namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

public class CommonSnippetsClassed
{


    public static CommonSnippetsClassed CreateTable { get; } = new CommonSnippetsClassed("CreateTable", false);
    public static CommonSnippetsClassed DeleteProcedure { get; } = new CommonSnippetsClassed("DeleteProcedure", true);
    public static CommonSnippetsClassed InsertProcedure { get; } = new CommonSnippetsClassed("InsertProcedure", false);
    public static CommonSnippetsClassed ReadProcedure { get; } = new CommonSnippetsClassed("ReadProcedure", true);
    public static CommonSnippetsClassed UpdateProcedure { get; } = new CommonSnippetsClassed("UpdateProcedure", false);
    public static CommonSnippetsClassed SaveProcedure { get; } = new CommonSnippetsClassed("SaveProcedure", false);
    public static CommonSnippetsClassed EventSteamScript { get; } = new CommonSnippetsClassed("EventSteamScript", false);
    public static CommonSnippetsClassed FilteringProcedures { get; } = new CommonSnippetsClassed("FilteringProcedures", false);
    public static CommonSnippetsClassed FullTreeView { get; } = new CommonSnippetsClassed("FullTreeView", false);
    
    public string Name { get; }
    
    public bool IsIdAware { get;}

    private CommonSnippetsClassed(string name, bool isIdAware)
    {
        Name = name;
        IsIdAware = isIdAware;
    }
}