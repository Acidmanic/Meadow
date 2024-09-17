using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.SnippetComposed;

namespace Meadow.Scaffolding.Macros.BuiltIn
{
    public class SnippetComposedWipAllMacro : SnippetComposedMacroBase
    {
        public override string Name { get; } = "WipAll";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.CreateTable);
            builder.Add(CommonSnippets.FullTreeView);
            builder.Add(CommonSnippets.InsertProcedure);
            builder.Add(CommonSnippets.UpdateProcedure);
            builder.Add(CommonSnippets.DeleteProcedure).BehaviorUseIdAware();
            builder.Add(CommonSnippets.ReadProcedure).BehaviorUseIdAware();
            builder.Add(CommonSnippets.SaveProcedure);
            builder.Add(CommonSnippets.DataBound).RepetitionHandling(RepetitionHandling.Alter);
            builder.Add(CommonSnippets.FindPaged);
            builder.Add(CommonSnippets.CreateTable)
                .BehaviorUseIdAgnostic()
                .OverrideEntityTypeBySearchIndex()
                .OverrideDbObjectNameToSearchIndexTableName();
        }
    }
    
    public class SnippetComposedSaveOnlyMacro : SnippetComposedMacroBase
    {
        public override string Name { get; } = "WipSave";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.CreateTable);
            builder.Add(CommonSnippets.FullTreeView);
            builder.Add(CommonSnippets.InsertProcedure);
            builder.Add(CommonSnippets.UpdateProcedure);
            builder.Add(CommonSnippets.DeleteProcedure).BehaviorUseIdAware();
            builder.Add(CommonSnippets.ReadProcedure).BehaviorUseIdAware();
            builder.Add(CommonSnippets.SaveProcedure);
            builder.Add(CommonSnippets.DataBound);
            builder.Add(CommonSnippets.FindPaged);
        }
    }
    
    public class SnippetComposedWipEventStreamsOnlyMacro : SnippetComposedMacroBase
    {
        public override string Name { get; } = "WipEventStream";

        protected override void BuildUpAssemblingBehavior(AssemblingBehaviorBuilder builder)
        {
            builder.Add(CommonSnippets.EventStreamScript).BehaviorUseIdAgnostic();
        }
    }
}