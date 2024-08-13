using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    [CommonSnippet(CommonSnippets.FindPaged)]
    public class FindPagedSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FindPagedSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new MySqlExpressionTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new MySqlDbTypeNameMapper()
            })
        {
        }

        protected override void DeclareUnSupportedFeatures(ISupportDeclaration declaration)
        {
            base.DeclareUnSupportedFeatures(declaration);

            declaration.NotSupportedRepetitionHandling();
            declaration.NotSupportedDbObjectNameOverriding();
        }
        
        private readonly string _keyDefaultOrderColumnName = GenerateKey();
        private readonly string _keyFindPagedProcedureName = GenerateKey();
        

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyFullTreeView = GenerateKey();

        private readonly string _keyIdFieldName = GenerateKey();
        private readonly string _keyIdFieldType = GenerateKey();
        private readonly string _keyIdFieldNameFullTree = GenerateKey();

        private readonly string _keyIndexProcedureName = GenerateKey();
        private readonly string _keySearchIndexTableName = GenerateKey();
        
        private readonly string _keyEntityFilterSegment = GenerateKey();
        
        private readonly string _keyColumns = GenerateKey();
        private readonly string _keyTableDotColumns = GenerateKey();
        private readonly string _keyCteDotColumns = GenerateKey();
        private readonly string _keyCorpusFieldType = GenerateKey();
        
        

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyFindPagedProcedureName,
                ProcessedType.NameConvention.FindPagedProcedureName);
            
            
            replacementList.Add(_keyDefaultOrderColumnName,
                ProcessedType.Parameters.First().Name);
           
           replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);
            replacementList.Add(_keyFullTreeView, ProcessedType.NameConvention.FullTreeViewName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");
            replacementList.Add(_keyIdFieldType,
                ProcessedType.HasId ? ProcessedType.IdParameter.Type : "[NO-ID-FIELD]");

            replacementList.Add(_keyIdFieldNameFullTree,
                ProcessedType.HasId ? ProcessedType.IdParameterFullTree.Name : "[NO-ID-FIELD]");
            
            replacementList.Add(_keyIndexProcedureName, ProcessedType.NameConvention.IndexEntityProcedureName);
            
            replacementList.Add(_keySearchIndexTableName, ProcessedType.NameConvention.SearchIndexTableName);
            
            replacementList.Add(_keyCorpusFieldType, ProcessedType.IndexCorpusParameter.Type);
            
            var entityFilterExpression = GetFiltersWhereClause(ColumnNameTranslation.ColumnNameOnly);
            
            var entityFilterSegment = entityFilterExpression.Success ? $" {entityFilterExpression.Value} " : " (1=1) ";
            
            replacementList.Add(_keyEntityFilterSegment,entityFilterSegment);

            var insertParameters = ProcessedType.GetInsertParameters();
            
            var columns = string.Join(',', insertParameters.Select(p => p.Name));
            
            var tableDotColumns = string.Join(',', insertParameters.Select(p => ProcessedType.NameConvention.TableName+"."+p.Name));
            var cteDotColumns = string.Join(',', insertParameters.Select(p => "Results_CTE."+p.Name));

            replacementList.Add(_keyColumns, columns);
            replacementList.Add(_keyTableDotColumns, tableDotColumns);
            replacementList.Add(_keyCteDotColumns, cteDotColumns);
        }
        
        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFindPagedProcedureName}(
                                            IN Offset bigint(16),
                                            IN Size bigint(16),
                                            IN FilterExpression nvarchar(1024),
                                            IN SearchExpression nvarchar(1024),
                                            IN OrderExpression nvarchar(2014))
BEGIN
    SET @query ='';
    SET @over  = ' ORDER BY {_keyTableName}.{_keyDefaultOrderColumnName} ASC';
    SET @where  = '';
    SET @searchJoin  = '';
    SET @hasWhere = 0;
    
    IF OrderExpression IS NOT NULL AND LENGTH(TRIM(OrderExpression))>0 THEN
        SET @over = CONCAT(' ORDER BY ',OrderExpression);
    END IF;
    
    IF FilterExpression IS NOT NULL AND LENGTH(TRIM(FilterExpression))>0 THEN
        SET @where = FilterExpression;
        SET @hasWhere = 1;
    END IF;

    IF SearchExpression IS NOT NULL AND LENGTH(TRIM(SearchExpression))>0 THEN
            
        SET @searchJoin = ' JOIN {_keySearchIndexTableName} ON {_keyTableName}.{_keyIdFieldName} = {_keySearchIndexTableName}.ResultId';

        IF @hasWhere=1 THEN
            SET @where = CONCAT(@where,' AND ', SearchExpression);
        ELSE
            SET @where = SearchExpression;
            SET @hasWhere = 1;
        END IF;
    END IF;

    IF @hasWhere=1 THEN
        SET @where = CONCAT(' WHERE ', @where);
    END IF;

    SET @query = CONCAT(' select {_keyTableDotColumns} FROM {_keyTableName}', @searchJoin ,@where, @over,' LIMIT ',Offset,',',Size,';');

    PREPARE stmt FROM @query;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt; 
END;
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}