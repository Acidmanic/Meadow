using System.Collections.Generic;
using System.Linq;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets;

public interface ICallBackParameterBuilder:IParameterBuilder
{

    ICallBackParameterBuilder Add();

    Parameter[] BuildParameters();
}

public class ParameterSetBuilder
{
    
    private class CallBackParameterBuilder:ParameterBuilder,ICallBackParameterBuilder
    {
        private readonly ParameterSetBuilder _originalBuilder;
        
        public CallBackParameterBuilder(IDbTypeNameMapper dbTypeNameMapper, ParameterSetBuilder originalBuilder) : base(dbTypeNameMapper)
        {
            _originalBuilder = originalBuilder;
        }

        public ICallBackParameterBuilder Add() => _originalBuilder.Add();

        public Parameter[] BuildParameters() => _originalBuilder.Build();
    }
    
    private readonly IDbTypeNameMapper _dbTypeNameMapper;
    
    private readonly List<ParameterBuilder> _parameterBuilders = new();

    public ParameterSetBuilder(IDbTypeNameMapper dbTypeNameMapper)
    {
        _dbTypeNameMapper = dbTypeNameMapper;
        
    }

    public  ICallBackParameterBuilder Add()
    {
        var currentBuilder = new CallBackParameterBuilder(_dbTypeNameMapper,this);
        
        _parameterBuilders.Add(currentBuilder);

        return currentBuilder;
    }

    public Parameter[] Build()
    {
        return _parameterBuilders.Select(b => b.Build()).ToArray();
    }
    
}