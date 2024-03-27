using System;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Dynamics;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Exceptions;

namespace Meadow.Requests.Common
{
    public static class ByIdMeadowRequestExtensions
    {
        
        public static object CreateIdShellFor<TId>(this Type entityType, TId id)
        {
            var idField = TypeIdentity.FindIdentityLeaf(entityType);

            if (idField == null)
            {
                throw new ModelMustHaveIdentifierException();
            }

            return new ModelBuilder("IdShell")
                .AddProperty(idField.Name, idField.Type,id)
                .BuildObject();
        }
        
        public static FieldKey? GetIdFieldKey(this Type entityType)
        {
            var idField = TypeIdentity.FindIdentityLeaf(entityType);

            if (idField == null)
            {
                return null;
            }

            return new ObjectEvaluator(entityType).FindCorrespondingKey(idField);
        }

        public static void IfHasIdField(this Type entityType, Action<FieldKey> onIdFound)
        {
            var idField = TypeIdentity.FindIdentityLeaf(entityType);

            if (idField != null)
            {
                var fieldKey =  new ObjectEvaluator(entityType).FindCorrespondingKey(idField);

                onIdFound(fieldKey);
            }
        }
        
        public static void IfHasIdField(this Type entityType, Action<AccessNode> onIdFound)
        {
            var idField = TypeIdentity.FindIdentityLeaf(entityType);

            if (idField != null)
            {
                onIdFound(idField);
            }
        }
    }
}