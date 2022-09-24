using System.Linq;
using Meadow.DataTypeMapping;

namespace Meadow.Postgre
{
    public static class TypeDatabaseDefinitionExtensions
    {
        public static TypeDatabaseDefinition UpdateForSerialTypes(this TypeDatabaseDefinition definition)
        {
            var mapper = new PostgreDbTypeNameMapper();

            foreach (var field in definition.FieldTypes)
            {
                if (field.Value == definition.IdField)
                {
                    var smallInt = mapper[typeof(byte)];
                    var bigInt = mapper[typeof(long)];

                    if (field.Value.DbTypeName == smallInt && field.Value.DbTypeName == bigInt)
                    {
                        field.Value.DbTypeName = "SERIAL";
                    }
                }
            }

            return definition;
        }
    }
}