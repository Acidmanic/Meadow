using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Requests.FieldManipulation
{
    public interface IFieldManipulator<TModel>
    {


        void Clear();

        IFieldManipulator<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
        IFieldManipulator<TModel> Exclude(FieldKey key);
        
        IFieldManipulator<TModel> UnExclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
        IFieldManipulator<TModel> UnExclude(FieldKey key);
        
        
        
        IFieldManipulator<TModel> Rename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector, string newName);
            
        IFieldManipulator<TModel> Rename(FieldKey key, string newName);

        IFieldManipulator<TModel> UnRename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);
        
        IFieldManipulator<TModel> UnRename(FieldKey key);
        

    }
}