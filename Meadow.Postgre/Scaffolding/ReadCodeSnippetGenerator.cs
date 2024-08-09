using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

namespace Meadow.Postgre.Scaffolding
{
    public class ReadCodeSnippetGeneratorPlainOnly : ReadCodeSnippetGenerator
    {
        public ReadCodeSnippetGeneratorPlainOnly(Type type, MeadowConfiguration configuration, bool actById) : base(
            new SnippetConstruction
            {
                EntityType = type,
                MeadowConfiguration = configuration
            }, SnippetConfigurations.IdAware(!actById))
        {
        }

        protected override bool DisableCreateFullTree => true;
    }

    [CommonSnippet(CommonSnippets.ReadProcedure)]
    public class ReadCodeSnippetGeneratorFullTree : ReadCodeSnippetGenerator
    {
        protected override bool DisableCreateFullTree => false;


        public ReadCodeSnippetGeneratorFullTree(SnippetConstruction construction, SnippetConfigurations configurations)
            : base(construction, configurations)
        {
        }
    }


    public abstract class ReadCodeSnippetGenerator : PostgreRepetitionHandlerProcedureGeneratorBase, IIdAware
    {
        public bool ActById { get; set; }


        protected virtual bool DisableCreateFullTree => false;


        protected ReadCodeSnippetGenerator(SnippetConstruction construction, SnippetConfigurations configurations) :
            base(construction, configurations)
        {
        }


        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyParametersFullTree = GenerateKey();

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeViewName = GenerateKey();

        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyIdFieldValue = GenerateKey();

        private readonly string _keyProcedureNameFullTree = GenerateKey();
        private readonly string _keyEntityFilterSegment = GenerateKey();


        protected override string GetProcedureName()
        {
            return ProvideDbObjectNameSupportingOverriding(() =>
                ActById
                    ? ProcessedType.NameConvention.SelectByIdProcedureName
                    : ProcessedType.NameConvention.SelectAllProcedureName).DoubleQuot();
        }

        protected string GetProcedureNameFullTree()
        {
            return ProvideDbObjectNameSupportingOverriding(() =>
                ActById
                    ? ProcessedType.NameConvention.SelectByIdProcedureNameFullTree
                    : ProcessedType.NameConvention.SelectAllProcedureNameFullTree).DoubleQuot();
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
            
            var whereForEntityFilter = ActById ? " AND " : " WHERE ";
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $"{whereForEntityFilter}({entityFilterExpression.Value}) " : "";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);
        }

        protected override string Template => ActById ? ByIdTemplate : AllTemplate;


        private string ByIdPlainObjectTemplate =>
            $@"{KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns setof {_keyTableName} as $$ 
begin
return query
    select * from {_keyTableName} where {_keyIdFieldName} = {_keyIdFieldValue}{_keyEntityFilterSegment};
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
    select * from {_keyTableName}{_keyEntityFilterSegment};
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