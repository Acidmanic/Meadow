using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.SQLite.SqlScriptsGenerators
{
    public class FilteringProceduresGenerator<TEntity> : FilteringProceduresGenerator
    {
        public FilteringProceduresGenerator(MeadowConfiguration configuration) : base(typeof(TEntity), configuration)
        {
        }
    }

    [CommonSnippet(CommonSnippets.FilteringProcedures)]
    public class FilteringProceduresGenerator : ByTemplateSqlGeneratorBase
    {
        protected ProcessedType ProcessedType { get; }

        public FilteringProceduresGenerator(Type type, MeadowConfiguration configuration)
            : base(new SqLiteTypeNameMapper(), configuration)
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
            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName);

            replacementList.Add(_keyIdFieldName,
                ProcessedType.HasId ? ProcessedType.IdParameter.Name : "[NO-ID-FIELD]");
        }

        protected override string Template => $@"
-- ---------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE spRemoveExpiredFilterResults(@ExpirationTimeStamp INTEGER)
AS
    DELETE FROM FilterResults WHERE FilterResults.ExpirationTimeStamp < @ExpirationTimeStamp;
GO
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spPerform{_keyTableName}FilterIfNeeded(@SearchId TEXT,
                                                  @ExpirationTimeStamp INTEGER,
                                                  @FilterExpression TEXT)
AS
    INSERT INTO FilterResults (SearchId, ResultId, ExpirationTimeStamp) 
    SELECT @SearchId,Persons.Id,@ExpirationTimeStamp FROM {_keyTableName} WHERE &@FilterExpression 
    AND IIF((select count(Id) from FilterResults where FilterResults.SearchId=@SearchId)>0,false,true);

    SELECT FilterResults.* FROM FilterResults WHERE FilterResults.SearchId=@SearchId;
GO
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE spRead{_keyTableName}Chunk(@Offset INTEGER,
                                      @Size INTEGER,
                                      @SearchId TEXT)
AS
    SELECT {_keyTableName}.* FROM {_keyTableName} INNER JOIN FilterResults ON {_keyTableName}.{_keyIdFieldName} = FilterResults.ResultId
    WHERE FilterResults.SearchId=SearchId LIMIT @Offset,@Size;  
GO
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
    }
}