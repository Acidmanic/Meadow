using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.DeleteProcedure)]
public class DeleteSnippet : ISnippet
{
    private class DeleteSnippetCase : ISnippet
    {
        public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

        private readonly bool _byId;

        public DeleteSnippetCase(bool byId)
        {
            _byId = byId;
        }

        public string ProcedureCreationPhrase => Toolbox.SqlTranslator.CreateProcedurePhrase(Toolbox.Configurations.RepetitionHandling,
            _byId ? Toolbox.ProcessedType.NameConvention.DeleteByIdProcedureName : Toolbox.ProcessedType.NameConvention.DeleteAllProcedureName);

        public string ByIdParameters => Toolbox.GetIdAwareProcedureDefinitionParametersPhrase(_byId);

        public string TableName => Toolbox.ProcessedType.NameConvention.TableName;

        public string KeyWhereClause => Toolbox.WhereByIdClause(_byId, false);

        public string Procedure(string body) => Toolbox.Procedure(
           Toolbox.Configurations.RepetitionHandling,
           _byId ? Toolbox.ProcessedType.NameConvention.DeleteByIdProcedureName : Toolbox.ProcessedType.NameConvention.DeleteAllProcedureName
           ,body,string.Empty,string.Empty,Toolbox.GetIdAwareProcedureDefinitionParameters(_byId));

        public string Template => @"
{Procedure}
    PRAGMA temp_store = 2; /* 2 means use in-memory */
    CREATE TEMP TABLE _Existing(Count INTEGER);
    INSERT INTO _Existing (Count) SELECT COUNT(*) FROM {TableName};
    DELETE FROM {TableName}{KeyWhereClause};
    INSERT INTO _Existing (Count) SELECT COUNT(*) FROM {TableName};
    SELECT CASE WHEN Count(DISTINCT Count)=2 THEN CAST(1 as bit) ELSE CAST(0 as bit) 
                END AS Success
                FROM _Existing;
    DROP TABLE _Existing;
{/Procedure}
".Trim();
    }

    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;


    public List<ISnippet> Items
    {
        get
        {
            var items = new List<ISnippet>();

            if (Toolbox is { } toolbox)
            {
                items.Add(new TitleBarSnippet($"Delete Procedures For Entity: {toolbox.ProcessedType.NameConvention.EntityType.Name}"));

                if (toolbox.ActsById())
                {
                    items.Add(new DeleteSnippetCase(true));

                    items.Add(new CommentLineSnippet());
                }

                if (toolbox.ActsAll())
                {
                    items.Add(new DeleteSnippetCase(false));

                    items.Add(new CommentLineSnippet());
                }

                items.ForEach(s => s.Toolbox = toolbox);
            }

            return items;
        }
    }


    public string Template => $"{{{nameof(Items)}}}";
}