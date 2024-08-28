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

        public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

        public string KeyHeaderCreation => Toolbox.CreateReadProcedurePhrase(_fullTree, _byId);

        public string KeyParametersDeclaration => Toolbox.GetIdAwareProcedureDefinitionParametersPhrase(_byId);
        public string KeyTableName => Toolbox.TableOrFullViewName(_fullTree);
        public string KeyWhereClause => Toolbox.WhereByIdClause(_byId, _fullTree);
        public string KeyEntityFilterSegment => _fullTree ? string.Empty : 
            Toolbox.GetEntityFiltersWhereClause($" {(_byId ? "AND " : "WHERE ")}", " ");

        public ISnippet Line => new CommentLineSnippet();

        public string Template => @"
{KeyHeaderCreation}{KeyParametersDeclaration} AS
    SELECT * FROM {KeyTableName}{KeyWhereClause}{KeyEntityFilterSegment}
GO
{Line}
".Trim();
    }

    public SnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public List<ISnippet> Items
    {
        get
        {
            var items = new List<ISnippet>();

            items.Add(new TitleBarSnippet("Read Procedures For Entity " + Toolbox.ProcessedType.EventIdType?.Name));

            if (Toolbox.ActsById())
            {
                items.Add(new ReadSnippetBundle(false, false));
                items.Add(new ReadSnippetBundle(true, false));
            }

            if (Toolbox.ActsAll())
            {
                items.Add(new ReadSnippetBundle(false, true));
                items.Add(new ReadSnippetBundle(true, true));
            }

            items.ForEach(s => s.Toolbox = Toolbox);

            return items;
        }
    }

    public string Template => "{Items}";
}