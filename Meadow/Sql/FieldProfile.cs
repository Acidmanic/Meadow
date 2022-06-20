using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Sql
{
    public class FieldProfile
    {
        public AccessNode Node { get; set; }

        public FieldKey Key { get; set; }

        public FieldProfile()
        {
        }

        public FieldProfile(FieldKey key, AccessNode node)
        {
            Node = node;
            Key = key;
        }
    }
}