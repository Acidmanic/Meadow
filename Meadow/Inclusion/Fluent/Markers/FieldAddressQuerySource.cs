using System;
using System.Linq.Expressions;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;

namespace Meadow.Inclusion.Fluent.Markers;

internal class FieldAddressQuerySource<TModel,TProperty>:IQuerySource<TModel,TProperty>
{

    private readonly Action<FieldKey> _onSourceFieldSelect;

    private readonly Action<Operators> _onOperationSelect;

    private readonly Action<TargetValueMark> _onTargetSelect;

    public FieldAddressQuerySource(Action<FieldKey> onSourceFieldSelect, Action<Operators> onOperationSelect, Action<TargetValueMark> onTargetSelect)
    {
        _onSourceFieldSelect = onSourceFieldSelect;
        _onOperationSelect = onOperationSelect;
        _onTargetSelect = onTargetSelect;
    }

    public IOperatorSelector<TModel, TProperty> Where<TField>(Expression<Func<TProperty, TField>> select)
    {
        var selectedSourceField = MemberOwnerUtilities.GetKey(select);

        _onSourceFieldSelect(selectedSourceField);
        
        return new OperatorSelector<TModel, TProperty>(_onOperationSelect,_onTargetSelect,this);
    }
}