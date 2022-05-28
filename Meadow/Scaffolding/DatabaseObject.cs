namespace Meadow.Scaffolding
{
    public class DatabaseObject
    {
        public string Name { get; set; }

        public DbObjectTypes Type { get; set; }

        public string UHash()
        {
            return UHash(this.Name, this.Type);
        }

        internal static string UHash(string name, DbObjectTypes type)
        {
            return type.ToString() + "::" + name;
        }
        
        
    }
}