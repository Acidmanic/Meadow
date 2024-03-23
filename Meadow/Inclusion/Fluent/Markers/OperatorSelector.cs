using System;

namespace Meadow.Inclusion.Fluent.Markers;

internal class OperatorSelector<TParametersModel, TModel, TProperty> : IOperatorSelector<TParametersModel, TModel, TProperty>
{
    
    private readonly Action<Operators> _onOperationSelect;

    private readonly Action<TargetValueMark> _onTargetSelect;

    private readonly IQuerySource<TParametersModel, TModel, TProperty> _source;

    public OperatorSelector(Action<Operators> onOperationSelect, Action<TargetValueMark> onTargetSelect, IQuerySource<TParametersModel, TModel, TProperty> source)
    {
        _onOperationSelect = onOperationSelect;
        _onTargetSelect = onTargetSelect;
        _source = source;
    }


    private QueryTarget<TParametersModel,TModel, TProperty> TargetSelect(Operators op)
    {
        _onOperationSelect(op);

        return new QueryTarget<TParametersModel, TModel, TProperty>(_onTargetSelect, _source);
    }
    
    public IToQueryTarget<TParametersModel, TModel, TProperty> IsEqual()
    {
        return TargetSelect(Operators.IsEqualTo);
    }

    public IToQueryTarget<TParametersModel, TModel, TProperty> IsNotEqual()
    {
        return TargetSelect(Operators.IsNotEqualTo);
    }

    public IThanQueryTarget<TParametersModel, TModel, TProperty> IsGreater()
    {
        return TargetSelect(Operators.IsGreaterThan);
    }

    public IThanQueryTarget<TParametersModel, TModel, TProperty> IsSmaller()
    {
        return TargetSelect(Operators.IsSmallerThan);
    }

    public IToQueryTarget<TParametersModel, TModel, TProperty> IsGreaterOrEqual()
    {
        return TargetSelect(Operators.IsGreaterOrEqualTo);
    }

    public IToQueryTarget<TParametersModel, TModel, TProperty> IsSmallerOrEqual()
    {
        return TargetSelect(Operators.IsSmallerOrEqualTo);
    }
}

internal class OperatorSelector<TModel, TProperty> : IOperatorSelector<TModel, TProperty>
{
    private readonly Action<Operators> _onOperationSelect;

    private readonly Action<TargetValueMark> _onTargetSelect;

    private readonly IQuerySource<TModel, TProperty> _source;

    private QueryTarget<TModel, TProperty> TargetSelect(Operators op)
    {
        _onOperationSelect(op);

        return new QueryTarget<TModel, TProperty>(_onTargetSelect, _source);
    }

    public OperatorSelector(Action<Operators> onOperationSelect, Action<TargetValueMark> onTargetSelect,
        IQuerySource<TModel, TProperty> source)
    {
        _onOperationSelect = onOperationSelect;

        _onTargetSelect = onTargetSelect;
        
        _source = source;
    }

    public IToQueryTarget<TModel, TProperty> IsEqual()
    {
        return TargetSelect(Operators.IsEqualTo);
    }

    public IToQueryTarget<TModel, TProperty> IsNotEqual()
    {
        return TargetSelect(Operators.IsNotEqualTo);
    }

    public IThanQueryTarget<TModel, TProperty> IsGreater()
    {
        return TargetSelect(Operators.IsGreaterThan);
    }

    public IThanQueryTarget<TModel, TProperty> IsSmaller()
    {
        return TargetSelect(Operators.IsSmallerThan);
    }

    public IToQueryTarget<TModel, TProperty> IsGreaterOrEqual()
    {
        return TargetSelect(Operators.IsGreaterOrEqualTo);
    }

    public IToQueryTarget<TModel, TProperty> IsSmallerOrEqual()
    {
        return TargetSelect(Operators.IsSmallerOrEqualTo);
    }
}