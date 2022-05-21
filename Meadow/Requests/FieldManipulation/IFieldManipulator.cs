using System;
using System.Linq.Expressions;

namespace Meadow.Requests.FieldManipulation
{
    public interface IFieldManipulator<TModel>
    {


        void Clear();

        FiledManipulationMarker<TModel> Exclude<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        FiledManipulationMarker<TModel> Rename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector,
            string newName);

        FiledManipulationMarker<TModel> UnRename<TProperty>(Expression<Func<TModel, TProperty>> propertySelector);

        FiledManipulationMarker<TModel> Exclude(string name);

        FiledManipulationMarker<TModel> UnExclude(string name);

        FiledManipulationMarker<TModel> UnExclude<TProperty>(
            Expression<Func<TModel, TProperty>> propertySelector);
        
        

    }
}