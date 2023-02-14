using System;
using System.Collections.Generic;
using Meadow.Contracts;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public class ReadSequenceProcedureGenerator<TEntity> : ByTemplateSqlGeneratorBase
    {
        public bool AllNotById { get; }

        public int Top { get; }

        public bool OrderAscending { get; }


        public ReadSequenceProcedureGenerator(bool allNotById, int top, bool orderAscending) : base(new MySqlDbTypeNameMapper())
        {
            AllNotById = allNotById;
            Top = top;
            OrderAscending = orderAscending;
        }

        private string GetOrder(bool useIdField, string idFieldName)
        {
            if (useIdField)
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

        protected string GetProcedureName(NameConvention nameConvention)
        {
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

        private readonly string _keyProcedureName = GenerateKey();
        private readonly string _keyIdParam = GenerateKey();
        private readonly string _keyTableName = GenerateKey();
        private readonly string _keyWhereClause = GenerateKey();
        private readonly string _keyTopClause = GenerateKey();
        private readonly string _keyOrderClause = GenerateKey();
        
        
        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            var process = Process<TEntity>();

            var name = GetProcedureName(process.NameConvention);
            
            replacementList.Add(_keyProcedureName,name);

            if (!AllNotById && !process.HasId)
            {
                throw new Exception("To be able to create a read-by-id procedure for a type, the type" +
                                    " must have an id field.");
            }

            replacementList.Add(_keyIdParam,AllNotById?"":("IN " + process.IdParameter.Name + " " + process.IdParameter.Type));
            
            replacementList.Add(_keyTableName,process.NameConvention.TableName);
            
            replacementList.Add(_keyWhereClause,AllNotById?"":
                ("WHERE " + process.NameConvention.TableName + "." 
                 + process.IdParameter.Name + " = " + process.IdParameter.Name ));

            var order = GetOrder(!AllNotById, process.IdParameter.Name);
            
            replacementList.Add(_keyOrderClause,order);

            replacementList.Add(_keyTopClause,GetTop());
            
            
        }

        protected override string Template => @$"
CREATE PROCEDURE {_keyProcedureName}({_keyIdParam})
BEGIN
    SELECT * FROM {_keyTableName} {_keyOrderClause} {_keyTopClause} {_keyWhereClause};
END;
";
    }
}