using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Utility;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class SqlSnippetGeneratorBase : ICodeGenerator
    {
        protected IDbTypeNameMapper TypeNameMapper { get; }

        protected SnippetConstruction Construction { get; }
        protected SnippetConfigurations Configurations { get; }
        
        protected ISqlExpressionTranslator SqlExpressionTranslator { get; }
        
        protected SnippetExecution SnippetExecution { get; }

        protected SqlSnippetGeneratorBase(
            SnippetConstruction construction,
            SnippetConfigurations configurations, 
            SnippetExecution execution)
        {

            Construction = construction;
            Configurations = configurations;
            SnippetExecution = execution;

            TypeNameMapper = execution.TypeNameMapper;
            SqlExpressionTranslator = execution.SqlExpressionTranslator;
            
            EntityType = Construction.EntityType;

            EntityTypeOrOverridenEntityType = Configurations.OverrideEntityType
                ? Configurations.OverrideEntityType.Value(Construction)
                : Construction.EntityType;

            ProcessedType = EntityTypeUtilities.Process(EntityTypeOrOverridenEntityType,
                Construction.MeadowConfiguration, execution.TypeNameMapper);

            RegisteredFilter = GetRegisteredFilter();
        }

        protected ProcessedType ProcessedType { get; }

        protected Type EntityType { get; }

        protected Type EntityTypeOrOverridenEntityType { get; }

        protected FilterQuery RegisteredFilter { get; }

        private FilterQuery GetRegisteredFilter()
        {
            if (Configurations.OverrideEntityType)
            {
                if (Construction.MeadowConfiguration.Filters.ContainsKey(EntityTypeOrOverridenEntityType))
                {
                    return Construction.MeadowConfiguration.Filters[EntityTypeOrOverridenEntityType];
                }
            }
            else
            {
                if (Construction.MeadowConfiguration.Filters.ContainsKey(EntityType))
                {
                    return Construction.MeadowConfiguration.Filters[EntityType];
                }
            }

            return new FilterQuery();
        }

        protected RepetitionHandling RepetitionHandling => Configurations.RepetitionHandling;

        public abstract Code Generate();


        protected string ParameterNameTypeJoint(Parameter p, string namePrefix = "")
        {
            return namePrefix + p.Name + " " + p.Type;
        }

        protected string ParameterNameValueSetJoint(Parameter p, string valuePrefix = "")
        {
            return p.Name + " = " + valuePrefix + p.Name;
        }

        protected string ParameterNameTypeJoint(IEnumerable<Parameter> parameters, string delimiter,
            string namePrefix = "")
        {
            return string.Join(delimiter, parameters.Select(p => ParameterNameTypeJoint(p, namePrefix)));
        }

        protected string ParameterNameValueSetJoint(IEnumerable<Parameter> parameters, string delimiter,
            string valuePrefix = "")
        {
            return string.Join(delimiter, parameters.Select(p => ParameterNameValueSetJoint(p, valuePrefix)));
        }

        protected string ProvideDbObjectNameSupportingOverriding(Func<string> originalDbObjectNameProvider)
        {
            return Configurations.OverrideDbObjectName
                ? Configurations.OverrideDbObjectName.Value(Construction)
                : originalDbObjectNameProvider();
        }
    }
}