using System;
using Meadow.DataTypeMapping;
using Meadow.Reflection;
using Meadow.Scaffolding.CodeGenerators;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    public class TableScriptGenerator : SqlGeneratorBase
    {

        public TableScriptGenerator(Type type) : base(type)
        {
        }
        
        public override Code Generate(bool alreadyExists)
        {
           
            var sep = "";
            var parameters = "";
            
            WalkThroughLeaves(false, leaf =>
            {
                string fieldName = leaf.Name;

                string typeName = TypeNameMapper[leaf.Type];

                string id = leaf.IsUnique ? " IDENTITY (1,1) NOT NULL PRIMARY KEY" : "";

                parameters += sep + fieldName + " " + typeName + id;

                sep = ", ";    
                
            });
            
            var tableScript = $"{CreateKeyWord(alreadyExists)} TABLE {TableName} (\n\t{parameters}\n\t)\n\n";

            return new Code
            {
                Name = TableName,
                Text = tableScript
            };
        }

        public override string SqlObjectName => TableName;
    }
}