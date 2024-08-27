using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public SnippetToolbox? Toolbox { get; set; }

        private readonly bool _byId;

        public DeleteSnippetCase(bool byId)
        {
            _byId = byId;
        }

        public string ProcedureCreationPhrase => T(t =>
            t.SqlTranslator.CreateProcedurePhrase(t.Configurations.RepetitionHandling,
                _byId ? t.ProcessedType.NameConvention.DeleteByIdProcedureName : t.ProcessedType.NameConvention.DeleteAllProcedureName));

        public string ByIdParameters => T(t => t.GetIdAwareProcedureDefinitionParametersPhrase(_byId));

        public string TableName => T(t => t.ProcessedType.NameConvention.TableName);

        public string KeyWhereClause => T(t => t.WhereByIdClause(_byId, false));


        private string T(Func<SnippetToolbox, string> pickValue)
        {
            if (Toolbox is { } toolbox)
            {
                return pickValue(toolbox);
            }

            return string.Empty;
        }

        public string Template => $@"
{{{nameof(ProcedureCreationPhrase)}}}{{{nameof(ByIdParameters)}}} AS

    PRAGMA temp_store = 2; /* 2 means use in-memory */
    CREATE TEMP TABLE _Existing(Count INTEGER);
    INSERT INTO _Existing (Count) SELECT COUNT(*) FROM {nameof(TableName)};
    DELETE FROM {{{nameof(TableName)}}}{{{nameof(KeyWhereClause)}}};
    INSERT INTO _Existing (Count) SELECT COUNT(*) FROM {{{nameof(TableName)}}};
    SELECT CASE WHEN Count(DISTINCT Count)=2 THEN CAST(1 as bit) ELSE CAST(0 as bit) 
                END AS Success
                FROM _Existing;
    DROP TABLE _Existing;
GO
".Trim();
    }

    public SnippetToolbox? Toolbox { get; set; }


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