using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class ReadSequenceProcedureGenerator<TEntity> : ReadSequenceProcedureGenerator
    {
        public ReadSequenceProcedureGenerator(MeadowConfiguration configuration, bool allNotById, int top,
            bool orderById = false,
            bool orderAscending = true)
            : base(typeof(TEntity), configuration, allNotById, top, orderById, orderAscending)
        {
        }
    }

    public class ReadSequenceProcedureGenerator : MySqlProcedureGeneratorBase
    {
        public bool AllNotById { get; }

        public int Top { get; }

        public bool OrderAscending { get; }


        public bool OrderById { get; }

        public ReadSequenceProcedureGenerator(Type type, MeadowConfiguration configuration, bool allNotById, int top,
            bool orderById = false, bool orderAscending = true) : base(type, configuration)
        {
            AllNotById = allNotById;
            Top = top;
            OrderAscending = orderAscending;
            OrderById = orderById;
        }

        private string GetOrder(string idFieldName)
        {
            if (OrderById)
            {
                if (Top > 0)
                {
                    var ascDesc = OrderAscending ? "ASC" : "DESC";

                    return $"ORDER BY {idFieldName} {ascDesc}";
                }
            }

            return "";
        }

        private string GetTop()
        {
            if (Top > 0)
            {
                return $"LIMIT {Top}";
            }

            return "";
        }

        protected override string GetProcedureName()
        {
            var nameConvention = Processed.NameConvention;

            if (IsDatabaseObjectNameForced)
            {
                return ForcedDatabaseObjectName;
            }

            if (AllNotById)
            {
                if (Top > 0)
                {
                    if (OrderAscending)
                    {
                        return nameConvention.SelectFirstProcedureName;
                    }
                    else
                    {
                        return nameConvention.SelectLastProcedureName;
                    }
                }
                else
                {
                    return nameConvention.SelectAllProcedureName;
                }
            }
            else
            {
                return nameConvention.SelectByIdProcedureName;
            }
        }

        private readonly string _keyIdParam = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyTopClause = GenerateKey();
        private readonly string _keyOrderClause = GenerateKey();


        protected override void AddBodyReplacements(Dictionary<string, string> replacementList)
        {
            if (!AllNotById && !Processed.HasId)
            {
                throw new Exception("To be able to create a read-by-id procedure for a type, the type" +
                                    " must have an id field.");
            }

            replacementList.Add(_keyIdParam,
                AllNotById ? "" : ("IN " + Processed.IdParameter.Name + " " + Processed.IdParameter.Type));

            replacementList.Add(_keyTableName, Processed.NameConvention.TableName);

            replacementList.Add(_keyWhereClause, AllNotById
                ? ""
                : ("WHERE " + Processed.NameConvention.TableName + "."
                   + Processed.IdParameter.Name + " = " + Processed.IdParameter.Name));

            var order = GetOrder(Processed.IdParameter.Name);

            replacementList.Add(_keyOrderClause, order);

            replacementList.Add(_keyTopClause, GetTop());
        }

        protected override string Template => @$"
{KeyCreationHeader} {KeyProcedureName}({_keyIdParam})
BEGIN
    SELECT * FROM {_keyTableName} {_keyWhereClause} {_keyOrderClause} {_keyTopClause};
END;
".Trim();
    }
}