using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.DataBound)]
    public class DataBoundProcedureGenerator : ICodeGenerator
    {
        public RepetitionHandling RepetitionHandling { get; set; }

        private readonly List<Type> _childrenTypes;
        private readonly MeadowConfiguration _configuration;

        public DataBoundProcedureGenerator(Type entityType, MeadowConfiguration configuration)
        {
            _configuration = configuration;
            var ev = new ObjectEvaluator(entityType);

            _childrenTypes = ev.Map.Nodes
                .Where(n => !n.IsLeaf && !n.IsCollection && TypeCheck.IsModel(n.Type))
                .Select(n => n.Type).ToList();
        }

        public Code Generate()
        {
            var sb = new StringBuilder();

            var codeSeparator = "";

            var classNamesSeparator = "";
            
            var classNames = "";

            foreach (var type in _childrenTypes)
            {
                var cg = new EntityDataBoundProcedureGenerator(type, _configuration);

                var code = cg.Generate();

                sb.Append(codeSeparator).Append(code.Text);

                classNames += classNamesSeparator + type.Name;
                
                codeSeparator = LineMacro.CommentLine + "\n";
                classNamesSeparator = ", ";
            }

            return new Code
            {
                Name = "Data Bound Procedures for " + classNames,
                Text = sb.ToString()
            };
        }
    }
}