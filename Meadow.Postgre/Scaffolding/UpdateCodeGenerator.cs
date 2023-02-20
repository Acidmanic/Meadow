using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.UpdateProcedure)]
    public class UpdateCodeGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }


        public UpdateCodeGenerator(Type type) : base(new PostgreDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNameValuesSet = GenerateKey();
        private readonly string _keyWhereExpression = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.UpdateProcedureName.DoubleQuot());

            var parameters = string.Join(",\n", ProcessedType.Parameters
                .Select(p => ("par_" + p.Name).DoubleQuot() + " " + p.Type));

            replacementList.Add(_keyParameters, parameters);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var nameValuesSet = string.Join(",\n", ProcessedType.NoneIdParameters
                .Select(p => p.Name.DoubleQuot() + " = " + ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyNameValuesSet, nameValuesSet);

            var id = ProcessedType.IdParameter.Name;
            
            replacementList.Add(_keyWhereExpression,$"\"{id}\" = \"par_{id}\"");
        }

        protected override string Template => $@"
create or replace function {_keyProcedureName}(
    {_keyParameters}) returns setof {_keyTableName} as $$
        begin
            return query
                update {_keyTableName} set 
                {_keyNameValuesSet}
            where {_keyWhereExpression}
            returning*;
        end;
$$ language plpgsql;
            ".Trim();
    }
}