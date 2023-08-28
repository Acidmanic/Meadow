using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;
using Meadow.Sql;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FullTreeView)]
    public class FullTreeViewGenerator : SqlFullTreeViewGeneratorBase
    {
        public FullTreeViewGenerator(Type type) : base(type, new SqLiteTypeNameMapper())
        {
        }

        protected override string TaleTemplateText()
        {
            return
                @"
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------"
                    .Trim();
        }
    }
}