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


        public bool IsCollection { get; }


        public AccessNode(string name, Type type, PropertyInfo info, bool isUnique,int depth)
        {
            Name = name;

            PropertyInfo = info;

            IsCollection = TypeCheck.IsCollection(type);

            Type = type;

            Parent = null;

            Children = new List<AccessNode>();

            IsUnique = isUnique;

            Depth = depth;
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

        public virtual void SetValue(object topLevelObject, object value)
        {
            var parentObject = Parent.GetSelfFromTopLevel(topLevelObject);

            PropertyInfo.SetValue(parentObject, value);
        }

        public virtual object GetValue(object topLevelObject)
        {
            return GetSelfFromTopLevel(topLevelObject);
        }

        private object GetSelfFromTopLevel(object topLevelObject)
        {
            if (IsTopLevel(this))
            {
                return topLevelObject;
            }

            var parentObject = Parent.GetSelfFromTopLevel(topLevelObject);

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

        public AccessNode GetTopLevelNode()
        {
            return GetTopLevelNode(this);
        }

        private AccessNode GetTopLevelNode(AccessNode node)
        {
            if (node.IsRoot)
            {
                if (node.IsCollection)
                {
                    return null;
                }

                return node;
            }

            if (IsTopLevel(node.Parent))
            {
                return node.Parent;
            }

            return GetTopLevelNode(node.Parent);
        }


        private bool IsTopLevel(AccessNode node)
        {
            if (!node.IsRoot)
            {
                return node.Parent.IsCollection;
            }

            return !node.IsCollection;
        }

        public bool IsCollectable
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.IsCollection;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return GetFullName();
        }


        public int Depth { get; protected set; }
    }
}