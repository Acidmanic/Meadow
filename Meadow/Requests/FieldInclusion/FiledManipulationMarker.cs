using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Requests.FieldInclusion
{
    public class FiledManipulationMarker<TModel> : IFieldInclusion<TModel>, IFieldInclusionMarker<TModel>
    {
        private readonly List<FieldKey> _excludedNames;
        private readonly Dictionary<FieldKey, string> _renames;
        private readonly MemberOwnerUtilities _memberOwnerUtilities;
        private readonly bool _fullTree;

        public FiledManipulationMarker(IDataOwnerNameProvider dataOwnerNameProvider, bool fullTree)
        {
            _fullTree = fullTree;
            _memberOwnerUtilities = new MemberOwnerUtilities(dataOwnerNameProvider);
            _excludedNames = new List<FieldKey>();
            _renames = new Dictionary<FieldKey, string>();
        }

        private Result<FieldKey> GetKey<T, TP>(Expression<Func<T, TP>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            var nameList = new List<string>();

            while (me != null)
            {
                string propertyName = me.Member.Name;

                Type propertyType = me.Type;

                nameList.Add(propertyName);

                me = me.Expression as MemberExpression;
            }

            var evaluator = new ObjectEvaluator(typeof(T));

            var node = evaluator.RootNode;

            nameList.Reverse();

            foreach (var name in nameList)
            {
                node = node.GetChildren().FirstOrDefault(c => c.Name == name);

                if (node == null)
                {
                    return Result.Failure<FieldKey>();
                }

                var currentKey = evaluator.Map.FieldKeyByNode(node);

                Console.WriteLine(currentKey.ToString());

                if (node.IsCollection)
                {
                    node = node.GetChildren()[0];
                }
            }

            var key = evaluator.Map.FieldKeyByNode(node);

            return Result.Successful(key);
        }

        public IFieldInclusionMarker<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = GetKey(propertySelector);

            return Exclude(key);
        }

        public IFieldInclusionMarker<TModel> Exclude(FieldKey key)
        {
            _excludedNames.Add(key);

            return this;
        }

        public IFieldInclusionMarker<TModel> UnExclude<TProperty>(
            Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = GetKey(propertySelector);

            return UnExclude(key);
        }

        public IFieldInclusionMarker<TModel> UnExclude(FieldKey key)
        {
            if (_excludedNames.Contains(key))
            {
                _excludedNames.Remove(key);
            }

            return this;
        }

        public IFieldInclusionMarker<TModel> Rename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector,
            string newName)
        {
            var key = GetKey(propertySelector);

            return Rename(key, newName);
        }

        public IFieldInclusionMarker<TModel> Rename(FieldKey key, string newName)
        {
            _renames.Add(key, newName);

            return this;
        }

        public IFieldInclusionMarker<TModel> UnRename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = GetKey(propertySelector);

            UnRename(key);

            return this;
        }

        public IFieldInclusionMarker<TModel> UnRename(FieldKey key)
        {
            if (_renames.ContainsKey(key))
            {
                _renames.Remove(key);
            }

            return this;
        }

        public IFieldInclusionMarker<TModel> Exclude(string name)
        {
            var key = FieldKey.Parse(name);

            if (key == null)
            {
                throw new Exception("You should enter a valid standard address of the field.");
            }

            _excludedNames.Add(key);

            return this;
        }


        public List<string> ExcludedNames()
        {
            return new List<string>(_excludedNames.Select(k => k.ToString()));
        }


        public bool IsIncluded(FieldKey key)
        {
            return !_excludedNames.Contains(key);
        }

        public bool IsIncluded(string address)
        {
            var key = FieldKey.Parse(address);

            if (key == null)
            {
                return false;
            }

            return IsIncluded(key);
        }


        public bool IsIncluded<TP>(Expression<Func<TModel, TP>> expr)
        {
            var key = GetKey(expr);

            if (!key)
            {
                return false;
            }

            return !_excludedNames.Contains(key);
        }

        public string GetPracticalName(FieldKey key)
        {
            if (_renames.ContainsKey(key))
            {
                return _renames[key];
            }

            return null;
        }

        public string GetPracticalName(string address)
        {
            var key = FieldKey.Parse(address);

            if (key == null)
            {
                return address;
            }

            return key.ToString();
        }

        public string GetPracticalName<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var key = GetKey(propertySelector);

            return GetPracticalName(key);
        }

        public void Clear()
        {
            _renames.Clear();
            _excludedNames.Clear();
        }
    }
}