using System;
using System.Collections.Generic;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class FilteringProceduresGenerator<TEntity> : FilteringProceduresGenerator
    {
        public FilteringProceduresGenerator() : base(typeof(TEntity))
        {
        }
    }
    
    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresGenerator:ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public FilteringProceduresGenerator(Type type) : base(new SqLiteTypeNameMapper())
        {
            if (RepetitionHandling != RepetitionHandling.Create)
            {
                LogUnSupportedRepetitionHandling("FilteringProcedures");
            }
            
            ProcessedType = Process(type);
        }

        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyIdFieldName = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyTableName,ProcessedType.NameConvention.TableName);
            
            replacementList.Add(_keyIdFieldName, ProcessedType.HasId?ProcessedType.IdParameter.Name:"[NO-ID-FIELD]");
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spRemoveExpiredFilterResults(@ExpirationTimeStamp INTEGER)
AS
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp >= @ExpirationTimeStamp;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerform{_keyTableName}FilterIfNeeded(@FilterHash TEXT,
                                                  @ExpirationTimeStamp INTEGER,
                                                  @FilterExpression TEXT)
AS
    INSERT INTO FilterResults (FilterHash, ResultId, ExpirationTimeStamp) 
    SELECT @FilterHash,Persons.Id,@ExpirationTimeStamp FROM {_keyTableName} WHERE &@FilterExpression 
    AND IIF((select count(Id) from FilterResults where FilterResults.FilterHash=@FilterHash)>0,false,true);

    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.FilterHash=@FilterHash;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spRead{_keyTableName}Chunk(@Offset INTEGER,
                                      @Size INTEGER,
                                      @FilterHash TEXT)
AS
    SELECT {_keyTableName}.* FROM {_keyTableName} INNER JOIN FilterResults ON {_keyTableName}.{_keyIdFieldName} = FilterResults.ResultId
    WHERE FilterResults.FilterHash=FilterHash LIMIT @Offset,@Size;  
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}