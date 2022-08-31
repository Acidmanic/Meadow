using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.RelationalTranslation
{
    public class RelationalTranslator : IRelationalTranslator
    {
        public IDataOwnerNameProvider DataOwnerNameProvider { get; private set; }

        public char Delimiter { get; private set; }

        public TableDouble GetFullTreeTable<TEntity>()
        {
            return GetFullTreeTable(typeof(TEntity));
        }

        public IRelationalTranslator Initialize(IDataOwnerNameProvider dataOwnerNameProvider, char delimiter)
        {
            DataOwnerNameProvider = dataOwnerNameProvider;
            Delimiter = delimiter;

            return this;
        }

        public TableDouble GetFullTreeTable(Type type)
        {
            var evaluator = new ObjectEvaluator(type);

            return GetFullTreeTable(evaluator.RootNode, evaluator);
        }

        public TableDouble GetFullTreeTable(AccessNode node, ObjectEvaluator evaluator)
        {
            var me = TableDoubles.Simple(node);

            var children = node.GetChildren();

            var leavesOnly = children.All(c => c.IsLeaf);

            if (leavesOnly)
            {
                return me;
            }

            var selections = new List<Select>();

            var fromMe = Select.From(me);

            me.From = fromMe.Phrase;

            var noneLeafChildren = children.Where(c => !c.IsLeaf);

            foreach (var child in noneLeafChildren)
            {
                var childFullTable = GetFullTreeTable(child, evaluator);

                var childSelection = Select.Join(me, childFullTable, Delimiter, !child.IsCollection);

                selections.Add(childSelection);
            }

            foreach (var selection in selections)
            {
                selection.OriginalFields.ForEach(f => me.Fields.Add(selection.Name + Delimiter + f));

                me.Joins.AddRange(selection.OriginalJoins);
            }

            me.Name += "FullTree";

            return me;
        }
    }
}