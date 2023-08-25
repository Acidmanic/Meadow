using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.SaveProcedure)]
    public class SaveCodeGenerator : PostgreByTemplateProcedureGeneratorBase
    {
        public SaveCodeGenerator(Type type) : base(type)
        {
        }
        
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyNameValuesSet = GenerateKey();
        private readonly string _keyWhereExpression = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();

        protected override string GetProcedureName()
        {
            return ProcessedType.NameConvention.SaveProcedureName.DoubleQuot();
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
            
            var columns = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name.DoubleQuot()));
            var values = string.Join(',', ProcessedType.NoneIdParameters.Select(p => ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
        }

        protected override string Template => $@"
{KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$
        declare 
            updateCount int := 0;
        begin
            updateCount := (select count(*) from {_keyTableName} where {_keyWhereExpression});
        if(updateCount>0) then
            return query
                update {_keyTableName} set 
                {_keyNameValuesSet}
                where {_keyWhereExpression}
                returning *;
        else
            return query
                insert into {_keyTableName} ({_keyColumns}) 
                values ({_keyValues})
                returning *;
        end if;
        end;
        $$ language plpgsql; ".Trim();
    }
}