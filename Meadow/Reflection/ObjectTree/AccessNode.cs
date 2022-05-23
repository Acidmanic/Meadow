using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Reflection.ObjectTree
{
    public class AccessNode
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public AccessNode Parent { get; private set; }

        protected List<AccessNode> Children { get; set; }

        public bool IsLeaf => Children.Count == 0;

        protected PropertyInfo PropertyInfo { get; set; }

        public bool IsRoot => Parent == null;

        public bool IsUnique { get; }


        public bool IsCollectable { get; }


        public AccessNode(string name, Type type, PropertyInfo info, bool isUnique)
        {
            Name = name;

            PropertyInfo = info;

            IsCollectable = TypeCheck.IsCollection(type);

            Type = type;

            Parent = null;

            Children = new List<AccessNode>();

            IsUnique = isUnique;
        }

        public void Add(AccessNode child)
        {
            child.Parent = this;

            Children.Add(child);
        }

        public string GetFullName()
        {
            if (IsRoot)
            {
                return Name;
            }

            return Parent.GetFullName() + "." + Name;
        }

        public virtual void SetValue(object rootObject, object value)
        {
            var parentObject = Parent.GetSelfFromRoot(rootObject);

            PropertyInfo.SetValue(parentObject, value);
        }

        public virtual object GetValue(object rootObject)
        {
            return GetSelfFromRoot(rootObject);
        }

        private object GetSelfFromRoot(object rootObject)
        {
            if (IsRoot)
            {
                return rootObject;
            }

            var parentObject = Parent.GetSelfFromRoot(rootObject);

            var me = PropertyInfo.GetValue(parentObject);

            return me;
        }

        public List<AccessNode> EnumerateLeavesBelow()
        {
            var result = new List<AccessNode>();

            EnumerateLeavesBelow(result);

            return result;
        }

        private void EnumerateLeavesBelow(ICollection<AccessNode> result)
        {
            if (IsLeaf)
            {
                result.Add(this);
            }
            else
            {
                foreach (var child in Children)
                {
                    child.EnumerateLeavesBelow(result);
                }
            }
        }

        public Type ElementType
        {
            get
            {
                if (TypeCheck.Implements<ICollection>(Type))
                {
                    return Type.GenericTypeArguments[0];
                }

                if (TypeCheck.Extends<Array>(Type))
                {
                    return Type.GetElementType();
                }

                return null;
            }
        }

        internal List<AccessNode> GetChildren()
        {
            return new List<AccessNode>(Children);
        }
    }
}