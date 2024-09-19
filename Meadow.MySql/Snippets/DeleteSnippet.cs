using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

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

        public string TableName => Toolbox.ProcessedType.NameConvention.TableName;

        public string KeyWhereClause => Toolbox.WhereByIdClause(_byId, false);

        public string Semicolon => Toolbox.Semicolon();

        public string Procedure(string body) => Toolbox.Procedure(
            Toolbox.Configurations.RepetitionHandling,
            _byId
                ? Toolbox.ProcessedType.NameConvention.DeleteByIdProcedureName
                : Toolbox.ProcessedType.NameConvention.DeleteAllProcedureName
            , body, string.Empty, string.Empty, Toolbox.GetIdAwareProcedureDefinitionParameters(_byId));

        public string ExistingVar => "@existingCount";
        public string RemainingVar => "@remained";


        public string Template => @"
{Procedure}
    SET {ExistingVar} = (SELECT COUNT(*) FROM {TableName}{KeyWhereClause}){Semicolon}
    DELETE FROM {TableName}{KeyWhereClause}{Semicolon}
    SET {RemainingVar} = (SELECT COUNT(*) FROM {TableName}{KeyWhereClause}){Semicolon}
    SELECT ({ExistingVar}>0 AND {RemainingVar} < {ExistingVar} ) AS 'Success'{Semicolon}
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
                items.Add(new TitleBarSnippet(
                    $"Delete Procedures For Entity: {toolbox.ProcessedType.NameConvention.EntityType.Name}"));

                if (toolbox.ActsById())
                {
                    if (toolbox.ProcessedType.HasId)
                    {
                        items.Add(new DeleteSnippetCase(true));

                        items.Add(new CommentLineSnippet());
                    }
                    else
                    {
                        items.Add(new TitleBarSnippet("Can not Add 'by-id' procedure for entity without Id field"));
                    }
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


    public string Template => "{Items}";
}