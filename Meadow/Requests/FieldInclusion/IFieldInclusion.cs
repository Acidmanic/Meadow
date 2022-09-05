using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Requests.FieldInclusion
{
    public interface IFieldInclusion<TModel>
    {

        bool IsIncluded(FieldKey key);
       
        bool IsIncluded(string address);

        bool IsIncluded<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        string GetPracticalName(FieldKey key);
        
        string GetPracticalName(string address);
        
        string GetPracticalName<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
    }
}