using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadCodeGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }
        
        private bool  ById { get; }

        public ReadCodeGenerator(Type type, bool byId) : base(new PostgreDbTypeNameMapper())
        {
            ById = byId;
            ProcessedType = Process(type);
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        
        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldValue = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ProcessedType.NameConvention.InsertProcedureName.DoubleQuot());

            replacementList.Add(_keyParameters, ("par_"+ProcessedType.IdParameter.Name).DoubleQuot() + " " + ProcessedType.IdParameter.Type);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());
            
            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name.DoubleQuot());
            replacementList.Add(_keyIdFieldValue, ("par_"+ProcessedType.IdParameter.Name).DoubleQuot());
            
        }

        protected override string Template => ById ? ByIdTemplate : AllTemplate; 

        private string ByIdTemplate => $@" 
        create or replace function {_keyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$ 
        begin
        return query
            select * from {_keyTableName} where {_keyIdFieldName} = {_keyIdFieldValue};
        end;
        $$ language plpgsql ;
            --SPLIT".Trim();
        
        private string AllTemplate => $@" 
        create or replace function {_keyProcedureName}() returns setof {_keyTableName} as $$ 
        begin
        return query
            select * from {_keyTableName};
        end;
        $$ language plpgsql ;
            --SPLIT".Trim();
    }
}