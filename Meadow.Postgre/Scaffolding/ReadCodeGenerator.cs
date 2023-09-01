using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Postgre.Scaffolding
{
    public class ReadCodeGeneratorPlainOnly : ReadCodeGenerator
    {
        public ReadCodeGeneratorPlainOnly(Type type, MeadowConfiguration configuration, bool byId) : base(type,
            configuration, byId)
        {
        }

        protected override bool DisableCreateFullTree => true;
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadCodeGeneratorFullTree : ReadCodeGenerator
    {
        public ReadCodeGeneratorFullTree(Type type, MeadowConfiguration configuration, bool byId) : base(type,
            configuration, byId)
        {
        }

        protected override bool DisableCreateFullTree => false;
    }


    public abstract class ReadCodeGenerator : PostgreByTemplateProcedureGeneratorBase
    {
        private bool ById { get; }

        protected virtual bool DisableCreateFullTree => false;

        public ReadCodeGenerator(Type type, MeadowConfiguration configuration, bool byId) : base(type, configuration)
        {
            ById = byId;
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyParametersFullTree = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeViewName = GenerateKey();

        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyIdFieldValue = GenerateKey();

        private readonly string _keyProcedureNameFullTree = GenerateKey();


        protected override string GetProcedureName()
        {
            return ById
                ? ProcessedType.NameConvention.SelectByIdProcedureName.DoubleQuot()
                : ProcessedType.NameConvention.SelectAllProcedureName.DoubleQuot();
        }

        protected string GetProcedureNameFullTree()
        {
            return ById
                ? ProcessedType.NameConvention.SelectByIdProcedureNameFullTree.DoubleQuot()
                : ProcessedType.NameConvention.SelectAllProcedureNameFullTree.DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyParameters,
                ("par_" + ProcessedType.IdParameter.Name).DoubleQuot() + " " + ProcessedType.IdParameter.Type);

            replacementList.Add(_keyParametersFullTree,
                ("par_" + ProcessedType.IdParameterFullTree.Name).DoubleQuot() + " " + ProcessedType.IdParameter.Type);

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());
            replacementList.Add(_keyFullTreeViewName, ProcessedType.NameConvention.FullTreeViewName.DoubleQuot());

            replacementList.Add(_keyIdFieldName, ProcessedType.IdParameter.Name.DoubleQuot());
            replacementList.Add(_keyIdFieldNameFullTree, ProcessedType.IdParameterFullTree.Name.DoubleQuot());

            replacementList.Add(_keyIdFieldValue, ("par_" + ProcessedType.IdParameter.Name).DoubleQuot());

            replacementList.Add(_keyProcedureNameFullTree, GetProcedureNameFullTree());
        }

        protected override string Template => ById ? ByIdTemplate : AllTemplate;


        private string ByIdPlainObjectTemplate =>
            $@"{KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$ 
begin
return query
    select * from {_keyTableName} where {_keyIdFieldName} = {_keyIdFieldValue};
end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();

        private string ByIdFullTreeTemplate => $@" 

{KeyCreationHeader} function {_keyProcedureNameFullTree}({_keyParameters}) returns setof {_keyFullTreeViewName} as $$ 
begin
return query
    select * from {_keyFullTreeViewName} where {_keyIdFieldNameFullTree} = {_keyIdFieldValue};
end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();

        private string ByIdTemplate => DisableCreateFullTree
            ? ByIdPlainObjectTemplate
            : ByIdPlainObjectTemplate + "\n" + ByIdFullTreeTemplate;


        private string AllPlainObjectTemplate => $@" 
{KeyCreationHeader} function {KeyProcedureName}() returns setof {_keyTableName} as $$ 
begin
return query
    select * from {_keyTableName};
end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();

        private string AllFullTreeTemplate => $@" 
{KeyCreationHeader} function {_keyProcedureNameFullTree}() returns setof {_keyFullTreeViewName} as $$ 
begin
return query
    select * from {_keyFullTreeViewName};
end;
$$ language plpgsql ;
-- ---------------------------------------------------------------------------------------------------------------------
-- SPLIT
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();

        private string AllTemplate => DisableCreateFullTree
            ? AllPlainObjectTemplate
            : AllPlainObjectTemplate + "\n" + AllFullTreeTemplate;
    }
}