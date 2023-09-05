using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
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

        protected override string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() => ProcessedType.NameConvention.InsertProcedureName)
                .DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            
            replacementList.Add(_keyParameters, string.Join(",",ProcessedType.NoneIdParameters
                .Select(p=> $"\"par_{p.Name}\" {p.Type}")));

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var columns = string.Join(',', ProcessedType.NoneIdParameters.Select(p => p.Name.DoubleQuot()));
            var values = string.Join(',', ProcessedType.NoneIdParameters.Select(p => ("par_" + p.Name).DoubleQuot()));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyValues, values);
        }

        protected override string Template => $@" 
        {KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$
        begin
        return query
            insert into {_keyTableName} ({_keyColumns}) 
            values ({_keyValues})
        returning * ;
        end;
        $$ language plpgsql;".Trim();
    }
}