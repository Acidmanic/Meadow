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
    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertCodeSnippetGenerator : PostgreRepetitionHandlerProcedureGeneratorBase
    {
        public InsertCodeSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) : base(construction, configurations)
        {
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();
        private readonly string _keyEntityFilterSegmentWhere = GenerateKey();

        protected override string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.InsertProcedureName)
                .DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {

            var insertParameters = ProcessedType.GetInsertParameters();
            
            replacementList.Add(_keyParameters, string.Join(",",insertParameters
                .Select(p=> $"\"par_{p.Name}\" {p.Type}")));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var columns = string.Join(',', insertParameters.Select(p => p.Name.DoubleQuot()));
            var values = string.Join(',', insertParameters.Select(p => ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            var entityFilterSegmentWhere = entityFilterExpression.Success ? $" WHERE ({entityFilterExpression.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegmentWhere,entityFilterSegmentWhere);
        }

        protected override string Template => $@" 
        {KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$
        begin
            return query
            with {"InsertedRows".DoubleQuot()} as(
                insert into {_keyTableName} ({_keyColumns}) 
                values ({_keyValues})
                returning *
            ) 
            select * from {"InsertedRows".DoubleQuot()}{_keyEntityFilterSegmentWhere};
        end;
        $$ language plpgsql;".Trim();
    }
}