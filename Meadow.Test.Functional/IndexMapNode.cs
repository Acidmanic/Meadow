using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Test.Functional
{
    public interface IIndexMapNode
    {
        public string ClearIndexAddress { get; set; }

        public int Index { get; }

        public List<IIndexMapNode> Children { get; }

        void Increment();

        void Reset();

        public IIndexMapNode Parent { get; }
    }


    public class CollectionIndexMapNode : IIndexMapNode
    {
        public string ClearIndexAddress { get; set; }

        public int Index { get; private set; }

        public List<IIndexMapNode> Children { get; private set; }

        private bool IsCollection { get; set; }

        public IIndexMapNode Parent { get; private set; }


        public CollectionIndexMapNode(IIndexMapNode parent)
        {
            Children = new List<IIndexMapNode>();
            IsCollection = true;
            Index = 0;
            Parent = parent;
        }


        public void Increment()
        {
            Index++;

            Children.ForEach(c => c.Reset());
        }


        public void Reset()
        {
            Index = 0;

            Children.ForEach(c => c.Reset());
        }
    }

    
    public class RedirectingIndexMapNode : IIndexMapNode
    {
        public string ClearIndexAddress { get; set; }

        public int Index { get; private set; }

        public List<IIndexMapNode> Children { get; private set; }

        private bool IsCollection { get; set; }

        public IIndexMapNode Parent { get; private set; }


        public RedirectingIndexMapNode(IIndexMapNode parent)
        {
            Children = new List<IIndexMapNode>();

            IsCollection = true;

            Index = -1;

            Parent = parent;
        }


        public void Increment()
        {
            this.Parent.Increment();
        }


        public void Reset()
        {
            Children.ForEach(c => c.Reset());
        }
    }


    public class NewObjectEventIndexMapNode : IIndexMapNode
    {
        public string ClearIndexAddress { get; set; }
        public int Index { get; }
        
        public List<IIndexMapNode> Children { get; }
        
        private readonly Action _onNewObject;

        public NewObjectEventIndexMapNode(Action onNewObject)
        {
            _onNewObject = onNewObject;
            Index = -1;

            ClearIndexAddress = null;
            
            Children = new List<IIndexMapNode>();

            Parent = null;
        }


        public void Increment()
        {
            _onNewObject();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public IIndexMapNode Parent { get; }
    }
}