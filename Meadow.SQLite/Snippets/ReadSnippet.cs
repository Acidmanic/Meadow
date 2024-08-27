using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.SQLite.Snippets;

[CommonSnippet(CommonSnippets.ReadProcedure)]
public class ReadSnippet : ISnippet
{
    private class ReadSnippetBundle : ISnippet
    {
        private readonly bool _fullTree;
        private readonly bool _byId;

        public ReadSnippetBundle(bool fullTree, bool byId)
        {
            this._fullTree = fullTree;
            _byId = byId;
        }

        public SnippetToolbox? Toolbox { get; set; }

        public string KeyHeaderCreation => T(t => t.CreateReadProcedurePhrase(_fullTree, _byId));

        public string KeyParametersDeclaration => T(t => t.GetReadProcedureDefinitionParametersPhrase(_byId));
        public string KeyTableName => T(t => t.TableOrFullViewName(_fullTree));
        public string KeyWhereClause => T(t => t.WhereByIdClause(_byId, _fullTree));
        public string KeyEntityFilterSegment => _fullTree ? string.Empty : T(t => t.GetEntityFiltersWhereClause($" {(_byId ? "AND " : string.Empty)}", " "));

        public ISnippet Line => new CommentLineSnippet();

        private string T(Func<SnippetToolbox, string> pickValue)
        {
            if (Toolbox is { } toolbox)
            {
                return pickValue(toolbox);
            }

            return string.Empty;
        }

        public string Template => @"
{KeyHeaderCreation}{KeyParametersDeclaration} AS
    SELECT * FROM {KeyTableName}{KeyWhereClause}{KeyEntityFilterSegment}
GO
{Line}
".Trim();
    }

    public SnippetToolbox? Toolbox { get; set; }

    public List<ISnippet> Items
    {
        get
        {
            if (Toolbox is { } toolbox)
            {
                var items = new List<ISnippet>();

                items.Add(new TitleBarSnippet("Read Procedures For Entity " + toolbox.ProcessedType.EventIdType?.Name));

                if (toolbox.ActsById())
                {
                    items.Add(new ReadSnippetBundle(false, false));
                    items.Add(new ReadSnippetBundle(true, false));
                }

                if (toolbox.ActsById())
                {
                    items.Add(new ReadSnippetBundle(false, true));
                    items.Add(new ReadSnippetBundle(true, true));
                }

                items.ForEach(s => s.Toolbox = toolbox);

                return items;
            }

            return new List<ISnippet>();
        }
    }

    public string Template => "{Items}";
}