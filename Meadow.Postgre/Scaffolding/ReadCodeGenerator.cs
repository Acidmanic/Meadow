using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadCodeGenerator : PostgreByTemplateProcedureGeneratorBase
    {
        private bool ById { get; }

        public ReadCodeGenerator(Type type, MeadowConfiguration configuration, bool byId) : base(type, configuration)
        {
            ById = byId;
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();

        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldValue = GenerateKey();

        protected override string GetProcedureName()
        {
            return ById
                ? ProcessedType.NameConvention.SelectByIdProcedureName.DoubleQuot()
                : ProcessedType.NameConvention.SelectAllProcedureName.DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyParameters,
                ("par_" + ProcessedType.IdParameter.Name).DoubleQuot() + " " + ProcessedType.IdParameter.Type);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name.DoubleQuot());
            replacementList.Add(_keyIdFieldValue, ("par_" + ProcessedType.IdParameter.Name).DoubleQuot());
        }

        protected override string Template => ById ? ByIdTemplate : AllTemplate;

        private string ByIdTemplate => $@" 
        {KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$ 
        begin
        return query
            select * from {_keyTableName} where {_keyIdFieldName} = {_keyIdFieldValue};
        end;
        $$ language plpgsql ;
            --SPLIT".Trim();

        private string AllTemplate => $@" 
        {KeyCreationHeader} function {KeyProcedureName}() returns setof {_keyTableName} as $$ 
        begin
        return query
            select * from {_keyTableName};
        end;
        $$ language plpgsql ;
            --SPLIT".Trim();
    }
}