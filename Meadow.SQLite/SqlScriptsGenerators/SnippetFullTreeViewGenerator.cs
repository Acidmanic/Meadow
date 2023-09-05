using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Sql;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class SnippetFullTreeViewGenerator : SqlSnippetFullTreeViewGeneratorBase
    {
        public SnippetFullTreeViewGenerator(Type type, MeadowConfiguration configuration)
            : base(type, configuration, new SqLiteTypeNameMapper())
        {
        }

        protected override string GetCreationHeader()
        {
            var creationHeader = "CREATE VIEW";

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                creationHeader = "DROP VIEW IF EXISTS " + GetViewName() + ";" +
                                 "\nCREATE";
            }

            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                creationHeader = "CREATE VIEW IF NOT EXISTS ";
            }

            return creationHeader;
        }

        protected override string TaleTemplateText()
        {
            return Split;
        }
    }
}