using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Meadow.Reflection.ObjectTree;

namespace Meadow.Requests.FieldManipulation
{
    public class FiledManipulationMarker<TModel>:IFieldMarks,IFieldManipulator<TModel>
    {
        private readonly List<string> _excludedNames;
        private readonly Dictionary<string, string> _renames;

        public FiledManipulationMarker()
        {
            _excludedNames = new List<string>();
            _renames = new Dictionary<string, string>();
        }

        private string GetPropertyName<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var memberExpression = (MemberExpression) propertySelector.Body;

            var selectedPropertyName = new TypeAnalyzer().GetFieldName<TModel>(memberExpression);

            return selectedPropertyName;
        }

        public FiledManipulationMarker<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var selectedPropertyName = GetPropertyName(propertySelector);

            _excludedNames.Add(selectedPropertyName);

            return this;
        }

        public FiledManipulationMarker<TModel> Rename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector,
            string newName)
        {
            var selectedPropertyName = GetPropertyName(propertySelector);

            _renames.Add(selectedPropertyName, newName);

            return this;
        }

        public FiledManipulationMarker<TModel> UnRename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var selectedPropertyName = GetPropertyName(propertySelector);

            if (_renames.ContainsKey(selectedPropertyName))
            {
                _renames.Remove(selectedPropertyName);
            }

            return this;
        }

        public FiledManipulationMarker<TModel> Exclude(string name)
        {
            _excludedNames.Add(name);

            return this;
        }


        public FiledManipulationMarker<TModel> UnExclude(string name)
        {
            if (_excludedNames.Contains(name))
            {
                _excludedNames.Remove(name);
            }

            return this;
        }

        public FiledManipulationMarker<TModel> UnExclude<TProperty>(
            Expression<Func<TModel, TProperty>> propertySelector)
        {
            var selectedPropertyName = GetPropertyName(propertySelector);

            UnExclude(selectedPropertyName);

            return this;
        }

        public List<string> ExcludedNames()
        {
            return new List<string>(_excludedNames);
        }


        public bool IsIncluded(string fieldName)
        {
            return !_excludedNames.Contains(fieldName);
        }

        public string GetPracticalName(string fieldname)
        {
            if (_renames.ContainsKey(fieldname))
            {
                return _renames[fieldname];
            }

            return fieldname;
        }

        public void Clear()
        {
            _renames.Clear();
            _excludedNames.Clear();
        }
    }
}