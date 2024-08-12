using System.Collections.Generic;
using System.Linq;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;

namespace Meadow.SqlServer.Scaffolding.SqlScriptsGenerators
{
    [CommonSnippet(CommonSnippets.FindPaged)]
    public class FindPagedSnippetGenerator : ByTemplateSqlSnippetGeneratorBase
    {
        public FindPagedSnippetGenerator(SnippetConstruction construction,
            SnippetConfigurations configurations)
            : base(construction, configurations, new SnippetExecution()
            {
                SqlExpressionTranslator = new SqlServerExpressionTranslator(construction.MeadowConfiguration),
                TypeNameMapper = new SqlDbTypeNameMapper()
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

        private readonly string _keyRemoveExpiredFilterProcedure = GenerateKey();
        private readonly string _keyFilterResultsTableName = GenerateKey();

        private readonly string _keyIndexProcedureName = GenerateKey();
        private readonly string _keySearchIndexTableName = GenerateKey();
        
        private readonly string _keyEntityFilterSegment = GenerateKey();
        
        private readonly string _keyColumns = GenerateKey();
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

            replacementList.Add(_keyColumns, columns);
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyIndexProcedureName}2(@ResultId {_keyIdFieldType},@IndexCorpus {_keyCorpusFieldType}) AS
    
    IF EXISTS(SELECT 1 FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=@ResultId)
        BEGIN
            UPDATE {_keySearchIndexTableName} SET IndexCorpus=@IndexCorpus WHERE {_keySearchIndexTableName}.ResultId=@ResultId;
            
            SELECT TOP 1 * FROM {_keySearchIndexTableName} WHERE {_keySearchIndexTableName}.ResultId=@ResultId; 
        END
    ELSE
        BEGIN
            INSERT INTO {_keySearchIndexTableName} (ResultId,IndexCorpus) VALUES (@ResultId,@IndexCorpus)
            DECLARE @newId {_keyIdFieldType}=(IDENT_CURRENT('{_keySearchIndexTableName}'));
            SELECT * FROM {_keySearchIndexTableName} WHERE Id=@newId;
        END
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE {_keyFindPagedProcedureName}(
                                            @Offset BIGINT,
                                            @Size BIGINT,
                                            @FilterExpression NVARCHAR(1024),
                                            @SearchExpression NVARCHAR(1024),
                                            @OrderExpression NVARCHAR(1024))
AS
    declare @query nvarchar(1600);
    declare @over nvarchar(1024) = 'ORDER BY {_keyTableName}.{_keyDefaultOrderColumnName} ASC';
    declare @where nvarchar(1024) = '';
    declare @searchJoin nvarchar(1024) = '';

    IF NOT ISNULL(@OrderExpression,'')=''
        SET @over = CONCAT('ORDER BY ',@OrderExpression);

    SET @where = @FilterExpression;

    IF NOT ISNULL(@SearchExpression,'')=''
            SET @searchJoin = ' JOIN SearchIndex ON {_keyTableName}.{_keyIdFieldName} = {_keySearchIndexTableName}.ResultId';

    IF NOT ISNULL(@SearchExpression,'')=''
        IF NOT ISNULL(@where,'')=''
            SET @where = CONCAT(@where,' AND ', @SearchExpression);
        ELSE
            SET @where = @SearchExpression;

    IF NOT ISNULL(@where,'')=''
        SET @where = CONCAT(' WHERE ', @where);
        
    SET @query = CONCAT(';WITH Results_CTE AS ( SELECT *,',' ROW_NUMBER() OVER (',@over,') AS RowNum FROM {_keyTableName} ',
                        @searchJoin,
                        @where,') SELECT * FROM Results_CTE WHERE RowNum >=', @Offset+1,' AND RowNum <= ',@Offset+@Size);

    execute sp_executesql @query
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}