using System;
using System.Collections.Generic;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.MySql.Scaffolding.MySqlScriptGenerators
{
    public abstract class MySqlProcedureGeneratorBase:ByTemplateSqlGeneratorBase
    {
        
        protected ProcessedType Processed { get; }
        protected readonly string KeyCreationHeader = GenerateKey();
        protected readonly string KeyProcedureName = GenerateKey();
        
        protected Type EntityType { get; }
        
        public MySqlProcedureGeneratorBase(Type entityType) : base(new MySqlDbTypeNameMapper())
        {
            EntityType = entityType;

            Processed = Process(entityType);
        }

        protected string GetCreationHeader()
        {
            if (RepetitionHandling == RepetitionHandling.Skip)
            {
                Console.WriteLine("WARNING: MySql Does not support skip repetition handling strategy, so " +
                                  "this code generator snippet would handle repetition as 'create' behavior.");
            }

            if (RepetitionHandling == RepetitionHandling.Alter)
            {
                return "DROP PROCEDURE IF EXISTS " + GetProcedureName() + ";" +
                       "\nCREATE PROCEDURE";
            }

            return "CREATE PROCEDURE";
        }

        protected override void AddReplacements(Dictionary<string, string> replacementList)
        {
            
            replacementList.Add(KeyProcedureName,GetProcedureName());
            
            replacementList.Add(KeyCreationHeader,GetCreationHeader());

            AddBodyReplacements(replacementList);
        }

        protected abstract string GetProcedureName();
        
        protected abstract void AddBodyReplacements(Dictionary<string, string> replacementList);
    }
}