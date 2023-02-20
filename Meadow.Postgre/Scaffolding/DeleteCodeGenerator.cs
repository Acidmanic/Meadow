using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Models;

namespace Meadow.Postgre.Scaffolding
{
    [CommonSnippet(CommonSnippets.DeleteProcedure)]
    public class DeleteCodeGenerator : ByTemplateSqlGeneratorBase
    {
        private ProcessedType ProcessedType { get; }

        private bool ById { get; set; }

        public DeleteCodeGenerator(Type type) : base(new PostgreDbTypeNameMapper())
        {
            ProcessedType = Process(type);
        }


        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyParameters = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            replacementList.Add(_keyProcedureName, ById
                ? ProcessedType.NameConvention.DeleteByIdProcedureName.DoubleQuot()
                : ProcessedType.NameConvention.DeleteAllProcedureName.DoubleQuot());

            replacementList.Add(_keyParameters,
                ById
                    ? (("par_" + ProcessedType.IdParameter.Name).DoubleQuot() + " " + ProcessedType.IdParameter.Type)
                    : "");

            replacementList.Add(_keyTableName, ProcessedType.NameConvention.TableName.DoubleQuot());

            var whereClause = ById
                ? $" where \"{ProcessedType.IdParameter.Name}\" = \"{ProcessedType.IdParameter.Name}\""
                : "";

            replacementList.Add(_keyWhereClause, whereClause);
        }

        protected override string Template => $@"
create or replace function {_keyProcedureName}({_keyParameters}) returns TABLE({"Success".DoubleQuot()} bool) as $$
        declare
            count int : = 0;
            change int : = 0;
        begin
            count : = (select Count(*) from {_keyTableName});
            delete from {_keyTableName}{_keyWhereClause};
            change : = (select Count(*) from {_keyTableName});
            if change<count THEN
                return query select true as {"Success".DoubleQuot()};
            else
                return query select false as {"Success".DoubleQuot()};
            end if;
        end;
$$ language plpgsql;".Trim();
    }
}