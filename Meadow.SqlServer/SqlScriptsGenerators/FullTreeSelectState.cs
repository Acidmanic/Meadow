using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.SqlServer.SqlScriptsGenerators
{
    public class FullTreeSelectState
    {
        public string Joins { get; set; }

        public string Parameters { get; set; }

        public string TableName { get; set; }

        public string Sep { get; set; }

        public AccessTreeInformation Info { get; set; }

        public override string ToString()
        {
            return $"SELECT {Parameters} FROM {TableName} {Joins}";
        }
    }
}