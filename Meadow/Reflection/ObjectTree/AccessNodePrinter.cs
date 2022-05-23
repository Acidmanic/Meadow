using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace Meadow.Reflection.ObjectTree
{
    public class AccessNodePrinter
    {
        public void Print(AccessNode node)
        {
            Print("", node);
        }

        private void Print(string indent, AccessNode node)
        {
            Console.Write(indent + "Name: " + node.Name);
            Console.Write(", Of Type: " + node.Type);
            Console.Write(", Collectable: " + node.IsCollectable);
            Console.Write(", Leaf: " + node.IsLeaf);
            Console.Write(", Root: " + node.IsRoot);
            Console.Write(", Unique: " + node.IsUnique);
            Console.WriteLine();
            var children = node.GetChildren();
            foreach (var child in children)
            {
                Print(indent + "    ", child);
            }
        }
    }
}