using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Extensions;

public static class ObjectEvaluatorIdentifierExtensions
{
    public static FieldKey? FindIdField(this ObjectEvaluator ev)
    {
        var idLeaf = TypeIdentity.FindIdentityLeaf(ev.RootNode.Type);

        var idKey = ev.FindCorrespondingKey(idLeaf);

        return idKey;
    }

}