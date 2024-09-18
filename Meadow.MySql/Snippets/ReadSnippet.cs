using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

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

        public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

        public string KeyTableName => Toolbox.TableOrFullViewName(_fullTree);
        public string KeyWhereClause => Toolbox.WhereByIdClause(_byId, _fullTree);
        public string KeyEntityFilterSegment => _fullTree ? string.Empty : Toolbox.GetEntityFiltersWhereClause($" {(_byId ? "AND " : "WHERE ")}", " ");

        public string Procedure(string content) => Toolbox.Procedure(
            Toolbox.Configurations.RepetitionHandling,
            Toolbox.GetReadProcedureName(_fullTree, _byId),
            content, string.Empty,
            Toolbox.ProcessedType.NameConvention.TableName,
            Toolbox.GetIdAwareProcedureDefinitionParameters(_byId));

        public ISnippet Line => new CommentLineSnippet();

        public string Semicolon => Toolbox.Semicolon();

        public string Template => @"
{Procedure}
    SELECT * FROM {KeyTableName}{KeyWhereClause}{KeyEntityFilterSegment}{Semicolon}
{/Procedure}
{Line}
".Trim();
    }

    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;

    public List<ISnippet> Items
    {
        get
        {
            var items = new List<ISnippet>();

            items.Add(new TitleBarSnippet("Read Procedures For Entity " + Toolbox.ProcessedType.EventIdType?.Name));

            if (Toolbox.ActsById())
            {
                if (Toolbox.ProcessedType.HasId)
                {
                    items.Add(new ReadSnippetBundle(false, true));
                    items.Add(new ReadSnippetBundle(true, true));
                }
                else
                {
                    
                    items.Add(new TitleBarSnippet("Can not Create 'By-Id' procedures for entity without Id."));
                }
            }

            if (Toolbox.ActsAll())
            {
                items.Add(new ReadSnippetBundle(false, false));
                items.Add(new ReadSnippetBundle(true, false));
            }

            items.ForEach(s => s.Toolbox = Toolbox);

            return items;
        }
    }

    public string Template => "{Items}";
}