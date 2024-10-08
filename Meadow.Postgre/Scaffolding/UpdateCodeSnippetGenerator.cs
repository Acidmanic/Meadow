using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateCodeSnippetGenerator : PostgreRepetitionHandlerProcedureGeneratorBase
    {
        public UpdateCodeSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNameValuesSet = GenerateKey();
        private readonly string _keyWhereExpression = GenerateKey();
        private readonly string _keyEntityFilterSegmentAnd = GenerateKey();
        private readonly string _keyEntityFilterSegmentWhere = GenerateKey();

        protected override string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.UpdateProcedureName)
                .DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameters = string.Join(",\n", ProcessedType.Parameters
                .Select(p => ("par_" + p.Name).DoubleQuot() + " " + p.Type));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var nameValuesSet = string.Join(",\n", ProcessedType.NoneIdParameters
                .Select(p => p.Name.DoubleQuot() + " = " + ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyNameValuesSet, nameValuesSet);

            var id = ProcessedType.IdParameter.Name;

            replacementList.Add(_keyWhereExpression, $"\"{id}\" = \"par_{id}\"");

            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.DataOwnerDotColumnName);

            var entityFilterSegmentAnd =
                entityFilterExpression.Success ? $" AND ({entityFilterExpression.Value}) " : "";
            var entityFilterSegmentWhere =
                entityFilterExpression.Success ? $" WHERE {entityFilterExpression.Value}" : "";

            replacementList.Add(_keyEntityFilterSegmentAnd, entityFilterSegmentAnd);
            replacementList.Add(_keyEntityFilterSegmentWhere, entityFilterSegmentWhere);
        }

        protected override string Template => $@"
{KeyCreationHeader} function {KeyProcedureName}(
    {_keyParameters}) returns setof {_keyTableName} as $$
        begin
            update {_keyTableName} set 
                {_keyNameValuesSet}
            where {_keyWhereExpression}{_keyEntityFilterSegmentAnd};

            return query
                 select * from {_keyTableName}where{_keyWhereExpression}{_keyEntityFilterSegmentAnd};
        end;
$$ language plpgsql;
            ".Trim();
    }
}