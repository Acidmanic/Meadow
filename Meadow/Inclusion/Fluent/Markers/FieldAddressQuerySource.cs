using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Inclusion.Enums;

namespace Meadow.Inclusion.Fluent.Markers;


internal class FieldAddressQuerySource<TParametersModel,TModel, TProperty> : IQuerySource<TParametersModel,TModel, TProperty>
{
    private readonly Action<FieldKey> _onSourceFieldSelect;

    private readonly Action<Operators> _onOperationSelect;

    private readonly Action<TargetValueMark> _onTargetSelect;

    private readonly Action<BooleanRelation> _onRelateToNext;

    public FieldAddressQuerySource(Action<FieldKey> onSourceFieldSelect, Action<Operators> onOperationSelect, Action<TargetValueMark> onTargetSelect, Action<BooleanRelation> onRelateToNext)
    {
        _onSourceFieldSelect = onSourceFieldSelect;
        _onOperationSelect = onOperationSelect;
        _onTargetSelect = onTargetSelect;
        _onRelateToNext = onRelateToNext;
    }

    public IOperatorSelector<TParametersModel, TModel, TProperty> Where<TField>(Expression<Func<TProperty, TField>> select)
    {
        var selectedSourceField = MemberOwnerUtilities.GetKey(select);

        _onSourceFieldSelect(selectedSourceField);
        
        return new OperatorSelector<TParametersModel,TModel, TProperty>(_onOperationSelect,_onTargetSelect,this,_onRelateToNext);
    }
}

internal class FieldAddressQuerySource<TModel,TProperty>:IQuerySource<TModel,TProperty>
{

    private readonly Action<FieldKey> _onSourceFieldSelect;

    private readonly Action<Operators> _onOperationSelect;

    private readonly Action<TargetValueMark> _onTargetSelect;
    
    private readonly Action<BooleanRelation> _onRelateToNext;

    public FieldAddressQuerySource(Action<FieldKey> onSourceFieldSelect, Action<Operators> onOperationSelect, Action<TargetValueMark> onTargetSelect, Action<BooleanRelation> onRelateToNext)
    {
        _onSourceFieldSelect = onSourceFieldSelect;
        _onOperationSelect = onOperationSelect;
        _onTargetSelect = onTargetSelect;
        _onRelateToNext = onRelateToNext;
    }

    public virtual IOperatorSelector<TModel, TProperty> Where<TField>(Expression<Func<TProperty, TField>> select)
    {
        var selectedSourceField = MemberOwnerUtilities.GetKey(select);

        _onSourceFieldSelect(selectedSourceField);
        
        return new OperatorSelector<TModel, TProperty>(_onOperationSelect,_onTargetSelect,this,_onRelateToNext);
    }
}