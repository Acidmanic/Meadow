using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.InsertProcedure)]
    public class InsertCodeGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }

        public InsertCodeGenerator(Type type) : base(new PostgreDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyValues = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.InsertProcedureName.DoubleQuot());

            replacementList.Add(_keyParameters, string.Join(",",ProcessedType.NoneIdParameters
                .Select(p=> $"\"par_{p.Name}\" {p.Type}")));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var columns = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name.DoubleQuot()));
            var values = string.Join(',', ProcessedType.NoneIdParameters.Select(p => ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
        }

        protected override string Template => $@" 
        create or replace function {_keyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$
        begin
        return query
            insert into {_keyTableName} ({_keyColumns}) 
            values ({_keyValues})
        returning * ;
        end;
        $$ language plpgsql;".Trim();
    }
}