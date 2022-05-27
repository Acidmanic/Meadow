using Meadow.Reflection;

namespace Meadow.Scaffolding.SqlScriptsGenerators
{
    internal class FullTreeSelectState
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