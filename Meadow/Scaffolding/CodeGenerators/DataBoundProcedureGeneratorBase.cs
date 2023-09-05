using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.Scaffolding.Macros;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class DataBoundProcedureGeneratorBase : ICodeGenerator
    {
        public RepetitionHandling RepetitionHandling { get; set; }

        private readonly List<Type> _childrenTypes;
        
        
        protected SnippetConstruction Construction { get; }
        protected SnippetConfigurations Configurations { get; }

        public DataBoundProcedureGeneratorBase(SnippetConstruction construction,
            SnippetConfigurations configurations)
        {
            Construction = construction;
            Configurations = configurations;
            
            var ev = new ObjectEvaluator(construction.EntityType);

            _childrenTypes = ev.Map.Nodes
                .Where(n => !n.IsLeaf && !n.IsCollection && TypeCheck.IsModel(n.Type,true))
                .Select(n => n.Type).ToList();
            
        }
        
        protected abstract bool DelimitByLineNotSplit { get; }

        protected string Delimiter => DelimitByLineNotSplit
            ? LineMacro.CommentLine + "\n"
            : LineMacro.CommentLine + "\n-- SPLIT\n" + LineMacro.CommentLine + "\n"; 

        public Code Generate()
        {
            var sb = new StringBuilder();

            var codeSeparator = "";

            var classNamesSeparator = "";
            
            var classNames = "";

            var delimiter = Delimiter;

            foreach (var type in _childrenTypes)
            {
                var cg = CreateEntityDataBoundProcedureGenerator(type,Construction.MeadowConfiguration);

                var code = cg.Generate();

                sb.Append(codeSeparator).Append(code.Text);

                classNames += classNamesSeparator + type.Name;

                codeSeparator = delimiter;
                
                classNamesSeparator = ", ";
            }

            return new Code
            {
                Name = "Data Bound Procedures for " + classNames,
                Text = sb.ToString()
            };
        }

        
        protected abstract ICodeGenerator CreateEntityDataBoundProcedureGenerator(Type entityType,MeadowConfiguration configuration);
    }
}