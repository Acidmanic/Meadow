using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveCodeSnippetGenerator : PostgreRepetitionHandlerProcedureGeneratorBase
    {
        public SaveCodeSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(
            construction, configurations)
        {
        }


        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNameValuesSet = GenerateKey();
        private readonly string _keyWhereExpression = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyEntityFilterSegmentAnd = GenerateKey();
        private readonly string _keyEntityFilterSegmentWhere = GenerateKey();

        protected override string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() =>
                ProcessedType.NameConvention.SaveProcedureName).DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            var parameters = string.Join(",\n", ProcessedType.Parameters
                .Select(p => ("par_" + p.Name).DoubleQuot() + " " + p.Type));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());


            var whereExpression = string.Join(" AND ", ProcessedType.NoneIdUniqueParameters
                .Select(p => p.Name.DoubleQuot() + " = " + ("par_" + p.Name).DoubleQuot()));

            if (ProcessedType.NoneIdUniqueParameters.Count == 0)
            {
                var id = ProcessedType.IdParameter.Name;

                whereExpression = $"\"{id}\" = \"par_{id}\"";
            }

            replacementList.Add(_keyWhereExpression, whereExpression);


            var nameValuesSet = string.Join(",\n", ProcessedType.NoneIdParameters
                .Select(p => p.Name.DoubleQuot() + " = " + ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyNameValuesSet, nameValuesSet);


            var insertParameters = ProcessedType.GetInsertParameters();
            
            var columns = string.Join(',', insertParameters.Select(p => p.Name.DoubleQuot()));
            var values = string.Join(',', insertParameters.Select(p => ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
            
            var entityFilterExpressionOwnerDotMember = GetFiltersWhereClause(ColumnNameTranslation.DataOwnerDotColumnName);
            var entityFilterExpressionColumnName = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegmentAnd = entityFilterExpressionOwnerDotMember.Success ? $" AND ({entityFilterExpressionOwnerDotMember.Value}) " : "";
            var entityFilterSegmentWhere = entityFilterExpressionColumnName.Success ? $" WHERE ({entityFilterExpressionColumnName.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegmentAnd,entityFilterSegmentAnd);
            replacementList.Add(_keyEntityFilterSegmentWhere,entityFilterSegmentWhere);
        }

        protected override string Template => $@"
{KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$
        declare 
            updateCount int := 0;
        begin
            updateCount := (select count(*) from {_keyTableName} where {_keyWhereExpression}{_keyEntityFilterSegmentAnd});
        if(updateCount>0) then
            return query
            with UpdatedRows as (
                update {_keyTableName} set 
                {_keyNameValuesSet}
                where {_keyWhereExpression}{_keyEntityFilterSegmentAnd}
                returning *
            )
            select * from UpdatedRows{_keyEntityFilterSegmentWhere};
        else
            return query
            with InsertedRows as(
                insert into {_keyTableName} ({_keyColumns}) 
                values ({_keyValues})
                returning *
            ) 
            select * from InsertedRows{_keyEntityFilterSegmentWhere};
        end if;
        end;
        $$ language plpgsql; ".Trim();
    }
}