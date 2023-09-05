using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteCodeSnippetGenerator : PostgreByTemplateProcedureSnippetGeneratorBase
    {
        private bool ById { get; }

        public DeleteCodeSnippetGenerator(Type type,MeadowConfiguration configuration, bool byId)
            : base(type,configuration)
        {
            ById = byId;
        }

        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();

        protected override string GetProcedureName()
        {
            return ById
                ? ProcessedType.NameConvention.DeleteByIdProcedureName.DoubleQuot()
                : ProcessedType.NameConvention.DeleteAllProcedureName.DoubleQuot();
        }

        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyParameters,
                ById
                    ? (("par_" + ProcessedType.IdParameter.Name).DoubleQuot() + " " + ProcessedType.IdParameter.Type)
                    : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var whereClause = ById
                ? $" where \"{ProcessedType.IdParameter.Name}\" = \"par_{ProcessedType.IdParameter.Name}\""
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
{KeyCreationHeader} function {KeyProcedureName}({_keyParameters}) returns TABLE({"Success".DoubleQuot()} bool) as $$
        declare
            count int := 0;
            change int := 0;
        begin
            count := (select Count(*) from {_keyTableName});
            delete from {_keyTableName}{_keyWhereClause};
            change := (select Count(*) from {_keyTableName});
            if change<count THEN
                return query select true as {"Success".DoubleQuot()};
            else
                return query select false as {"Success".DoubleQuot()};
            end if;
        end;
$$ language plpgsql;".Trim();
    }
}