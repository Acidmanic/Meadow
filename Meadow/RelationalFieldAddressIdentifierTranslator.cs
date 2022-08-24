using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Contracts;
using Meadow.Extensions;

namespace Meadow
{
    /// <summary>
    /// [OwnerName][MemberName]....[MemberName]
    /// </summary>
    public class RelationalFieldAddressIdentifierTranslator : IFieldAddressIdentifierTranslator
    {
        public string Separator { get; set; } = "_";

        public IDataOwnerNameProvider DataOwnerNameProvider { get; set; } = new PluralDataOwnerNameProvider();


        public Dictionary<string, FieldKey> MapAddressesByIdentifier<TModel>()
        {
            return MapAddressesByIdentifier(typeof(TModel));
        }

        public Dictionary<string, FieldKey> MapAddressesByIdentifier(Type type)
        {
            var fullLengthMap = CreateFullLengthMap(type);

            var optimized = OptimizeLengths(fullLengthMap);

            var result = new Dictionary<string, FieldKey>();

            foreach (var item in optimized)
            {
                result.Add(item.Key.ToString(Separator), item.Value);
            }

            return result;
        }


        private class FieldKeyOptimization
        {
            public FieldKey Source;
            public int Length = 1;
            public int count = 0;
            public string Address;

            public FieldKeyOptimization(FieldKey source)
            {
                Source = source;
                Length = 1;
                Update();
            }

            public FieldKey Optimized()
            {
                int totalLength = Source.Count;

                int index = totalLength - Length;

                var sub = Source.Subkey(index, Length);

                return sub;
            }

            public FieldKeyOptimization Update()
            {
                Address = Optimized().ToString();

                return this;
            }
        }

        private Dictionary<FieldKey, FieldKey> OptimizeLengths(Dictionary<FieldKey, FieldKey> fullLengthMap)
        {
            var optimization =
                new List<FieldKeyOptimization>(fullLengthMap.Keys.Select(k => new FieldKeyOptimization(k)));

            var dirty = true;

            while (dirty)
            {
                Count(optimization);

                IncrementLength(optimization);

                dirty = optimization.Any(o => o.count > 1);
            }

            var optimized = new Dictionary<FieldKey, FieldKey>();

            foreach (var op in optimization)
            {
                var standard = fullLengthMap[op.Source];

                var optimizedKey = op.Optimized();

                optimized.Add(optimizedKey, standard);
            }

            return optimized;
        }

        private void IncrementLength(List<FieldKeyOptimization> optimization)
        {
            foreach (var op in optimization)
            {
                if (op.count > 1)
                {
                    op.Length++;
                    op.Update();
                }
            }
        }

        private void Count(IEnumerable<FieldKeyOptimization> items)
        {
            var addresses = new Dictionary<string, int>();

            foreach (var item in items)
            {
                if (addresses.ContainsKey(item.Address))
                {
                    addresses[item.Address] += 1;
                }
                else
                {
                    addresses.Add(item.Address, 1);
                }
            }

            foreach (var item in items)
            {
                item.count = addresses[item.Address];
            }
        }

        private Dictionary<FieldKey, FieldKey> CreateFullLengthMap(Type modelType)
        {
            var evaluator = new ObjectEvaluator(modelType);

            var map = new Dictionary<FieldKey, FieldKey>();

            evaluator.Map.Nodes.ForEach(node =>
            {
                if (node.IsLeaf)
                {
                    var key = evaluator.Map.FieldKeyByNode(node);

                    var translated = TranslateNode(key, evaluator);

                    if (map.ContainsKey(translated))
                    {
                        throw new Exception("What the hell??");
                    }

                    map.Add(translated, key);
                }
            });

            return map;
        }

        private FieldKey TranslateNode(FieldKey key, ObjectEvaluator evaluator)
        {
            if (key == null || key.Count == 0)
            {
                throw new Exception("What the hell??");
            }

            if (key.Count == 1)
            {
                return new FieldKey(new List<Segment>
                {
                    new Segment(GetMemberName(key, 0, evaluator))
                });
            }

            var translated = new FieldKey();

            int lastOne = key.Count - 1;

            for (int i = 0; i < lastOne; i++)
            {
                //OwnerName
                translated = translated.Append(new Segment(GetOwnerName(key, i, evaluator)));
            }

            //MemberName
            translated = translated.Append(new Segment(GetMemberName(key, lastOne, evaluator)));

            return translated;
        }


        private string GetOwnerName(FieldKey key, int index, ObjectEvaluator evaluator)
        {
            var k = key.Subkey(0, index + 1);

            var n = evaluator.Map.NodeByKey(k);

            if (n.IsCollection)
            {
                return "";
            }

            return DataOwnerNameProvider.GetNameForOwnerType(n.Type);
        }

        private string GetMemberName(FieldKey key, int index, ObjectEvaluator evaluator)
        {
            return key[index].Name;
        }
    }
}