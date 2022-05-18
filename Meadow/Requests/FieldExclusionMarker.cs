using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Meadow.Requests
{
    public class FieldExclusionMarker<TModel>
    {
        private readonly List<string> _excludedNames;

        public FieldExclusionMarker()
        {
            _excludedNames = new List<string>();
        }

        public FieldExclusionMarker<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var selectedPropertyName = ((MemberExpression) propertySelector.Body).Member.Name;

            _excludedNames.Add(selectedPropertyName);

            return this;
        }

        public FieldExclusionMarker<TModel> Exclude(string name)
        {
            _excludedNames.Add(name);

            return this;
        }


        public FieldExclusionMarker<TModel> UnExclude(string name)
        {
            if (_excludedNames.Contains(name))
            {
                _excludedNames.Remove(name);
            }

            return this;
        }

        public FieldExclusionMarker<TModel> UnExclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector)
        {
            var selectedPropertyName = ((MemberExpression) propertySelector.Body).Member.Name;

            UnExclude(selectedPropertyName);

            return this;
        }

        public List<string> ExcludedNames()
        {
            return new List<string>(_excludedNames);
        }
    }
}