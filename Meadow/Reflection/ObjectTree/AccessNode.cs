using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Reflection.ObjectTree
{
    public class AccessNode
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public AccessNode Parent { get; private set; }

        private List<AccessNode> Children { get; set; }

        public bool IsLeaf => Children.Count == 0;

        private PropertyInfo PropertyInfo { get; set; }

        public bool IsRoot => Parent == null;

        public AccessNode(string name, PropertyInfo info)
        {
            Name = name;

            PropertyInfo = info;

            Type = info.PropertyType;

            Parent = null;

            Children = new List<AccessNode>();
        }


        public AccessNode(string name, Type type)
        {
            Name = name;

            PropertyInfo = null;

            Type = type;

            Parent = null;

            Children = new List<AccessNode>();
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

        public void SetValue(object rootObject, object value)
        {
            var parentObject = Parent.GetSelfFromRoot(rootObject);

            PropertyInfo.SetValue(parentObject, value);
        }

        public object GetValue(object rootObject)
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
    }
}